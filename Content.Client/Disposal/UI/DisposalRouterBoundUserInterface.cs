using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disposal.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client.Disposal.UI
{
	// Token: 0x02000349 RID: 841
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DisposalRouterBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060014D8 RID: 5336 RVA: 0x000021BC File Offset: 0x000003BC
		public DisposalRouterBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x0007A930 File Offset: 0x00078B30
		protected override void Open()
		{
			base.Open();
			this._window = new DisposalRouterWindow();
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.Confirm.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.ButtonPressed(SharedDisposalRouterComponent.UiAction.Ok, this._window.TagInput.Text);
			};
			this._window.TagInput.OnTextEntered += delegate(LineEdit.LineEditEventArgs args)
			{
				this.ButtonPressed(SharedDisposalRouterComponent.UiAction.Ok, args.Text);
			};
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x0007A9A8 File Offset: 0x00078BA8
		private void ButtonPressed(SharedDisposalRouterComponent.UiAction action, string tag)
		{
			base.SendMessage(new SharedDisposalRouterComponent.UiActionMessage(action, tag));
			DisposalRouterWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Close();
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0007A9C8 File Offset: 0x00078BC8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			SharedDisposalRouterComponent.DisposalRouterUserInterfaceState disposalRouterUserInterfaceState = state as SharedDisposalRouterComponent.DisposalRouterUserInterfaceState;
			if (disposalRouterUserInterfaceState == null)
			{
				return;
			}
			DisposalRouterWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(disposalRouterUserInterfaceState);
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x0007A9F8 File Offset: 0x00078BF8
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				DisposalRouterWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000ADE RID: 2782
		[Nullable(2)]
		private DisposalRouterWindow _window;
	}
}
