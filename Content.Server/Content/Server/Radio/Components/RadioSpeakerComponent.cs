using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Radio.Components
{
	// Token: 0x02000261 RID: 609
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RadioDeviceSystem)
	})]
	public sealed class RadioSpeakerComponent : Component
	{
		// Token: 0x0400078C RID: 1932
		[Nullable(1)]
		[DataField("channels", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
		public HashSet<string> Channels = new HashSet<string>
		{
			"Common"
		};

		// Token: 0x0400078D RID: 1933
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled;
	}
}
