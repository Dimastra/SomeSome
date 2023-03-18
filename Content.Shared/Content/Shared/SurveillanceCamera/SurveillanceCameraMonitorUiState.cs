using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera
{
	// Token: 0x020000F7 RID: 247
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SurveillanceCameraMonitorUiState : BoundUserInterfaceState
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060002BD RID: 701 RVA: 0x0000CBBC File Offset: 0x0000ADBC
		public EntityUid? ActiveCamera { get; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060002BE RID: 702 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
		public HashSet<string> Subnets { get; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060002BF RID: 703 RVA: 0x0000CBCC File Offset: 0x0000ADCC
		public string ActiveSubnet { get; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x0000CBD4 File Offset: 0x0000ADD4
		public Dictionary<string, string> Cameras { get; }

		// Token: 0x060002C1 RID: 705 RVA: 0x0000CBDC File Offset: 0x0000ADDC
		public SurveillanceCameraMonitorUiState(EntityUid? activeCamera, HashSet<string> subnets, string activeAddress, string activeSubnet, Dictionary<string, string> cameras)
		{
			this.ActiveCamera = activeCamera;
			this.Subnets = subnets;
			this.ActiveAddress = activeAddress;
			this.ActiveSubnet = activeSubnet;
			this.Cameras = cameras;
		}

		// Token: 0x0400030E RID: 782
		public string ActiveAddress;
	}
}
