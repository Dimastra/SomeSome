using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x020003FC RID: 1020
	[RegisterComponent]
	public sealed class AutoLinkReceiverComponent : Component
	{
		// Token: 0x04000CDA RID: 3290
		[Nullable(1)]
		[DataField("channel", true, 1, true, false, null)]
		public string AutoLinkChannel;
	}
}
