using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Teleportation.Components
{
	// Token: 0x020000E1 RID: 225
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class HandTeleporterComponent : Component
	{
		// Token: 0x040002DD RID: 733
		[ViewVariables]
		[DataField("firstPortal", false, 1, false, false, null)]
		public EntityUid? FirstPortal;

		// Token: 0x040002DE RID: 734
		[ViewVariables]
		[DataField("secondPortal", false, 1, false, false, null)]
		public EntityUid? SecondPortal;

		// Token: 0x040002DF RID: 735
		[DataField("firstPortalPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string FirstPortalPrototype = "PortalRed";

		// Token: 0x040002E0 RID: 736
		[DataField("secondPortalPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string SecondPortalPrototype = "PortalBlue";

		// Token: 0x040002E1 RID: 737
		[DataField("newPortalSound", false, 1, false, false, null)]
		public SoundSpecifier NewPortalSound = new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(-2f)
		};

		// Token: 0x040002E2 RID: 738
		[DataField("clearPortalsSound", false, 1, false, false, null)]
		public SoundSpecifier ClearPortalsSound = new SoundPathSpecifier("/Audio/Machines/button.ogg", null);

		// Token: 0x040002E3 RID: 739
		[DataField("portalCreationDelay", false, 1, false, false, null)]
		public float PortalCreationDelay = 2.5f;
	}
}
