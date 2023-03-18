using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Viewport
{
	// Token: 0x02000057 RID: 87
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class ScalingViewport : Control, IViewportControl
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000190 RID: 400 RVA: 0x0000BFBD File Offset: 0x0000A1BD
		public int CurrentRenderScale
		{
			get
			{
				return this._curRenderScale;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000BFC5 File Offset: 0x0000A1C5
		// (set) Token: 0x06000192 RID: 402 RVA: 0x0000BFCD File Offset: 0x0000A1CD
		[Nullable(2)]
		public IEye Eye
		{
			[NullableContext(2)]
			get
			{
				return this._eye;
			}
			[NullableContext(2)]
			set
			{
				this._eye = value;
				if (this._viewport != null)
				{
					this._viewport.Eye = value;
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000193 RID: 403 RVA: 0x0000BFEA File Offset: 0x0000A1EA
		// (set) Token: 0x06000194 RID: 404 RVA: 0x0000BFF2 File Offset: 0x0000A1F2
		public Vector2i ViewportSize
		{
			get
			{
				return this._viewportSize;
			}
			set
			{
				this._viewportSize = value;
				this.InvalidateViewport();
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000C001 File Offset: 0x0000A201
		// (set) Token: 0x06000196 RID: 406 RVA: 0x0000C009 File Offset: 0x0000A209
		[ViewVariables]
		public Vector2i? FixedStretchSize { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000C012 File Offset: 0x0000A212
		// (set) Token: 0x06000198 RID: 408 RVA: 0x0000C01A File Offset: 0x0000A21A
		[ViewVariables]
		public ScalingViewportStretchMode StretchMode
		{
			get
			{
				return this._stretchMode;
			}
			set
			{
				this._stretchMode = value;
				this.InvalidateViewport();
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000C029 File Offset: 0x0000A229
		// (set) Token: 0x0600019A RID: 410 RVA: 0x0000C031 File Offset: 0x0000A231
		[ViewVariables]
		public ScalingViewportRenderScaleMode RenderScaleMode
		{
			get
			{
				return this._renderScaleMode;
			}
			set
			{
				this._renderScaleMode = value;
				this.InvalidateViewport();
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000C040 File Offset: 0x0000A240
		// (set) Token: 0x0600019C RID: 412 RVA: 0x0000C048 File Offset: 0x0000A248
		[ViewVariables]
		public int FixedRenderScale
		{
			get
			{
				return this._fixedRenderScale;
			}
			set
			{
				this._fixedRenderScale = value;
				this.InvalidateViewport();
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000C057 File Offset: 0x0000A257
		public ScalingViewport()
		{
			IoCManager.InjectDependencies<ScalingViewport>(this);
			base.RectClipContent = true;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000C07F File Offset: 0x0000A27F
		protected override void KeyBindDown(GUIBoundKeyEventArgs args)
		{
			base.KeyBindDown(args);
			if (args.Handled)
			{
				return;
			}
			this._inputManager.ViewportKeyEvent(this, args);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000C09E File Offset: 0x0000A29E
		protected override void KeyBindUp(GUIBoundKeyEventArgs args)
		{
			base.KeyBindUp(args);
			if (args.Handled)
			{
				return;
			}
			this._inputManager.ViewportKeyEvent(this, args);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000C0C0 File Offset: 0x0000A2C0
		protected override void Draw(DrawingHandleScreen handle)
		{
			this.EnsureViewportCreated();
			this._viewport.Render();
			if (this._queuedScreenshots.Count != 0)
			{
				CopyPixelsDelegate<Rgba32>[] callbacks = this._queuedScreenshots.ToArray();
				this._viewport.RenderTarget.CopyPixelsToMemory<Rgba32>(delegate(Image<Rgba32> image)
				{
					CopyPixelsDelegate<Rgba32>[] callbacks = callbacks;
					for (int i = 0; i < callbacks.Length; i++)
					{
						callbacks[i].Invoke(image);
					}
				}, null);
				this._queuedScreenshots.Clear();
			}
			UIBox2i drawBox = this.GetDrawBox();
			UIBox2i uibox2i = drawBox.Translated(base.GlobalPixelPosition);
			this._viewport.RenderScreenOverlaysBelow(handle, this, ref uibox2i);
			handle.DrawTextureRect(this._viewport.RenderTarget.Texture, drawBox, null);
			this._viewport.RenderScreenOverlaysAbove(handle, this, ref uibox2i);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000C18B File Offset: 0x0000A38B
		public void Screenshot(CopyPixelsDelegate<Rgba32> callback)
		{
			this._queuedScreenshots.Add(callback);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000C19C File Offset: 0x0000A39C
		private UIBox2i GetDrawBox()
		{
			Vector2i size = this._viewport.Size;
			Vector2 vector = base.PixelSize;
			if (this.FixedStretchSize == null)
			{
				float num;
				float num2;
				(vector / size).Deconstruct(ref num, ref num2);
				float val = num;
				float val2 = num2;
				float num3 = Math.Min(val, val2);
				Vector2 vector2 = size * num3;
				return (UIBox2i)UIBox2.FromDimensions((vector - vector2) / 2f, vector2);
			}
			return (UIBox2i)UIBox2.FromDimensions((vector - this.FixedStretchSize.Value) / 2f, this.FixedStretchSize.Value);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000C260 File Offset: 0x0000A460
		private void RegenerateViewport()
		{
			Vector2i viewportSize = this.ViewportSize;
			float num;
			float num2;
			(base.PixelSize / viewportSize).Deconstruct(ref num, ref num2);
			float val = num;
			float val2 = num2;
			float num3 = Math.Min(val, val2);
			int num4 = 1;
			switch (this._renderScaleMode)
			{
			case ScalingViewportRenderScaleMode.Fixed:
				num4 = this._fixedRenderScale;
				break;
			case ScalingViewportRenderScaleMode.FloorInt:
				num4 = (int)Math.Floor((double)num3);
				break;
			case ScalingViewportRenderScaleMode.CeilInt:
				num4 = (int)Math.Ceiling((double)num3);
				break;
			}
			num4 = Math.Max(1, num4);
			this._curRenderScale = num4;
			IClyde clyde = this._clyde;
			Vector2i vector2i = this.ViewportSize * num4;
			TextureSampleParameters value = default(TextureSampleParameters);
			value.Filter = (this.StretchMode == ScalingViewportStretchMode.Bilinear);
			this._viewport = clyde.CreateViewport(vector2i, new TextureSampleParameters?(value), null);
			this._viewport.RenderScale = new ValueTuple<float, float>((float)num4, (float)num4);
			this._viewport.Eye = this._eye;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000C355 File Offset: 0x0000A555
		protected override void Resized()
		{
			base.Resized();
			this.InvalidateViewport();
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000C363 File Offset: 0x0000A563
		private void InvalidateViewport()
		{
			IClydeViewport viewport = this._viewport;
			if (viewport != null)
			{
				viewport.Dispose();
			}
			this._viewport = null;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000C380 File Offset: 0x0000A580
		public MapCoordinates ScreenToMap(Vector2 coords)
		{
			if (this._eye == null)
			{
				return default(MapCoordinates);
			}
			this.EnsureViewportCreated();
			Matrix3 localToScreenMatrix = this.GetLocalToScreenMatrix();
			Matrix3 matrix = Matrix3.Invert(ref localToScreenMatrix);
			return this._viewport.LocalToWorld(matrix.Transform(coords));
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000C3C8 File Offset: 0x0000A5C8
		public Vector2 WorldToScreen(Vector2 map)
		{
			if (this._eye == null)
			{
				return default(Vector2);
			}
			this.EnsureViewportCreated();
			Vector2 vector = this._viewport.WorldToLocal(map);
			return this.GetLocalToScreenMatrix().Transform(vector);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000C40C File Offset: 0x0000A60C
		public Matrix3 GetWorldToScreenMatrix()
		{
			this.EnsureViewportCreated();
			Matrix3 worldToLocalMatrix = this._viewport.GetWorldToLocalMatrix();
			Matrix3 localToScreenMatrix = this.GetLocalToScreenMatrix();
			return ref worldToLocalMatrix * ref localToScreenMatrix;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000C43C File Offset: 0x0000A63C
		public Matrix3 GetLocalToScreenMatrix()
		{
			this.EnsureViewportCreated();
			UIBox2i drawBox = this.GetDrawBox();
			Vector2 vector = drawBox.Size / this._viewport.Size;
			if (vector.X == 0f || vector.Y == 0f)
			{
				return Matrix3.Identity;
			}
			Vector2 vector2 = base.GlobalPixelPosition + drawBox.TopLeft;
			Angle angle = 0f;
			return Matrix3.CreateTransform(ref vector2, ref angle, ref vector);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000C4C5 File Offset: 0x0000A6C5
		private void EnsureViewportCreated()
		{
			if (this._viewport == null)
			{
				this.RegenerateViewport();
			}
		}

		// Token: 0x04000104 RID: 260
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x04000105 RID: 261
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000106 RID: 262
		[Nullable(2)]
		private IClydeViewport _viewport;

		// Token: 0x04000107 RID: 263
		[Nullable(2)]
		private IEye _eye;

		// Token: 0x04000108 RID: 264
		private Vector2i _viewportSize;

		// Token: 0x04000109 RID: 265
		private int _curRenderScale;

		// Token: 0x0400010A RID: 266
		private ScalingViewportStretchMode _stretchMode;

		// Token: 0x0400010B RID: 267
		private ScalingViewportRenderScaleMode _renderScaleMode;

		// Token: 0x0400010C RID: 268
		private int _fixedRenderScale = 1;

		// Token: 0x0400010D RID: 269
		private readonly List<CopyPixelsDelegate<Rgba32>> _queuedScreenshots = new List<CopyPixelsDelegate<Rgba32>>();
	}
}
