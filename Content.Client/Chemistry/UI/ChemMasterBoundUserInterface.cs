using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Chemistry.UI
{
	// Token: 0x020003D0 RID: 976
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChemMasterBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060017FD RID: 6141 RVA: 0x000021BC File Offset: 0x000003BC
		public ChemMasterBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x00089E8C File Offset: 0x0008808C
		protected override void Open()
		{
			base.Open();
			this._window = new ChemMasterWindow
			{
				Title = this._entityManager.GetComponent<MetaDataComponent>(base.Owner.Owner).EntityName
			};
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.InputEjectButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ItemSlotButtonPressedEvent("beakerSlot", true, true));
			};
			this._window.OutputEjectButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ItemSlotButtonPressedEvent("outputSlot", true, true));
			};
			this._window.BufferTransferButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ChemMasterSetModeMessage(ChemMasterMode.Transfer));
			};
			this._window.BufferDiscardButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ChemMasterSetModeMessage(ChemMasterMode.Discard));
			};
			this._window.CreatePillButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ChemMasterCreatePillsMessage((uint)this._window.PillDosage.Value, (uint)this._window.PillNumber.Value, this._window.LabelLine));
			};
			this._window.CreateBottleButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ChemMasterOutputToBottleMessage((uint)this._window.BottleDosage.Value, this._window.LabelLine));
			};
			uint num = 0U;
			while ((ulong)num < (ulong)((long)this._window.PillTypeButtons.Length))
			{
				uint pillType = num;
				this._window.PillTypeButtons[(int)num].OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.SendMessage(new ChemMasterSetPillTypeMessage(pillType));
				};
				num += 1U;
			}
			this._window.OnReagentButtonPressed += delegate(BaseButton.ButtonEventArgs args, ReagentButton button)
			{
				base.SendMessage(new ChemMasterReagentAmountButtonMessage(button.Id, button.Amount, button.IsBuffer));
			};
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x00089FF8 File Offset: 0x000881F8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ChemMasterBoundUserInterfaceState state2 = (ChemMasterBoundUserInterfaceState)state;
			ChemMasterWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(state2);
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x0008A024 File Offset: 0x00088224
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				ChemMasterWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000C4C RID: 3148
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000C4D RID: 3149
		[Nullable(2)]
		private ChemMasterWindow _window;
	}
}
