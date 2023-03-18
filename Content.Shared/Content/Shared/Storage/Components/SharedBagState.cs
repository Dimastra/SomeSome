using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000138 RID: 312
	[NetSerializable]
	[Serializable]
	public enum SharedBagState : byte
	{
		// Token: 0x040003B4 RID: 948
		Open,
		// Token: 0x040003B5 RID: 949
		Closed
	}
}
