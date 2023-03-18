using System;
using System.IO;
using System.Runtime.CompilerServices;
using Content.Server.Acz;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Afk;
using Content.Server.Chat.Managers;
using Content.Server.Connection;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Server.GameTicking;
using Content.Server.GhostKick;
using Content.Server.GuideGenerator;
using Content.Server.Info;
using Content.Server.IoC;
using Content.Server.Maps;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Preferences.Managers;
using Content.Server.ServerInfo;
using Content.Server.ServerUpdates;
using Content.Server.Station.Systems;
using Content.Server.UtkaIntegration;
using Content.Server.Voting.Managers;
using Content.Server.White.JoinQueue;
using Content.Server.White.Sponsors;
using Content.Server.White.Stalin;
using Content.Server.White.TTS;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Kitchen;
using Content.Shared.Localizations;
using Robust.Server;
using Robust.Server.Bql;
using Robust.Server.ServerStatus;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Entry
{
	// Token: 0x02000527 RID: 1319
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntryPoint : GameServer
	{
		// Token: 0x06001B76 RID: 7030 RVA: 0x00092D9C File Offset: 0x00090F9C
		public override void Init()
		{
			base.Init();
			IConfigurationManager cfg = IoCManager.Resolve<IConfigurationManager>();
			IResourceManager res = IoCManager.Resolve<IResourceManager>();
			ILogManager logManager = IoCManager.Resolve<ILogManager>();
			EntryPoint.LoadConfigPresets(cfg, res, logManager.GetSawmill("configpreset"));
			ContentMagicAczProvider aczProvider = new ContentMagicAczProvider(IoCManager.Resolve<IDependencyCollection>());
			IoCManager.Resolve<IStatusHost>().SetMagicAczProvider(aczProvider);
			IComponentFactory factory = IoCManager.Resolve<IComponentFactory>();
			IPrototypeManager prototypes = IoCManager.Resolve<IPrototypeManager>();
			factory.DoAutoRegistrations();
			factory.IgnoreMissingComponents("Visuals");
			foreach (string ignoreName in IgnoredComponents.List)
			{
				factory.RegisterIgnore(ignoreName, false);
			}
			prototypes.RegisterIgnore("parallax");
			prototypes.RegisterIgnore("guideEntry");
			ServerContentIoC.Register();
			foreach (ModuleTestingCallbacks moduleTestingCallbacks in base.TestingCallbacks)
			{
				Action serverBeforeIoC = ((ServerModuleTestingCallbacks)moduleTestingCallbacks).ServerBeforeIoC;
				if (serverBeforeIoC != null)
				{
					serverBeforeIoC();
				}
			}
			IoCManager.BuildGraph();
			factory.GenerateNetIds();
			string cvar = IoCManager.Resolve<IConfigurationManager>().GetCVar<string>(CCVars.DestinationFile);
			IoCManager.Resolve<ContentLocalizationManager>().Initialize();
			if (string.IsNullOrEmpty(cvar))
			{
				this._euiManager = IoCManager.Resolve<EuiManager>();
				this._voteManager = IoCManager.Resolve<IVoteManager>();
				this._updateManager = IoCManager.Resolve<ServerUpdateManager>();
				this._playTimeTracking = IoCManager.Resolve<PlayTimeTrackingManager>();
				this._sysMan = IoCManager.Resolve<IEntitySystemManager>();
				logManager.GetSawmill("Storage").Level = new LogLevel?(2);
				logManager.GetSawmill("db.ef").Level = new LogLevel?(2);
				IoCManager.Resolve<IAdminLogManager>().Initialize();
				IoCManager.Resolve<IConnectionManager>().Initialize();
				IoCManager.Resolve<IServerDbManager>().Init();
				IoCManager.Resolve<IServerPreferencesManager>().Init();
				IoCManager.Resolve<INodeGroupFactory>().Initialize();
				IoCManager.Resolve<IGamePrototypeLoadManager>().Initialize();
				IoCManager.Resolve<NetworkResourceManager>().Initialize();
				IoCManager.Resolve<GhostKickManager>().Initialize();
				IoCManager.Resolve<SponsorsManager>().Initialize();
				IoCManager.Resolve<JoinQueueManager>().Initialize();
				IoCManager.Resolve<TTSManager>().Initialize();
				IoCManager.Resolve<ServerInfoManager>().Initialize();
				IoCManager.Resolve<StalinManager>().Initialize();
				this._voteManager.Initialize();
				this._updateManager.Initialize();
				this._playTimeTracking.Initialize();
			}
		}

		// Token: 0x06001B77 RID: 7031 RVA: 0x00092FD4 File Offset: 0x000911D4
		public override void PostInit()
		{
			base.PostInit();
			IoCManager.Resolve<IChatSanitizationManager>().Initialize();
			IoCManager.Resolve<IChatManager>().Initialize();
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			IResourceManager resourceManager = IoCManager.Resolve<IResourceManager>();
			string dest = configurationManager.GetCVar<string>(CCVars.DestinationFile);
			if (!string.IsNullOrEmpty(dest))
			{
				ResourcePath resPath = new ResourcePath(dest, "/").ToRootedPath();
				StreamWriter streamWriter = WritableDirProviderExt.OpenWriteText(resourceManager.UserData, resPath.WithName("chem_" + dest));
				ChemistryJsonGenerator.PublishJson(streamWriter);
				streamWriter.Flush();
				StreamWriter streamWriter2 = WritableDirProviderExt.OpenWriteText(resourceManager.UserData, resPath.WithName("react_" + dest));
				ReactionJsonGenerator.PublishJson(streamWriter2);
				streamWriter2.Flush();
				IoCManager.Resolve<IBaseServer>().Shutdown("Data generation done");
				return;
			}
			IoCManager.Resolve<RecipeManager>().Initialize();
			IoCManager.Resolve<IAdminManager>().Initialize();
			IoCManager.Resolve<IAfkManager>().Initialize();
			IoCManager.Resolve<RulesManager>().Initialize();
			this._euiManager.Initialize();
			IoCManager.Resolve<IGameMapManager>().Initialize();
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<GameTicker>().PostInitialize();
			IoCManager.Resolve<IBqlQueryManager>().DoAutoRegistrations();
			IoCManager.Resolve<RoleBanManager>().Initialize();
			IoCManager.Resolve<UtkaTCPWrapper>().Initialize();
			UtkaTCPServer.RegisterCommands();
		}

		// Token: 0x06001B78 RID: 7032 RVA: 0x000930F8 File Offset: 0x000912F8
		public override void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
		{
			base.Update(level, frameEventArgs);
			if (level == 2)
			{
				this._euiManager.SendUpdates();
				this._voteManager.Update();
				return;
			}
			if (level != 3)
			{
				return;
			}
			this._updateManager.Update();
			PlayTimeTrackingManager playTimeTracking = this._playTimeTracking;
			if (playTimeTracking == null)
			{
				return;
			}
			playTimeTracking.Update();
		}

		// Token: 0x06001B79 RID: 7033 RVA: 0x00093148 File Offset: 0x00091348
		protected override void Dispose(bool disposing)
		{
			PlayTimeTrackingManager playTimeTracking = this._playTimeTracking;
			if (playTimeTracking != null)
			{
				playTimeTracking.Shutdown();
			}
			IEntitySystemManager sysMan = this._sysMan;
			if (sysMan == null)
			{
				return;
			}
			StationSystem entitySystemOrNull = sysMan.GetEntitySystemOrNull<StationSystem>();
			if (entitySystemOrNull == null)
			{
				return;
			}
			entitySystemOrNull.OnServerDispose();
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x00093178 File Offset: 0x00091378
		private static void LoadConfigPresets(IConfigurationManager cfg, IResourceManager res, ISawmill sawmill)
		{
			EntryPoint.LoadBuildConfigPresets(cfg, res, sawmill);
			string presets = cfg.GetCVar<string>(CCVars.ConfigPresets);
			if (presets == "")
			{
				return;
			}
			foreach (string preset in presets.Split(',', StringSplitOptions.None))
			{
				string path = "/ConfigPresets/" + preset + ".toml";
				Stream file;
				if (!res.TryContentFileRead(path, ref file))
				{
					sawmill.Error("Unable to load config preset {Preset}!", new object[]
					{
						path
					});
				}
				else
				{
					cfg.LoadDefaultsFromTomlStream(file);
					sawmill.Info("Loaded config preset: {Preset}", new object[]
					{
						path
					});
				}
			}
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x00093218 File Offset: 0x00091418
		private static void LoadBuildConfigPresets(IConfigurationManager cfg, IResourceManager res, ISawmill sawmill)
		{
			EntryPoint.<>c__DisplayClass12_0 CS$<>8__locals1;
			CS$<>8__locals1.cfg = cfg;
			CS$<>8__locals1.res = res;
			CS$<>8__locals1.sawmill = sawmill;
		}

		// Token: 0x06001B7D RID: 7037 RVA: 0x00093248 File Offset: 0x00091448
		[CompilerGenerated]
		internal static void <LoadBuildConfigPresets>g__Load|12_0(CVarDef<bool> cVar, string name, ref EntryPoint.<>c__DisplayClass12_0 A_2)
		{
			string path = "/ConfigPresets/Build/" + name + ".toml";
			Stream file;
			if (A_2.cfg.GetCVar<bool>(cVar) && A_2.res.TryContentFileRead(path, ref file))
			{
				A_2.cfg.LoadDefaultsFromTomlStream(file);
				A_2.sawmill.Info("Loaded config preset: {Preset}", new object[]
				{
					path
				});
			}
		}

		// Token: 0x040011A0 RID: 4512
		private const string ConfigPresetsDir = "/ConfigPresets/";

		// Token: 0x040011A1 RID: 4513
		private const string ConfigPresetsDirBuild = "/ConfigPresets/Build/";

		// Token: 0x040011A2 RID: 4514
		private EuiManager _euiManager;

		// Token: 0x040011A3 RID: 4515
		private IVoteManager _voteManager;

		// Token: 0x040011A4 RID: 4516
		private ServerUpdateManager _updateManager;

		// Token: 0x040011A5 RID: 4517
		[Nullable(2)]
		private PlayTimeTrackingManager _playTimeTracking;

		// Token: 0x040011A6 RID: 4518
		[Nullable(2)]
		private IEntitySystemManager _sysMan;
	}
}
