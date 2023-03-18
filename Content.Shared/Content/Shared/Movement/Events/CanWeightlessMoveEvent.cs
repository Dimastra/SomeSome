using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events
{
	// Token: 0x020002E5 RID: 741
	[ByRefEvent]
	public struct CanWeightlessMoveEvent
	{
		// Token: 0x06000858 RID: 2136 RVA: 0x0001C8A9 File Offset: 0x0001AAA9
		public CanWeightlessMoveEvent()
		{
			this.CanMove = false;
		}

		// Token: 0x04000869 RID: 2153
		public bool CanMove;
	}
}
