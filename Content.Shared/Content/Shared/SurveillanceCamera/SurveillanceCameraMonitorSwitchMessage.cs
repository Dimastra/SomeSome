using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera
{
	// Token: 0x020000F8 RID: 248
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SurveillanceCameraMonitorSwitchMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0000CC09 File Offset: 0x0000AE09
		public string Address { get; }

		// Token: 0x060002C3 RID: 707 RVA: 0x0000CC11 File Offset: 0x0000AE11
		public SurveillanceCameraMonitorSwitchMessage(string address)
		{
			this.Address = address;
		}
	}
}
