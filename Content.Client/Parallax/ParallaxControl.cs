using System;
using System.Runtime.CompilerServices;
using Content.Client.Parallax.Managers;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Parallax
{
	// Token: 0x020001D1 RID: 465
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ParallaxControl : Control
	{
		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000C2D RID: 3117 RVA: 0x00046DE5 File Offset: 0x00044FE5
		// (set) Token: 0x06000C2E RID: 3118 RVA: 0x00046DED File Offset: 0x00044FED
		[ViewVariables]
		public Vector2 Offset { get; set; }

		// Token: 0x06000C2F RID: 3119 RVA: 0x00046DF8 File Offset: 0x00044FF8
		public ParallaxControl()
		{
			IoCManager.InjectDependencies<ParallaxControl>(this);
			this.Offset = new ValueTuple<float, float>((float)this._random.Next(0, 1000), (float)this._random.Next(0, 1000));
			base.RectClipContent = true;
			this._parallaxManager.LoadParallaxByName("FastSpace");
			this._grainShader = this._prototype.Index<ShaderPrototype>("Grain").Instance().Duplicate();
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x00046E7E File Offset: 0x0004507E
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			IRenderTexture buffer = this._buffer;
			if (buffer == null)
			{
				return;
			}
			buffer.Dispose();
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x00046E98 File Offset: 0x00045098
		protected override void Resized()
		{
			base.Resized();
			IRenderTexture buffer = this._buffer;
			if (buffer != null)
			{
				buffer.Dispose();
			}
			this._buffer = this._clyde.CreateRenderTarget(base.PixelSize, 1, null, "parallax");
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x00046EE8 File Offset: 0x000450E8
		protected override void Draw(DrawingHandleScreen handle)
		{
			if (this._buffer == null)
			{
				return;
			}
			handle.RenderInRenderTarget(this._buffer, delegate()
			{
				foreach (ParallaxLayerPrepared parallaxLayerPrepared in this._parallaxManager.GetParallaxLayers("FastSpace"))
				{
					Texture texture = parallaxLayerPrepared.Texture;
					Vector2i vector2i = new ValueTuple<int, int>(texture.Size.X * (int)this.Size.X, texture.Size.Y * (int)this.Size.X) * parallaxLayerPrepared.Config.Scale.Floored() / 1920;
					Vector2i pixelSize = this.PixelSize;
					float num = (float)this._timing.RealTime.TotalSeconds;
					Vector2 vector = this.Offset + new Vector2(num * 100f, num * 0f);
					if (parallaxLayerPrepared.Config.Tiled)
					{
						Vector2i vector2i2 = (vector * parallaxLayerPrepared.Config.Slowness).Floored();
						vector2i2.X %= vector2i.X;
						vector2i2.Y %= vector2i.Y;
						for (int j = -vector2i2.X; j < pixelSize.X; j += vector2i.X)
						{
							for (int k = -vector2i2.Y; k < pixelSize.Y; k += vector2i.Y)
							{
								handle.DrawTextureRect(texture, UIBox2.FromDimensions(new ValueTuple<float, float>((float)j, (float)k), vector2i), null);
							}
						}
					}
					else
					{
						Vector2 vector2 = (pixelSize - vector2i) / 2 + parallaxLayerPrepared.Config.ControlHomePosition;
						handle.DrawTextureRect(texture, UIBox2.FromDimensions(vector2, vector2i), null);
					}
				}
			}, new Color?(Color.Transparent));
			this._grainShader.SetParameter("SCREEN_TEXTURE", this._buffer.Texture);
			this._grainShader.SetParameter("strength", 100f);
			handle.UseShader(this._grainShader);
			handle.DrawTextureRect(this._buffer.Texture, base.PixelSizeBox, null);
			handle.UseShader(null);
		}

		// Token: 0x040005E5 RID: 1509
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040005E6 RID: 1510
		[Dependency]
		private readonly IParallaxManager _parallaxManager;

		// Token: 0x040005E7 RID: 1511
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040005E8 RID: 1512
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x040005E9 RID: 1513
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x040005EA RID: 1514
		[Nullable(2)]
		private IRenderTexture _buffer;

		// Token: 0x040005EB RID: 1515
		private ShaderInstance _grainShader;
	}
}
