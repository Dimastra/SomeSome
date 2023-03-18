using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Drone
{
	// Token: 0x020004D4 RID: 1236
	public abstract class SharedDroneSystem : EntitySystem
	{
		// Token: 0x0200081A RID: 2074
		[NetSerializable]
		[Serializable]
		public enum DroneVisuals : byte
		{
			// Token: 0x040018DF RID: 6367
			Status
		}

		// Token: 0x0200081B RID: 2075
		[NetSerializable]
		[Serializable]
		public enum DroneStatus : byte
		{
			// Token: 0x040018E1 RID: 6369
			Off,
			// Token: 0x040018E2 RID: 6370
			On
		}
	}
}
