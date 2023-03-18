using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x020003FD RID: 1021
	[RegisterComponent]
	public sealed class AutoLinkTransmitterComponent : Component
	{
		// Token: 0x04000CDB RID: 3291
		[Nullable(1)]
		[DataField("channel", true, 1, true, false, null)]
		public string AutoLinkChannel;
	}
}
