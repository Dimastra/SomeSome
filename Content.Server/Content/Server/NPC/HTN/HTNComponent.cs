using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.NPC.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.HTN
{
	// Token: 0x02000344 RID: 836
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(NPCComponent))]
	public sealed class HTNComponent : NPCComponent
	{
		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06001185 RID: 4485 RVA: 0x0005C589 File Offset: 0x0005A789
		[ViewVariables]
		public bool Planning
		{
			get
			{
				return this.PlanningJob != null;
			}
		}

		// Token: 0x04000A81 RID: 2689
		[Nullable(1)]
		[ViewVariables]
		[DataField("rootTask", false, 1, true, false, typeof(PrototypeIdSerializer<HTNCompoundTask>))]
		public string RootTask;

		// Token: 0x04000A82 RID: 2690
		[ViewVariables]
		public HTNPlan Plan;

		// Token: 0x04000A83 RID: 2691
		[ViewVariables]
		[DataField("planCooldown", false, 1, false, false, null)]
		public float PlanCooldown = 0.45f;

		// Token: 0x04000A84 RID: 2692
		[ViewVariables]
		public float PlanAccumulator;

		// Token: 0x04000A85 RID: 2693
		[ViewVariables]
		public HTNPlanJob PlanningJob;

		// Token: 0x04000A86 RID: 2694
		[ViewVariables]
		public CancellationTokenSource PlanningToken;
	}
}
