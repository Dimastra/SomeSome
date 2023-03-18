using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Borgs
{
	// Token: 0x0200064C RID: 1612
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class LawsUpdateState : BoundUserInterfaceState
	{
		// Token: 0x06001377 RID: 4983 RVA: 0x000409F1 File Offset: 0x0003EBF1
		public LawsUpdateState(HashSet<string> laws)
		{
			this.Laws = laws;
		}

		// Token: 0x04001379 RID: 4985
		public HashSet<string> Laws;
	}
}
