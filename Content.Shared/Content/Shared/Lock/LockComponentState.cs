using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Lock
{
	// Token: 0x02000359 RID: 857
	[NetSerializable]
	[Serializable]
	public sealed class LockComponentState : ComponentState
	{
		// Token: 0x060009F8 RID: 2552 RVA: 0x00020840 File Offset: 0x0001EA40
		public LockComponentState(bool locked, bool lockOnClick)
		{
			this.Locked = locked;
			this.LockOnClick = lockOnClick;
		}

		// Token: 0x040009C4 RID: 2500
		public bool Locked;

		// Token: 0x040009C5 RID: 2501
		public bool LockOnClick;
	}
}
