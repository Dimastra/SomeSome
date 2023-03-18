using System;
using System.Runtime.CompilerServices;
using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Shuttles.BUI
{
	// Token: 0x02000159 RID: 345
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IFFConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000913 RID: 2323 RVA: 0x000021BC File Offset: 0x000003BC
		public IFFConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0003593C File Offset: 0x00033B3C
		protected override void Open()
		{
			base.Open();
			this._window = new IFFConsoleWindow();
			this._window.OnClose += base.Close;
			this._window.ShowIFF += this.SendIFFMessage;
			this._window.ShowVessel += this.SendVesselMessage;
			this._window.OpenCenteredLeft();
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x000359AC File Offset: 0x00033BAC
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			IFFConsoleBoundUserInterfaceState iffconsoleBoundUserInterfaceState = state as IFFConsoleBoundUserInterfaceState;
			if (iffconsoleBoundUserInterfaceState == null)
			{
				return;
			}
			IFFConsoleWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.UpdateState(iffconsoleBoundUserInterfaceState);
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x000359DC File Offset: 0x00033BDC
		private void SendIFFMessage(bool obj)
		{
			base.SendMessage(new IFFShowIFFMessage
			{
				Show = obj
			});
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x000359F0 File Offset: 0x00033BF0
		private void SendVesselMessage(bool obj)
		{
			base.SendMessage(new IFFShowVesselMessage
			{
				Show = obj
			});
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x00035A04 File Offset: 0x00033C04
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				IFFConsoleWindow window = this._window;
				if (window != null)
				{
					window.Close();
				}
				this._window = null;
			}
		}

		// Token: 0x0400048F RID: 1167
		[Nullable(2)]
		private IFFConsoleWindow _window;
	}
}
