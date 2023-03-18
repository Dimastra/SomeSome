using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Flash
{
	// Token: 0x02000489 RID: 1161
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class FlashableComponent : Component
	{
		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x0002D6EC File Offset: 0x0002B8EC
		public override bool SendOnlyToOwner
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04000D4D RID: 3405
		public float Duration;

		// Token: 0x04000D4E RID: 3406
		public TimeSpan LastFlash;
	}
}
