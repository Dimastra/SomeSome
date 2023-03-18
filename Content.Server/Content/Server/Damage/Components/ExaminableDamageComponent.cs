using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Damage.Components
{
	// Token: 0x020005D0 RID: 1488
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ExaminableDamageComponent : Component
	{
		// Token: 0x040013B1 RID: 5041
		[DataField("messages", false, 1, true, false, typeof(PrototypeIdSerializer<ExaminableDamagePrototype>))]
		public string MessagesProtoId;

		// Token: 0x040013B2 RID: 5042
		public ExaminableDamagePrototype MessagesProto;
	}
}
