using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction
{
	// Token: 0x02000568 RID: 1384
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ConstructionGuide
	{
		// Token: 0x060010E2 RID: 4322 RVA: 0x00037C3A File Offset: 0x00035E3A
		public ConstructionGuide(ConstructionGuideEntry[] entries)
		{
			this.Entries = entries;
		}

		// Token: 0x04000FD2 RID: 4050
		public readonly ConstructionGuideEntry[] Entries;
	}
}
