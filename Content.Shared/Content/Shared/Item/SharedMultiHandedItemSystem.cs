using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Shared.Item
{
	// Token: 0x020003A9 RID: 937
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedMultiHandedItemSystem : EntitySystem
	{
		// Token: 0x06000ABA RID: 2746 RVA: 0x00022FE4 File Offset: 0x000211E4
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MultiHandedItemComponent, GettingPickedUpAttemptEvent>(new ComponentEventHandler<MultiHandedItemComponent, GettingPickedUpAttemptEvent>(this.OnAttemptPickup), null, null);
			base.SubscribeLocalEvent<MultiHandedItemComponent, VirtualItemDeletedEvent>(new ComponentEventHandler<MultiHandedItemComponent, VirtualItemDeletedEvent>(this.OnVirtualItemDeleted), null, null);
			base.SubscribeLocalEvent<MultiHandedItemComponent, GotEquippedHandEvent>(new ComponentEventHandler<MultiHandedItemComponent, GotEquippedHandEvent>(this.OnEquipped), null, null);
			base.SubscribeLocalEvent<MultiHandedItemComponent, GotUnequippedHandEvent>(new ComponentEventHandler<MultiHandedItemComponent, GotUnequippedHandEvent>(this.OnUnequipped), null, null);
		}

		// Token: 0x06000ABB RID: 2747
		protected abstract void OnEquipped(EntityUid uid, MultiHandedItemComponent component, GotEquippedHandEvent args);

		// Token: 0x06000ABC RID: 2748
		protected abstract void OnUnequipped(EntityUid uid, MultiHandedItemComponent component, GotUnequippedHandEvent args);

		// Token: 0x06000ABD RID: 2749 RVA: 0x00023044 File Offset: 0x00021244
		private void OnAttemptPickup(EntityUid uid, MultiHandedItemComponent component, GettingPickedUpAttemptEvent args)
		{
			SharedHandsComponent hands;
			if (base.TryComp<SharedHandsComponent>(args.User, ref hands) && hands.CountFreeHands() >= component.HandsNeeded)
			{
				return;
			}
			args.Cancel();
			if (this._timing.IsFirstTimePredicted)
			{
				this._popup.PopupCursor(Loc.GetString("multi-handed-item-pick-up-fail", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("number", component.HandsNeeded - 1),
					new ValueTuple<string, object>("item", uid)
				}), args.User, PopupType.Small);
			}
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x000230DC File Offset: 0x000212DC
		private void OnVirtualItemDeleted(EntityUid uid, MultiHandedItemComponent component, VirtualItemDeletedEvent args)
		{
			if (args.BlockingEntity != uid)
			{
				return;
			}
			this._hands.TryDrop(args.User, uid, null, true, true, null);
		}

		// Token: 0x04000AB0 RID: 2736
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000AB1 RID: 2737
		[Dependency]
		private readonly SharedHandsSystem _hands;

		// Token: 0x04000AB2 RID: 2738
		[Dependency]
		private readonly SharedPopupSystem _popup;
	}
}
