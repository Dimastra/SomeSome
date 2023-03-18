using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Nuke
{
	// Token: 0x02000327 RID: 807
	[RegisterComponent]
	public sealed class NukeLabelComponent : Component
	{
		// Token: 0x040009E4 RID: 2532
		[Nullable(1)]
		[DataField("prefix", false, 1, false, false, null)]
		public string NukeLabel = "nuke-label-nanotrasen";

		// Token: 0x040009E5 RID: 2533
		[DataField("serialLength", false, 1, false, false, null)]
		public int SerialLength = 6;
	}
}
