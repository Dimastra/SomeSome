using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F3 RID: 1267
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DoAfterComponentState : ComponentState
	{
		// Token: 0x06000F58 RID: 3928 RVA: 0x00031892 File Offset: 0x0002FA92
		public DoAfterComponentState(Dictionary<byte, DoAfter> doAfters)
		{
			this.DoAfters = doAfters;
		}

		// Token: 0x04000E96 RID: 3734
		public Dictionary<byte, DoAfter> DoAfters;
	}
}
