using System;
using System.Runtime.CompilerServices;
using Content.Client.Changelog;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client.UserInterface.Systems.EscapeMenu
{
	// Token: 0x02000095 RID: 149
	public sealed class ChangelogUIController : UIController
	{
		// Token: 0x0600037F RID: 895 RVA: 0x000153B5 File Offset: 0x000135B5
		public void OpenWindow()
		{
			this.EnsureWindow();
			this._changeLogWindow.OpenCentered();
			this._changeLogWindow.MoveToFront();
		}

		// Token: 0x06000380 RID: 896 RVA: 0x000153D4 File Offset: 0x000135D4
		private void EnsureWindow()
		{
			ChangelogWindow changeLogWindow = this._changeLogWindow;
			if (changeLogWindow != null && !changeLogWindow.Disposed)
			{
				return;
			}
			this._changeLogWindow = this.UIManager.CreateWindow<ChangelogWindow>();
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00015405 File Offset: 0x00013605
		public void ToggleWindow()
		{
			this.EnsureWindow();
			if (this._changeLogWindow.IsOpen)
			{
				this._changeLogWindow.Close();
				return;
			}
			this.OpenWindow();
		}

		// Token: 0x040001AA RID: 426
		[Nullable(1)]
		private ChangelogWindow _changeLogWindow;
	}
}
