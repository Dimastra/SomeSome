using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Body.Components
{
	// Token: 0x02000718 RID: 1816
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StomachSystem)
	})]
	public sealed class StomachComponent : Component
	{
		// Token: 0x040017BA RID: 6074
		public float AccumulatedFrameTime;

		// Token: 0x040017BB RID: 6075
		[DataField("updateInterval", false, 1, false, false, null)]
		public float UpdateInterval = 1f;

		// Token: 0x040017BC RID: 6076
		[DataField("bodySolutionName", false, 1, false, false, null)]
		public string BodySolutionName = BloodstreamComponent.DefaultChemicalsSolutionName;

		// Token: 0x040017BD RID: 6077
		[DataField("initialMaxVolume", true, 1, false, false, null)]
		public readonly FixedPoint2 InitialMaxVolume = FixedPoint2.New(50);

		// Token: 0x040017BE RID: 6078
		[DataField("digestionDelay", false, 1, false, false, null)]
		public float DigestionDelay = 20f;

		// Token: 0x040017BF RID: 6079
		[ViewVariables]
		public readonly List<StomachComponent.ReagentDelta> ReagentDeltas = new List<StomachComponent.ReagentDelta>();

		// Token: 0x02000B0D RID: 2829
		[Nullable(0)]
		public sealed class ReagentDelta
		{
			// Token: 0x17000870 RID: 2160
			// (get) Token: 0x060036DE RID: 14046 RVA: 0x00122214 File Offset: 0x00120414
			// (set) Token: 0x060036DF RID: 14047 RVA: 0x0012221C File Offset: 0x0012041C
			public float Lifetime { get; private set; }

			// Token: 0x060036E0 RID: 14048 RVA: 0x00122225 File Offset: 0x00120425
			public ReagentDelta(string reagentId, FixedPoint2 quantity)
			{
				this.ReagentId = reagentId;
				this.Quantity = quantity;
				this.Lifetime = 0f;
			}

			// Token: 0x060036E1 RID: 14049 RVA: 0x00122246 File Offset: 0x00120446
			public void Increment(float delta)
			{
				this.Lifetime += delta;
			}

			// Token: 0x040028D1 RID: 10449
			public readonly string ReagentId;

			// Token: 0x040028D2 RID: 10450
			public readonly FixedPoint2 Quantity;
		}
	}
}
