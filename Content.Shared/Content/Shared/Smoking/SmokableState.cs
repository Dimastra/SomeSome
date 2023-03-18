using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Smoking
{
	// Token: 0x02000191 RID: 401
	[NetSerializable]
	[Serializable]
	public enum SmokableState : byte
	{
		// Token: 0x04000465 RID: 1125
		Unlit,
		// Token: 0x04000466 RID: 1126
		Lit,
		// Token: 0x04000467 RID: 1127
		Burnt
	}
}
