using System;
using System.Runtime.CompilerServices;
using Content.Shared.CartridgeLoader;
using Robust.Shared.Serialization;

namespace Content.Shared.PDA
{
	// Token: 0x02000288 RID: 648
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class PDAUpdateState : CartridgeLoaderUiState
	{
		// Token: 0x06000757 RID: 1879 RVA: 0x00018FC2 File Offset: 0x000171C2
		public PDAUpdateState(bool flashlightEnabled, bool hasPen, PDAIdInfoText pdaOwnerInfo, string stationName, bool hasUplink = false, bool canPlayMusic = false, string address = null)
		{
			this.FlashlightEnabled = flashlightEnabled;
			this.HasPen = hasPen;
			this.PDAOwnerInfo = pdaOwnerInfo;
			this.HasUplink = hasUplink;
			this.CanPlayMusic = canPlayMusic;
			this.StationName = stationName;
			this.Address = address;
		}

		// Token: 0x0400075E RID: 1886
		public bool FlashlightEnabled;

		// Token: 0x0400075F RID: 1887
		public bool HasPen;

		// Token: 0x04000760 RID: 1888
		public PDAIdInfoText PDAOwnerInfo;

		// Token: 0x04000761 RID: 1889
		public string StationName;

		// Token: 0x04000762 RID: 1890
		public bool HasUplink;

		// Token: 0x04000763 RID: 1891
		public bool CanPlayMusic;

		// Token: 0x04000764 RID: 1892
		public string Address;
	}
}
