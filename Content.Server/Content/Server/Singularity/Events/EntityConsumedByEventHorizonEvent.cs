using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Server.Singularity.Events
{
	// Token: 0x020001E3 RID: 483
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntityConsumedByEventHorizonEvent : EntityEventArgs
	{
		// Token: 0x06000928 RID: 2344 RVA: 0x0002E1CC File Offset: 0x0002C3CC
		public EntityConsumedByEventHorizonEvent(EntityUid entity, EventHorizonComponent eventHorizon, [Nullable(2)] IContainer container = null)
		{
			this.Entity = entity;
			this.EventHorizon = eventHorizon;
			this.Container = container;
		}

		// Token: 0x0400058D RID: 1421
		public readonly EntityUid Entity;

		// Token: 0x0400058E RID: 1422
		public readonly EventHorizonComponent EventHorizon;

		// Token: 0x0400058F RID: 1423
		[Nullable(2)]
		public readonly IContainer Container;
	}
}
