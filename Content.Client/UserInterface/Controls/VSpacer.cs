using System;
using Robust.Client.UserInterface;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000E6 RID: 230
	public sealed class VSpacer : Control
	{
		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x000229E6 File Offset: 0x00020BE6
		// (set) Token: 0x06000695 RID: 1685 RVA: 0x000229EE File Offset: 0x00020BEE
		public float Spacing
		{
			get
			{
				return base.MinWidth;
			}
			set
			{
				base.MinWidth = value;
			}
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x000229F7 File Offset: 0x00020BF7
		public VSpacer()
		{
			base.MinWidth = this.Spacing;
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x00022A0B File Offset: 0x00020C0B
		public VSpacer(float width = 5f)
		{
			this.Spacing = width;
			base.MinWidth = width;
		}
	}
}
