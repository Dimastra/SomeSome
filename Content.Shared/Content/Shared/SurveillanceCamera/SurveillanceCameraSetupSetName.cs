using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera
{
	// Token: 0x020000FF RID: 255
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SurveillanceCameraSetupSetName : BoundUserInterfaceMessage
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060002CF RID: 719 RVA: 0x0000CCA4 File Offset: 0x0000AEA4
		public string Name { get; }

		// Token: 0x060002D0 RID: 720 RVA: 0x0000CCAC File Offset: 0x0000AEAC
		public SurveillanceCameraSetupSetName(string name)
		{
			this.Name = name;
		}
	}
}
