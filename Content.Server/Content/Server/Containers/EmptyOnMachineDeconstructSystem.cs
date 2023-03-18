using System;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.Containers
{
	// Token: 0x020005EC RID: 1516
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmptyOnMachineDeconstructSystem : EntitySystem
	{
		// Token: 0x0600204E RID: 8270 RVA: 0x000A8668 File Offset: 0x000A6868
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EmptyOnMachineDeconstructComponent, MachineDeconstructedEvent>(new ComponentEventHandler<EmptyOnMachineDeconstructComponent, MachineDeconstructedEvent>(this.OnDeconstruct), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, MachineDeconstructedEvent>(new ComponentEventHandler<ItemSlotsComponent, MachineDeconstructedEvent>(this.OnSlotsDeconstruct), null, null);
		}

		// Token: 0x0600204F RID: 8271 RVA: 0x000A8698 File Offset: 0x000A6898
		private void OnSlotsDeconstruct(EntityUid uid, ItemSlotsComponent component, MachineDeconstructedEvent args)
		{
			foreach (ItemSlot slot in component.Slots.Values)
			{
				if (slot.EjectOnDeconstruct && slot.Item != null)
				{
					ContainerSlot containerSlot = slot.ContainerSlot;
					if (containerSlot != null)
					{
						containerSlot.Remove(slot.Item.Value, null, null, null, true, false, null, null);
					}
				}
			}
		}

		// Token: 0x06002050 RID: 8272 RVA: 0x000A873C File Offset: 0x000A693C
		private void OnDeconstruct(EntityUid uid, EmptyOnMachineDeconstructComponent component, MachineDeconstructedEvent ev)
		{
			IContainerManager mComp;
			if (!this.EntityManager.TryGetComponent<IContainerManager>(uid, ref mComp))
			{
				return;
			}
			EntityCoordinates baseCoords = this.EntityManager.GetComponent<TransformComponent>(component.Owner).Coordinates;
			foreach (string v in component.Containers)
			{
				IContainer container;
				if (mComp.TryGetContainer(v, ref container))
				{
					ContainerHelpers.EmptyContainer(container, true, new EntityCoordinates?(baseCoords), false, null);
				}
			}
		}
	}
}
