using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Conveyor
{
	// Token: 0x0200055E RID: 1374
	[NetSerializable]
	[Serializable]
	public sealed class ConveyorComponentState : ComponentState
	{
		// Token: 0x060010AC RID: 4268 RVA: 0x00036425 File Offset: 0x00034625
		public ConveyorComponentState(Angle angle, float speed, ConveyorState state, bool powered)
		{
			this.Angle = angle;
			this.Speed = speed;
			this.State = state;
			this.Powered = powered;
		}

		// Token: 0x04000FA0 RID: 4000
		public bool Powered;

		// Token: 0x04000FA1 RID: 4001
		public Angle Angle;

		// Token: 0x04000FA2 RID: 4002
		public float Speed;

		// Token: 0x04000FA3 RID: 4003
		public ConveyorState State;
	}
}
