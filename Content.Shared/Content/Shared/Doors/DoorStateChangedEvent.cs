using System;
using Content.Shared.Doors.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Doors
{
	// Token: 0x020004E1 RID: 1249
	public sealed class DoorStateChangedEvent : EntityEventArgs
	{
		// Token: 0x06000F21 RID: 3873 RVA: 0x0003082B File Offset: 0x0002EA2B
		public DoorStateChangedEvent(DoorState state)
		{
			this.State = state;
		}

		// Token: 0x04000E31 RID: 3633
		public readonly DoorState State;
	}
}
