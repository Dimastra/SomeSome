using System;
using Content.Server.Singularity.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.Components
{
	// Token: 0x020001EF RID: 495
	[RegisterComponent]
	public sealed class GravityWellComponent : Component
	{
		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060009A6 RID: 2470 RVA: 0x00031093 File Offset: 0x0002F293
		// (set) Token: 0x060009A7 RID: 2471 RVA: 0x0003109B File Offset: 0x0002F29B
		[DataField("gravPulsePeriod", false, 1, false, false, null)]
		[ViewVariables]
		[Access(new Type[]
		{
			typeof(GravityWellSystem)
		})]
		public TimeSpan TargetPulsePeriod { get; internal set; } = TimeSpan.FromSeconds(0.5);

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060009A8 RID: 2472 RVA: 0x000310A4 File Offset: 0x0002F2A4
		// (set) Token: 0x060009A9 RID: 2473 RVA: 0x000310AC File Offset: 0x0002F2AC
		[ViewVariables]
		[Access(new Type[]
		{
			typeof(GravityWellSystem)
		})]
		public TimeSpan NextPulseTime { get; internal set; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060009AA RID: 2474 RVA: 0x000310B5 File Offset: 0x0002F2B5
		// (set) Token: 0x060009AB RID: 2475 RVA: 0x000310BD File Offset: 0x0002F2BD
		[ViewVariables]
		[Access(new Type[]
		{
			typeof(GravityWellSystem)
		})]
		public TimeSpan LastPulseTime { get; internal set; }

		// Token: 0x040005C1 RID: 1473
		[DataField("maxRange", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxRange;

		// Token: 0x040005C2 RID: 1474
		[DataField("minRange", false, 1, false, false, null)]
		[ViewVariables]
		public float MinRange;

		// Token: 0x040005C3 RID: 1475
		[DataField("baseRadialAcceleration", false, 1, false, false, null)]
		[ViewVariables]
		public float BaseRadialAcceleration;

		// Token: 0x040005C4 RID: 1476
		[DataField("baseTangentialAcceleration", false, 1, false, false, null)]
		[ViewVariables]
		public float BaseTangentialAcceleration;
	}
}
