using System;
using System.Runtime.CompilerServices;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ghost
{
	// Token: 0x02000451 RID: 1105
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedGhostSystem : EntitySystem
	{
		// Token: 0x06000D77 RID: 3447 RVA: 0x0002C7CC File Offset: 0x0002A9CC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedGhostComponent, UseAttemptEvent>(new ComponentEventHandler<SharedGhostComponent, UseAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<SharedGhostComponent, InteractionAttemptEvent>(new ComponentEventHandler<SharedGhostComponent, InteractionAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<SharedGhostComponent, EmoteAttemptEvent>(new ComponentEventHandler<SharedGhostComponent, EmoteAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<SharedGhostComponent, DropAttemptEvent>(new ComponentEventHandler<SharedGhostComponent, DropAttemptEvent>(this.OnAttempt), null, null);
			base.SubscribeLocalEvent<SharedGhostComponent, PickupAttemptEvent>(new ComponentEventHandler<SharedGhostComponent, PickupAttemptEvent>(this.OnAttempt), null, null);
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x0002C843 File Offset: 0x0002AA43
		private void OnAttempt(EntityUid uid, SharedGhostComponent component, CancellableEntityEventArgs args)
		{
			if (!component.CanGhostInteract)
			{
				args.Cancel();
			}
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x0002C853 File Offset: 0x0002AA53
		public void SetCanReturnToBody(SharedGhostComponent component, bool value)
		{
			component.CanReturnToBody = value;
		}
	}
}
