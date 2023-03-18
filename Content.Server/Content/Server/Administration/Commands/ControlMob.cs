using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind.Components;
using Content.Server.Players;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000836 RID: 2102
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class ControlMob : IConsoleCommand
	{
		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x06002E04 RID: 11780 RVA: 0x000F09DF File Offset: 0x000EEBDF
		public string Command
		{
			get
			{
				return "controlmob";
			}
		}

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x06002E05 RID: 11781 RVA: 0x000F09E6 File Offset: 0x000EEBE6
		public string Description
		{
			get
			{
				return Loc.GetString("control-mob-command-description");
			}
		}

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06002E06 RID: 11782 RVA: 0x000F09F2 File Offset: 0x000EEBF2
		public string Help
		{
			get
			{
				return Loc.GetString("control-mob-command-help-text");
			}
		}

		// Token: 0x06002E07 RID: 11783 RVA: 0x000F0A00 File Offset: 0x000EEC00
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("shell-server-cannot");
				return;
			}
			if (args.Length != 1)
			{
				shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			int targetId;
			if (!int.TryParse(args[0], out targetId))
			{
				shell.WriteLine(Loc.GetString("shell-argument-must-be-number"));
				return;
			}
			EntityUid target;
			target..ctor(targetId);
			if (!target.IsValid() || !this._entities.EntityExists(target))
			{
				shell.WriteLine(Loc.GetString("shell-invalid-entity-id"));
				return;
			}
			if (!this._entities.HasComponent<MindComponent>(target))
			{
				shell.WriteLine(Loc.GetString("shell-entity-is-not-mob"));
				return;
			}
			PlayerData playerData = player.ContentData();
			((playerData != null) ? playerData.Mind : null).TransferTo(new EntityUid?(target), false, false);
		}

		// Token: 0x04001C4F RID: 7247
		[Dependency]
		private readonly IEntityManager _entities;
	}
}
