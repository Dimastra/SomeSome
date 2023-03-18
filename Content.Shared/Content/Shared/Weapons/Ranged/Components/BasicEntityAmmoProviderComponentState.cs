using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x0200005E RID: 94
	[NetSerializable]
	[Serializable]
	public sealed class BasicEntityAmmoProviderComponentState : ComponentState
	{
		// Token: 0x06000133 RID: 307 RVA: 0x00006E2A File Offset: 0x0000502A
		public BasicEntityAmmoProviderComponentState(int? capacity, int? count)
		{
			this.Capacity = capacity;
			this.Count = count;
		}

		// Token: 0x04000120 RID: 288
		public int? Capacity;

		// Token: 0x04000121 RID: 289
		public int? Count;
	}
}
