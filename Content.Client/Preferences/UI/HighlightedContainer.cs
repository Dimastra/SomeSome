using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Preferences.UI
{
	// Token: 0x02000192 RID: 402
	public sealed class HighlightedContainer : PanelContainer
	{
		// Token: 0x06000AD0 RID: 2768 RVA: 0x0003ED74 File Offset: 0x0003CF74
		public HighlightedContainer()
		{
			base.PanelOverride = new StyleBoxFlat
			{
				BackgroundColor = new Color(47, 47, 53, byte.MaxValue),
				ContentMarginTopOverride = new float?((float)10),
				ContentMarginBottomOverride = new float?((float)10),
				ContentMarginLeftOverride = new float?((float)10),
				ContentMarginRightOverride = new float?((float)10)
			};
		}
	}
}
