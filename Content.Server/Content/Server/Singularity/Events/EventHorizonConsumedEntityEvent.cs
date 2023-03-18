using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Server.Singularity.Events
{
	// Token: 0x020001E5 RID: 485
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EventHorizonConsumedEntityEvent : EntityEventArgs
	{
		// Token: 0x0600092A RID: 2346 RVA: 0x0002E1FF File Offset: 0x0002C3FF
		public EventHorizonConsumedEntityEvent(EntityUid entity, EventHorizonComponent eventHorizon, [Nullable(2)] IContainer container = null)
		{
			this.Entity = entity;
			this.EventHorizon = eventHorizon;
			this.Container = container;
		}

		// Token: 0x04000592 RID: 1426
		public readonly EntityUid Entity;

		// Token: 0x04000593 RID: 1427
		public readonly EventHorizonComponent EventHorizon;

		// Token: 0x04000594 RID: 1428
		[Nullable(2)]
		public readonly IContainer Container;
	}
}
