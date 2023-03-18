using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000614 RID: 1556
	[NetSerializable]
	[Serializable]
	public enum InstallationStatus
	{
		// Token: 0x040012C8 RID: 4808
		Cartridge,
		// Token: 0x040012C9 RID: 4809
		Installed,
		// Token: 0x040012CA RID: 4810
		Readonly
	}
}
