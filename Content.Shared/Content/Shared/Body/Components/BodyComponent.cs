using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Part;
using Content.Shared.Body.Prototypes;
using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Body.Components
{
	// Token: 0x0200066A RID: 1642
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedBodySystem)
	})]
	public sealed class BodyComponent : Component
	{
		// Token: 0x040013C2 RID: 5058
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<BodyPrototype>))]
		public readonly string Prototype;

		// Token: 0x040013C3 RID: 5059
		[DataField("root", false, 1, false, false, null)]
		public BodyPartSlot Root;

		// Token: 0x040013C4 RID: 5060
		[Nullable(1)]
		[DataField("gibSound", false, 1, false, false, null)]
		public SoundSpecifier GibSound = new SoundCollectionSpecifier("gib", null);

		// Token: 0x040013C5 RID: 5061
		[DataField("requiredLegs", false, 1, false, false, null)]
		public int RequiredLegs;
	}
}
