using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Radiation.Systems;
using Content.Shared.Radiation.Systems;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Radiation.Overlays
{
	// Token: 0x0200017B RID: 379
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadiationDebugOverlay : Overlay
	{
		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x060009D8 RID: 2520 RVA: 0x000392B9 File Offset: 0x000374B9
		public override OverlaySpace Space
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x000392BC File Offset: 0x000374BC
		public RadiationDebugOverlay()
		{
			IoCManager.InjectDependencies<RadiationDebugOverlay>(this);
			this._radiation = this._entityManager.System<RadiationSystem>();
			IResourceCache resourceCache = IoCManager.Resolve<IResourceCache>();
			this._font = new VectorFont(resourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x00039308 File Offset: 0x00037508
		protected override void Draw(in OverlayDrawArgs args)
		{
			OverlaySpace space = args.Space;
			if (space == 2)
			{
				this.DrawScreenRays(args);
				this.DrawScreenResistance(args);
				return;
			}
			if (space != 4)
			{
				return;
			}
			this.DrawWorld(args);
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x00039348 File Offset: 0x00037548
		private void DrawScreenRays(OverlayDrawArgs args)
		{
			List<RadiationRay> rays = this._radiation.Rays;
			if (rays == null || args.ViewportControl == null)
			{
				return;
			}
			DrawingHandleScreen screenHandle = args.ScreenHandle;
			foreach (RadiationRay radiationRay in rays)
			{
				if (!(radiationRay.MapId != args.MapId))
				{
					if (radiationRay.ReachedDestination)
					{
						Vector2 vector = args.ViewportControl.WorldToScreen(radiationRay.Destination);
						screenHandle.DrawString(this._font, vector, radiationRay.Rads.ToString("F2"), 2f, Color.White);
					}
					foreach (KeyValuePair<EntityUid, List<ValueTuple<Vector2i, float>>> keyValuePair in radiationRay.Blockers)
					{
						EntityUid entityUid;
						List<ValueTuple<Vector2i, float>> list;
						keyValuePair.Deconstruct(out entityUid, out list);
						EntityUid value = entityUid;
						List<ValueTuple<Vector2i, float>> list2 = list;
						MapGridComponent mapGridComponent;
						if (this._mapManager.TryGetGrid(new EntityUid?(value), ref mapGridComponent))
						{
							foreach (ValueTuple<Vector2i, float> valueTuple in list2)
							{
								Vector2i item = valueTuple.Item1;
								float item2 = valueTuple.Item2;
								Vector2 vector2 = mapGridComponent.GridTileToWorldPos(item);
								Vector2 vector3 = args.ViewportControl.WorldToScreen(vector2);
								screenHandle.DrawString(this._font, vector3, item2.ToString("F2"), 1.5f, Color.White);
							}
						}
					}
				}
			}
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x00039530 File Offset: 0x00037730
		private void DrawScreenResistance(OverlayDrawArgs args)
		{
			Dictionary<EntityUid, Dictionary<Vector2i, float>> resistanceGrids = this._radiation.ResistanceGrids;
			if (resistanceGrids == null || args.ViewportControl == null)
			{
				return;
			}
			DrawingHandleScreen screenHandle = args.ScreenHandle;
			EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
			foreach (KeyValuePair<EntityUid, Dictionary<Vector2i, float>> keyValuePair in resistanceGrids)
			{
				EntityUid entityUid;
				Dictionary<Vector2i, float> dictionary;
				keyValuePair.Deconstruct(out entityUid, out dictionary);
				EntityUid entityUid2 = entityUid;
				Dictionary<Vector2i, float> dictionary2 = dictionary;
				MapGridComponent mapGridComponent;
				TransformComponent transformComponent;
				if (this._mapManager.TryGetGrid(new EntityUid?(entityUid2), ref mapGridComponent) && (!entityQuery.TryGetComponent(entityUid2, ref transformComponent) || !(transformComponent.MapID != args.MapId)))
				{
					Vector2 vector = new Vector2((float)mapGridComponent.TileSize, (float)(-(float)mapGridComponent.TileSize)) * 0.25f;
					foreach (KeyValuePair<Vector2i, float> keyValuePair2 in dictionary2)
					{
						Vector2i vector2i;
						float num;
						keyValuePair2.Deconstruct(out vector2i, out num);
						Vector2i vector2i2 = vector2i;
						float num2 = num;
						Vector2 vector2 = mapGridComponent.GridTileToLocal(vector2i2).Position + vector;
						Vector2 vector3 = mapGridComponent.LocalToWorld(vector2);
						Vector2 vector4 = args.ViewportControl.WorldToScreen(vector3);
						screenHandle.DrawString(this._font, vector4, num2.ToString("F2"), Color.White);
					}
				}
			}
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x000396D4 File Offset: 0x000378D4
		private void DrawWorld(in OverlayDrawArgs args)
		{
			List<RadiationRay> rays = this._radiation.Rays;
			if (rays == null)
			{
				return;
			}
			DrawingHandleWorld worldHandle = args.WorldHandle;
			foreach (RadiationRay radiationRay in rays)
			{
				if (!(radiationRay.MapId != args.MapId))
				{
					if (radiationRay.ReachedDestination)
					{
						worldHandle.DrawLine(radiationRay.Source, radiationRay.Destination, Color.Red);
					}
					else
					{
						foreach (KeyValuePair<EntityUid, List<ValueTuple<Vector2i, float>>> keyValuePair in radiationRay.Blockers)
						{
							EntityUid entityUid;
							List<ValueTuple<Vector2i, float>> list;
							keyValuePair.Deconstruct(out entityUid, out list);
							EntityUid value = entityUid;
							List<ValueTuple<Vector2i, float>> source = list;
							MapGridComponent mapGridComponent;
							if (this._mapManager.TryGetGrid(new EntityUid?(value), ref mapGridComponent))
							{
								Vector2i item = source.Last<ValueTuple<Vector2i, float>>().Item1;
								Vector2 vector = mapGridComponent.GridTileToWorldPos(item);
								worldHandle.DrawLine(radiationRay.Source, vector, Color.Red);
							}
						}
					}
				}
			}
		}

		// Token: 0x040004E3 RID: 1251
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040004E4 RID: 1252
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040004E5 RID: 1253
		private readonly RadiationSystem _radiation;

		// Token: 0x040004E6 RID: 1254
		private readonly Font _font;
	}
}
