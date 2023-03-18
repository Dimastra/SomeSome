using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Weather;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.Utility;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Weather
{
	// Token: 0x02000028 RID: 40
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WeatherOverlay : Overlay
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00005516 File Offset: 0x00003716
		public override OverlaySpace Space
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005BC9 File Offset: 0x00003DC9
		public WeatherOverlay(SharedTransformSystem transform, SpriteSystem sprite, WeatherSystem weather)
		{
			base.ZIndex = new int?(1);
			this._transform = transform;
			this._weather = weather;
			this._sprite = sprite;
			IoCManager.InjectDependencies<WeatherOverlay>(this);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005BFC File Offset: 0x00003DFC
		protected override bool BeforeDraw(in OverlayDrawArgs args)
		{
			WeatherComponent weatherComponent;
			return !(args.MapId == MapId.Nullspace) && this._entManager.TryGetComponent<WeatherComponent>(this._mapManager.GetMapEntityId(args.MapId), ref weatherComponent) && weatherComponent.Weather != null && base.BeforeDraw(ref args);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005C50 File Offset: 0x00003E50
		protected override void Draw(in OverlayDrawArgs args)
		{
			EntityUid mapEntityId = this._mapManager.GetMapEntityId(args.MapId);
			WeatherComponent weatherComponent;
			WeatherPrototype weatherProto;
			if (!this._entManager.TryGetComponent<WeatherComponent>(mapEntityId, ref weatherComponent) || weatherComponent.Weather == null || !this._protoManager.TryIndex<WeatherPrototype>(weatherComponent.Weather, ref weatherProto))
			{
				return;
			}
			float percent = this._weather.GetPercent(weatherComponent, mapEntityId, weatherProto);
			this.DrawWorld(args, weatherProto, percent);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005CB8 File Offset: 0x00003EB8
		private void DrawWorld(in OverlayDrawArgs args, WeatherPrototype weatherProto, float alpha)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			MapId mapId = args.MapId;
			Box2 worldAABB = args.WorldAABB;
			Box2Rotated worldBounds = args.WorldBounds;
			Matrix3 invMatrix = args.Viewport.GetWorldToLocalMatrix();
			IEye eye = args.Viewport.Eye;
			Vector2 vector = (eye != null) ? eye.Position.Position : Vector2.Zero;
			IRenderTexture blep = this._blep;
			Vector2i? vector2i = (blep != null) ? new Vector2i?(blep.Texture.Size) : null;
			Vector2i size = args.Viewport.Size;
			if (vector2i == null || (vector2i != null && vector2i.GetValueOrDefault() != size))
			{
				IRenderTexture blep2 = this._blep;
				if (blep2 != null)
				{
					blep2.Dispose();
				}
				this._blep = this._clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters(1, false), null, "weather-stencil");
			}
			worldHandle.RenderInRenderTarget(this._blep, delegate()
			{
				EntityQuery<PhysicsComponent> entityQuery = this._entManager.GetEntityQuery<PhysicsComponent>();
				EntityQuery<TransformComponent> entityQuery2 = this._entManager.GetEntityQuery<TransformComponent>();
				foreach (MapGridComponent mapGridComponent in this._mapManager.FindGridsIntersecting(mapId, worldAABB, false))
				{
					Matrix3 worldMatrix = this._transform.GetWorldMatrix(mapGridComponent.Owner, entityQuery2);
					Matrix3 matrix;
					Matrix3.Multiply(ref worldMatrix, ref invMatrix, ref matrix);
					worldHandle.SetTransform(ref matrix);
					foreach (TileRef tileRef in mapGridComponent.GetTilesIntersecting(worldAABB, true, null))
					{
						if (!this._weather.CanWeatherAffect(mapGridComponent, tileRef, entityQuery))
						{
							Box2 box2;
							box2..ctor(tileRef.GridIndices * (int)mapGridComponent.TileSize, (tileRef.GridIndices + Vector2i.One) * (int)mapGridComponent.TileSize);
							worldHandle.DrawRect(box2, Color.White, true);
						}
					}
				}
			}, new Color?(Color.Transparent));
			worldHandle.SetTransform(ref Matrix3.Identity);
			worldHandle.UseShader(this._protoManager.Index<ShaderPrototype>("StencilMask").Instance());
			worldHandle.DrawTextureRect(this._blep.Texture, ref worldBounds, null);
			Texture texture = null;
			TimeSpan realTime = this._timing.RealTime;
			SpriteSpecifier sprite = weatherProto.Sprite;
			SpriteSpecifier.Rsi rsi = sprite as SpriteSpecifier.Rsi;
			if (rsi == null)
			{
				SpriteSpecifier.Texture texture2 = sprite as SpriteSpecifier.Texture;
				if (texture2 == null)
				{
					throw new NotImplementedException();
				}
				texture = SpriteSpecifierExt.GetTexture(texture2, this._cache);
			}
			else
			{
				RSI.State state;
				this._cache.GetResource<RSIResource>(rsi.RsiPath, true).RSI.TryGetState(rsi.RsiState, ref state);
				Texture[] frames = state.GetFrames(0);
				float[] delays = state.GetDelays();
				float num = delays.Sum();
				double num2 = realTime.TotalSeconds % (double)num;
				float num3 = 0f;
				for (int i = 0; i < delays.Length; i++)
				{
					float num4 = delays[i];
					num3 += num4;
					if (num2 <= (double)num3)
					{
						texture = frames[i];
						break;
					}
				}
				if (texture == null)
				{
					texture = this._sprite.Frame0(weatherProto.Sprite);
				}
			}
			worldHandle.UseShader(this._protoManager.Index<ShaderPrototype>("StencilDraw").Instance());
			Vector2 zero = Vector2.Zero;
			Vector2 vector2 = texture.Size / 32f * 1f;
			Vector2 vector3 = zero * (float)realTime.TotalSeconds;
			Vector2 vector4 = vector * 0f + vector3;
			vector4 -= vector2 / 2f;
			Vector2 vector5 = args.WorldAABB.BottomLeft - vector4;
			vector5 = (vector5 / vector2).Floored() * vector2;
			vector5 += vector4;
			for (float num5 = vector5.X; num5 < args.WorldAABB.Right; num5 += vector2.X)
			{
				for (float num6 = vector5.Y; num6 < args.WorldAABB.Top; num6 += vector2.Y)
				{
					Box2 box = Box2.FromDimensions(new ValueTuple<float, float>(num5, num6), vector2);
					worldHandle.DrawTextureRect(texture, box, new Color?((weatherProto.Color ?? Color.White).WithAlpha(alpha)));
				}
			}
			worldHandle.SetTransform(ref Matrix3.Identity);
			worldHandle.UseShader(null);
		}

		// Token: 0x0400005F RID: 95
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x04000060 RID: 96
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000061 RID: 97
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000062 RID: 98
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000063 RID: 99
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x04000064 RID: 100
		[Dependency]
		private readonly IResourceCache _cache;

		// Token: 0x04000065 RID: 101
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000066 RID: 102
		private readonly SpriteSystem _sprite;

		// Token: 0x04000067 RID: 103
		private readonly WeatherSystem _weather;

		// Token: 0x04000068 RID: 104
		[Nullable(2)]
		private IRenderTexture _blep;
	}
}
