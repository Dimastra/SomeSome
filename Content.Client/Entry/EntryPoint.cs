using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Client.Audio;
using Content.Client.Changelog;
using Content.Client.Chat.Managers;
using Content.Client.Eui;
using Content.Client.Flash;
using Content.Client.GhostKick;
using Content.Client.Guidebook;
using Content.Client.Info;
using Content.Client.Input;
using Content.Client.IoC;
using Content.Client.Launcher;
using Content.Client.MainMenu;
using Content.Client.Overlays;
using Content.Client.Parallax.Managers;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.Preferences;
using Content.Client.Radiation.Overlays;
using Content.Client.Reflection;
using Content.Client.Screenshot;
using Content.Client.Singularity;
using Content.Client.Stylesheets;
using Content.Client.Viewport;
using Content.Client.Voting;
using Content.Client.White.JoinQueue;
using Content.Client.White.Sponsors;
using Content.Client.White.Stalin;
using Content.Shared.Administration;
using Content.Shared.AME;
using Content.Shared.Gravity;
using Content.Shared.Localizations;
using Content.Shared.Markers;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;

namespace Content.Client.Entry
{
	// Token: 0x02000330 RID: 816
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntryPoint : GameClient
	{
		// Token: 0x06001473 RID: 5235 RVA: 0x00077DC4 File Offset: 0x00075FC4
		public override void Init()
		{
			ClientContentIoC.Register();
			foreach (ModuleTestingCallbacks moduleTestingCallbacks in base.TestingCallbacks)
			{
				Action clientBeforeIoC = ((ClientModuleTestingCallbacks)moduleTestingCallbacks).ClientBeforeIoC;
				if (clientBeforeIoC != null)
				{
					clientBeforeIoC();
				}
			}
			IoCManager.BuildGraph();
			IoCManager.InjectDependencies<EntryPoint>(this);
			this._contentLoc.Initialize();
			this._componentFactory.DoAutoRegistrations();
			this._componentFactory.IgnoreMissingComponents("");
			this._componentFactory.RegisterClass<SharedSpawnPointComponent>(false);
			this._componentFactory.RegisterClass<SharedGravityGeneratorComponent>(false);
			this._componentFactory.RegisterClass<SharedAMEControllerComponent>(false);
			this._prototypeManager.RegisterIgnore("accent");
			this._prototypeManager.RegisterIgnore("material");
			this._prototypeManager.RegisterIgnore("reaction");
			this._prototypeManager.RegisterIgnore("gasReaction");
			this._prototypeManager.RegisterIgnore("seed");
			this._prototypeManager.RegisterIgnore("barSign");
			this._prototypeManager.RegisterIgnore("objective");
			this._prototypeManager.RegisterIgnore("holiday");
			this._prototypeManager.RegisterIgnore("aiFaction");
			this._prototypeManager.RegisterIgnore("htnCompound");
			this._prototypeManager.RegisterIgnore("htnPrimitive");
			this._prototypeManager.RegisterIgnore("gameMap");
			this._prototypeManager.RegisterIgnore("gameMapPool");
			this._prototypeManager.RegisterIgnore("faction");
			this._prototypeManager.RegisterIgnore("lobbyBackground");
			this._prototypeManager.RegisterIgnore("advertisementsPack");
			this._prototypeManager.RegisterIgnore("metabolizerType");
			this._prototypeManager.RegisterIgnore("metabolismGroup");
			this._prototypeManager.RegisterIgnore("salvageMap");
			this._prototypeManager.RegisterIgnore("gamePreset");
			this._prototypeManager.RegisterIgnore("gameRule");
			this._prototypeManager.RegisterIgnore("worldSpell");
			this._prototypeManager.RegisterIgnore("entitySpell");
			this._prototypeManager.RegisterIgnore("instantSpell");
			this._prototypeManager.RegisterIgnore("roundAnnouncement");
			this._prototypeManager.RegisterIgnore("wireLayout");
			this._prototypeManager.RegisterIgnore("alertLevels");
			this._prototypeManager.RegisterIgnore("nukeopsRole");
			this._prototypeManager.RegisterIgnore("stationGoal");
			this._prototypeManager.RegisterIgnore("flavor");
			this._prototypeManager.RegisterIgnore("loadout");
			this._componentFactory.GenerateNetIds();
			this._adminManager.Initialize();
			this._stylesheetManager.Initialize();
			this._screenshotHook.Initialize();
			this._changelogManager.Initialize();
			this._rulesManager.Initialize();
			this._viewportManager.Initialize();
			this._ghostKick.Initialize();
			this._extendedDisconnectInformation.Initialize();
			this._playTimeTracking.Initialize();
			this._stalinManager.Initialize();
			this._configManager.SetCVar("interface.resolutionAutoScaleUpperCutoffX", 1080, false);
			this._configManager.SetCVar("interface.resolutionAutoScaleUpperCutoffY", 720, false);
			this._configManager.SetCVar("interface.resolutionAutoScaleLowerCutoffX", 520, false);
			this._configManager.SetCVar("interface.resolutionAutoScaleLowerCutoffY", 240, false);
			this._configManager.SetCVar("interface.resolutionAutoScaleMinimum", 0.5f, false);
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x00078170 File Offset: 0x00076370
		public override void PostInit()
		{
			base.PostInit();
			ContentContexts.SetupContexts(this._inputManager.Contexts);
			this._parallaxManager.LoadDefaultParallax();
			this._overlayManager.AddOverlay(new SingularityOverlay());
			this._overlayManager.AddOverlay(new FlashOverlay());
			this._overlayManager.AddOverlay(new RadiationPulseOverlay());
			this._overlayManager.AddOverlay(new GrainOverlay());
			this._chatManager.Initialize();
			this._clientPreferencesManager.Initialize();
			this._euiManager.Initialize();
			this._voteManager.Initialize();
			this._gamePrototypeLoadManager.Initialize();
			this._networkResources.Initialize();
			this._userInterfaceManager.SetDefaultTheme("SS14DefaultTheme");
			this._sponsorsManager.Initialize();
			this._queueManager.Initialize();
			this._uiAudio.Initialize();
			this._documentParsingManager.Initialize();
			this._baseClient.RunLevelChanged += delegate([Nullable(2)] object _, RunLevelChangedEventArgs args)
			{
				if (args.NewLevel == 1 && args.OldLevel != 5)
				{
					this.SwitchToDefaultState(args.OldLevel == 3 || args.OldLevel == 4);
				}
			};
			this._userInterfaceManager.MainViewport.Visible = false;
			this.SwitchToDefaultState(false);
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x00078290 File Offset: 0x00076490
		private void SwitchToDefaultState(bool disconnected = false)
		{
			if (this._gameController.LaunchState.FromLauncher)
			{
				this._stateManager.RequestStateChange<LauncherConnecting>();
				LauncherConnecting launcherConnecting = (LauncherConnecting)this._stateManager.CurrentState;
				if (disconnected)
				{
					launcherConnecting.SetDisconnected();
					return;
				}
			}
			else
			{
				if (!this._refl.IsInIntegrationTest())
				{
					this._baseClient.StartSinglePlayer();
				}
				this._stateManager.RequestStateChange<MainScreen>();
			}
		}

		// Token: 0x04000A58 RID: 2648
		[Dependency]
		private readonly IBaseClient _baseClient;

		// Token: 0x04000A59 RID: 2649
		[Dependency]
		private readonly IGameController _gameController;

		// Token: 0x04000A5A RID: 2650
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x04000A5B RID: 2651
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x04000A5C RID: 2652
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000A5D RID: 2653
		[Dependency]
		private readonly IClientAdminManager _adminManager;

		// Token: 0x04000A5E RID: 2654
		[Dependency]
		private readonly IParallaxManager _parallaxManager;

		// Token: 0x04000A5F RID: 2655
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000A60 RID: 2656
		[Dependency]
		private readonly IStylesheetManager _stylesheetManager;

		// Token: 0x04000A61 RID: 2657
		[Dependency]
		private readonly IScreenshotHook _screenshotHook;

		// Token: 0x04000A62 RID: 2658
		[Dependency]
		private readonly ChangelogManager _changelogManager;

		// Token: 0x04000A63 RID: 2659
		[Dependency]
		private readonly RulesManager _rulesManager;

		// Token: 0x04000A64 RID: 2660
		[Dependency]
		private readonly ViewportManager _viewportManager;

		// Token: 0x04000A65 RID: 2661
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x04000A66 RID: 2662
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000A67 RID: 2663
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000A68 RID: 2664
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x04000A69 RID: 2665
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000A6A RID: 2666
		[Dependency]
		private readonly IClientPreferencesManager _clientPreferencesManager;

		// Token: 0x04000A6B RID: 2667
		[Dependency]
		private readonly EuiManager _euiManager;

		// Token: 0x04000A6C RID: 2668
		[Dependency]
		private readonly IVoteManager _voteManager;

		// Token: 0x04000A6D RID: 2669
		[Dependency]
		private readonly IGamePrototypeLoadManager _gamePrototypeLoadManager;

		// Token: 0x04000A6E RID: 2670
		[Dependency]
		private readonly NetworkResourceManager _networkResources;

		// Token: 0x04000A6F RID: 2671
		[Dependency]
		private readonly DocumentParsingManager _documentParsingManager;

		// Token: 0x04000A70 RID: 2672
		[Dependency]
		private readonly GhostKickManager _ghostKick;

		// Token: 0x04000A71 RID: 2673
		[Dependency]
		private readonly ExtendedDisconnectInformationManager _extendedDisconnectInformation;

		// Token: 0x04000A72 RID: 2674
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;

		// Token: 0x04000A73 RID: 2675
		[Dependency]
		private readonly ContentLocalizationManager _contentLoc;

		// Token: 0x04000A74 RID: 2676
		[Dependency]
		private readonly SponsorsManager _sponsorsManager;

		// Token: 0x04000A75 RID: 2677
		[Dependency]
		private readonly JoinQueueManager _queueManager;

		// Token: 0x04000A76 RID: 2678
		[Dependency]
		private readonly IReflectionManager _refl;

		// Token: 0x04000A77 RID: 2679
		[Dependency]
		private readonly UIAudioManager _uiAudio;

		// Token: 0x04000A78 RID: 2680
		[Dependency]
		private readonly StalinManager _stalinManager;
	}
}
