using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disposal.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client.Disposal.UI
{
	// Token: 0x0200034C RID: 844
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DisposalTaggerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060014E8 RID: 5352 RVA: 0x000021BC File Offset: 0x000003BC
		public DisposalTaggerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x0007AD50 File Offset: 0x00078F50
		protected override void Open()
		{
			base.Open();
			this._window = new DisposalTaggerWindow();
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.Confirm.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.ButtonPressed(SharedDisposalTaggerComponent.UiAction.Ok, this._window.TagInput.Text);
			};
			this._window.TagInput.OnTextEntered += delegate(LineEdit.LineEditEventArgs args)
			{
				this.ButtonPressed(SharedDisposalTaggerComponent.UiAction.Ok, args.Text);
			};
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x0007ADC8 File Offset: 0x00078FC8
		private void ButtonPressed(SharedDisposalTaggerComponent.UiAction action, string tag)
		{
			base.SendMessage(new SharedDisposalTaggerComponent.UiActionMessage(action, tag));
			DisposalTaggerWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Close();
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0007ADE8 File Offset: 0x00078FE8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			SharedDisposalTaggerComponent.DisposalTaggerUserInterfaceState disposalTaggerUserInterfaceState = state as SharedDisposalTaggerComponent.DisposalTaggerUserInterfaceState;
			if (disposalTaggerUserInterfaceState == null)
			{
				return;
			}
			DisposalTaggerWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(disposalTaggerUserInterfaceState);
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x0007AE18 File Offset: 0x00079018
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				DisposalTaggerWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000AE1 RID: 2785
		[Nullable(2)]
		private DisposalTaggerWindow _window;
	}
}
