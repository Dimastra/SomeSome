using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Binary.Components
{
	// Token: 0x02000772 RID: 1906
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasVolumePumpComponent : Component
	{
		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x0600286E RID: 10350 RVA: 0x000D3491 File Offset: 0x000D1691
		// (set) Token: 0x0600286F RID: 10351 RVA: 0x000D3499 File Offset: 0x000D1699
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06002870 RID: 10352 RVA: 0x000D34A2 File Offset: 0x000D16A2
		// (set) Token: 0x06002871 RID: 10353 RVA: 0x000D34AA File Offset: 0x000D16AA
		[ViewVariables]
		public bool Overclocked { get; set; }

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06002872 RID: 10354 RVA: 0x000D34B3 File Offset: 0x000D16B3
		// (set) Token: 0x06002873 RID: 10355 RVA: 0x000D34BB File Offset: 0x000D16BB
		[ViewVariables]
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName { get; set; } = "inlet";

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06002874 RID: 10356 RVA: 0x000D34C4 File Offset: 0x000D16C4
		// (set) Token: 0x06002875 RID: 10357 RVA: 0x000D34CC File Offset: 0x000D16CC
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName { get; set; } = "outlet";

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06002876 RID: 10358 RVA: 0x000D34D5 File Offset: 0x000D16D5
		// (set) Token: 0x06002877 RID: 10359 RVA: 0x000D34DD File Offset: 0x000D16DD
		[ViewVariables]
		[DataField("transferRate", false, 1, false, false, null)]
		public float TransferRate { get; set; } = 200f;

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06002878 RID: 10360 RVA: 0x000D34E6 File Offset: 0x000D16E6
		// (set) Token: 0x06002879 RID: 10361 RVA: 0x000D34EE File Offset: 0x000D16EE
		[ViewVariables]
		[DataField("maxTransferRate", false, 1, false, false, null)]
		public float MaxTransferRate { get; set; } = 200f;

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x0600287A RID: 10362 RVA: 0x000D34F7 File Offset: 0x000D16F7
		// (set) Token: 0x0600287B RID: 10363 RVA: 0x000D34FF File Offset: 0x000D16FF
		[DataField("leakRatio", false, 1, false, false, null)]
		public float LeakRatio { get; set; } = 0.1f;

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x0600287C RID: 10364 RVA: 0x000D3508 File Offset: 0x000D1708
		// (set) Token: 0x0600287D RID: 10365 RVA: 0x000D3510 File Offset: 0x000D1710
		[DataField("lowerThreshold", false, 1, false, false, null)]
		public float LowerThreshold { get; set; } = 0.01f;

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x0600287E RID: 10366 RVA: 0x000D3519 File Offset: 0x000D1719
		// (set) Token: 0x0600287F RID: 10367 RVA: 0x000D3521 File Offset: 0x000D1721
		[DataField("higherThreshold", false, 1, false, false, null)]
		public float HigherThreshold { get; set; } = GasVolumePumpComponent.DefaultHigherThreshold;

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x06002880 RID: 10368 RVA: 0x000D352A File Offset: 0x000D172A
		// (set) Token: 0x06002881 RID: 10369 RVA: 0x000D3532 File Offset: 0x000D1732
		[DataField("overclockThreshold", false, 1, false, false, null)]
		public float OverclockThreshold { get; set; } = 1000f;

		// Token: 0x0400192B RID: 6443
		public static readonly float DefaultHigherThreshold = 9000f;
	}
}
