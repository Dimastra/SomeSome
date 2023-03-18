using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Body.Part
{
	// Token: 0x0200065E RID: 1630
	[NetSerializable]
	[Serializable]
	public enum BodyPartType
	{
		// Token: 0x040013A9 RID: 5033
		Other,
		// Token: 0x040013AA RID: 5034
		Torso,
		// Token: 0x040013AB RID: 5035
		Head,
		// Token: 0x040013AC RID: 5036
		Arm,
		// Token: 0x040013AD RID: 5037
		Hand,
		// Token: 0x040013AE RID: 5038
		Leg,
		// Token: 0x040013AF RID: 5039
		Foot,
		// Token: 0x040013B0 RID: 5040
		Tail
	}
}
