using System;
using Content.Shared.Database;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004C7 RID: 1223
	public sealed class AdminLogTypeButton : Button
	{
		// Token: 0x06001F1C RID: 7964 RVA: 0x000B651C File Offset: 0x000B471C
		public AdminLogTypeButton(LogType type)
		{
			this.Type = type;
			base.ClipText = true;
			base.ToggleMode = true;
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x06001F1D RID: 7965 RVA: 0x000B6539 File Offset: 0x000B4739
		public LogType Type { get; }
	}
}
