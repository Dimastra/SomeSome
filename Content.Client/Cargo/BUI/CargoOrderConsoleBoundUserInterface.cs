using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Cargo.UI;
using Content.Shared.Cargo;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Events;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.IdentityManagement;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client.Cargo.BUI
{
	// Token: 0x0200040C RID: 1036
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class CargoOrderConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06001987 RID: 6535 RVA: 0x00092B43 File Offset: 0x00090D43
		// (set) Token: 0x06001988 RID: 6536 RVA: 0x00092B4B File Offset: 0x00090D4B
		[ViewVariables]
		public string AccountName { get; private set; }

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06001989 RID: 6537 RVA: 0x00092B54 File Offset: 0x00090D54
		// (set) Token: 0x0600198A RID: 6538 RVA: 0x00092B5C File Offset: 0x00090D5C
		[ViewVariables]
		public int BankBalance { get; private set; }

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x0600198B RID: 6539 RVA: 0x00092B65 File Offset: 0x00090D65
		// (set) Token: 0x0600198C RID: 6540 RVA: 0x00092B6D File Offset: 0x00090D6D
		[ViewVariables]
		public int OrderCapacity { get; private set; }

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x0600198D RID: 6541 RVA: 0x00092B76 File Offset: 0x00090D76
		// (set) Token: 0x0600198E RID: 6542 RVA: 0x00092B7E File Offset: 0x00090D7E
		[ViewVariables]
		public int OrderCount { get; private set; }

		// Token: 0x0600198F RID: 6543 RVA: 0x000021BC File Offset: 0x000003BC
		[NullableContext(1)]
		public CargoOrderConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x00092B88 File Offset: 0x00090D88
		protected override void Open()
		{
			base.Open();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			SpriteSystem entitySystem = entityManager.EntitySysManager.GetEntitySystem<SpriteSystem>();
			this._menu = new CargoConsoleMenu(IoCManager.Resolve<IPrototypeManager>(), entitySystem);
			IPlayerManager playerManager = IoCManager.Resolve<IPlayerManager>();
			EntityUid? entityUid;
			if (playerManager == null)
			{
				entityUid = null;
			}
			else
			{
				LocalPlayer localPlayer = playerManager.LocalPlayer;
				entityUid = ((localPlayer != null) ? localPlayer.ControlledEntity : null);
			}
			EntityUid? entityUid2 = entityUid;
			FormattedMessage description = new FormattedMessage();
			string orderRequester;
			MetaDataComponent metaDataComponent;
			if (entityManager.TryGetComponent<MetaDataComponent>(entityUid2, ref metaDataComponent))
			{
				orderRequester = Identity.Name(entityUid2.Value, entityManager, null);
			}
			else
			{
				orderRequester = string.Empty;
			}
			this._orderMenu = new CargoConsoleOrderMenu();
			this._menu.OnClose += base.Close;
			this._menu.OnItemSelected += delegate(BaseButton.ButtonEventArgs args)
			{
				CargoProductRow cargoProductRow = args.Button.Parent as CargoProductRow;
				if (cargoProductRow == null)
				{
					return;
				}
				description.Clear();
				description.PushColor(Color.White);
				if (cargoProductRow.MainButton.ToolTip != null)
				{
					description.AddText(cargoProductRow.MainButton.ToolTip);
				}
				this._orderMenu.Description.SetMessage(description);
				this._product = cargoProductRow.Product;
				this._orderMenu.ProductName.Text = cargoProductRow.ProductName.Text;
				this._orderMenu.PointCost.Text = cargoProductRow.PointCost.Text;
				this._orderMenu.Requester.Text = orderRequester;
				this._orderMenu.Reason.Text = "";
				this._orderMenu.Amount.Value = 1;
				this._orderMenu.OpenCentered();
			};
			this._menu.OnOrderApproved += this.ApproveOrder;
			this._menu.OnOrderCanceled += this.RemoveOrder;
			this._orderMenu.SubmitButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				if (this.AddOrder())
				{
					this._orderMenu.Close();
				}
			};
			this._menu.OpenCentered();
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x00092CC9 File Offset: 0x00090EC9
		[NullableContext(1)]
		private void Populate(List<CargoOrderData> orders)
		{
			if (this._menu == null)
			{
				return;
			}
			this._menu.PopulateProducts();
			this._menu.PopulateCategories();
			this._menu.PopulateOrders(orders);
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x00092CF8 File Offset: 0x00090EF8
		[NullableContext(1)]
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			CargoConsoleInterfaceState cargoConsoleInterfaceState = state as CargoConsoleInterfaceState;
			if (cargoConsoleInterfaceState == null)
			{
				return;
			}
			this.OrderCapacity = cargoConsoleInterfaceState.Capacity;
			this.OrderCount = cargoConsoleInterfaceState.Count;
			this.BankBalance = cargoConsoleInterfaceState.Balance;
			this.AccountName = cargoConsoleInterfaceState.Name;
			this.Populate(cargoConsoleInterfaceState.Orders);
			CargoConsoleMenu menu = this._menu;
			if (menu != null)
			{
				menu.UpdateCargoCapacity(this.OrderCount, this.OrderCapacity);
			}
			CargoConsoleMenu menu2 = this._menu;
			if (menu2 == null)
			{
				return;
			}
			menu2.UpdateBankData(this.AccountName, this.BankBalance);
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x00092D8C File Offset: 0x00090F8C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			CargoConsoleMenu menu = this._menu;
			if (menu != null)
			{
				menu.Dispose();
			}
			CargoConsoleOrderMenu orderMenu = this._orderMenu;
			if (orderMenu == null)
			{
				return;
			}
			orderMenu.Dispose();
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x00092DBC File Offset: 0x00090FBC
		private bool AddOrder()
		{
			CargoConsoleOrderMenu orderMenu = this._orderMenu;
			int num = (orderMenu != null) ? orderMenu.Amount.Value : 0;
			if (num < 1 || num > this.OrderCapacity)
			{
				return false;
			}
			CargoConsoleOrderMenu orderMenu2 = this._orderMenu;
			string requester = ((orderMenu2 != null) ? orderMenu2.Requester.Text : null) ?? "";
			CargoConsoleOrderMenu orderMenu3 = this._orderMenu;
			string reason = ((orderMenu3 != null) ? orderMenu3.Reason.Text : null) ?? "";
			CargoProductPrototype product = this._product;
			base.SendMessage(new CargoConsoleAddOrderMessage(requester, reason, ((product != null) ? product.ID : null) ?? "", num));
			return true;
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x00092E58 File Offset: 0x00091058
		[NullableContext(1)]
		private void RemoveOrder(BaseButton.ButtonEventArgs args)
		{
			Control parent = args.Button.Parent;
			CargoOrderRow cargoOrderRow = ((parent != null) ? parent.Parent : null) as CargoOrderRow;
			if (cargoOrderRow == null || cargoOrderRow.Order == null)
			{
				return;
			}
			base.SendMessage(new CargoConsoleRemoveOrderMessage(cargoOrderRow.Order.OrderIndex));
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x00092EA4 File Offset: 0x000910A4
		[NullableContext(1)]
		private void ApproveOrder(BaseButton.ButtonEventArgs args)
		{
			Control parent = args.Button.Parent;
			CargoOrderRow cargoOrderRow = ((parent != null) ? parent.Parent : null) as CargoOrderRow;
			if (cargoOrderRow == null || cargoOrderRow.Order == null)
			{
				return;
			}
			if (this.OrderCount >= this.OrderCapacity)
			{
				return;
			}
			base.SendMessage(new CargoConsoleApproveOrderMessage(cargoOrderRow.Order.OrderIndex));
		}

		// Token: 0x04000CF4 RID: 3316
		[ViewVariables]
		private CargoConsoleMenu _menu;

		// Token: 0x04000CF5 RID: 3317
		[ViewVariables]
		private CargoConsoleOrderMenu _orderMenu;

		// Token: 0x04000CFA RID: 3322
		private CargoProductPrototype _product;
	}
}
