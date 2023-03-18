using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Commands;
using Content.Shared.Administration;
using Content.Shared.Alert;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Alert.Commands
{
	// Token: 0x020007E4 RID: 2020
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class ClearAlert : IConsoleCommand
	{
		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x06002BDC RID: 11228 RVA: 0x000E60BD File Offset: 0x000E42BD
		public string Command
		{
			get
			{
				return "clearalert";
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x06002BDD RID: 11229 RVA: 0x000E60C4 File Offset: 0x000E42C4
		public string Description
		{
			get
			{
				return "Clears an alert for a player, defaulting to current player";
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x06002BDE RID: 11230 RVA: 0x000E60CB File Offset: 0x000E42CB
		public string Help
		{
			get
			{
				return "clearalert <alertType> <name or userID, omit for current player>";
			}
		}

		// Token: 0x06002BDF RID: 11231 RVA: 0x000E60D4 File Offset: 0x000E42D4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null || player.AttachedEntity == null)
			{
				shell.WriteLine("You don't have an entity.");
				return;
			}
			EntityUid attachedEntity = player.AttachedEntity.Value;
			if (args.Length > 1)
			{
				string target = args[1];
				if (!CommandUtils.TryGetAttachedEntityByUsernameOrId(shell, target, player, out attachedEntity))
				{
					return;
				}
			}
			AlertsComponent alertsComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<AlertsComponent>(attachedEntity, ref alertsComponent))
			{
				shell.WriteLine("user has no alerts component");
				return;
			}
			string alertType = args[0];
			AlertsSystem alertsSystem = EntitySystem.Get<AlertsSystem>();
			AlertPrototype alert;
			if (!alertsSystem.TryGet(Enum.Parse<AlertType>(alertType), out alert))
			{
				shell.WriteLine("unrecognized alertType " + alertType);
				return;
			}
			alertsSystem.ClearAlert(attachedEntity, alert.AlertType);
		}
	}
}
