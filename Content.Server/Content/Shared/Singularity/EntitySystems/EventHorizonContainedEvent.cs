using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Shared.Singularity.EntitySystems
{
	// Token: 0x02000015 RID: 21
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EventHorizonContainedEvent : EntityEventArgs
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00002D28 File Offset: 0x00000F28
		public EventHorizonContainedEvent(EntityUid entity, EventHorizonComponent eventHorizon, EntGotInsertedIntoContainerMessage args)
		{
			this.Entity = entity;
			this.EventHorizon = eventHorizon;
			this.Args = args;
		}

		// Token: 0x04000022 RID: 34
		public readonly EntityUid Entity;

		// Token: 0x04000023 RID: 35
		public readonly EventHorizonComponent EventHorizon;

		// Token: 0x04000024 RID: 36
		public readonly EntGotInsertedIntoContainerMessage Args;
	}
}
