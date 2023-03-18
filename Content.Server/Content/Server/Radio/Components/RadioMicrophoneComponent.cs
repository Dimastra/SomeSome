using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Radio.Components
{
	// Token: 0x02000260 RID: 608
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RadioDeviceSystem)
	})]
	public sealed class RadioMicrophoneComponent : Component
	{
		// Token: 0x04000786 RID: 1926
		[Nullable(1)]
		[ViewVariables]
		[DataField("broadcastChannel", false, 1, false, false, typeof(PrototypeIdSerializer<RadioChannelPrototype>))]
		public string BroadcastChannel = "Common";

		// Token: 0x04000787 RID: 1927
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ViewVariables]
		[DataField("supportedChannels", false, 1, false, false, typeof(PrototypeIdListSerializer<RadioChannelPrototype>))]
		public List<string> SupportedChannels;

		// Token: 0x04000788 RID: 1928
		[ViewVariables]
		[DataField("listenRange", false, 1, false, false, null)]
		public int ListenRange = 4;

		// Token: 0x04000789 RID: 1929
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled;

		// Token: 0x0400078A RID: 1930
		[DataField("powerRequired", false, 1, false, false, null)]
		public bool PowerRequired;

		// Token: 0x0400078B RID: 1931
		[DataField("unobstructedRequired", false, 1, false, false, null)]
		public bool UnobstructedRequired;
	}
}
