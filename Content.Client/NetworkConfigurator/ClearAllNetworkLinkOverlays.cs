using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.NetworkConfigurator
{
	// Token: 0x02000226 RID: 550
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClearAllNetworkLinkOverlays : IConsoleCommand
	{
		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06000E4C RID: 3660 RVA: 0x00056983 File Offset: 0x00054B83
		public string Command
		{
			get
			{
				return "clearnetworklinkoverlays";
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06000E4D RID: 3661 RVA: 0x0005698A File Offset: 0x00054B8A
		public string Description
		{
			get
			{
				return "Clear all network link overlays.";
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06000E4E RID: 3662 RVA: 0x00056991 File Offset: 0x00054B91
		public string Help
		{
			get
			{
				return this.Command;
			}
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x00056999 File Offset: 0x00054B99
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IoCManager.Resolve<IEntityManager>().System<NetworkConfiguratorSystem>().ClearAllOverlays();
		}
	}
}
