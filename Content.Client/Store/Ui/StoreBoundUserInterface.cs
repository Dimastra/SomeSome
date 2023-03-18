using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Store;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Client.Store.Ui
{
	// Token: 0x02000119 RID: 281
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StoreBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060007C1 RID: 1985 RVA: 0x0002D036 File Offset: 0x0002B236
		public StoreBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x0002D050 File Offset: 0x0002B250
		protected override void Open()
		{
			this._menu = new StoreMenu(this._windowName);
			this._menu.OpenCentered();
			this._menu.OnClose += base.Close;
			this._menu.OnListingButtonPressed += delegate(BaseButton.ButtonEventArgs _, ListingData listing)
			{
				base.SendMessage(new StoreBuyListingMessage(listing));
			};
			this._menu.OnCategoryButtonPressed += delegate(BaseButton.ButtonEventArgs _, string category)
			{
				this._menu.CurrentCategory = category;
				base.SendMessage(new StoreRequestUpdateInterfaceMessage());
			};
			this._menu.OnWithdrawAttempt += delegate(BaseButton.ButtonEventArgs _, string type, int amount)
			{
				base.SendMessage(new StoreRequestWithdrawMessage(type, amount));
			};
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x0002D0D8 File Offset: 0x0002B2D8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._menu == null)
			{
				return;
			}
			StoreUpdateState storeUpdateState = state as StoreUpdateState;
			if (storeUpdateState != null)
			{
				this._menu.UpdateBalance(storeUpdateState.Balance);
				this._menu.PopulateStoreCategoryButtons(storeUpdateState.Listings);
				this._menu.UpdateListing(storeUpdateState.Listings.ToList<ListingData>());
				return;
			}
			StoreInitializeState storeInitializeState = state as StoreInitializeState;
			if (storeInitializeState == null)
			{
				return;
			}
			this._windowName = storeInitializeState.Name;
			if (this._menu != null && this._menu.Window != null)
			{
				this._menu.Window.Title = storeInitializeState.Name;
			}
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x0002D17A File Offset: 0x0002B37A
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			StoreMenu menu = this._menu;
			if (menu != null)
			{
				menu.Close();
			}
			StoreMenu menu2 = this._menu;
			if (menu2 == null)
			{
				return;
			}
			menu2.Dispose();
		}

		// Token: 0x040003F2 RID: 1010
		[Nullable(2)]
		private StoreMenu _menu;

		// Token: 0x040003F3 RID: 1011
		private string _windowName = Loc.GetString("store-ui-default-title");
	}
}
