using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;

namespace Content.Client.NPC
{
	// Token: 0x0200020A RID: 522
	public sealed class NPCEui : BaseEui
	{
		// Token: 0x06000DAF RID: 3503 RVA: 0x000528C8 File Offset: 0x00050AC8
		public override void Opened()
		{
			base.Opened();
			this._window = new NPCWindow();
			this._window.OpenCentered();
		}

		// Token: 0x06000DB0 RID: 3504 RVA: 0x000528E6 File Offset: 0x00050AE6
		public override void Closed()
		{
			base.Closed();
			NPCWindow window = this._window;
			if (window != null)
			{
				window.Close();
			}
			this._window = null;
		}

		// Token: 0x040006BF RID: 1727
		[Nullable(2)]
		private NPCWindow _window = new NPCWindow();
	}
}
