using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Implants.Components
{
	// Token: 0x020003F4 RID: 1012
	[NetSerializable]
	[Serializable]
	public enum ImplanterToggleMode : byte
	{
		// Token: 0x04000BD9 RID: 3033
		Inject,
		// Token: 0x04000BDA RID: 3034
		Draw
	}
}
