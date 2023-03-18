using System;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Atmos.Overlays
{
	// Token: 0x02000446 RID: 1094
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasTileOverlay : Overlay
	{
		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06001B03 RID: 6915 RVA: 0x0009B80C File Offset: 0x00099A0C
		public override OverlaySpace Space
		{
			get
			{
				return 16;
			}
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x0009B810 File Offset: 0x00099A10
		public GasTileOverlay(GasTileOverlaySystem system, IEntityManager entManager, IResourceCache resourceCache, IPrototypeManager protoMan, SpriteSystem spriteSys)
		{
			this._entManager = entManager;
			this._mapManager = IoCManager.Resolve<IMapManager>();
			this._shader = protoMan.Index<ShaderPrototype>("unshaded").Instance();
			base.ZIndex = new int?(7);
			this._gasCount = system.VisibleGasId.Length;
			this._timer = new float[this._gasCount];
			this._frameDelays = new float[this._gasCount][];
			this._frameCounter = new int[this._gasCount];
			this._frames = new Texture[this._gasCount][];
			int i = 0;
			while (i < this._gasCount)
			{
				GasPrototype gasPrototype = protoMan.Index<GasPrototype>(system.VisibleGasId[i].ToString());
				SpriteSpecifier spriteSpecifier;
				if (!string.IsNullOrEmpty(gasPrototype.GasOverlaySprite) && !string.IsNullOrEmpty(gasPrototype.GasOverlayState))
				{
					spriteSpecifier = new SpriteSpecifier.Rsi(new ResourcePath(gasPrototype.GasOverlaySprite, "/"), gasPrototype.GasOverlayState);
					goto IL_13B;
				}
				if (!string.IsNullOrEmpty(gasPrototype.GasOverlayTexture))
				{
					spriteSpecifier = new SpriteSpecifier.Texture(new ResourcePath(gasPrototype.GasOverlayTexture, "/"));
					goto IL_13B;
				}
				IL_1D2:
				i++;
				continue;
				IL_13B:
				SpriteSpecifier.Rsi rsi = spriteSpecifier as SpriteSpecifier.Rsi;
				if (rsi == null)
				{
					SpriteSpecifier.Texture texture = spriteSpecifier as SpriteSpecifier.Texture;
					if (texture == null)
					{
						goto IL_1D2;
					}
					this._frames[i] = new Texture[]
					{
						spriteSys.Frame0(texture)
					};
					this._frameDelays[i] = Array.Empty<float>();
					goto IL_1D2;
				}
				else
				{
					RSI rsi2 = resourceCache.GetResource<RSIResource>(rsi.RsiPath, true).RSI;
					string rsiState = rsi.RsiState;
					RSI.State state;
					if (rsi2.TryGetState(rsiState, ref state))
					{
						this._frames[i] = state.GetFrames(0);
						this._frameDelays[i] = state.GetDelays();
						this._frameCounter[i] = 0;
						goto IL_1D2;
					}
					goto IL_1D2;
				}
			}
			RSI rsi3 = resourceCache.GetResource<RSIResource>("/Textures/Effects/fire.rsi", true).RSI;
			for (int j = 0; j < 3; j++)
			{
				RSI.State state2;
				if (!rsi3.TryGetState((j + 1).ToString(), ref state2))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Fire RSI doesn't have state \"");
					defaultInterpolatedStringHandler.AppendFormatted<int>(j);
					defaultInterpolatedStringHandler.AppendLiteral("\"!");
					throw new ArgumentOutOfRangeException(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				this._fireFrames[j] = state2.GetFrames(0);
				this._fireFrameDelays[j] = state2.GetDelays();
				this._fireFrameCounter[j] = 0;
			}
		}

		// Token: 0x06001B05 RID: 6917 RVA: 0x0009BAA8 File Offset: 0x00099CA8
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			for (int i = 0; i < this._gasCount; i++)
			{
				float[] array = this._frameDelays[i];
				if (array.Length != 0)
				{
					int num = this._frameCounter[i];
					this._timer[i] += args.DeltaSeconds;
					float num2 = array[num];
					if (this._timer[i] >= num2)
					{
						this._timer[i] -= num2;
						this._frameCounter[i] = (num + 1) % this._frames[i].Length;
					}
				}
			}
			for (int j = 0; j < 3; j++)
			{
				float[] array2 = this._fireFrameDelays[j];
				if (array2.Length != 0)
				{
					int num3 = this._fireFrameCounter[j];
					this._fireTimer[j] += args.DeltaSeconds;
					float num4 = array2[num3];
					if (this._fireTimer[j] >= num4)
					{
						this._fireTimer[j] -= num4;
						this._fireFrameCounter[j] = (num3 + 1) % this._fireFrames[j].Length;
					}
				}
			}
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x0009BBB4 File Offset: 0x00099DB4
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
			EntityQuery<GasTileOverlayComponent> entityQuery2 = this._entManager.GetEntityQuery<GasTileOverlayComponent>();
			foreach (MapGridComponent mapGridComponent in this._mapManager.FindGridsIntersecting(args.MapId, args.WorldBounds, false))
			{
				GasTileOverlayComponent gasTileOverlayComponent;
				TransformComponent transformComponent;
				if (entityQuery2.TryGetComponent(mapGridComponent.Owner, ref gasTileOverlayComponent) && entityQuery.TryGetComponent(mapGridComponent.Owner, ref transformComponent))
				{
					ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = transformComponent.GetWorldPositionRotationMatrixWithInv();
					Matrix3 item = worldPositionRotationMatrixWithInv.Item3;
					Matrix3 item2 = worldPositionRotationMatrixWithInv.Item4;
					worldHandle.SetTransform(ref item);
					Box2 box = item2.TransformBox(ref args.WorldBounds).Enlarged((float)mapGridComponent.TileSize);
					Box2i box2i;
					box2i..ctor((int)MathF.Floor(box.Left), (int)MathF.Floor(box.Bottom), (int)MathF.Ceiling(box.Right), (int)MathF.Ceiling(box.Top));
					worldHandle.UseShader(null);
					foreach (GasOverlayChunk gasOverlayChunk in gasTileOverlayComponent.Chunks.Values)
					{
						GasChunkEnumerator gasChunkEnumerator = new GasChunkEnumerator(gasOverlayChunk);
						SharedGasTileOverlaySystem.GasOverlayData gasOverlayData;
						while (gasChunkEnumerator.MoveNext(out gasOverlayData))
						{
							if (gasOverlayData.Opacity != null)
							{
								Vector2i vector2i = gasOverlayChunk.Origin + new ValueTuple<int, int>(gasChunkEnumerator.X, gasChunkEnumerator.Y);
								if (box2i.Contains(vector2i, true))
								{
									for (int i = 0; i < this._gasCount; i++)
									{
										byte b = gasOverlayData.Opacity[i];
										if (b > 0)
										{
											worldHandle.DrawTexture(this._frames[i][this._frameCounter[i]], vector2i, new Color?(Color.White.WithAlpha(b)));
										}
									}
								}
							}
						}
					}
					worldHandle.UseShader(this._shader);
					foreach (GasOverlayChunk gasOverlayChunk2 in gasTileOverlayComponent.Chunks.Values)
					{
						GasChunkEnumerator gasChunkEnumerator2 = new GasChunkEnumerator(gasOverlayChunk2);
						SharedGasTileOverlaySystem.GasOverlayData gasOverlayData2;
						while (gasChunkEnumerator2.MoveNext(out gasOverlayData2))
						{
							if (gasOverlayData2.FireState != 0)
							{
								Vector2i vector2i2 = gasOverlayChunk2.Origin + new ValueTuple<int, int>(gasChunkEnumerator2.X, gasChunkEnumerator2.Y);
								if (box2i.Contains(vector2i2, true))
								{
									int num = (int)(gasOverlayData2.FireState - 1);
									Texture texture = this._fireFrames[num][this._fireFrameCounter[num]];
									worldHandle.DrawTexture(texture, vector2i2, null);
								}
							}
						}
					}
				}
			}
			worldHandle.UseShader(null);
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x04000D8A RID: 3466
		private readonly IEntityManager _entManager;

		// Token: 0x04000D8B RID: 3467
		private readonly IMapManager _mapManager;

		// Token: 0x04000D8C RID: 3468
		private readonly ShaderInstance _shader;

		// Token: 0x04000D8D RID: 3469
		private readonly float[] _timer;

		// Token: 0x04000D8E RID: 3470
		private readonly float[][] _frameDelays;

		// Token: 0x04000D8F RID: 3471
		private readonly int[] _frameCounter;

		// Token: 0x04000D90 RID: 3472
		private readonly Texture[][] _frames;

		// Token: 0x04000D91 RID: 3473
		private const int FireStates = 3;

		// Token: 0x04000D92 RID: 3474
		private const string FireRsiPath = "/Textures/Effects/fire.rsi";

		// Token: 0x04000D93 RID: 3475
		private readonly float[] _fireTimer = new float[3];

		// Token: 0x04000D94 RID: 3476
		private readonly float[][] _fireFrameDelays = new float[3][];

		// Token: 0x04000D95 RID: 3477
		private readonly int[] _fireFrameCounter = new int[3];

		// Token: 0x04000D96 RID: 3478
		private readonly Texture[][] _fireFrames = new Texture[3][];

		// Token: 0x04000D97 RID: 3479
		private int _gasCount;

		// Token: 0x04000D98 RID: 3480
		public const int GasOverlayZIndex = 7;
	}
}
