using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Chemistry.Components
{
	// Token: 0x020003E2 RID: 994
	[RegisterComponent]
	public sealed class InjectorComponent : SharedInjectorComponent
	{
		// Token: 0x04000C74 RID: 3188
		[ViewVariables]
		public FixedPoint2 CurrentVolume;

		// Token: 0x04000C75 RID: 3189
		[ViewVariables]
		public FixedPoint2 TotalVolume;

		// Token: 0x04000C76 RID: 3190
		[ViewVariables]
		public SharedInjectorComponent.InjectorToggleMode CurrentMode;

		// Token: 0x04000C77 RID: 3191
		[ViewVariables]
		public bool UiUpdateNeeded;
	}
}
