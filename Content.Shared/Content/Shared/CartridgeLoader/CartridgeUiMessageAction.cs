using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000617 RID: 1559
	[NetSerializable]
	[Serializable]
	public enum CartridgeUiMessageAction
	{
		// Token: 0x040012D6 RID: 4822
		Activate,
		// Token: 0x040012D7 RID: 4823
		Deactivate,
		// Token: 0x040012D8 RID: 4824
		Install,
		// Token: 0x040012D9 RID: 4825
		Uninstall,
		// Token: 0x040012DA RID: 4826
		UIReady
	}
}
