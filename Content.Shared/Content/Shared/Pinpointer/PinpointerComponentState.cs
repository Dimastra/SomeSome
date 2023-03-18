using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer
{
	// Token: 0x02000274 RID: 628
	[NetSerializable]
	[Serializable]
	public sealed class PinpointerComponentState : ComponentState
	{
		// Token: 0x04000714 RID: 1812
		public bool IsActive;

		// Token: 0x04000715 RID: 1813
		public Angle ArrowAngle;

		// Token: 0x04000716 RID: 1814
		public Distance DistanceToTarget;
	}
}
