using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Blocking
{
	// Token: 0x02000670 RID: 1648
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BlockingComponent : Component
	{
		// Token: 0x040013D5 RID: 5077
		[ViewVariables]
		public EntityUid? User;

		// Token: 0x040013D6 RID: 5078
		[ViewVariables]
		public bool IsBlocking;

		// Token: 0x040013D7 RID: 5079
		public const string BlockFixtureID = "blocking-active";

		// Token: 0x040013D8 RID: 5080
		[ViewVariables]
		[DataField("shape", false, 1, false, false, null)]
		public IPhysShape Shape = new PhysShapeCircle(0.5f);

		// Token: 0x040013D9 RID: 5081
		[ViewVariables]
		[DataField("passiveBlockModifier", false, 1, false, false, null)]
		public string PassiveBlockDamageModifer = "Metallic";

		// Token: 0x040013DA RID: 5082
		[ViewVariables]
		[DataField("activeBlockModifier", false, 1, false, false, null)]
		public string ActiveBlockDamageModifier = "Metallic";

		// Token: 0x040013DB RID: 5083
		[DataField("blockingToggleActionId", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string BlockingToggleActionId = "ToggleBlock";

		// Token: 0x040013DC RID: 5084
		[Nullable(2)]
		[DataField("blockingToggleAction", false, 1, false, false, null)]
		public InstantAction BlockingToggleAction;

		// Token: 0x040013DD RID: 5085
		[DataField("blockSound", false, 1, false, false, null)]
		public SoundSpecifier BlockSound = new SoundPathSpecifier("/Audio/Weapons/block_metal1.ogg", null);
	}
}
