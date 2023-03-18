using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.BarSign
{
	// Token: 0x0200067D RID: 1661
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class BarSignComponentState : ComponentState
	{
		// Token: 0x06001450 RID: 5200 RVA: 0x00044050 File Offset: 0x00042250
		public BarSignComponentState(string current)
		{
			this.CurrentSign = current;
		}

		// Token: 0x040013F9 RID: 5113
		public string CurrentSign;
	}
}
