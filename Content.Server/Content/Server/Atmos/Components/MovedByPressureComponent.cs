using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007AD RID: 1965
	[RegisterComponent]
	public sealed class MovedByPressureComponent : Component
	{
		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06002AA4 RID: 10916 RVA: 0x000DFF76 File Offset: 0x000DE176
		// (set) Token: 0x06002AA5 RID: 10917 RVA: 0x000DFF7E File Offset: 0x000DE17E
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06002AA6 RID: 10918 RVA: 0x000DFF87 File Offset: 0x000DE187
		// (set) Token: 0x06002AA7 RID: 10919 RVA: 0x000DFF8F File Offset: 0x000DE18F
		[ViewVariables]
		[DataField("pressureResistance", false, 1, false, false, null)]
		public float PressureResistance { get; set; } = 1f;

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06002AA8 RID: 10920 RVA: 0x000DFF98 File Offset: 0x000DE198
		// (set) Token: 0x06002AA9 RID: 10921 RVA: 0x000DFFA0 File Offset: 0x000DE1A0
		[ViewVariables]
		[DataField("moveResist", false, 1, false, false, null)]
		public float MoveResist { get; set; } = 100f;

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06002AAA RID: 10922 RVA: 0x000DFFA9 File Offset: 0x000DE1A9
		// (set) Token: 0x06002AAB RID: 10923 RVA: 0x000DFFB1 File Offset: 0x000DE1B1
		[ViewVariables]
		public int LastHighPressureMovementAirCycle { get; set; }

		// Token: 0x04001A74 RID: 6772
		public const float MoveForcePushRatio = 1f;

		// Token: 0x04001A75 RID: 6773
		public const float MoveForceForcePushRatio = 1f;

		// Token: 0x04001A76 RID: 6774
		public const float ProbabilityOffset = 25f;

		// Token: 0x04001A77 RID: 6775
		public const float ProbabilityBasePercent = 10f;

		// Token: 0x04001A78 RID: 6776
		public const float ThrowForce = 100f;

		// Token: 0x04001A79 RID: 6777
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;
	}
}
