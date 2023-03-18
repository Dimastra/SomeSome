using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Revenant.Components
{
	// Token: 0x020001FA RID: 506
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class RevenantOverloadedLightsComponent : Component
	{
		// Token: 0x040005BE RID: 1470
		[ViewVariables]
		public EntityUid? Target;

		// Token: 0x040005BF RID: 1471
		[ViewVariables]
		public float Accumulator;

		// Token: 0x040005C0 RID: 1472
		[ViewVariables]
		public float ZapDelay = 3f;

		// Token: 0x040005C1 RID: 1473
		[ViewVariables]
		public float ZapRange = 4f;

		// Token: 0x040005C2 RID: 1474
		[Nullable(1)]
		[DataField("zapBeamEntityId", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ZapBeamEntityId = "LightningRevenant";

		// Token: 0x040005C3 RID: 1475
		public float? OriginalEnergy;

		// Token: 0x040005C4 RID: 1476
		public bool OriginalEnabled;
	}
}
