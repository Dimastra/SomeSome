using System;
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
	// Token: 0x0200022B RID: 555
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RemoveRoleCommand : IConsoleCommand
	{
		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000B17 RID: 2839 RVA: 0x0003A21C File Offset: 0x0003841C
		public string Command
		{
			get
			{
				return "rmrole";
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0003A223 File Offset: 0x00038423
		public string Description
		{
			get
			{
				return "Removes a role from a player's mind.";
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000B19 RID: 2841 RVA: 0x0003A22A File Offset: 0x0003842A
		public string Help
		{
			get
			{
				return "rmrole <session ID> <Role Type>\nThat role type is the actual C# type name.";
			}
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0003A234 File Offset: 0x00038434
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
			Job role = new Job(mind, this._prototypeManager.Index<JobPrototype>(args[1]));
			mind.RemoveRole(role);
		}

		// Token: 0x040006CB RID: 1739
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;
	}
}
