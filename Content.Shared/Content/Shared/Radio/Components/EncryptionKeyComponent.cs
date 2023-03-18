using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Radio.Components
{
	// Token: 0x02000223 RID: 547
	[RegisterComponent]
	public sealed class EncryptionKeyComponent : Component
	{
		// Token: 0x04000613 RID: 1555
		[Nullable(1)]
		[DataField("channels", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
		public HashSet<string> Channels = new HashSet<string>();

		// Token: 0x04000614 RID: 1556
		[Nullable(2)]
		[DataField("defaultChannel", false, 1, false, false, typeof(PrototypeIdSerializer<RadioChannelPrototype>))]
		public readonly string DefaultChannel;
	}
}
