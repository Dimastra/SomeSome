using System;
using System.Runtime.CompilerServices;
using Content.Client.Cargo.UI;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Events;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Cargo.BUI
{
	// Token: 0x0200040E RID: 1038
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CargoShuttleConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600199A RID: 6554 RVA: 0x000021BC File Offset: 0x000003BC
		public CargoShuttleConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x00093048 File Offset: 0x00091248
		protected override void Open()
		{
			base.Open();
			IDependencyCollection instance = IoCManager.Instance;
			if (instance == null)
			{
				return;
			}
			this._menu = new CargoShuttleMenu(instance.Resolve<IGameTiming>(), instance.Resolve<IPrototypeManager>(), instance.Resolve<IEntitySystemManager>().GetEntitySystem<SpriteSystem>());
			CargoShuttleMenu menu = this._menu;
			menu.ShuttleCallRequested = (Action)Delegate.Combine(menu.ShuttleCallRequested, new Action(this.OnShuttleCall));
			CargoShuttleMenu menu2 = this._menu;
			menu2.ShuttleRecallRequested = (Action)Delegate.Combine(menu2.ShuttleRecallRequested, new Action(this.OnShuttleRecall));
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x000930F7 File Offset: 0x000912F7
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				CargoShuttleMenu menu = this._menu;
				if (menu == null)
				{
					return;
				}
				menu.Dispose();
			}
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x00093113 File Offset: 0x00091313
		private void OnShuttleRecall()
		{
			base.SendMessage(new CargoRecallShuttleMessage());
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x00093120 File Offset: 0x00091320
		private void OnShuttleCall()
		{
			base.SendMessage(new CargoCallShuttleMessage());
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x00093130 File Offset: 0x00091330
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			CargoShuttleConsoleBoundUserInterfaceState cargoShuttleConsoleBoundUserInterfaceState = state as CargoShuttleConsoleBoundUserInterfaceState;
			if (cargoShuttleConsoleBoundUserInterfaceState == null)
			{
				return;
			}
			CargoShuttleMenu menu = this._menu;
			if (menu != null)
			{
				menu.SetAccountName(cargoShuttleConsoleBoundUserInterfaceState.AccountName);
			}
			CargoShuttleMenu menu2 = this._menu;
			if (menu2 != null)
			{
				menu2.SetShuttleName(cargoShuttleConsoleBoundUserInterfaceState.ShuttleName);
			}
			CargoShuttleMenu menu3 = this._menu;
			if (menu3 != null)
			{
				menu3.SetShuttleETA(cargoShuttleConsoleBoundUserInterfaceState.ShuttleETA);
			}
			CargoShuttleMenu menu4 = this._menu;
			if (menu4 != null)
			{
				menu4.SetOrders(cargoShuttleConsoleBoundUserInterfaceState.Orders);
			}
			CargoShuttleMenu menu5 = this._menu;
			if (menu5 == null)
			{
				return;
			}
			menu5.SetCanRecall(cargoShuttleConsoleBoundUserInterfaceState.CanRecall);
		}

		// Token: 0x04000CFE RID: 3326
		[Nullable(2)]
		private CargoShuttleMenu _menu;
	}
}
