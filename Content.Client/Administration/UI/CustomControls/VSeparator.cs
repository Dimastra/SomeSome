using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.CustomControls
{
	// Token: 0x020004CE RID: 1230
	public sealed class VSeparator : PanelContainer
	{
		// Token: 0x06001F4F RID: 8015 RVA: 0x000B6EEF File Offset: 0x000B50EF
		public VSeparator(Color color)
		{
			base.MinSize = new ValueTuple<float, float>(2f, 5f);
			base.AddChild(new PanelContainer
			{
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = color
				}
			});
		}

		// Token: 0x06001F50 RID: 8016 RVA: 0x000B6F2E File Offset: 0x000B512E
		public VSeparator() : this(VSeparator.SeparatorColor)
		{
		}

		// Token: 0x04000F0D RID: 3853
		private static readonly Color SeparatorColor = Color.FromHex("#3D4059", null);
	}
}
