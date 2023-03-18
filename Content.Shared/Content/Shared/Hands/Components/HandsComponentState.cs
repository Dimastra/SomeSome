using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands.Components
{
	// Token: 0x02000441 RID: 1089
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class HandsComponentState : ComponentState
	{
		// Token: 0x06000D38 RID: 3384 RVA: 0x0002BC94 File Offset: 0x00029E94
		public HandsComponentState(SharedHandsComponent handComp)
		{
			this.Hands = new List<Hand>(handComp.Hands.Values);
			this.HandNames = handComp.SortedHands;
			Hand activeHand = handComp.ActiveHand;
			this.ActiveHand = ((activeHand != null) ? activeHand.Name : null);
		}

		// Token: 0x04000CBE RID: 3262
		public readonly List<Hand> Hands;

		// Token: 0x04000CBF RID: 3263
		public readonly List<string> HandNames;

		// Token: 0x04000CC0 RID: 3264
		[Nullable(2)]
		public readonly string ActiveHand;
	}
}
