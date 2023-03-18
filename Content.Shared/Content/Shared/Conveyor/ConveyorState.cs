using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Conveyor
{
	// Token: 0x02000560 RID: 1376
	[NetSerializable]
	[Serializable]
	public enum ConveyorState : byte
	{
		// Token: 0x04000FA7 RID: 4007
		Off,
		// Token: 0x04000FA8 RID: 4008
		Forward,
		// Token: 0x04000FA9 RID: 4009
		Reverse
	}
}
