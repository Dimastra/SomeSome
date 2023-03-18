using System;
using System.Runtime.CompilerServices;
using Content.Shared.Paper;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Paper.UI
{
	// Token: 0x020001EF RID: 495
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PaperBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000C9B RID: 3227 RVA: 0x000021BC File Offset: 0x000003BC
		public PaperBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x0004969C File Offset: 0x0004789C
		protected override void Open()
		{
			base.Open();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			this._window = new PaperWindow();
			this._window.OnClose += base.Close;
			this._window.Input.OnTextEntered += this.Input_OnTextEntered;
			PaperVisualsComponent visuals;
			if (entityManager.TryGetComponent<PaperVisualsComponent>(base.Owner.Owner, ref visuals))
			{
				this._window.InitVisuals(visuals);
			}
			this._window.OpenCentered();
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x0004971D File Offset: 0x0004791D
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			PaperWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Populate((SharedPaperComponent.PaperBoundUserInterfaceState)state);
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x0004973C File Offset: 0x0004793C
		private void Input_OnTextEntered(LineEdit.LineEditEventArgs obj)
		{
			if (!string.IsNullOrEmpty(obj.Text))
			{
				base.SendMessage(new SharedPaperComponent.PaperInputTextMessage(obj.Text));
				if (this._window != null)
				{
					this._window.Input.Text = string.Empty;
				}
			}
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x00049779 File Offset: 0x00047979
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			PaperWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000670 RID: 1648
		[Nullable(2)]
		private PaperWindow _window;
	}
}
