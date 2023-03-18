using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Borgs
{
	// Token: 0x02000650 RID: 1616
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class LawsComponentState : ComponentState
	{
		// Token: 0x0600137B RID: 4987 RVA: 0x00040A3E File Offset: 0x0003EC3E
		public LawsComponentState(List<string> laws)
		{
			this.Laws = laws;
		}

		// Token: 0x0400137E RID: 4990
		public readonly List<string> Laws;
	}
}
