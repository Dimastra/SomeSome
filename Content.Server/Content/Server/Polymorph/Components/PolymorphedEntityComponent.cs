using System;
using System.Runtime.CompilerServices;
using Content.Shared.Polymorph;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Polymorph.Components
{
	// Token: 0x020002C9 RID: 713
	[RegisterComponent]
	public sealed class PolymorphedEntityComponent : Component
	{
		// Token: 0x04000874 RID: 2164
		[Nullable(1)]
		[DataField("prototype", false, 1, true, false, typeof(PrototypeIdSerializer<PolymorphPrototype>))]
		public string Prototype = string.Empty;

		// Token: 0x04000875 RID: 2165
		[DataField("parent", false, 1, true, false, null)]
		public EntityUid Parent;

		// Token: 0x04000876 RID: 2166
		[DataField("time", false, 1, false, false, null)]
		public float Time;
	}
}
