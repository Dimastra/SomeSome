using System;
using System.Runtime.CompilerServices;
using Content.Server.Shuttles.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Shuttles.Events
{
	// Token: 0x020001FD RID: 509
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DockEvent : EntityEventArgs
	{
		// Token: 0x04000632 RID: 1586
		public DockingComponent DockA;

		// Token: 0x04000633 RID: 1587
		public DockingComponent DockB;

		// Token: 0x04000634 RID: 1588
		public EntityUid GridAUid;

		// Token: 0x04000635 RID: 1589
		public EntityUid GridBUid;
	}
}
