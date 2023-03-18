using System;
using System.Runtime.CompilerServices;
using Content.Server.Animals.Systems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Animals.Components
{
	// Token: 0x020007D3 RID: 2003
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(UdderSystem)
	})]
	internal sealed class UdderComponent : Component
	{
		// Token: 0x04001AF5 RID: 6901
		[ViewVariables]
		[DataField("reagentId", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
		public string ReagentId = "Milk";

		// Token: 0x04001AF6 RID: 6902
		[ViewVariables]
		[DataField("targetSolution", false, 1, false, false, null)]
		public string TargetSolutionName = "udder";

		// Token: 0x04001AF7 RID: 6903
		[ViewVariables]
		[DataField("quantity", false, 1, false, false, null)]
		public FixedPoint2 QuantityPerUpdate = 1;

		// Token: 0x04001AF8 RID: 6904
		[ViewVariables]
		[DataField("updateRate", false, 1, false, false, null)]
		public float UpdateRate = 5f;

		// Token: 0x04001AF9 RID: 6905
		public float AccumulatedFrameTime;

		// Token: 0x04001AFA RID: 6906
		public bool BeingMilked;
	}
}
