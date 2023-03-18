using System;
using Content.Shared.Pulling;
using Content.Shared.Pulling.Components;
using Robust.Client.Physics;
using Robust.Shared.GameObjects;

namespace Content.Client.Pulling
{
	// Token: 0x0200017E RID: 382
	public sealed class PullingSystem : SharedPullingSystem
	{
		// Token: 0x060009F1 RID: 2545 RVA: 0x00039ED0 File Offset: 0x000380D0
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesAfter.Add(typeof(PhysicsSystem));
			base.SubscribeLocalEvent<SharedPullableComponent, PullableMoveMessage>(new ComponentEventHandler<SharedPullableComponent, PullableMoveMessage>(base.OnPullableMove), null, null);
			base.SubscribeLocalEvent<SharedPullableComponent, PullableStopMovingMessage>(new ComponentEventHandler<SharedPullableComponent, PullableStopMovingMessage>(base.OnPullableStopMove), null, null);
		}
	}
}
