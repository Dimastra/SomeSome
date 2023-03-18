using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Components
{
	// Token: 0x020004EF RID: 1263
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DoorComponentState : ComponentState
	{
		// Token: 0x06000F53 RID: 3923 RVA: 0x0003172E File Offset: 0x0002F92E
		public DoorComponentState(DoorComponent door)
		{
			this.DoorState = door.State;
			this.CurrentlyCrushing = door.CurrentlyCrushing;
			this.NextStateChange = door.NextStateChange;
			this.Partial = door.Partial;
		}

		// Token: 0x04000E80 RID: 3712
		public readonly DoorState DoorState;

		// Token: 0x04000E81 RID: 3713
		public readonly HashSet<EntityUid> CurrentlyCrushing;

		// Token: 0x04000E82 RID: 3714
		public readonly TimeSpan? NextStateChange;

		// Token: 0x04000E83 RID: 3715
		public readonly bool Partial;
	}
}
