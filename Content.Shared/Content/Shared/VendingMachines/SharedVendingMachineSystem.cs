using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Emag.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.VendingMachines
{
	// Token: 0x02000093 RID: 147
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedVendingMachineSystem : EntitySystem
	{
		// Token: 0x060001B7 RID: 439 RVA: 0x00009805 File Offset: 0x00007A05
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<VendingMachineComponent, ComponentInit>(new ComponentEventHandler<VendingMachineComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00009822 File Offset: 0x00007A22
		protected virtual void OnComponentInit(EntityUid uid, VendingMachineComponent component, ComponentInit args)
		{
			this.RestockInventoryFromPrototype(uid, component);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000982C File Offset: 0x00007A2C
		[NullableContext(2)]
		public void RestockInventoryFromPrototype(EntityUid uid, VendingMachineComponent component = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref component, true))
			{
				return;
			}
			VendingMachineInventoryPrototype packPrototype;
			if (!this._prototypeManager.TryIndex<VendingMachineInventoryPrototype>(component.PackPrototypeId, ref packPrototype))
			{
				return;
			}
			this.AddInventoryFromPrototype(uid, packPrototype.StartingInventory, InventoryType.Regular, component);
			this.AddInventoryFromPrototype(uid, packPrototype.EmaggedInventory, InventoryType.Emagged, component);
			this.AddInventoryFromPrototype(uid, packPrototype.ContrabandInventory, InventoryType.Contraband, component);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000988C File Offset: 0x00007A8C
		public List<VendingMachineInventoryEntry> GetAllInventory(EntityUid uid, [Nullable(2)] VendingMachineComponent component = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref component, true))
			{
				return new List<VendingMachineInventoryEntry>();
			}
			List<VendingMachineInventoryEntry> inventory = new List<VendingMachineInventoryEntry>(component.Inventory.Values);
			if (base.HasComp<EmaggedComponent>(uid))
			{
				inventory.AddRange(component.EmaggedInventory.Values);
			}
			if (component.Contraband)
			{
				inventory.AddRange(component.ContrabandInventory.Values);
			}
			return inventory;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x000098F0 File Offset: 0x00007AF0
		public List<VendingMachineInventoryEntry> GetAvailableInventory(EntityUid uid, [Nullable(2)] VendingMachineComponent component = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref component, true))
			{
				return new List<VendingMachineInventoryEntry>();
			}
			return (from _ in this.GetAllInventory(uid, component)
			where _.Amount > 0U
			select _).ToList<VendingMachineInventoryEntry>();
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00009940 File Offset: 0x00007B40
		[NullableContext(2)]
		private void AddInventoryFromPrototype(EntityUid uid, [Nullable(new byte[]
		{
			2,
			1
		})] Dictionary<string, uint> entries, InventoryType type, VendingMachineComponent component = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref component, true) || entries == null)
			{
				return;
			}
			Dictionary<string, VendingMachineInventoryEntry> inventory;
			switch (type)
			{
			case InventoryType.Regular:
				inventory = component.Inventory;
				break;
			case InventoryType.Emagged:
				inventory = component.EmaggedInventory;
				break;
			case InventoryType.Contraband:
				inventory = component.ContrabandInventory;
				break;
			default:
				return;
			}
			foreach (KeyValuePair<string, uint> keyValuePair in entries)
			{
				string text;
				uint num;
				keyValuePair.Deconstruct(out text, out num);
				string id = text;
				uint amount = num;
				if (this._prototypeManager.HasIndex<EntityPrototype>(id))
				{
					VendingMachineInventoryEntry entry;
					if (inventory.TryGetValue(id, out entry))
					{
						entry.Amount = Math.Min(entry.Amount + amount, 3U * amount);
					}
					else
					{
						inventory.Add(id, new VendingMachineInventoryEntry(type, id, amount));
					}
				}
			}
		}

		// Token: 0x040001F8 RID: 504
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;
	}
}
