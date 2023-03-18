using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001A5 RID: 421
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class EventHorizonComponent : Component
	{
		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000500 RID: 1280 RVA: 0x0001336B File Offset: 0x0001156B
		// (set) Token: 0x06000501 RID: 1281 RVA: 0x00013373 File Offset: 0x00011573
		[DataField("consumePeriod", false, 1, false, false, null)]
		[ViewVariables]
		[Access(new Type[]
		{
			typeof(SharedEventHorizonSystem)
		})]
		public TimeSpan TargetConsumePeriod { get; set; } = TimeSpan.FromSeconds(0.5);

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000502 RID: 1282 RVA: 0x0001337C File Offset: 0x0001157C
		// (set) Token: 0x06000503 RID: 1283 RVA: 0x00013384 File Offset: 0x00011584
		[ViewVariables]
		[Access(new Type[]
		{
			typeof(SharedEventHorizonSystem)
		})]
		public TimeSpan LastConsumeWaveTime { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x0001338D File Offset: 0x0001158D
		// (set) Token: 0x06000505 RID: 1285 RVA: 0x00013395 File Offset: 0x00011595
		[ViewVariables]
		[Access(new Type[]
		{
			typeof(SharedEventHorizonSystem)
		})]
		public TimeSpan NextConsumeWaveTime { get; set; }

		// Token: 0x040004AC RID: 1196
		[DataField("radius", false, 1, false, false, null)]
		[Access(new Type[]
		{
			typeof(SharedEventHorizonSystem)
		})]
		public float Radius;

		// Token: 0x040004AD RID: 1197
		[DataField("canBreachContainment", false, 1, false, false, null)]
		[Access(new Type[]
		{
			typeof(SharedEventHorizonSystem)
		})]
		public bool CanBreachContainment;

		// Token: 0x040004AE RID: 1198
		[DataField("wasDetectedInBreach", false, 1, false, false, null)]
		[ViewVariables]
		public bool WasDetectedInBreach;

		// Token: 0x040004AF RID: 1199
		[Nullable(2)]
		[DataField("horizonFixtureId", false, 1, false, false, null)]
		[Access(new Type[]
		{
			typeof(SharedEventHorizonSystem)
		})]
		public string HorizonFixtureId = "EventHorizon";

		// Token: 0x040004B0 RID: 1200
		[ViewVariables]
		public bool BeingConsumedByAnotherEventHorizon;
	}
}
