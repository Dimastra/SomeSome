using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Shuttles.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Shuttles.Commands
{
	// Token: 0x0200020F RID: 527
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class DockEmergencyShuttleCommand : IConsoleCommand
	{
		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000A72 RID: 2674 RVA: 0x000371D7 File Offset: 0x000353D7
		public string Command
		{
			get
			{
				return "dockemergencyshuttle";
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000A73 RID: 2675 RVA: 0x000371DE File Offset: 0x000353DE
		public string Description
		{
			get
			{
				return Loc.GetString("emergency-shuttle-command-dock-desc");
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000A74 RID: 2676 RVA: 0x000371EA File Offset: 0x000353EA
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x000371FB File Offset: 0x000353FB
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ShuttleSystem>().CallEmergencyShuttle();
		}
	}
}
