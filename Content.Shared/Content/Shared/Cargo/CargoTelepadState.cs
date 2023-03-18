using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo
{
	// Token: 0x0200062A RID: 1578
	[NetSerializable]
	[Serializable]
	public enum CargoTelepadState : byte
	{
		// Token: 0x040012FE RID: 4862
		Unpowered,
		// Token: 0x040012FF RID: 4863
		Idle,
		// Token: 0x04001300 RID: 4864
		Teleporting
	}
}
