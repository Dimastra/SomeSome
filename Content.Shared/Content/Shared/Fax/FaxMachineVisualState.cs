using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax
{
	// Token: 0x0200048F RID: 1167
	[NetSerializable]
	[Serializable]
	public enum FaxMachineVisualState : byte
	{
		// Token: 0x04000D59 RID: 3417
		Normal,
		// Token: 0x04000D5A RID: 3418
		Inserting,
		// Token: 0x04000D5B RID: 3419
		Printing
	}
}
