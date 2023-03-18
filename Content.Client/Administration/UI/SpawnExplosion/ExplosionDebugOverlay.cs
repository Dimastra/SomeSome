using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.SpawnExplosion
{
	// Token: 0x020004B1 RID: 1201
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExplosionDebugOverlay : Overlay
	{
		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06001DF5 RID: 7669 RVA: 0x000392B9 File Offset: 0x000374B9
		public override OverlaySpace Space
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x06001DF6 RID: 7670 RVA: 0x000AFB64 File Offset: 0x000ADD64
		public ExplosionDebugOverlay()
		{
			IoCManager.InjectDependencies<ExplosionDebugOverlay>(this);
			IResourceCache resourceCache = IoCManager.Resolve<IResourceCache>();
			this._font = new VectorFont(resourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
		}

		// Token: 0x06001DF7 RID: 7671 RVA: 0x000AFBB4 File Offset: 0x000ADDB4
		protected override void Draw(in OverlayDrawArgs args)
		{
			MapId map = this.Map;
			IEye eye = args.Viewport.Eye;
			if (map != ((eye != null) ? new MapId?(eye.Position.MapId) : null))
			{
				return;
			}
			if (this.Tiles.Count == 0 && this.SpaceTiles == null)
			{
				return;
			}
			OverlaySpace space = args.Space;
			if (space == 2)
			{
				this.DrawScreen(args);
				return;
			}
			if (space != 4)
			{
				return;
			}
			this.DrawWorld(args);
		}

		// Token: 0x06001DF8 RID: 7672 RVA: 0x000AFC48 File Offset: 0x000ADE48
		private void DrawScreen(OverlayDrawArgs args)
		{
			DrawingHandleScreen screenHandle = args.ScreenHandle;
			EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
			Box2 gridBounds;
			foreach (KeyValuePair<EntityUid, Dictionary<int, List<Vector2i>>> keyValuePair in this.Tiles)
			{
				EntityUid entityUid;
				Dictionary<int, List<Vector2i>> dictionary;
				keyValuePair.Deconstruct(out entityUid, out dictionary);
				EntityUid value = entityUid;
				Dictionary<int, List<Vector2i>> tileSets = dictionary;
				MapGridComponent mapGridComponent;
				if (this._mapManager.TryGetGrid(new EntityUid?(value), ref mapGridComponent))
				{
					ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = entityQuery.GetComponent(mapGridComponent.Owner).GetWorldPositionRotationMatrixWithInv(entityQuery);
					Matrix3 item = worldPositionRotationMatrixWithInv.Item3;
					Matrix3 item2 = worldPositionRotationMatrixWithInv.Item4;
					gridBounds = item2.TransformBox(ref args.WorldBounds).Enlarged((float)(mapGridComponent.TileSize * 2));
					this.DrawText(screenHandle, gridBounds, item, tileSets, mapGridComponent.TileSize);
				}
			}
			if (this.SpaceTiles == null)
			{
				return;
			}
			gridBounds = Matrix3.Invert(ref this.SpaceMatrix).TransformBox(ref args.WorldBounds);
			this.DrawText(screenHandle, gridBounds, this.SpaceMatrix, this.SpaceTiles, this.SpaceTileSize);
		}

		// Token: 0x06001DF9 RID: 7673 RVA: 0x000AFD70 File Offset: 0x000ADF70
		private void DrawText(DrawingHandleScreen handle, Box2 gridBounds, Matrix3 transform, Dictionary<int, List<Vector2i>> tileSets, ushort tileSize)
		{
			for (int i = 1; i < this.Intensity.Count; i++)
			{
				List<Vector2i> list;
				if (tileSets.TryGetValue(i, out list))
				{
					foreach (Vector2i vector2i in list)
					{
						Vector2 vector = (vector2i + 0.5f) * (float)tileSize;
						if (gridBounds.Contains(vector, true))
						{
							Vector2 vector2 = transform.Transform(vector);
							Vector2 vector3 = this._eyeManager.WorldToScreen(vector2);
							if (this.Intensity[i] > 9f)
							{
								vector3 += new ValueTuple<float, float>(-12f, -8f);
							}
							else
							{
								vector3 += new ValueTuple<float, float>(-8f, -8f);
							}
							handle.DrawString(this._font, vector3, this.Intensity[i].ToString("F2"));
						}
					}
				}
			}
			if (tileSets.ContainsKey(0))
			{
				Vector2i vector2i2 = tileSets[0].First<Vector2i>();
				Vector2 vector4 = transform.Transform((vector2i2 + 0.5f) * (float)tileSize);
				Vector2 vector5 = this._eyeManager.WorldToScreen(vector4) + new ValueTuple<float, float>(-24f, -24f);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
				defaultInterpolatedStringHandler.AppendFormatted<float>(this.Intensity[0], "F2");
				defaultInterpolatedStringHandler.AppendLiteral("\nΣ=");
				defaultInterpolatedStringHandler.AppendFormatted<float>(this.TotalIntensity, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("\nΔ=");
				defaultInterpolatedStringHandler.AppendFormatted<float>(this.Slope, "F1");
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				handle.DrawString(this._font, vector5, text);
			}
		}

		// Token: 0x06001DFA RID: 7674 RVA: 0x000AFF7C File Offset: 0x000AE17C
		private void DrawWorld(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
			Box2 gridBounds;
			foreach (KeyValuePair<EntityUid, Dictionary<int, List<Vector2i>>> keyValuePair in this.Tiles)
			{
				EntityUid entityUid;
				Dictionary<int, List<Vector2i>> dictionary;
				keyValuePair.Deconstruct(out entityUid, out dictionary);
				EntityUid value = entityUid;
				Dictionary<int, List<Vector2i>> tileSets = dictionary;
				MapGridComponent mapGridComponent;
				if (this._mapManager.TryGetGrid(new EntityUid?(value), ref mapGridComponent))
				{
					ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = entityQuery.GetComponent(mapGridComponent.Owner).GetWorldPositionRotationMatrixWithInv(entityQuery);
					Matrix3 item = worldPositionRotationMatrixWithInv.Item3;
					Matrix3 item2 = worldPositionRotationMatrixWithInv.Item4;
					gridBounds = item2.TransformBox(ref args.WorldBounds).Enlarged((float)(mapGridComponent.TileSize * 2));
					worldHandle.SetTransform(ref item);
					this.DrawTiles(worldHandle, gridBounds, tileSets, this.SpaceTileSize);
				}
			}
			if (this.SpaceTiles == null)
			{
				return;
			}
			gridBounds = Matrix3.Invert(ref this.SpaceMatrix).TransformBox(ref args.WorldBounds).Enlarged(2f);
			worldHandle.SetTransform(ref this.SpaceMatrix);
			this.DrawTiles(worldHandle, gridBounds, this.SpaceTiles, this.SpaceTileSize);
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x000B00C4 File Offset: 0x000AE2C4
		private void DrawTiles(DrawingHandleWorld handle, Box2 gridBounds, Dictionary<int, List<Vector2i>> tileSets, ushort tileSize)
		{
			for (int i = 0; i < this.Intensity.Count; i++)
			{
				Color color = this.ColorMap(this.Intensity[i]);
				Color color2 = color;
				color2.A = 0.2f;
				List<Vector2i> list;
				if (tileSets.TryGetValue(i, out list))
				{
					foreach (Vector2i vector2i in list)
					{
						Vector2 vector = (vector2i + 0.5f) * (float)tileSize;
						if (gridBounds.Contains(vector, true))
						{
							Box2 box = Box2.UnitCentered.Translated(vector);
							handle.DrawRect(box, color, false);
							handle.DrawRect(box, color2, true);
						}
					}
				}
			}
		}

		// Token: 0x06001DFC RID: 7676 RVA: 0x000B019C File Offset: 0x000AE39C
		private Color ColorMap(float intensity)
		{
			float num = 1f - intensity / this.Intensity[0];
			Color result;
			if (num < 0.5f)
			{
				result = Color.InterpolateBetween(Color.Red, Color.Orange, num * 2f);
			}
			else
			{
				result = Color.InterpolateBetween(Color.Orange, Color.Yellow, (num - 0.5f) * 2f);
			}
			return result;
		}

		// Token: 0x04000EA6 RID: 3750
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000EA7 RID: 3751
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000EA8 RID: 3752
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000EA9 RID: 3753
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Dictionary<int, List<Vector2i>> SpaceTiles;

		// Token: 0x04000EAA RID: 3754
		public Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> Tiles = new Dictionary<EntityUid, Dictionary<int, List<Vector2i>>>();

		// Token: 0x04000EAB RID: 3755
		public List<float> Intensity = new List<float>();

		// Token: 0x04000EAC RID: 3756
		public float TotalIntensity;

		// Token: 0x04000EAD RID: 3757
		public float Slope;

		// Token: 0x04000EAE RID: 3758
		public ushort SpaceTileSize;

		// Token: 0x04000EAF RID: 3759
		public Matrix3 SpaceMatrix;

		// Token: 0x04000EB0 RID: 3760
		public MapId Map;

		// Token: 0x04000EB1 RID: 3761
		private readonly Font _font;
	}
}
