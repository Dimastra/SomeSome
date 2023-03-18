using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Pulling.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Pulling.Systems
{
	// Token: 0x02000238 RID: 568
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SharedPullableSystem : EntitySystem
	{
		// Token: 0x0600065B RID: 1627 RVA: 0x00016DB1 File Offset: 0x00014FB1
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedPullableComponent, MoveInputEvent>(new ComponentEventRefHandler<SharedPullableComponent, MoveInputEvent>(this.OnRelayMoveInput), null, null);
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x00016DD0 File Offset: 0x00014FD0
		private void OnRelayMoveInput(EntityUid uid, SharedPullableComponent component, ref MoveInputEvent args)
		{
			EntityUid entity = args.Entity;
			if (this._mobState.IsIncapacitated(entity, null) || !this._blocker.CanMove(entity, null))
			{
				return;
			}
			this._pullSystem.TryStopPull(component, null);
		}

		// Token: 0x0400065E RID: 1630
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x0400065F RID: 1631
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000660 RID: 1632
		[Dependency]
		private readonly SharedPullingSystem _pullSystem;
	}
}
