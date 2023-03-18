using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.VoiceMask
{
	// Token: 0x020000CA RID: 202
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class VoiceMaskComponent : Component
	{
		// Token: 0x04000237 RID: 567
		[ViewVariables]
		public bool Enabled = true;

		// Token: 0x04000238 RID: 568
		[ViewVariables]
		public string VoiceName = "Unknown";

		// Token: 0x04000239 RID: 569
		[ViewVariables]
		public string VoiceId = "Garithos";
	}
}
