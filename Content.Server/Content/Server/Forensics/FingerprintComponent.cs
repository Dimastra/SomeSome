using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Forensics
{
	// Token: 0x020004E1 RID: 1249
	[RegisterComponent]
	public sealed class FingerprintComponent : Component
	{
		// Token: 0x0400102A RID: 4138
		[Nullable(2)]
		[DataField("fingerprint", false, 1, false, false, null)]
		public string Fingerprint;
	}
}
