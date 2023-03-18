using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Mapping
{
	// Token: 0x020003DE RID: 990
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Server | AdminFlags.Mapping)]
	internal sealed class MappingCommand : IConsoleCommand
	{
		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06001459 RID: 5209 RVA: 0x00069348 File Offset: 0x00067548
		public string Command
		{
			get
			{
				return "mapping";
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x0600145A RID: 5210 RVA: 0x0006934F File Offset: 0x0006754F
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-mapping-desc");
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x0600145B RID: 5211 RVA: 0x0006935B File Offset: 0x0006755B
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-mapping-help");
			}
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x00069368 File Offset: 0x00067568
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			int num = args.Length;
			if (num == 1)
			{
				return CompletionResult.FromHint(Loc.GetString("cmd-hint-mapping-id"));
			}
			if (num != 2)
			{
				return CompletionResult.Empty;
			}
			IResourceManager res = IoCManager.Resolve<IResourceManager>();
			return CompletionResult.FromHintOptions(CompletionHelper.UserFilePath(args[1], res.UserData).Concat(CompletionHelper.ContentFilePath(args[1], res)), Loc.GetString("cmd-hint-mapping-path"));
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x000693CC File Offset: 0x000675CC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteError(Loc.GetString("cmd-savemap-server"));
				return;
			}
			if (args.Length > 2)
			{
				shell.WriteLine(this.Help);
				return;
			}
			IMapManager mapManager = IoCManager.Resolve<IMapManager>();
			int num = args.Length;
			MapId mapId;
			if (num == 1 || num == 2)
			{
				int intMapId;
				if (!int.TryParse(args[0], out intMapId))
				{
					shell.WriteError(Loc.GetString("cmd-mapping-failure-integer", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("arg", args[0])
					}));
					return;
				}
				mapId..ctor(intMapId);
				if (mapId == MapId.Nullspace)
				{
					shell.WriteError(Loc.GetString("cmd-mapping-nullspace"));
					return;
				}
				if (mapManager.MapExists(mapId))
				{
					shell.WriteError(Loc.GetString("cmd-mapping-exists", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("mapId", mapId)
					}));
					return;
				}
			}
			else
			{
				mapId = mapManager.NextMapId();
			}
			string toLoad = null;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (args.Length <= 1)
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler.AppendLiteral("addmap ");
				defaultInterpolatedStringHandler.AppendFormatted<MapId>(mapId);
				defaultInterpolatedStringHandler.AppendLiteral(" false");
				shell.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				toLoad = CommandParsing.Escape(args[1]);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
				defaultInterpolatedStringHandler.AppendLiteral("loadmap ");
				defaultInterpolatedStringHandler.AppendFormatted<MapId>(mapId);
				defaultInterpolatedStringHandler.AppendLiteral(" \"");
				defaultInterpolatedStringHandler.AppendFormatted(toLoad);
				defaultInterpolatedStringHandler.AppendLiteral("\" 0 0 0 true");
				shell.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (!mapManager.MapExists(mapId))
			{
				shell.WriteError(Loc.GetString("cmd-mapping-error"));
				return;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid playerEntity = attachedEntity.GetValueOrDefault();
				if (playerEntity.Valid)
				{
					EntityPrototype entityPrototype = this._entities.GetComponent<MetaDataComponent>(playerEntity).EntityPrototype;
					if (((entityPrototype != null) ? entityPrototype.ID : null) != "AdminObserver")
					{
						shell.ExecuteCommand("aghost");
					}
				}
			}
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			shell.ExecuteCommand("sudo cvar events.enabled false");
			shell.ExecuteCommand("sudo cvar shuttle.auto_call_time 0");
			if (configurationManager.GetCVar<bool>(CCVars.AutosaveEnabled))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
				defaultInterpolatedStringHandler.AppendLiteral("toggleautosave ");
				defaultInterpolatedStringHandler.AppendFormatted<MapId>(mapId);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(toLoad ?? "NEWMAP");
				shell.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
			defaultInterpolatedStringHandler.AppendLiteral("tp 0 0 ");
			defaultInterpolatedStringHandler.AppendFormatted<MapId>(mapId);
			shell.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
			shell.RemoteExecuteCommand("mappingclientsidesetup");
			mapManager.SetMapPaused(mapId, true);
			if (args.Length == 2)
			{
				shell.WriteLine(Loc.GetString("cmd-mapping-success-load", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mapId", mapId),
					new ValueTuple<string, object>("path", args[1])
				}));
				return;
			}
			shell.WriteLine(Loc.GetString("cmd-mapping-success", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("mapId", mapId)
			}));
		}

		// Token: 0x04000C8E RID: 3214
		[Dependency]
		private readonly IEntityManager _entities;
	}
}
