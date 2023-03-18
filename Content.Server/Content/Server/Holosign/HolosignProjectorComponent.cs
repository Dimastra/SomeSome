using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Holosign
{
	// Token: 0x0200045E RID: 1118
	[RegisterComponent]
	public sealed class HolosignProjectorComponent : Component
	{
		// Token: 0x04000E17 RID: 3607
		[Nullable(1)]
		[ViewVariables]
		[DataField("signProto", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string SignProto = "HolosignWetFloor";

		// Token: 0x04000E18 RID: 3608
		[ViewVariables]
		[DataField("chargeUse", false, 1, false, false, null)]
		public float ChargeUse = 50f;
	}
}
