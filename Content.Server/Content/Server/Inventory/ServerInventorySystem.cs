using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Clothing.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Inventory
{
	// Token: 0x02000444 RID: 1092
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerInventorySystem : InventorySystem
	{
		// Token: 0x06001606 RID: 5638 RVA: 0x0007460A File Offset: 0x0007280A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ClothingComponent, UseInHandEvent>(new ComponentEventHandler<ClothingComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeNetworkEvent<OpenSlotStorageNetworkMessage>(new EntitySessionEventHandler<OpenSlotStorageNetworkMessage>(this.OnOpenSlotStorage), null, null);
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x0007463A File Offset: 0x0007283A
		private void OnUseInHand(EntityUid uid, ClothingComponent component, UseInHandEvent args)
		{
			if (args.Handled || !component.QuickEquip)
			{
				return;
			}
			base.QuickEquip(uid, component, args);
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x00074658 File Offset: 0x00072858
		private void OnOpenSlotStorage(OpenSlotStorageNetworkMessage ev, EntitySessionEventArgs args)
		{
			EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid uid = attachedEntity.GetValueOrDefault();
				if (uid.Valid)
				{
					EntityUid? entityUid;
					ServerStorageComponent storageComponent;
					if (base.TryGetSlotEntity(uid, ev.Slot, out entityUid, null, null) && base.TryComp<ServerStorageComponent>(entityUid, ref storageComponent))
					{
						this._storageSystem.OpenStorageUI(entityUid.Value, uid, storageComponent);
					}
					return;
				}
			}
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x000746C0 File Offset: 0x000728C0
		public void TransferEntityInventories(EntityUid uid, EntityUid target)
		{
			InventorySystem.ContainerSlotEnumerator enumerator;
			if (base.TryGetContainerSlotEnumerator(uid, out enumerator, null))
			{
				Dictionary<string, EntityUid?> inventoryEntities = new Dictionary<string, EntityUid?>();
				SlotDefinition[] slots = base.GetSlots(uid, null);
				ContainerSlot containerSlot;
				while (enumerator.MoveNext(out containerSlot))
				{
					foreach (SlotDefinition slot in slots)
					{
						ContainerSlot conslot;
						SlotDefinition slotDefinition;
						if (base.TryGetSlotContainer(target, slot.Name, out conslot, out slotDefinition, null, null) && conslot.ID == containerSlot.ID)
						{
							inventoryEntities.Add(slot.Name, containerSlot.ContainedEntity);
						}
					}
					ContainerHelpers.EmptyContainer(containerSlot, false, null, false, null);
				}
				foreach (KeyValuePair<string, EntityUid?> item in inventoryEntities)
				{
					if (item.Value != null)
					{
						base.TryEquip(target, item.Value.Value, item.Key, true, false, false, null, null);
					}
				}
			}
		}

		// Token: 0x04000DC9 RID: 3529
		[Dependency]
		private readonly StorageSystem _storageSystem;
	}
}
