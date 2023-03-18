using System;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004C6 RID: 1222
	public sealed class AdminLogPlayerButton : Button
	{
		// Token: 0x06001F1A RID: 7962 RVA: 0x000B64F7 File Offset: 0x000B46F7
		public AdminLogPlayerButton(Guid id)
		{
			this.Id = id;
			base.ClipText = true;
			base.ToggleMode = true;
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x06001F1B RID: 7963 RVA: 0x000B6514 File Offset: 0x000B4714
		public Guid Id { get; }
	}
}
