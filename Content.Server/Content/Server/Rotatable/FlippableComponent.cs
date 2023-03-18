using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Rotatable
{
	// Token: 0x02000226 RID: 550
	[RegisterComponent]
	public sealed class FlippableComponent : Component
	{
		// Token: 0x040006C6 RID: 1734
		[Nullable(1)]
		[DataField("mirrorEntity", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string MirrorEntity;
	}
}
