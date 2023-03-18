using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Chemistry.Components
{
	// Token: 0x020003E1 RID: 993
	[RegisterComponent]
	public sealed class HyposprayComponent : SharedHyposprayComponent
	{
		// Token: 0x04000C71 RID: 3185
		[ViewVariables]
		public FixedPoint2 CurrentVolume;

		// Token: 0x04000C72 RID: 3186
		[ViewVariables]
		public FixedPoint2 TotalVolume;

		// Token: 0x04000C73 RID: 3187
		[ViewVariables]
		public bool UiUpdateNeeded;
	}
}
