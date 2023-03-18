using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Administration;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI
{
	// Token: 0x0200048A RID: 1162
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminAnnounceEui : BaseEui
	{
		// Token: 0x06001C9E RID: 7326 RVA: 0x000A60A0 File Offset: 0x000A42A0
		public AdminAnnounceEui()
		{
			this._window = new AdminAnnounceWindow();
			this._window.OnClose += delegate()
			{
				base.SendMessage(new AdminAnnounceEuiMsg.Close());
			};
			this._window.AnnounceButton.OnPressed += this.AnnounceButtonOnOnPressed;
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x000A60F4 File Offset: 0x000A42F4
		private void AnnounceButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
		{
			base.SendMessage(new AdminAnnounceEuiMsg.DoAnnounce
			{
				Announcement = this._window.Announcement.Text,
				Announcer = this._window.Announcer.Text,
				AnnounceType = (AdminAnnounceType)(this._window.AnnounceMethod.SelectedMetadata ?? AdminAnnounceType.Station),
				CloseAfter = !this._window.KeepWindowOpen.Pressed
			});
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x000A6176 File Offset: 0x000A4376
		public override void Opened()
		{
			this._window.OpenCentered();
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x000A6183 File Offset: 0x000A4383
		public override void Closed()
		{
			this._window.Close();
		}

		// Token: 0x04000E4F RID: 3663
		private readonly AdminAnnounceWindow _window;
	}
}
