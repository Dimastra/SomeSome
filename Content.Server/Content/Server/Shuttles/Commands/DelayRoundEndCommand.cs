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
	// Token: 0x0200020E RID: 526
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class DelayRoundEndCommand : IConsoleCommand
	{
		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000A6D RID: 2669 RVA: 0x00037177 File Offset: 0x00035377
		public string Command
		{
			get
			{
				return "delayroundend";
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000A6E RID: 2670 RVA: 0x0003717E File Offset: 0x0003537E
		public string Description
		{
			get
			{
				return Loc.GetString("emergency-shuttle-command-round-desc");
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000A6F RID: 2671 RVA: 0x0003718A File Offset: 0x0003538A
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0003719B File Offset: 0x0003539B
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ShuttleSystem>().DelayEmergencyRoundEnd())
			{
				shell.WriteLine(Loc.GetString("emergency-shuttle-command-round-yes"));
				return;
			}
			shell.WriteLine(Loc.GetString("emergency-shuttle-command-round-no"));
		}
	}
}
