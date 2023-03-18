using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.PowerCell.Components;
using Content.Shared.Rejuvenate;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.PowerCell
{
	// Token: 0x02000251 RID: 593
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedPowerCellSystem : EntitySystem
	{
		// Token: 0x060006E9 RID: 1769 RVA: 0x00018258 File Offset: 0x00016458
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PowerCellSlotComponent, RejuvenateEvent>(new ComponentEventHandler<PowerCellSlotComponent, RejuvenateEvent>(this.OnRejuventate), null, null);
			base.SubscribeLocalEvent<PowerCellSlotComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<PowerCellSlotComponent, EntInsertedIntoContainerMessage>(this.OnCellInserted), null, null);
			base.SubscribeLocalEvent<PowerCellSlotComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PowerCellSlotComponent, EntRemovedFromContainerMessage>(this.OnCellRemoved), null, null);
			base.SubscribeLocalEvent<PowerCellSlotComponent, ContainerIsInsertingAttemptEvent>(new ComponentEventHandler<PowerCellSlotComponent, ContainerIsInsertingAttemptEvent>(this.OnCellInsertAttempt), null, null);
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x000182BC File Offset: 0x000164BC
		private void OnRejuventate(EntityUid uid, PowerCellSlotComponent component, RejuvenateEvent args)
		{
			ItemSlot itemSlot;
			if (!this._itemSlots.TryGetSlot(uid, component.CellSlotId, out itemSlot, null) || itemSlot.Item == null)
			{
				return;
			}
			base.RaiseLocalEvent<RejuvenateEvent>(itemSlot.Item.Value, args, false);
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x00018307 File Offset: 0x00016507
		private void OnCellInsertAttempt(EntityUid uid, PowerCellSlotComponent component, ContainerIsInsertingAttemptEvent args)
		{
			if (!component.Initialized)
			{
				return;
			}
			if (args.Container.ID != component.CellSlotId)
			{
				return;
			}
			if (!base.HasComp<PowerCellComponent>(args.EntityUid))
			{
				args.Cancel();
			}
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0001833F File Offset: 0x0001653F
		private void OnCellInserted(EntityUid uid, PowerCellSlotComponent component, EntInsertedIntoContainerMessage args)
		{
			if (!component.Initialized)
			{
				return;
			}
			if (args.Container.ID != component.CellSlotId)
			{
				return;
			}
			base.RaiseLocalEvent<PowerCellChangedEvent>(uid, new PowerCellChangedEvent(false), false);
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x00018371 File Offset: 0x00016571
		private void OnCellRemoved(EntityUid uid, PowerCellSlotComponent component, EntRemovedFromContainerMessage args)
		{
			if (args.Container.ID != component.CellSlotId)
			{
				return;
			}
			base.RaiseLocalEvent<PowerCellChangedEvent>(uid, new PowerCellChangedEvent(true), false);
		}

		// Token: 0x040006AB RID: 1707
		[Dependency]
		private readonly ItemSlotsSystem _itemSlots;
	}
}
