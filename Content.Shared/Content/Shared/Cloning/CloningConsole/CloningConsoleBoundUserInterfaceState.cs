using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning.CloningConsole
{
	// Token: 0x020005BD RID: 1469
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CloningConsoleBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x060011D9 RID: 4569 RVA: 0x0003A908 File Offset: 0x00038B08
		public CloningConsoleBoundUserInterfaceState(string scannerBodyInfo, string cloningBodyInfo, bool mindPresent, ClonerStatus cloningStatus, bool scannerConnected, bool scannerInRange, bool clonerConnected, bool clonerInRange)
		{
			this.ScannerBodyInfo = scannerBodyInfo;
			this.ClonerBodyInfo = cloningBodyInfo;
			this.MindPresent = mindPresent;
			this.CloningStatus = cloningStatus;
			this.ScannerConnected = scannerConnected;
			this.ScannerInRange = scannerInRange;
			this.ClonerConnected = clonerConnected;
			this.ClonerInRange = clonerInRange;
		}

		// Token: 0x04001090 RID: 4240
		public readonly string ScannerBodyInfo;

		// Token: 0x04001091 RID: 4241
		public readonly string ClonerBodyInfo;

		// Token: 0x04001092 RID: 4242
		public readonly bool MindPresent;

		// Token: 0x04001093 RID: 4243
		public readonly ClonerStatus CloningStatus;

		// Token: 0x04001094 RID: 4244
		public readonly bool ScannerConnected;

		// Token: 0x04001095 RID: 4245
		public readonly bool ScannerInRange;

		// Token: 0x04001096 RID: 4246
		public readonly bool ClonerConnected;

		// Token: 0x04001097 RID: 4247
		public readonly bool ClonerInRange;
	}
}
