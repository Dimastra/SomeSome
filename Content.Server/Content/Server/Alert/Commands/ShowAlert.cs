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
	// Token: 0x020007E5 RID: 2021
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class ShowAlert : IConsoleCommand
	{
		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x06002BE1 RID: 11233 RVA: 0x000E619C File Offset: 0x000E439C
		public string Command
		{
			get
			{
				return "showalert";
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x06002BE2 RID: 11234 RVA: 0x000E61A3 File Offset: 0x000E43A3
		public string Description
		{
			get
			{
				return "Shows an alert for a player, defaulting to current player";
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x06002BE3 RID: 11235 RVA: 0x000E61AA File Offset: 0x000E43AA
		public string Help
		{
			get
			{
				return "showalert <alertType> <severity, -1 if no severity> <name or userID, omit for current player>";
			}
		}

		// Token: 0x06002BE4 RID: 11236 RVA: 0x000E61B4 File Offset: 0x000E43B4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null || player.AttachedEntity == null)
			{
				shell.WriteLine("You cannot run this from the server or without an attached entity.");
				return;
			}
			EntityUid attachedEntity = player.AttachedEntity.Value;
			if (args.Length > 2)
			{
				string target = args[2];
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
			string severity = args[1];
			AlertsSystem alertsSystem = EntitySystem.Get<AlertsSystem>();
			AlertPrototype alert;
			if (!alertsSystem.TryGet(Enum.Parse<AlertType>(alertType), out alert))
			{
				shell.WriteLine("unrecognized alertType " + alertType);
				return;
			}
			short sevint;
			if (!short.TryParse(severity, out sevint))
			{
				shell.WriteLine("invalid severity " + sevint.ToString());
				return;
			}
			short? severity2 = (sevint == -1) ? null : new short?(sevint);
			alertsSystem.ShowAlert(attachedEntity, alert.AlertType, severity2, null);
		}
	}
}
