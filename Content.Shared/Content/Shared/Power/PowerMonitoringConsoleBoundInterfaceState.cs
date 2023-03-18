using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Power
{
	// Token: 0x02000259 RID: 601
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class PowerMonitoringConsoleBoundInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x060006F1 RID: 1777 RVA: 0x000183CB File Offset: 0x000165CB
		public PowerMonitoringConsoleBoundInterfaceState(double totalSources, double totalLoads, PowerMonitoringConsoleEntry[] sources, PowerMonitoringConsoleEntry[] loads)
		{
			this.TotalSources = totalSources;
			this.TotalLoads = totalLoads;
			this.Sources = sources;
			this.Loads = loads;
		}

		// Token: 0x040006C8 RID: 1736
		public double TotalSources;

		// Token: 0x040006C9 RID: 1737
		public double TotalLoads;

		// Token: 0x040006CA RID: 1738
		public PowerMonitoringConsoleEntry[] Sources;

		// Token: 0x040006CB RID: 1739
		public PowerMonitoringConsoleEntry[] Loads;
	}
}
