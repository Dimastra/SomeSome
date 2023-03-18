using System;
using System.Runtime.CompilerServices;
using Content.Shared.AME;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.AME.UI
{
	// Token: 0x02000477 RID: 1143
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AMEControllerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001C3B RID: 7227 RVA: 0x000021BC File Offset: 0x000003BC
		public AMEControllerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x000A3E11 File Offset: 0x000A2011
		protected override void Open()
		{
			base.Open();
			this._window = new AMEWindow(this);
			this._window.OnClose += base.Close;
			this._window.OpenCentered();
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x000A3E48 File Offset: 0x000A2048
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			SharedAMEControllerComponent.AMEControllerBoundUserInterfaceState state2 = (SharedAMEControllerComponent.AMEControllerBoundUserInterfaceState)state;
			AMEWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(state2);
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x000A3E74 File Offset: 0x000A2074
		public void ButtonPressed(SharedAMEControllerComponent.UiButton button, int dispenseIndex = -1)
		{
			base.SendMessage(new SharedAMEControllerComponent.UiButtonPressedMessage(button));
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x000A3E82 File Offset: 0x000A2082
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				AMEWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000E2D RID: 3629
		[Nullable(2)]
		private AMEWindow _window;
	}
}
