using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Binary.Components
{
	// Token: 0x0200076D RID: 1901
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasPassiveGateComponent : Component
	{
		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x0600284B RID: 10315 RVA: 0x000D3252 File Offset: 0x000D1452
		// (set) Token: 0x0600284C RID: 10316 RVA: 0x000D325A File Offset: 0x000D145A
		[ViewVariables]
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName { get; set; } = "inlet";

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x0600284D RID: 10317 RVA: 0x000D3263 File Offset: 0x000D1463
		// (set) Token: 0x0600284E RID: 10318 RVA: 0x000D326B File Offset: 0x000D146B
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName { get; set; } = "outlet";

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x0600284F RID: 10319 RVA: 0x000D3274 File Offset: 0x000D1474
		// (set) Token: 0x06002850 RID: 10320 RVA: 0x000D327C File Offset: 0x000D147C
		[ViewVariables]
		[DataField("flowRate", false, 1, false, false, null)]
		public float FlowRate { get; set; }
	}
}
