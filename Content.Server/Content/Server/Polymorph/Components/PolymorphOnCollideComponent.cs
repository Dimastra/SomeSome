using System;
using System.Runtime.CompilerServices;
using Content.Shared.Polymorph;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Polymorph.Components
{
	// Token: 0x020002CA RID: 714
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PolymorphOnCollideComponent : Component
	{
		// Token: 0x04000877 RID: 2167
		[DataField("polymorph", false, 1, true, false, typeof(PrototypeIdSerializer<PolymorphPrototype>))]
		public string Polymorph;

		// Token: 0x04000878 RID: 2168
		[DataField("whitelist", false, 1, true, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000879 RID: 2169
		[Nullable(2)]
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist;

		// Token: 0x0400087A RID: 2170
		public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Magic/forcewall.ogg", null);
	}
}
