using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Holiday.Christmas
{
	// Token: 0x02000471 RID: 1137
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RandomGiftSystem)
	})]
	public sealed class RandomGiftComponent : Component
	{
		// Token: 0x04000E45 RID: 3653
		[DataField("wrapper", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Wrapper;

		// Token: 0x04000E46 RID: 3654
		[DataField("sound", false, 1, true, false, null)]
		public SoundSpecifier Sound;

		// Token: 0x04000E47 RID: 3655
		[DataField("insaneMode", false, 1, true, false, null)]
		[ViewVariables]
		public bool InsaneMode;

		// Token: 0x04000E48 RID: 3656
		[Nullable(1)]
		[DataField("contentsViewers", false, 1, true, false, null)]
		public EntityWhitelist ContentsViewers;

		// Token: 0x04000E49 RID: 3657
		[DataField("selectedEntity", false, 1, false, false, null)]
		[ViewVariables]
		public string SelectedEntity;
	}
}
