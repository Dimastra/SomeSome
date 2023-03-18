using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Trinary.Components
{
	// Token: 0x0200075B RID: 1883
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PressureControlledValveComponent : Component
	{
		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x060027D1 RID: 10193 RVA: 0x000D139B File Offset: 0x000CF59B
		// (set) Token: 0x060027D2 RID: 10194 RVA: 0x000D13A3 File Offset: 0x000CF5A3
		[ViewVariables]
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName { get; set; } = "inlet";

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x060027D3 RID: 10195 RVA: 0x000D13AC File Offset: 0x000CF5AC
		// (set) Token: 0x060027D4 RID: 10196 RVA: 0x000D13B4 File Offset: 0x000CF5B4
		[ViewVariables]
		[DataField("control", false, 1, false, false, null)]
		public string ControlName { get; set; } = "control";

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x060027D5 RID: 10197 RVA: 0x000D13BD File Offset: 0x000CF5BD
		// (set) Token: 0x060027D6 RID: 10198 RVA: 0x000D13C5 File Offset: 0x000CF5C5
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName { get; set; } = "outlet";

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x060027D7 RID: 10199 RVA: 0x000D13CE File Offset: 0x000CF5CE
		// (set) Token: 0x060027D8 RID: 10200 RVA: 0x000D13D6 File Offset: 0x000CF5D6
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; }

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x060027D9 RID: 10201 RVA: 0x000D13DF File Offset: 0x000CF5DF
		// (set) Token: 0x060027DA RID: 10202 RVA: 0x000D13E7 File Offset: 0x000CF5E7
		[ViewVariables]
		[DataField("gain", false, 1, false, false, null)]
		public float Gain { get; set; } = 10f;

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x060027DB RID: 10203 RVA: 0x000D13F0 File Offset: 0x000CF5F0
		// (set) Token: 0x060027DC RID: 10204 RVA: 0x000D13F8 File Offset: 0x000CF5F8
		[ViewVariables]
		[DataField("threshold", false, 1, false, false, null)]
		public float Threshold { get; set; } = 101.325f;

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x060027DD RID: 10205 RVA: 0x000D1401 File Offset: 0x000CF601
		// (set) Token: 0x060027DE RID: 10206 RVA: 0x000D1409 File Offset: 0x000CF609
		[DataField("maxTransferRate", false, 1, false, false, null)]
		public float MaxTransferRate { get; set; } = 200f;
	}
}
