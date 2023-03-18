using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Singularity.Events
{
	// Token: 0x020001E4 RID: 484
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EventHorizonAttemptConsumeEntityEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000929 RID: 2345 RVA: 0x0002E1E9 File Offset: 0x0002C3E9
		public EventHorizonAttemptConsumeEntityEvent(EntityUid entity, EventHorizonComponent eventHorizon)
		{
			this.Entity = entity;
			this.EventHorizon = eventHorizon;
		}

		// Token: 0x04000590 RID: 1424
		public readonly EntityUid Entity;

		// Token: 0x04000591 RID: 1425
		public readonly EventHorizonComponent EventHorizon;
	}
}
