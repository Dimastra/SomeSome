using System;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera
{
	// Token: 0x02000103 RID: 259
	[NetSerializable]
	[Serializable]
	public enum SurveillanceCameraVisuals : byte
	{
		// Token: 0x04000323 RID: 803
		Active,
		// Token: 0x04000324 RID: 804
		InUse,
		// Token: 0x04000325 RID: 805
		Disabled,
		// Token: 0x04000326 RID: 806
		Xray,
		// Token: 0x04000327 RID: 807
		Emp
	}
}
