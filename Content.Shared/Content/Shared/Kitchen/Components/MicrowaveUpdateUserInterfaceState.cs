using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components
{
	// Token: 0x0200039A RID: 922
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MicrowaveUpdateUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06000A93 RID: 2707 RVA: 0x00022A1B File Offset: 0x00020C1B
		public MicrowaveUpdateUserInterfaceState(EntityUid[] containedSolids, bool isMicrowaveBusy, int activeButtonIndex, uint currentCookTime)
		{
			this.ContainedSolids = containedSolids;
			this.IsMicrowaveBusy = isMicrowaveBusy;
			this.ActiveButtonIndex = activeButtonIndex;
			this.CurrentCookTime = currentCookTime;
		}

		// Token: 0x04000A85 RID: 2693
		public EntityUid[] ContainedSolids;

		// Token: 0x04000A86 RID: 2694
		public bool IsMicrowaveBusy;

		// Token: 0x04000A87 RID: 2695
		public int ActiveButtonIndex;

		// Token: 0x04000A88 RID: 2696
		public uint CurrentCookTime;
	}
}
