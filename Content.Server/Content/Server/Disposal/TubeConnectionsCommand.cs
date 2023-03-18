using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Disposal.Tube.Components;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Disposal
{
	// Token: 0x0200054B RID: 1355
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class TubeConnectionsCommand : IConsoleCommand
	{
		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06001C6D RID: 7277 RVA: 0x000977EF File Offset: 0x000959EF
		public string Command
		{
			get
			{
				return "tubeconnections";
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06001C6E RID: 7278 RVA: 0x000977F6 File Offset: 0x000959F6
		public string Description
		{
			get
			{
				return Loc.GetString("tube-connections-command-description");
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06001C6F RID: 7279 RVA: 0x00097802 File Offset: 0x00095A02
		public string Help
		{
			get
			{
				return Loc.GetString("tube-connections-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x0009782C File Offset: 0x00095A2C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null || player.AttachedEntity == null)
			{
				shell.WriteLine(Loc.GetString("shell-only-players-can-run-this-command"));
				return;
			}
			if (args.Length < 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			EntityUid id;
			if (!EntityUid.TryParse(args[0], ref id))
			{
				shell.WriteLine(Loc.GetString("shell-invalid-entity-uid", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("uid", args[0])
				}));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			if (!entityManager.EntityExists(id))
			{
				shell.WriteLine(Loc.GetString("shell-could-not-find-entity-with-uid", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("uid", id)
				}));
				return;
			}
			IDisposalTubeComponent tube;
			if (!entityManager.TryGetComponent<IDisposalTubeComponent>(id, ref tube))
			{
				shell.WriteLine(Loc.GetString("shell-entity-with-uid-lacks-component", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("uid", id),
					new ValueTuple<string, object>("componentName", "IDisposalTubeComponent")
				}));
				return;
			}
			tube.PopupDirections(player.AttachedEntity.Value);
		}
	}
}
