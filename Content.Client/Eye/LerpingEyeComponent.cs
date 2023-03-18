using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Eye
{
	// Token: 0x0200031B RID: 795
	[RegisterComponent]
	public sealed class LerpingEyeComponent : Component
	{
		// Token: 0x04000A10 RID: 2576
		public bool ManuallyAdded;

		// Token: 0x04000A11 RID: 2577
		[ViewVariables]
		public Angle LastRotation;

		// Token: 0x04000A12 RID: 2578
		[ViewVariables]
		public Angle TargetRotation;
	}
}
