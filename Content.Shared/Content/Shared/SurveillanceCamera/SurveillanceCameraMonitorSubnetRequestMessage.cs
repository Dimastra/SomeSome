using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera
{
	// Token: 0x020000F9 RID: 249
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SurveillanceCameraMonitorSubnetRequestMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x0000CC20 File Offset: 0x0000AE20
		public string Subnet { get; }

		// Token: 0x060002C5 RID: 709 RVA: 0x0000CC28 File Offset: 0x0000AE28
		public SurveillanceCameraMonitorSubnetRequestMessage(string subnet)
		{
			this.Subnet = subnet;
		}
	}
}
