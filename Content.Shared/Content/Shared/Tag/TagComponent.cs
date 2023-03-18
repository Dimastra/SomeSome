using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Tag
{
	// Token: 0x020000E9 RID: 233
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(TagSystem)
	})]
	public sealed class TagComponent : Component
	{
		// Token: 0x040002F2 RID: 754
		[Nullable(1)]
		[DataField("tags", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<TagPrototype>))]
		[Access]
		public readonly HashSet<string> Tags = new HashSet<string>();
	}
}
