using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Explosion;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client.Explosion
{
	// Token: 0x02000323 RID: 803
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExplosionOverlay : Overlay
	{
		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06001437 RID: 5175 RVA: 0x00005516 File Offset: 0x00003716
		public override OverlaySpace Space
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x00076940 File Offset: 0x00074B40
		public ExplosionOverlay()
		{
			IoCManager.InjectDependencies<ExplosionOverlay>(this);
			this._shader = this._proto.Index<ShaderPrototype>("unshaded").Instance();
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x0007696C File Offset: 0x00074B6C
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			worldHandle.UseShader(this._shader);
			EntityQuery<TransformComponent> entityQuery = this._entMan.GetEntityQuery<TransformComponent>();
			foreach (ValueTuple<ExplosionVisualsComponent, AppearanceComponent> valueTuple in this._entMan.EntityQuery<ExplosionVisualsComponent, AppearanceComponent>(true))
			{
				ExplosionVisualsComponent item = valueTuple.Item1;
				AppearanceComponent item2 = valueTuple.Item2;
				int num;
				if (!(item.Epicenter.MapId != args.MapId) && item2.TryGetData<int>(ExplosionAppearanceData.Progress, ref num))
				{
					num = Math.Min(num, item.Intensity.Count - 1);
					this.DrawExplosion(worldHandle, args.WorldBounds, item, num, entityQuery);
				}
			}
			worldHandle.SetTransform(ref Matrix3.Identity);
			worldHandle.UseShader(null);
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x00076A48 File Offset: 0x00074C48
		private void DrawExplosion(DrawingHandleWorld drawHandle, Box2Rotated worldBounds, ExplosionVisualsComponent exp, int index, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xforms)
		{
			Box2 gridBounds;
			foreach (KeyValuePair<EntityUid, Dictionary<int, List<Vector2i>>> keyValuePair in exp.Tiles)
			{
				EntityUid entityUid;
				Dictionary<int, List<Vector2i>> dictionary;
				keyValuePair.Deconstruct(out entityUid, out dictionary);
				EntityUid value = entityUid;
				Dictionary<int, List<Vector2i>> tileSets = dictionary;
				MapGridComponent mapGridComponent;
				if (this._mapManager.TryGetGrid(new EntityUid?(value), ref mapGridComponent))
				{
					ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = xforms.GetComponent(mapGridComponent.Owner).GetWorldPositionRotationMatrixWithInv(xforms);
					Matrix3 item = worldPositionRotationMatrixWithInv.Item3;
					Matrix3 item2 = worldPositionRotationMatrixWithInv.Item4;
					gridBounds = item2.TransformBox(ref worldBounds).Enlarged((float)(mapGridComponent.TileSize * 2));
					drawHandle.SetTransform(ref item);
					this.DrawTiles(drawHandle, gridBounds, index, tileSets, exp, mapGridComponent.TileSize);
				}
			}
			if (exp.SpaceTiles == null)
			{
				return;
			}
			gridBounds = Matrix3.Invert(ref exp.SpaceMatrix).TransformBox(ref worldBounds).Enlarged(2f);
			drawHandle.SetTransform(ref exp.SpaceMatrix);
			this.DrawTiles(drawHandle, gridBounds, index, exp.SpaceTiles, exp, exp.SpaceTileSize);
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x00076B70 File Offset: 0x00074D70
		private void DrawTiles(DrawingHandleWorld drawHandle, Box2 gridBounds, int index, Dictionary<int, List<Vector2i>> tileSets, ExplosionVisualsComponent exp, ushort tileSize)
		{
			for (int i = 0; i <= index; i++)
			{
				List<Vector2i> list;
				if (tileSets.TryGetValue(i, out list))
				{
					int index2 = (int)Math.Min(exp.Intensity[i] / exp.IntensityPerState, (float)(exp.FireFrames.Count - 1));
					Texture[] array = exp.FireFrames[index2];
					foreach (Vector2i vector2i in list)
					{
						Vector2 vector = (vector2i + 0.5f) * (float)tileSize;
						if (gridBounds.Contains(vector, true))
						{
							Texture texture = RandomExtensions.Pick<Texture>(this._robustRandom, array);
							drawHandle.DrawTextureRect(texture, Box2.CenteredAround(vector, new ValueTuple<float, float>((float)tileSize, (float)tileSize)), exp.FireColor);
						}
					}
				}
			}
		}

		// Token: 0x04000A27 RID: 2599
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000A28 RID: 2600
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000A29 RID: 2601
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000A2A RID: 2602
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x04000A2B RID: 2603
		private ShaderInstance _shader;
	}
}
