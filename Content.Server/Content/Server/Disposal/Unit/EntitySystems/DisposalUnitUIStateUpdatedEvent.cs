using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disposal.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Disposal.Unit.EntitySystems
{
	// Token: 0x0200054F RID: 1359
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DisposalUnitUIStateUpdatedEvent : EntityEventArgs
	{
		// Token: 0x06001CA1 RID: 7329 RVA: 0x000994DE File Offset: 0x000976DE
		public DisposalUnitUIStateUpdatedEvent(SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState state)
		{
			this.State = state;
		}

		// Token: 0x04001257 RID: 4695
		public SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState State;
	}
}
