using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x0200043B RID: 1083
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedHandVirtualItemSystem : EntitySystem
	{
		// Token: 0x06000CEF RID: 3311 RVA: 0x0002A5FD File Offset: 0x000287FD
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HandVirtualItemComponent, BeingEquippedAttemptEvent>(new ComponentEventHandler<HandVirtualItemComponent, BeingEquippedAttemptEvent>(this.OnBeingEquippedAttempt), null, null);
			ComponentEventHandler<HandVirtualItemComponent, BeforeRangedInteractEvent> componentEventHandler;
			if ((componentEventHandler = SharedHandVirtualItemSystem.<>O.<0>__HandleBeforeInteract) == null)
			{
				componentEventHandler = (SharedHandVirtualItemSystem.<>O.<0>__HandleBeforeInteract = new ComponentEventHandler<HandVirtualItemComponent, BeforeRangedInteractEvent>(SharedHandVirtualItemSystem.HandleBeforeInteract));
			}
			base.SubscribeLocalEvent<HandVirtualItemComponent, BeforeRangedInteractEvent>(componentEventHandler, null, null);
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x0002A63C File Offset: 0x0002883C
		private void OnBeingEquippedAttempt(EntityUid uid, HandVirtualItemComponent component, BeingEquippedAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x0002A644 File Offset: 0x00028844
		private static void HandleBeforeInteract(EntityUid uid, HandVirtualItemComponent component, BeforeRangedInteractEvent args)
		{
			args.Handled = true;
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x0002A650 File Offset: 0x00028850
		public void Delete(HandVirtualItemComponent comp, EntityUid user)
		{
			VirtualItemDeletedEvent userEv = new VirtualItemDeletedEvent(comp.BlockingEntity, user);
			base.RaiseLocalEvent<VirtualItemDeletedEvent>(user, userEv, false);
			VirtualItemDeletedEvent targEv = new VirtualItemDeletedEvent(comp.BlockingEntity, user);
			base.RaiseLocalEvent<VirtualItemDeletedEvent>(comp.BlockingEntity, targEv, false);
			this.EntityManager.QueueDeleteEntity(comp.Owner);
		}

		// Token: 0x020007FD RID: 2045
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04001886 RID: 6278
			[Nullable(new byte[]
			{
				0,
				1,
				1
			})]
			public static ComponentEventHandler<HandVirtualItemComponent, BeforeRangedInteractEvent> <0>__HandleBeforeInteract;
		}
	}
}
