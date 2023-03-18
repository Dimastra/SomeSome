using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Payload.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Payload.EntitySystems
{
	// Token: 0x02000294 RID: 660
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChemicalPayloadSystem : EntitySystem
	{
		// Token: 0x06000765 RID: 1893 RVA: 0x000191DC File Offset: 0x000173DC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ChemicalPayloadComponent, ComponentInit>(new ComponentEventHandler<ChemicalPayloadComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<ChemicalPayloadComponent, ComponentRemove>(new ComponentEventHandler<ChemicalPayloadComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<ChemicalPayloadComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ChemicalPayloadComponent, EntInsertedIntoContainerMessage>(this.OnContainerModified), null, null);
			base.SubscribeLocalEvent<ChemicalPayloadComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ChemicalPayloadComponent, EntRemovedFromContainerMessage>(this.OnContainerModified), null, null);
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x0001923F File Offset: 0x0001743F
		private void OnContainerModified(EntityUid uid, ChemicalPayloadComponent component, ContainerModifiedMessage args)
		{
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x0001924C File Offset: 0x0001744C
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, ChemicalPayloadComponent component = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<ChemicalPayloadComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
			{
				return;
			}
			ChemicalPayloadFilledSlots filled = ChemicalPayloadFilledSlots.None;
			if (component.BeakerSlotA.HasItem)
			{
				filled |= ChemicalPayloadFilledSlots.Left;
			}
			if (component.BeakerSlotB.HasItem)
			{
				filled |= ChemicalPayloadFilledSlots.Right;
			}
			this._appearance.SetData(uid, ChemicalPayloadVisuals.Slots, filled, appearance);
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x000192A5 File Offset: 0x000174A5
		private void OnComponentInit(EntityUid uid, ChemicalPayloadComponent payload, ComponentInit args)
		{
			this._itemSlotsSystem.AddItemSlot(uid, "BeakerSlotA", payload.BeakerSlotA, null);
			this._itemSlotsSystem.AddItemSlot(uid, "BeakerSlotB", payload.BeakerSlotB, null);
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x000192D7 File Offset: 0x000174D7
		private void OnComponentRemove(EntityUid uid, ChemicalPayloadComponent payload, ComponentRemove args)
		{
			this._itemSlotsSystem.RemoveItemSlot(uid, payload.BeakerSlotA, null);
			this._itemSlotsSystem.RemoveItemSlot(uid, payload.BeakerSlotB, null);
		}

		// Token: 0x04000783 RID: 1923
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x04000784 RID: 1924
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
