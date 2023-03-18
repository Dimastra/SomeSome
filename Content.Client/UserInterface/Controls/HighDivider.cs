using System;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000D6 RID: 214
	public sealed class HighDivider : Control
	{
		// Token: 0x060005FE RID: 1534 RVA: 0x00020AAA File Offset: 0x0001ECAA
		public HighDivider()
		{
			base.Children.Add(new PanelContainer
			{
				StyleClasses = 
				{
					"HighDivider"
				}
			});
		}
	}
}
