using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Radio.Components
{
	// Token: 0x0200025F RID: 607
	[RegisterComponent]
	public sealed class IntrinsicRadioTransmitterComponent : Component
	{
		// Token: 0x04000785 RID: 1925
		[Nullable(1)]
		[DataField("channels", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
		public readonly HashSet<string> Channels = new HashSet<string>
		{
			"Common"
		};
	}
}
