using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Forensics
{
	// Token: 0x02000470 RID: 1136
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ForensicScannerBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06000DCB RID: 3531 RVA: 0x0002D064 File Offset: 0x0002B264
		public ForensicScannerBoundUserInterfaceState(List<string> fingerprints, List<string> fibers, string lastScannedName, TimeSpan printCooldown, TimeSpan printReadyAt)
		{
			this.Fingerprints = fingerprints;
			this.Fibers = fibers;
			this.LastScannedName = lastScannedName;
			this.PrintCooldown = printCooldown;
			this.PrintReadyAt = printReadyAt;
		}

		// Token: 0x04000D1F RID: 3359
		public readonly List<string> Fingerprints = new List<string>();

		// Token: 0x04000D20 RID: 3360
		public readonly List<string> Fibers = new List<string>();

		// Token: 0x04000D21 RID: 3361
		public readonly string LastScannedName = string.Empty;

		// Token: 0x04000D22 RID: 3362
		public readonly TimeSpan PrintCooldown = TimeSpan.Zero;

		// Token: 0x04000D23 RID: 3363
		public readonly TimeSpan PrintReadyAt = TimeSpan.Zero;
	}
}
