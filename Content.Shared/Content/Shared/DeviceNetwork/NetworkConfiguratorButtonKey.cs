using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x02000513 RID: 1299
	[NetSerializable]
	[Serializable]
	public enum NetworkConfiguratorButtonKey
	{
		// Token: 0x04000EFD RID: 3837
		Set,
		// Token: 0x04000EFE RID: 3838
		Add,
		// Token: 0x04000EFF RID: 3839
		Edit,
		// Token: 0x04000F00 RID: 3840
		Clear,
		// Token: 0x04000F01 RID: 3841
		Copy,
		// Token: 0x04000F02 RID: 3842
		Show
	}
}
