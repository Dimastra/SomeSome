using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.PowerCell;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Power;
using Content.Shared.PowerCell.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200028F RID: 655
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ChargerSystem : EntitySystem
	{
		// Token: 0x06000D22 RID: 3362 RVA: 0x00044D68 File Offset: 0x00042F68
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ChargerComponent, ComponentStartup>(new ComponentEventHandler<ChargerComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<ChargerComponent, PowerChangedEvent>(new ComponentEventRefHandler<ChargerComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<ChargerComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ChargerComponent, EntInsertedIntoContainerMessage>(this.OnInserted), null, null);
			base.SubscribeLocalEvent<ChargerComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ChargerComponent, EntRemovedFromContainerMessage>(this.OnRemoved), null, null);
			base.SubscribeLocalEvent<ChargerComponent, ContainerIsInsertingAttemptEvent>(new ComponentEventHandler<ChargerComponent, ContainerIsInsertingAttemptEvent>(this.OnInsertAttempt), null, null);
			base.SubscribeLocalEvent<ChargerComponent, ExaminedEvent>(new ComponentEventHandler<ChargerComponent, ExaminedEvent>(this.OnChargerExamine), null, null);
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x00044DED File Offset: 0x00042FED
		private void OnStartup(EntityUid uid, ChargerComponent component, ComponentStartup args)
		{
			this.UpdateStatus(uid, component);
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x00044DF8 File Offset: 0x00042FF8
		private void OnChargerExamine(EntityUid uid, ChargerComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("charger-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", "yellow"),
				new ValueTuple<string, object>("chargeRate", component.ChargeRate)
			}));
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x00044E50 File Offset: 0x00043050
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<ActiveChargerComponent, ChargerComponent, ItemSlotsComponent> valueTuple in this.EntityManager.EntityQuery<ActiveChargerComponent, ChargerComponent, ItemSlotsComponent>(false))
			{
				ChargerComponent charger = valueTuple.Item2;
				ItemSlotsComponent slotComp = valueTuple.Item3;
				ItemSlot slot;
				if (this._itemSlotsSystem.TryGetSlot(charger.Owner, charger.SlotId, out slot, slotComp) && charger.Status != CellChargerStatus.Empty && charger.Status != CellChargerStatus.Charged && slot.HasItem)
				{
					this.TransferPower(charger.Owner, slot.Item.Value, charger, frameTime);
				}
			}
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x00044EFC File Offset: 0x000430FC
		private void OnPowerChanged(EntityUid uid, ChargerComponent component, ref PowerChangedEvent args)
		{
			this.UpdateStatus(uid, component);
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x00044F06 File Offset: 0x00043106
		private void OnInserted(EntityUid uid, ChargerComponent component, EntInsertedIntoContainerMessage args)
		{
			if (!component.Initialized)
			{
				return;
			}
			if (args.Container.ID != component.SlotId)
			{
				return;
			}
			this.UpdateStatus(uid, component);
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x00044F32 File Offset: 0x00043132
		private void OnRemoved(EntityUid uid, ChargerComponent component, EntRemovedFromContainerMessage args)
		{
			if (args.Container.ID != component.SlotId)
			{
				return;
			}
			this.UpdateStatus(uid, component);
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x00044F58 File Offset: 0x00043158
		private void OnInsertAttempt(EntityUid uid, ChargerComponent component, ContainerIsInsertingAttemptEvent args)
		{
			if (!component.Initialized)
			{
				return;
			}
			if (args.Container.ID != component.SlotId)
			{
				return;
			}
			PowerCellSlotComponent cellSlot;
			if (!base.TryComp<PowerCellSlotComponent>(args.EntityUid, ref cellSlot))
			{
				return;
			}
			ItemSlot itemSlot;
			if (!this._itemSlotsSystem.TryGetSlot(args.EntityUid, cellSlot.CellSlotId, out itemSlot, null))
			{
				return;
			}
			if (!cellSlot.FitsInCharger || !itemSlot.HasItem)
			{
				args.Cancel();
			}
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x00044FCC File Offset: 0x000431CC
		private void UpdateStatus(EntityUid uid, ChargerComponent component)
		{
			CellChargerStatus status = this.GetStatus(uid, component);
			ApcPowerReceiverComponent receiver;
			if (component.Status == status || !base.TryComp<ApcPowerReceiverComponent>(uid, ref receiver))
			{
				return;
			}
			ItemSlot slot;
			if (!this._itemSlotsSystem.TryGetSlot(uid, component.SlotId, out slot, null))
			{
				return;
			}
			AppearanceComponent appearance;
			base.TryComp<AppearanceComponent>(uid, ref appearance);
			component.Status = status;
			if (component.Status == CellChargerStatus.Charging)
			{
				base.AddComp<ActiveChargerComponent>(uid);
			}
			else
			{
				base.RemComp<ActiveChargerComponent>(uid);
			}
			switch (component.Status)
			{
			case CellChargerStatus.Off:
				receiver.Load = 0f;
				this._sharedAppearanceSystem.SetData(uid, CellVisual.Light, CellChargerStatus.Off, appearance);
				break;
			case CellChargerStatus.Empty:
				receiver.Load = 0f;
				this._sharedAppearanceSystem.SetData(uid, CellVisual.Light, CellChargerStatus.Empty, appearance);
				break;
			case CellChargerStatus.Charging:
				receiver.Load = (float)component.ChargeRate;
				this._sharedAppearanceSystem.SetData(uid, CellVisual.Light, CellChargerStatus.Charging, appearance);
				break;
			case CellChargerStatus.Charged:
				receiver.Load = 0f;
				this._sharedAppearanceSystem.SetData(uid, CellVisual.Light, CellChargerStatus.Charged, appearance);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this._sharedAppearanceSystem.SetData(uid, CellVisual.Occupied, slot.HasItem, appearance);
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x00045120 File Offset: 0x00043320
		private CellChargerStatus GetStatus(EntityUid uid, ChargerComponent component)
		{
			TransformComponent transformComponent;
			if (!base.TryComp<TransformComponent>(uid, ref transformComponent))
			{
				return CellChargerStatus.Off;
			}
			if (!transformComponent.Anchored)
			{
				return CellChargerStatus.Off;
			}
			ApcPowerReceiverComponent apcPowerReceiverComponent;
			if (!base.TryComp<ApcPowerReceiverComponent>(uid, ref apcPowerReceiverComponent))
			{
				return CellChargerStatus.Off;
			}
			if (!apcPowerReceiverComponent.Powered)
			{
				return CellChargerStatus.Off;
			}
			ItemSlot slot;
			if (!this._itemSlotsSystem.TryGetSlot(uid, component.SlotId, out slot, null))
			{
				return CellChargerStatus.Off;
			}
			if (!slot.HasItem)
			{
				return CellChargerStatus.Empty;
			}
			BatteryComponent heldBattery;
			if (!this.SearchForBattery(slot.Item.Value, out heldBattery))
			{
				return CellChargerStatus.Off;
			}
			if (heldBattery != null && (double)Math.Abs(heldBattery.MaxCharge - heldBattery.CurrentCharge) < 0.01)
			{
				return CellChargerStatus.Charged;
			}
			return CellChargerStatus.Charging;
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x000451C0 File Offset: 0x000433C0
		private void TransferPower(EntityUid uid, EntityUid targetEntity, ChargerComponent component, float frameTime)
		{
			ApcPowerReceiverComponent receiverComponent;
			if (!base.TryComp<ApcPowerReceiverComponent>(uid, ref receiverComponent))
			{
				return;
			}
			if (!receiverComponent.Powered)
			{
				return;
			}
			BatteryComponent heldBattery;
			if (!this.SearchForBattery(targetEntity, out heldBattery))
			{
				return;
			}
			heldBattery.CurrentCharge += (float)component.ChargeRate * frameTime;
			if ((double)(heldBattery.MaxCharge - heldBattery.CurrentCharge) < 0.01)
			{
				heldBattery.CurrentCharge = heldBattery.MaxCharge;
			}
			this.UpdateStatus(uid, component);
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x00045232 File Offset: 0x00043432
		[NullableContext(2)]
		private bool SearchForBattery(EntityUid uid, [NotNullWhen(true)] out BatteryComponent component)
		{
			return base.TryComp<BatteryComponent>(uid, ref component) || this._cellSystem.TryGetBatteryFromSlot(uid, out component, null);
		}

		// Token: 0x040007F1 RID: 2033
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x040007F2 RID: 2034
		[Dependency]
		private readonly PowerCellSystem _cellSystem;

		// Token: 0x040007F3 RID: 2035
		[Dependency]
		private readonly SharedAppearanceSystem _sharedAppearanceSystem;
	}
}
