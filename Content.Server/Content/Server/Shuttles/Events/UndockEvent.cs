using System;
using System.Runtime.CompilerServices;
using Content.Server.Shuttles.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Shuttles.Events
{
	// Token: 0x02000200 RID: 512
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UndockEvent : EntityEventArgs
	{
		// Token: 0x04000636 RID: 1590
		public DockingComponent DockA;

		// Token: 0x04000637 RID: 1591
		public DockingComponent DockB;

		// Token: 0x04000638 RID: 1592
		public EntityUid GridAUid;

		// Token: 0x04000639 RID: 1593
		public EntityUid GridBUid;
	}
}
