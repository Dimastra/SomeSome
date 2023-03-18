using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Chemistry.UI
{
	// Token: 0x020003DA RID: 986
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReagentDispenserBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600183E RID: 6206 RVA: 0x000021BC File Offset: 0x000003BC
		public ReagentDispenserBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x0008C038 File Offset: 0x0008A238
		protected override void Open()
		{
			base.Open();
			this._window = new ReagentDispenserWindow
			{
				Title = this._entityManager.GetComponent<MetaDataComponent>(base.Owner.Owner).EntityName
			};
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.EjectButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ItemSlotButtonPressedEvent("beakerSlot", true, true));
			};
			this._window.ClearButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserClearContainerSolutionMessage());
			};
			this._window.DispenseButton1.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U1));
			};
			this._window.DispenseButton5.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U5));
			};
			this._window.DispenseButton10.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U10));
			};
			this._window.DispenseButton15.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U15));
			};
			this._window.DispenseButton20.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U20));
			};
			this._window.DispenseButton25.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U25));
			};
			this._window.DispenseButton30.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U30));
			};
			this._window.DispenseButton50.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U50));
			};
			this._window.DispenseButton100.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount.U100));
			};
			this._window.OnDispenseReagentButtonPressed += delegate(BaseButton.ButtonEventArgs args, DispenseReagentButton button)
			{
				base.SendMessage(new ReagentDispenserDispenseReagentMessage(button.ReagentId));
			};
			this._window.OnDispenseReagentButtonMouseEntered += delegate(GUIMouseHoverEventArgs args, DispenseReagentButton button)
			{
				if (this._lastState != null)
				{
					this._window.UpdateContainerInfo(this._lastState, button.ReagentId);
				}
			};
			this._window.OnDispenseReagentButtonMouseExited += delegate(GUIMouseHoverEventArgs args, DispenseReagentButton button)
			{
				if (this._lastState != null)
				{
					this._window.UpdateContainerInfo(this._lastState, null);
				}
			};
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x0008C214 File Offset: 0x0008A414
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ReagentDispenserBoundUserInterfaceState reagentDispenserBoundUserInterfaceState = (ReagentDispenserBoundUserInterfaceState)state;
			this._lastState = reagentDispenserBoundUserInterfaceState;
			ReagentDispenserWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(reagentDispenserBoundUserInterfaceState);
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x0008C247 File Offset: 0x0008A447
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				ReagentDispenserWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000C66 RID: 3174
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000C67 RID: 3175
		[Nullable(2)]
		private ReagentDispenserWindow _window;

		// Token: 0x04000C68 RID: 3176
		[Nullable(2)]
		private ReagentDispenserBoundUserInterfaceState _lastState;
	}
}
