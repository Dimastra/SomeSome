using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004C9 RID: 1225
	public sealed class HSeparator : Control
	{
		// Token: 0x06001F25 RID: 7973 RVA: 0x000B6622 File Offset: 0x000B4822
		public HSeparator(Color color)
		{
			base.AddChild(new PanelContainer
			{
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = color,
					ContentMarginBottomOverride = new float?((float)2),
					ContentMarginLeftOverride = new float?((float)2)
				}
			});
		}

		// Token: 0x06001F26 RID: 7974 RVA: 0x000B6661 File Offset: 0x000B4861
		public HSeparator() : this(HSeparator.SeparatorColor)
		{
		}

		// Token: 0x04000F00 RID: 3840
		private static readonly Color SeparatorColor = Color.FromHex("#3D4059", null);
	}
}
