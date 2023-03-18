using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen
{
	// Token: 0x02000393 RID: 915
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ReagentGrinderInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06000A8C RID: 2700 RVA: 0x00022962 File Offset: 0x00020B62
		public ReagentGrinderInterfaceState(bool isBusy, bool hasBeaker, bool powered, bool canJuice, bool canGrind, EntityUid[] chamberContents, [Nullable(2)] Solution.ReagentQuantity[] heldBeakerContents)
		{
			this.IsBusy = isBusy;
			this.HasBeakerIn = hasBeaker;
			this.Powered = powered;
			this.CanJuice = canJuice;
			this.CanGrind = canGrind;
			this.ChamberContents = chamberContents;
			this.ReagentQuantities = heldBeakerContents;
		}

		// Token: 0x04000A78 RID: 2680
		public bool IsBusy;

		// Token: 0x04000A79 RID: 2681
		public bool HasBeakerIn;

		// Token: 0x04000A7A RID: 2682
		public bool Powered;

		// Token: 0x04000A7B RID: 2683
		public bool CanJuice;

		// Token: 0x04000A7C RID: 2684
		public bool CanGrind;

		// Token: 0x04000A7D RID: 2685
		public EntityUid[] ChamberContents;

		// Token: 0x04000A7E RID: 2686
		[Nullable(2)]
		public Solution.ReagentQuantity[] ReagentQuantities;
	}
}
