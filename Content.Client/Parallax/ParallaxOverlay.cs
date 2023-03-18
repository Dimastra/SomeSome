using System;
using System.Runtime.CompilerServices;
using Content.Client.Parallax.Managers;
using Content.Shared.CCVar;
using Content.Shared.Parallax.Biomes;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Parallax
{
	// Token: 0x020001DB RID: 475
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ParallaxOverlay : Overlay
	{
		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000C4B RID: 3147 RVA: 0x00048063 File Offset: 0x00046263
		public override OverlaySpace Space
		{
			get
			{
				return 128;
			}
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x0004806A File Offset: 0x0004626A
		public ParallaxOverlay()
		{
			base.ZIndex = new int?(0);
			IoCManager.InjectDependencies<ParallaxOverlay>(this);
			this._parallax = this._entManager.System<ParallaxSystem>();
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x00048096 File Offset: 0x00046296
		protected override bool BeforeDraw(in OverlayDrawArgs args)
		{
			return !(args.MapId == MapId.Nullspace) && !this._entManager.HasComponent<BiomeComponent>(this._mapManager.GetMapEntityId(args.MapId));
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x000480CC File Offset: 0x000462CC
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (args.MapId == MapId.Nullspace)
			{
				return;
			}
			if (!this._configurationManager.GetCVar<bool>(CCVars.ParallaxEnabled))
			{
				return;
			}
			IEye eye = args.Viewport.Eye;
			Vector2 vector = (eye != null) ? eye.Position.Position : Vector2.Zero;
			DrawingHandleWorld worldHandle = args.WorldHandle;
			ParallaxLayerPrepared[] parallaxLayers = this._parallax.GetParallaxLayers(args.MapId);
			float num = (float)this._timing.RealTime.TotalSeconds;
			foreach (ParallaxLayerPrepared parallaxLayerPrepared in parallaxLayers)
			{
				ShaderInstance shaderInstance;
				if (!string.IsNullOrEmpty(parallaxLayerPrepared.Config.Shader))
				{
					shaderInstance = this._prototypeManager.Index<ShaderPrototype>(parallaxLayerPrepared.Config.Shader).Instance();
				}
				else
				{
					shaderInstance = null;
				}
				worldHandle.UseShader(shaderInstance);
				Texture texture = parallaxLayerPrepared.Texture;
				Vector2 vector2 = texture.Size / 32f * parallaxLayerPrepared.Config.Scale;
				Vector2 vector3 = parallaxLayerPrepared.Config.WorldHomePosition + this._manager.ParallaxAnchor;
				Vector2 vector4 = parallaxLayerPrepared.Config.Scrolling * num;
				Vector2 vector5 = (vector - vector3) * parallaxLayerPrepared.Config.Slowness + vector4;
				vector5 += vector3;
				vector5 += parallaxLayerPrepared.Config.WorldAdjustPosition;
				vector5 -= vector2 / 2f;
				if (parallaxLayerPrepared.Config.Tiled)
				{
					Vector2 vector6 = args.WorldAABB.BottomLeft - vector5;
					vector6 = (vector6 / vector2).Floored() * vector2;
					vector6 += vector5;
					for (float num2 = vector6.X; num2 < args.WorldAABB.Right; num2 += vector2.X)
					{
						for (float num3 = vector6.Y; num3 < args.WorldAABB.Top; num3 += vector2.Y)
						{
							worldHandle.DrawTextureRect(texture, Box2.FromDimensions(new ValueTuple<float, float>(num2, num3), vector2), null);
						}
					}
				}
				else
				{
					worldHandle.DrawTextureRect(texture, Box2.FromDimensions(vector5, vector2), null);
				}
			}
			worldHandle.UseShader(null);
		}

		// Token: 0x0400060F RID: 1551
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000610 RID: 1552
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000611 RID: 1553
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000612 RID: 1554
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000613 RID: 1555
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000614 RID: 1556
		[Dependency]
		private readonly IParallaxManager _manager;

		// Token: 0x04000615 RID: 1557
		private readonly ParallaxSystem _parallax;
	}
}
