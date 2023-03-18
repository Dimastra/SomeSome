using System;
using Robust.Client.UserInterface;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000D8 RID: 216
	public sealed class HSpacer : Control
	{
		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x00020BF4 File Offset: 0x0001EDF4
		// (set) Token: 0x06000605 RID: 1541 RVA: 0x00020BFC File Offset: 0x0001EDFC
		public float Spacing
		{
			get
			{
				return base.MinHeight;
			}
			set
			{
				base.MinHeight = value;
			}
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x00020C05 File Offset: 0x0001EE05
		public HSpacer()
		{
			base.MinHeight = this.Spacing;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x00020C19 File Offset: 0x0001EE19
		public HSpacer(float height = 5f)
		{
			this.Spacing = height;
			base.MinHeight = height;
		}
	}
}
