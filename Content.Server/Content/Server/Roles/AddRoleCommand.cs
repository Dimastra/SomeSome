using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Shared.Administration;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Roles
{
	// Token: 0x02000228 RID: 552
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AddRoleCommand : IConsoleCommand
	{
		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000B05 RID: 2821 RVA: 0x00039F7A File Offset: 0x0003817A
		public string Command
		{
			get
			{
				return "addrole";
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000B06 RID: 2822 RVA: 0x00039F81 File Offset: 0x00038181
		public string Description
		{
			get
			{
				return "Adds a role to a player's mind.";
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000B07 RID: 2823 RVA: 0x00039F88 File Offset: 0x00038188
		public string Help
		{
			get
			{
				return "addrole <session ID> <role>";
			}
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00039F90 File Offset: 0x00038190
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteLine("Expected exactly 2 arguments.");
				return;
			}
			IPlayerData data;
			if (!IoCManager.Resolve<IPlayerManager>().TryGetPlayerDataByUsername(args[0], ref data))
			{
				shell.WriteLine("Can't find that mind");
				return;
			}
			PlayerData playerData = data.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			if (mind == null)
			{
				shell.WriteLine("Can't find that mind");
				return;
			}
			JobPrototype jobPrototype;
			if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<JobPrototype>(args[1], ref jobPrototype))
			{
				shell.WriteLine("Can't find that role");
				return;
			}
			if (mind.AllRoles.Any((Role r) => r.Name == jobPrototype.Name))
			{
				shell.WriteLine("Mind already has that role");
				return;
			}
			Job role = new Job(mind, jobPrototype);
			mind.AddRole(role);
		}
	}
}
