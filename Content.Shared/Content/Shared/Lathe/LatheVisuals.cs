using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe
{
	// Token: 0x0200037F RID: 895
	[NetSerializable]
	[Serializable]
	public enum LatheVisuals : byte
	{
		// Token: 0x04000A55 RID: 2645
		IsRunning,
		// Token: 0x04000A56 RID: 2646
		IsInserting,
		// Token: 0x04000A57 RID: 2647
		InsertingColor
	}
}
