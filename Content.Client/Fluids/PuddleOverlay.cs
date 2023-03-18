using System;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Fluids
{
	// Token: 0x02000311 RID: 785
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PuddleOverlay : Overlay
	{
		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x060013CE RID: 5070 RVA: 0x000392B9 File Offset: 0x000374B9
		public override OverlaySpace Space
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x000746AC File Offset: 0x000728AC
		public PuddleOverlay()
		{
			IoCManager.InjectDependencies<PuddleOverlay>(this);
			this._debugOverlaySystem = this._entitySystemManager.GetEntitySystem<PuddleDebugOverlaySystem>();
			IResourceCache resourceCache = IoCManager.Resolve<IResourceCache>();
			this._font = new VectorFont(resourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x0007473C File Offset: 0x0007293C
		protected override void Draw(in OverlayDrawArgs args)
		{
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

		// Token: 0x060013D1 RID: 5073 RVA: 0x00074768 File Offset: 0x00072968
		private void DrawWorld(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
			foreach (EntityUid entityUid in this._debugOverlaySystem.TileData.Keys)
			{
				MapGridComponent mapGridComponent;
				if (this._mapManager.TryGetGrid(new EntityUid?(entityUid), ref mapGridComponent))
				{
					ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = entityQuery.GetComponent(entityUid).GetWorldPositionRotationMatrixWithInv(entityQuery);
					Matrix3 item = worldPositionRotationMatrixWithInv.Item3;
					Matrix3 item2 = worldPositionRotationMatrixWithInv.Item4;
					Box2 box = item2.TransformBox(ref args.WorldBounds).Enlarged((float)(mapGridComponent.TileSize * 2));
					worldHandle.SetTransform(ref item);
					foreach (PuddleDebugOverlayData puddleDebugOverlayData in this._debugOverlaySystem.GetData(mapGridComponent.Owner))
					{
						Vector2 vector = (puddleDebugOverlayData.Pos + 0.5f) * (float)mapGridComponent.TileSize;
						if (box.Contains(vector, true))
						{
							Box2 box2 = Box2.UnitCentered.Translated(vector);
							worldHandle.DrawRect(box2, Color.Blue, false);
							worldHandle.DrawRect(box2, this.ColorMap(puddleDebugOverlayData.CurrentVolume), true);
						}
					}
				}
			}
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x000748E4 File Offset: 0x00072AE4
		private void DrawScreen(in OverlayDrawArgs args)
		{
			DrawingHandleScreen screenHandle = args.ScreenHandle;
			EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
			foreach (EntityUid entityUid in this._debugOverlaySystem.TileData.Keys)
			{
				MapGridComponent mapGridComponent;
				if (this._mapManager.TryGetGrid(new EntityUid?(entityUid), ref mapGridComponent))
				{
					ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = entityQuery.GetComponent(entityUid).GetWorldPositionRotationMatrixWithInv(entityQuery);
					Matrix3 item = worldPositionRotationMatrixWithInv.Item3;
					Matrix3 item2 = worldPositionRotationMatrixWithInv.Item4;
					Box2 box = item2.TransformBox(ref args.WorldBounds).Enlarged((float)(mapGridComponent.TileSize * 2));
					foreach (PuddleDebugOverlayData puddleDebugOverlayData in this._debugOverlaySystem.GetData(mapGridComponent.Owner))
					{
						Vector2 vector = (puddleDebugOverlayData.Pos + 0.5f) * (float)mapGridComponent.TileSize;
						if (box.Contains(vector, true))
						{
							Vector2 vector2 = this._eyeManager.WorldToScreen(item.Transform(vector));
							screenHandle.DrawString(this._font, vector2, puddleDebugOverlayData.CurrentVolume.ToString(), Color.White);
						}
					}
				}
			}
		}

		// Token: 0x060013D3 RID: 5075 RVA: 0x00074A58 File Offset: 0x00072C58
		private Color ColorMap(FixedPoint2 intensity)
		{
			FixedPoint2 a = 1 - intensity / FixedPoint2.New(20f);
			if (!(a < 0.5f))
			{
				return Color.InterpolateBetween(this._lightPuddle, this._mediumPuddle, (a.Float() - 0.5f) * 2f);
			}
			return Color.InterpolateBetween(this._mediumPuddle, this._heavyPuddle, a.Float() * 2f);
		}

		// Token: 0x040009EA RID: 2538
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040009EB RID: 2539
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x040009EC RID: 2540
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040009ED RID: 2541
		[Dependency]
		private readonly IEntitySystemManager _entitySystemManager;

		// Token: 0x040009EE RID: 2542
		private readonly PuddleDebugOverlaySystem _debugOverlaySystem;

		// Token: 0x040009EF RID: 2543
		private readonly Color _heavyPuddle = new Color(0, byte.MaxValue, byte.MaxValue, 50);

		// Token: 0x040009F0 RID: 2544
		private readonly Color _mediumPuddle = new Color(0, 150, byte.MaxValue, 50);

		// Token: 0x040009F1 RID: 2545
		private readonly Color _lightPuddle = new Color(0, 50, byte.MaxValue, 50);

		// Token: 0x040009F2 RID: 2546
		private readonly Font _font;
	}
}
