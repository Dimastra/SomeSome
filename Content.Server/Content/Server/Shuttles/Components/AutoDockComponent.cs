using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x02000201 RID: 513
	[RegisterComponent]
	public sealed class AutoDockComponent : Component
	{
		// Token: 0x0400063A RID: 1594
		[Nullable(1)]
		public HashSet<EntityUid> Requesters = new HashSet<EntityUid>();
	}
}
