using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.VendingMachines.UI;
using Content.Shared.VendingMachines;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.VendingMachines
{
	// Token: 0x02000064 RID: 100
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VendingMachineBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060001D5 RID: 469 RVA: 0x0000D292 File Offset: 0x0000B492
		public VendingMachineBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000D2A8 File Offset: 0x0000B4A8
		protected override void Open()
		{
			base.Open();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			VendingMachineSystem vendingMachineSystem = entityManager.System<VendingMachineSystem>();
			this._cachedInventory = vendingMachineSystem.GetAllInventory(base.Owner.Owner, null);
			this._menu = new VendingMachineMenu
			{
				Title = entityManager.GetComponent<MetaDataComponent>(base.Owner.Owner).EntityName
			};
			this._menu.OnClose += base.Close;
			this._menu.OnItemSelected += this.OnItemSelected;
			this._menu.Populate(this._cachedInventory);
			this._menu.OpenCentered();
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000D354 File Offset: 0x0000B554
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			VendingMachineInterfaceState vendingMachineInterfaceState = state as VendingMachineInterfaceState;
			if (vendingMachineInterfaceState == null)
			{
				return;
			}
			this._cachedInventory = vendingMachineInterfaceState.Inventory;
			VendingMachineMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Populate(this._cachedInventory);
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000D398 File Offset: 0x0000B598
		private void OnItemSelected(ItemList.ItemListSelectedEventArgs args)
		{
			if (this._cachedInventory.Count == 0)
			{
				return;
			}
			VendingMachineInventoryEntry vendingMachineInventoryEntry = this._cachedInventory.ElementAtOrDefault(args.ItemIndex);
			if (vendingMachineInventoryEntry == null)
			{
				return;
			}
			base.SendMessage(new VendingMachineEjectMessage(vendingMachineInventoryEntry.Type, vendingMachineInventoryEntry.ID));
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000D3E0 File Offset: 0x0000B5E0
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			if (this._menu == null)
			{
				return;
			}
			this._menu.OnItemSelected -= this.OnItemSelected;
			this._menu.OnClose -= base.Close;
			this._menu.Dispose();
		}

		// Token: 0x0400013B RID: 315
		[Nullable(2)]
		[ViewVariables]
		private VendingMachineMenu _menu;

		// Token: 0x0400013C RID: 316
		private List<VendingMachineInventoryEntry> _cachedInventory = new List<VendingMachineInventoryEntry>();
	}
}
