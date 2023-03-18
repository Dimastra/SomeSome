using System;
using System.Runtime.CompilerServices;
using Content.Client.DoAfter;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000E2 RID: 226
	public sealed class ProgressTextureRect : TextureRect
	{
		// Token: 0x06000657 RID: 1623 RVA: 0x00021DE4 File Offset: 0x0001FFE4
		[NullableContext(1)]
		protected override void Draw(DrawingHandleScreen handle)
		{
			UIBox2 uibox = (base.Texture != null) ? base.GetDrawDimensions(base.Texture) : UIBox2.FromDimensions(Vector2.Zero, base.PixelSize);
			uibox.Top = Math.Max(uibox.Bottom - uibox.Bottom * this.Progress, 0f);
			handle.DrawRect(uibox, DoAfterOverlay.GetProgressColor(this.Progress), true);
			base.Draw(handle);
		}

		// Token: 0x040002DB RID: 731
		public float Progress;
	}
}
