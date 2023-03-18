using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.AME.Components
{
	// Token: 0x020007D8 RID: 2008
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AMEPartComponent : Component
	{
		// Token: 0x04001B18 RID: 6936
		[DataField("unwrapSound", false, 1, false, false, null)]
		public SoundSpecifier UnwrapSound = new SoundPathSpecifier("/Audio/Effects/unwrap.ogg", null);

		// Token: 0x04001B19 RID: 6937
		[DataField("qualityNeeded", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string QualityNeeded = "Pulsing";
	}
}
