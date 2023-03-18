using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Power.Components
{
	// Token: 0x020002B1 RID: 689
	[RegisterComponent]
	public sealed class CablePlacerComponent : Component
	{
		// Token: 0x04000839 RID: 2105
		[Nullable(2)]
		[DataField("cablePrototypeID", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string CablePrototypeId = "CableHV";

		// Token: 0x0400083A RID: 2106
		[DataField("blockingWireType", false, 1, false, false, null)]
		public CableType BlockingCableType;
	}
}
