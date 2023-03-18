using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Forensics
{
	// Token: 0x020004E3 RID: 1251
	[RegisterComponent]
	public sealed class ForensicPadComponent : Component
	{
		// Token: 0x0400102B RID: 4139
		[DataField("scanDelay", false, 1, false, false, null)]
		public float ScanDelay = 3f;

		// Token: 0x0400102C RID: 4140
		public bool Used;

		// Token: 0x0400102D RID: 4141
		[Nullable(1)]
		public string Sample = string.Empty;
	}
}
