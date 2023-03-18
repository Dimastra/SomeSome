using System;
using System.Runtime.CompilerServices;
using Content.Client.Gameplay;
using Content.Client.Info;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client.UserInterface.Systems.Info
{
	// Token: 0x0200007E RID: 126
	public sealed class InfoUIController : UIController, IOnStateExited<GameplayState>
	{
		// Token: 0x060002AF RID: 687 RVA: 0x00011909 File Offset: 0x0000FB09
		[NullableContext(1)]
		public void OnStateExited(GameplayState state)
		{
			if (this._infoWindow == null)
			{
				return;
			}
			this._infoWindow.Dispose();
			this._infoWindow = null;
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00011926 File Offset: 0x0000FB26
		public void OpenWindow()
		{
			if (this._infoWindow == null || this._infoWindow.Disposed)
			{
				this._infoWindow = this.UIManager.CreateWindow<RulesAndInfoWindow>();
			}
			RulesAndInfoWindow infoWindow = this._infoWindow;
			if (infoWindow == null)
			{
				return;
			}
			infoWindow.OpenCentered();
		}

		// Token: 0x04000173 RID: 371
		[Nullable(2)]
		private RulesAndInfoWindow _infoWindow;
	}
}
