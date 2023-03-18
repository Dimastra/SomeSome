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
	// Token: 0x02000211 RID: 529
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class LaunchEmergencyShuttleCommand : IConsoleCommand
	{
		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000A7C RID: 2684 RVA: 0x00037323 File Offset: 0x00035523
		public string Command
		{
			get
			{
				return "launchemergencyshuttle";
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000A7D RID: 2685 RVA: 0x0003732A File Offset: 0x0003552A
		public string Description
		{
			get
			{
				return Loc.GetString("emergency-shuttle-command-launch-desc");
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000A7E RID: 2686 RVA: 0x00037336 File Offset: 0x00035536
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x00037347 File Offset: 0x00035547
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ShuttleSystem>().EarlyLaunch();
		}
	}
}
