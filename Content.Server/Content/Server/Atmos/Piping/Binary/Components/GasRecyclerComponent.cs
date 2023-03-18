using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Binary.Components
{
	// Token: 0x02000770 RID: 1904
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasRecyclerComponent : Component
	{
		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x0600285F RID: 10335 RVA: 0x000D3359 File Offset: 0x000D1559
		// (set) Token: 0x06002860 RID: 10336 RVA: 0x000D3361 File Offset: 0x000D1561
		[ViewVariables]
		[DataField("reacting", false, 1, false, false, null)]
		public bool Reacting { get; set; }

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x06002861 RID: 10337 RVA: 0x000D336A File Offset: 0x000D156A
		// (set) Token: 0x06002862 RID: 10338 RVA: 0x000D3372 File Offset: 0x000D1572
		[ViewVariables]
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName { get; set; } = "inlet";

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06002863 RID: 10339 RVA: 0x000D337B File Offset: 0x000D157B
		// (set) Token: 0x06002864 RID: 10340 RVA: 0x000D3383 File Offset: 0x000D1583
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName { get; set; } = "outlet";

		// Token: 0x04001916 RID: 6422
		[ViewVariables]
		public float MinTemp = 573.15f;

		// Token: 0x04001917 RID: 6423
		[DataField("BaseMinTemp", false, 1, false, false, null)]
		public float BaseMinTemp = 573.15f;

		// Token: 0x04001918 RID: 6424
		[DataField("machinePartMinTemp", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartMinTemp = "Laser";

		// Token: 0x04001919 RID: 6425
		[DataField("partRatingMinTempMultiplier", false, 1, false, false, null)]
		public float PartRatingMinTempMultiplier = 0.95f;

		// Token: 0x0400191A RID: 6426
		[ViewVariables]
		public float MinPressure = 3039.75f;

		// Token: 0x0400191B RID: 6427
		[DataField("BaseMinPressure", false, 1, false, false, null)]
		public float BaseMinPressure = 3039.75f;

		// Token: 0x0400191C RID: 6428
		[DataField("machinePartMinPressure", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartMinPressure = "Manipulator";

		// Token: 0x0400191D RID: 6429
		[DataField("partRatingMinPressureMultiplier", false, 1, false, false, null)]
		public float PartRatingMinPressureMultiplier = 0.8f;
	}
}
