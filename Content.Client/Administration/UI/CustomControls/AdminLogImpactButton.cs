using System;
using Content.Shared.Database;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004C4 RID: 1220
	public sealed class AdminLogImpactButton : Button
	{
		// Token: 0x06001F13 RID: 7955 RVA: 0x000B6419 File Offset: 0x000B4619
		public AdminLogImpactButton(LogImpact impact)
		{
			this.Impact = impact;
			base.ToggleMode = true;
			base.Pressed = true;
		}

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06001F14 RID: 7956 RVA: 0x000B6436 File Offset: 0x000B4636
		public LogImpact Impact { get; }
	}
}
