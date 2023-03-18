using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Doors
{
	// Token: 0x020004E3 RID: 1251
	public sealed class BeforeDoorClosedEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000F23 RID: 3875 RVA: 0x00030842 File Offset: 0x0002EA42
		public BeforeDoorClosedEvent(bool performCollisionCheck)
		{
			this.PerformCollisionCheck = performCollisionCheck;
		}

		// Token: 0x04000E32 RID: 3634
		public bool PerformCollisionCheck;
	}
}
