using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Humanoid
{
	// Token: 0x020002CE RID: 718
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class EyeColorPicker : Control
	{
		// Token: 0x14000065 RID: 101
		// (add) Token: 0x060011F1 RID: 4593 RVA: 0x0006A97C File Offset: 0x00068B7C
		// (remove) Token: 0x060011F2 RID: 4594 RVA: 0x0006A9B4 File Offset: 0x00068BB4
		public event Action<Color> OnEyeColorPicked;

		// Token: 0x060011F3 RID: 4595 RVA: 0x0006A9E9 File Offset: 0x00068BE9
		public void SetData(Color color)
		{
			this._lastColor = color;
			this._colorSelectors.Color = color;
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x0006AA00 File Offset: 0x00068C00
		public EyeColorPicker()
		{
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 1
			};
			base.AddChild(boxContainer);
			boxContainer.AddChild(this._colorSelectors = new ColorSelectorSliders());
			ColorSelectorSliders colorSelectors = this._colorSelectors;
			colorSelectors.OnColorChanged = (Action<Color>)Delegate.Combine(colorSelectors.OnColorChanged, new Action<Color>(this.ColorValueChanged));
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x0006AA62 File Offset: 0x00068C62
		private void ColorValueChanged(Color newColor)
		{
			Action<Color> onEyeColorPicked = this.OnEyeColorPicked;
			if (onEyeColorPicked != null)
			{
				onEyeColorPicked(newColor);
			}
			this._lastColor = newColor;
		}

		// Token: 0x040008E6 RID: 2278
		[Nullable(1)]
		private readonly ColorSelectorSliders _colorSelectors;

		// Token: 0x040008E7 RID: 2279
		private Color _lastColor;
	}
}
