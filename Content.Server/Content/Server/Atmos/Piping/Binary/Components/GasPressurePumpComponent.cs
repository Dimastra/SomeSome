using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Binary.Components
{
	// Token: 0x0200076F RID: 1903
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasPressurePumpComponent : Component
	{
		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06002856 RID: 10326 RVA: 0x000D32DA File Offset: 0x000D14DA
		// (set) Token: 0x06002857 RID: 10327 RVA: 0x000D32E2 File Offset: 0x000D14E2
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06002858 RID: 10328 RVA: 0x000D32EB File Offset: 0x000D14EB
		// (set) Token: 0x06002859 RID: 10329 RVA: 0x000D32F3 File Offset: 0x000D14F3
		[ViewVariables]
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName { get; set; } = "inlet";

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x0600285A RID: 10330 RVA: 0x000D32FC File Offset: 0x000D14FC
		// (set) Token: 0x0600285B RID: 10331 RVA: 0x000D3304 File Offset: 0x000D1504
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName { get; set; } = "outlet";

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x0600285C RID: 10332 RVA: 0x000D330D File Offset: 0x000D150D
		// (set) Token: 0x0600285D RID: 10333 RVA: 0x000D3315 File Offset: 0x000D1515
		[ViewVariables]
		[DataField("targetPressure", false, 1, false, false, null)]
		public float TargetPressure { get; set; } = 101.325f;

		// Token: 0x04001912 RID: 6418
		[ViewVariables]
		[DataField("maxTargetPressure", false, 1, false, false, null)]
		public float MaxTargetPressure = 4500f;
	}
}
