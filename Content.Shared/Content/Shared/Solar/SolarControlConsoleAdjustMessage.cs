using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Solar
{
	// Token: 0x0200018F RID: 399
	[NetSerializable]
	[Serializable]
	public sealed class SolarControlConsoleAdjustMessage : BoundUserInterfaceMessage
	{
		// Token: 0x04000460 RID: 1120
		public Angle Rotation;

		// Token: 0x04000461 RID: 1121
		public Angle AngularVelocity;
	}
}
