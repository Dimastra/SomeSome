using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax
{
	// Token: 0x02000491 RID: 1169
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class FaxUiState : BoundUserInterfaceState
	{
		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06000E39 RID: 3641 RVA: 0x0002DC5C File Offset: 0x0002BE5C
		public string DeviceName { get; }

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06000E3A RID: 3642 RVA: 0x0002DC64 File Offset: 0x0002BE64
		public Dictionary<string, string> AvailablePeers { get; }

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06000E3B RID: 3643 RVA: 0x0002DC6C File Offset: 0x0002BE6C
		[Nullable(2)]
		public string DestinationAddress { [NullableContext(2)] get; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06000E3C RID: 3644 RVA: 0x0002DC74 File Offset: 0x0002BE74
		public bool IsPaperInserted { get; }

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06000E3D RID: 3645 RVA: 0x0002DC7C File Offset: 0x0002BE7C
		public bool CanSend { get; }

		// Token: 0x06000E3E RID: 3646 RVA: 0x0002DC84 File Offset: 0x0002BE84
		public FaxUiState(string deviceName, Dictionary<string, string> peers, bool canSend, bool isPaperInserted, [Nullable(2)] string destAddress)
		{
			this.DeviceName = deviceName;
			this.AvailablePeers = peers;
			this.IsPaperInserted = isPaperInserted;
			this.CanSend = canSend;
			this.DestinationAddress = destAddress;
		}
	}
}
