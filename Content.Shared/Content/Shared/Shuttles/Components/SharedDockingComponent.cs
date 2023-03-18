using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Shuttles.Components
{
	// Token: 0x020001CD RID: 461
	public abstract class SharedDockingComponent : Component
	{
		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000542 RID: 1346
		public abstract bool Docked { get; }

		// Token: 0x04000536 RID: 1334
		[ViewVariables]
		public bool Enabled;
	}
}
