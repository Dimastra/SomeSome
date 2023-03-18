using System;
using System.Runtime.CompilerServices;
using Content.Shared.NameIdentifier;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.NameIdentifier
{
	// Token: 0x0200038A RID: 906
	[RegisterComponent]
	public sealed class NameIdentifierComponent : Component
	{
		// Token: 0x04000B6C RID: 2924
		[Nullable(1)]
		[DataField("group", false, 1, true, false, typeof(PrototypeIdSerializer<NameIdentifierGroupPrototype>))]
		public string Group = string.Empty;
	}
}
