using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Cloning.Components
{
	// Token: 0x02000643 RID: 1603
	[RegisterComponent]
	public sealed class BeingClonedComponent : Component
	{
		// Token: 0x040014EE RID: 5358
		[Nullable(2)]
		[ViewVariables]
		public Mind Mind;

		// Token: 0x040014EF RID: 5359
		[ViewVariables]
		public EntityUid Parent;
	}
}
