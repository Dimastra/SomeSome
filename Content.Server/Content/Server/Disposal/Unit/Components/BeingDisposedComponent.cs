using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disposal.Unit.Components
{
	// Token: 0x02000553 RID: 1363
	[RegisterComponent]
	public sealed class BeingDisposedComponent : Component
	{
		// Token: 0x0400125C RID: 4700
		[ViewVariables]
		public EntityUid Holder;
	}
}
