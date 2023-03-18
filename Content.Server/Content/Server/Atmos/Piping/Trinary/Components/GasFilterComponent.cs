using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Trinary.Components
{
	// Token: 0x02000759 RID: 1881
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasFilterComponent : Component
	{
		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x060027C1 RID: 10177 RVA: 0x000D1268 File Offset: 0x000CF468
		// (set) Token: 0x060027C2 RID: 10178 RVA: 0x000D1270 File Offset: 0x000CF470
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x060027C3 RID: 10179 RVA: 0x000D1279 File Offset: 0x000CF479
		// (set) Token: 0x060027C4 RID: 10180 RVA: 0x000D1281 File Offset: 0x000CF481
		[ViewVariables]
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName { get; set; } = "inlet";

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x060027C5 RID: 10181 RVA: 0x000D128A File Offset: 0x000CF48A
		// (set) Token: 0x060027C6 RID: 10182 RVA: 0x000D1292 File Offset: 0x000CF492
		[ViewVariables]
		[DataField("filter", false, 1, false, false, null)]
		public string FilterName { get; set; } = "filter";

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x060027C7 RID: 10183 RVA: 0x000D129B File Offset: 0x000CF49B
		// (set) Token: 0x060027C8 RID: 10184 RVA: 0x000D12A3 File Offset: 0x000CF4A3
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName { get; set; } = "outlet";

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x060027C9 RID: 10185 RVA: 0x000D12AC File Offset: 0x000CF4AC
		// (set) Token: 0x060027CA RID: 10186 RVA: 0x000D12B4 File Offset: 0x000CF4B4
		[ViewVariables]
		[DataField("transferRate", false, 1, false, false, null)]
		public float TransferRate { get; set; } = 200f;

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x060027CB RID: 10187 RVA: 0x000D12BD File Offset: 0x000CF4BD
		// (set) Token: 0x060027CC RID: 10188 RVA: 0x000D12C5 File Offset: 0x000CF4C5
		[DataField("maxTransferRate", false, 1, false, false, null)]
		public float MaxTransferRate { get; set; } = 200f;

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x060027CD RID: 10189 RVA: 0x000D12CE File Offset: 0x000CF4CE
		// (set) Token: 0x060027CE RID: 10190 RVA: 0x000D12D6 File Offset: 0x000CF4D6
		[ViewVariables]
		public Gas? FilteredGas { get; set; }
	}
}
