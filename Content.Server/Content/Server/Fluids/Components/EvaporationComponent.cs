using System;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Fluids.Components
{
	// Token: 0x020004F5 RID: 1269
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(EvaporationSystem)
	})]
	public sealed class EvaporationComponent : Component
	{
		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06001A24 RID: 6692 RVA: 0x0008A047 File Offset: 0x00088247
		// (set) Token: 0x06001A25 RID: 6693 RVA: 0x0008A04F File Offset: 0x0008824F
		[DataField("evaporateTime", false, 1, false, false, null)]
		public float EvaporateTime { get; set; } = 5f;

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06001A26 RID: 6694 RVA: 0x0008A058 File Offset: 0x00088258
		// (set) Token: 0x06001A27 RID: 6695 RVA: 0x0008A060 File Offset: 0x00088260
		[DataField("solution", false, 1, false, false, null)]
		public string SolutionName { get; set; } = "puddle";

		// Token: 0x0400107A RID: 4218
		[DataField("evaporationToggle", false, 1, false, false, null)]
		public bool EvaporationToggle = true;

		// Token: 0x0400107D RID: 4221
		[DataField("lowerLimit", false, 1, false, false, null)]
		public FixedPoint2 LowerLimit = FixedPoint2.Zero;

		// Token: 0x0400107E RID: 4222
		[DataField("upperLimit", false, 1, false, false, null)]
		public FixedPoint2 UpperLimit = FixedPoint2.New(100);

		// Token: 0x0400107F RID: 4223
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;
	}
}
