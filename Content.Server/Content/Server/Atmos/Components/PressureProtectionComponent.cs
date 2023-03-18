using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007AF RID: 1967
	[RegisterComponent]
	public sealed class PressureProtectionComponent : Component
	{
		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06002AAE RID: 10926 RVA: 0x000DFFE7 File Offset: 0x000DE1E7
		[DataField("highPressureMultiplier", false, 1, false, false, null)]
		public float HighPressureMultiplier { get; } = 1f;

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06002AAF RID: 10927 RVA: 0x000DFFEF File Offset: 0x000DE1EF
		[DataField("highPressureModifier", false, 1, false, false, null)]
		public float HighPressureModifier { get; }

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06002AB0 RID: 10928 RVA: 0x000DFFF7 File Offset: 0x000DE1F7
		[DataField("lowPressureMultiplier", false, 1, false, false, null)]
		public float LowPressureMultiplier { get; } = 1f;

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x06002AB1 RID: 10929 RVA: 0x000DFFFF File Offset: 0x000DE1FF
		[DataField("lowPressureModifier", false, 1, false, false, null)]
		public float LowPressureModifier { get; }
	}
}
