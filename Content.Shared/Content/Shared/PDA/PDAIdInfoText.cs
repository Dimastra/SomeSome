using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.PDA
{
	// Token: 0x02000289 RID: 649
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public struct PDAIdInfoText
	{
		// Token: 0x04000765 RID: 1893
		public string ActualOwnerName;

		// Token: 0x04000766 RID: 1894
		public string IdOwner;

		// Token: 0x04000767 RID: 1895
		public string JobTitle;
	}
}
