using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Disposal.Unit.EntitySystems
{
	// Token: 0x02000550 RID: 1360
	public sealed class BeforeDisposalFlushEvent : CancellableEntityEventArgs
	{
		// Token: 0x04001258 RID: 4696
		[Nullable(1)]
		public readonly List<string> Tags = new List<string>();
	}
}
