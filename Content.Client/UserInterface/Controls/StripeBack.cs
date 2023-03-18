using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000E5 RID: 229
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StripeBack : Container
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x0002269D File Offset: 0x0002089D
		// (set) Token: 0x06000689 RID: 1673 RVA: 0x000226A5 File Offset: 0x000208A5
		public bool HasTopEdge
		{
			get
			{
				return this._hasTopEdge;
			}
			set
			{
				this._hasTopEdge = value;
				base.InvalidateMeasure();
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x000226B4 File Offset: 0x000208B4
		// (set) Token: 0x0600068B RID: 1675 RVA: 0x000226BC File Offset: 0x000208BC
		public bool HasBottomEdge
		{
			get
			{
				return this._hasBottomEdge;
			}
			set
			{
				this._hasBottomEdge = value;
				base.InvalidateMeasure();
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x000226CB File Offset: 0x000208CB
		// (set) Token: 0x0600068D RID: 1677 RVA: 0x000226D3 File Offset: 0x000208D3
		public bool HasMargins
		{
			get
			{
				return this._hasMargins;
			}
			set
			{
				this._hasMargins = value;
				base.InvalidateMeasure();
			}
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x000226E4 File Offset: 0x000208E4
		protected override Vector2 MeasureOverride(Vector2 availableSize)
		{
			float num = this.HasMargins ? 4f : 0f;
			float num2 = 0f;
			if (this.HasBottomEdge)
			{
				num2 += num + 2f;
			}
			if (this.HasTopEdge)
			{
				num2 += num + 2f;
			}
			Vector2 vector = Vector2.Zero;
			availableSize.Y -= num2;
			foreach (Control control in base.Children)
			{
				control.Measure(availableSize);
				vector = Vector2.ComponentMax(vector, control.DesiredSize);
			}
			return vector + new ValueTuple<float, float>(0f, num2);
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x000227B0 File Offset: 0x000209B0
		protected override Vector2 ArrangeOverride(Vector2 finalSize)
		{
			UIBox2 uibox;
			uibox..ctor(Vector2.Zero, finalSize);
			float num = this.HasMargins ? 4f : 0f;
			if (this.HasTopEdge)
			{
				uibox += new ValueTuple<float, float, float, float>(0f, num + 2f, 0f, 0f);
			}
			if (this.HasBottomEdge)
			{
				uibox += new ValueTuple<float, float, float, float>(0f, 0f, 0f, -(num + 2f));
			}
			foreach (Control control in base.Children)
			{
				control.Arrange(uibox);
			}
			return finalSize;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0002287C File Offset: 0x00020A7C
		protected override void Draw(DrawingHandleScreen handle)
		{
			UIBox2 uibox = base.PixelSizeBox;
			float num = this.HasMargins ? 4f : 0f;
			if (this.HasTopEdge)
			{
				uibox += new ValueTuple<float, float, float, float>(0f, (num + 2f) * this.UIScale, 0f, 0f);
				handle.DrawRect(new UIBox2(0f, num * this.UIScale, (float)base.PixelWidth, uibox.Top), StripeBack.EdgeColor, true);
			}
			if (this.HasBottomEdge)
			{
				uibox += new ValueTuple<float, float, float, float>(0f, 0f, 0f, -((num + 2f) * this.UIScale));
				handle.DrawRect(new UIBox2(0f, uibox.Bottom, (float)base.PixelWidth, (float)base.PixelHeight - num * this.UIScale), StripeBack.EdgeColor, true);
			}
			StyleBox actualStyleBox = this.GetActualStyleBox();
			if (actualStyleBox == null)
			{
				return;
			}
			actualStyleBox.Draw(handle, uibox);
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x00022980 File Offset: 0x00020B80
		[NullableContext(2)]
		private StyleBox GetActualStyleBox()
		{
			StyleBox result;
			if (!base.TryGetStyleProperty<StyleBox>("background", ref result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x040002F0 RID: 752
		private const float PadSize = 4f;

		// Token: 0x040002F1 RID: 753
		private const float EdgeSize = 2f;

		// Token: 0x040002F2 RID: 754
		private static readonly Color EdgeColor = Color.FromHex("#525252ff", null);

		// Token: 0x040002F3 RID: 755
		private bool _hasTopEdge = true;

		// Token: 0x040002F4 RID: 756
		private bool _hasBottomEdge = true;

		// Token: 0x040002F5 RID: 757
		private bool _hasMargins = true;

		// Token: 0x040002F6 RID: 758
		public const string StylePropertyBackground = "background";
	}
}
