using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera
{
	// Token: 0x02000100 RID: 256
	[NetSerializable]
	[Serializable]
	public sealed class SurveillanceCameraSetupSetNetwork : BoundUserInterfaceMessage
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x0000CCBB File Offset: 0x0000AEBB
		public int Network { get; }

		// Token: 0x060002D2 RID: 722 RVA: 0x0000CCC3 File Offset: 0x0000AEC3
		public SurveillanceCameraSetupSetNetwork(int network)
		{
			this.Network = network;
		}
	}
}
