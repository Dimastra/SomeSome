using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Power
{
	// Token: 0x0200025A RID: 602
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class PowerMonitoringConsoleEntry
	{
		// Token: 0x060006F2 RID: 1778 RVA: 0x000183F0 File Offset: 0x000165F0
		public PowerMonitoringConsoleEntry(string nl, string ipi, double size, bool isBattery)
		{
			this.NameLocalized = nl;
			this.IconEntityPrototypeId = ipi;
			this.Size = size;
			this.IsBattery = isBattery;
		}

		// Token: 0x040006CC RID: 1740
		public string NameLocalized;

		// Token: 0x040006CD RID: 1741
		public string IconEntityPrototypeId;

		// Token: 0x040006CE RID: 1742
		public double Size;

		// Token: 0x040006CF RID: 1743
		public bool IsBattery;
	}
}
