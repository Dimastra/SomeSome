using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.IconSmoothing;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.IconSmoothing
{
	// Token: 0x020002C9 RID: 713
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IconSmoothSystem : EntitySystem
	{
		// Token: 0x060011DE RID: 4574 RVA: 0x00069B38 File Offset: 0x00067D38
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeEdge();
			base.SubscribeLocalEvent<IconSmoothComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<IconSmoothComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<IconSmoothComponent, ComponentShutdown>(new ComponentEventHandler<IconSmoothComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<IconSmoothComponent, ComponentStartup>(new ComponentEventHandler<IconSmoothComponent, ComponentStartup>(this.OnStartup), null, null);
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x00069B90 File Offset: 0x00067D90
		private void OnStartup(EntityUid uid, IconSmoothComponent component, ComponentStartup args)
		{
			TransformComponent transformComponent = base.Transform(uid);
			if (transformComponent.Anchored)
			{
				MapGridComponent mapGridComponent;
				component.LastPosition = (this._mapManager.TryGetGrid(transformComponent.GridUid, ref mapGridComponent) ? new ValueTuple<EntityUid?, Vector2i>?(new ValueTuple<EntityUid?, Vector2i>(new EntityUid?(transformComponent.GridUid.Value), mapGridComponent.TileIndicesFor(transformComponent.Coordinates))) : new ValueTuple<EntityUid?, Vector2i>?(new ValueTuple<EntityUid?, Vector2i>(null, new Vector2i(0, 0))));
				this.DirtyNeighbours(uid, component, null, null);
			}
			SpriteComponent spriteComponent;
			if (component.Mode != IconSmoothingMode.Corners || !base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			string text = component.StateBase + "0";
			spriteComponent.LayerMapSet(IconSmoothSystem.CornerLayers.SE, spriteComponent.AddLayerState(text, null));
			spriteComponent.LayerSetDirOffset(IconSmoothSystem.CornerLayers.SE, 0);
			spriteComponent.LayerMapSet(IconSmoothSystem.CornerLayers.NE, spriteComponent.AddLayerState(text, null));
			spriteComponent.LayerSetDirOffset(IconSmoothSystem.CornerLayers.NE, 2);
			spriteComponent.LayerMapSet(IconSmoothSystem.CornerLayers.NW, spriteComponent.AddLayerState(text, null));
			spriteComponent.LayerSetDirOffset(IconSmoothSystem.CornerLayers.NW, 3);
			spriteComponent.LayerMapSet(IconSmoothSystem.CornerLayers.SW, spriteComponent.AddLayerState(text, null));
			spriteComponent.LayerSetDirOffset(IconSmoothSystem.CornerLayers.SW, 1);
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x00069CF0 File Offset: 0x00067EF0
		private void OnShutdown(EntityUid uid, IconSmoothComponent component, ComponentShutdown args)
		{
			this.DirtyNeighbours(uid, component, null, null);
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x00069D10 File Offset: 0x00067F10
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			EntityQuery<TransformComponent> entityQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<IconSmoothComponent> entityQuery2 = base.GetEntityQuery<IconSmoothComponent>();
			EntityUid entityUid;
			while (this._anchorChangedEntities.TryDequeue(out entityUid))
			{
				TransformComponent transformComponent;
				if (entityQuery.TryGetComponent(entityUid, ref transformComponent) && !(transformComponent.MapID == MapId.Nullspace))
				{
					this.DirtyNeighbours(entityUid, null, transformComponent, new EntityQuery<IconSmoothComponent>?(entityQuery2));
				}
			}
			if (this._dirtyEntities.Count == 0)
			{
				return;
			}
			this._generation++;
			EntityQuery<SpriteComponent> entityQuery3 = base.GetEntityQuery<SpriteComponent>();
			EntityUid uid;
			while (this._dirtyEntities.TryDequeue(out uid))
			{
				this.CalculateNewSprite(uid, entityQuery3, entityQuery2, entityQuery, null);
			}
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x00069DB4 File Offset: 0x00067FB4
		[NullableContext(2)]
		public void DirtyNeighbours(EntityUid uid, IconSmoothComponent comp = null, TransformComponent transform = null, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<IconSmoothComponent>? smoothQuery = null)
		{
			EntityQuery<IconSmoothComponent> value = smoothQuery.GetValueOrDefault();
			if (smoothQuery == null)
			{
				value = base.GetEntityQuery<IconSmoothComponent>();
				smoothQuery = new EntityQuery<IconSmoothComponent>?(value);
			}
			if (!smoothQuery.Value.Resolve(uid, ref comp, true))
			{
				return;
			}
			this._dirtyEntities.Enqueue(uid);
			if (!base.Resolve<TransformComponent>(uid, ref transform, true))
			{
				return;
			}
			MapGridComponent mapGridComponent;
			Vector2i vector2i;
			if (!transform.Anchored || !this._mapManager.TryGetGrid(transform.GridUid, ref mapGridComponent))
			{
				ValueTuple<EntityUid?, Vector2i>? lastPosition = comp.LastPosition;
				if (lastPosition != null)
				{
					ValueTuple<EntityUid?, Vector2i> valueOrDefault = lastPosition.GetValueOrDefault();
					EntityUid? item = valueOrDefault.Item1;
					if (item != null)
					{
						EntityUid valueOrDefault2 = item.GetValueOrDefault();
						Vector2i item2 = valueOrDefault.Item2;
						if (!this._mapManager.TryGetGrid(new EntityUid?(valueOrDefault2), ref mapGridComponent))
						{
							return;
						}
						vector2i = item2;
						goto IL_D4;
					}
				}
				return;
			}
			vector2i = mapGridComponent.CoordinatesToTile(transform.Coordinates);
			IL_D4:
			this.DirtyEntities(mapGridComponent.GetAnchoredEntities(vector2i + new Vector2i(1, 0)));
			this.DirtyEntities(mapGridComponent.GetAnchoredEntities(vector2i + new Vector2i(-1, 0)));
			this.DirtyEntities(mapGridComponent.GetAnchoredEntities(vector2i + new Vector2i(0, 1)));
			this.DirtyEntities(mapGridComponent.GetAnchoredEntities(vector2i + new Vector2i(0, -1)));
			IconSmoothingMode mode = comp.Mode;
			if (mode == IconSmoothingMode.Corners || mode == IconSmoothingMode.NoSprite || mode == IconSmoothingMode.Diagonal)
			{
				this.DirtyEntities(mapGridComponent.GetAnchoredEntities(vector2i + new Vector2i(1, 1)));
				this.DirtyEntities(mapGridComponent.GetAnchoredEntities(vector2i + new Vector2i(-1, -1)));
				this.DirtyEntities(mapGridComponent.GetAnchoredEntities(vector2i + new Vector2i(-1, 1)));
				this.DirtyEntities(mapGridComponent.GetAnchoredEntities(vector2i + new Vector2i(1, -1)));
			}
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x00069F74 File Offset: 0x00068174
		private void DirtyEntities(IEnumerable<EntityUid> entities)
		{
			foreach (EntityUid item in entities)
			{
				this._dirtyEntities.Enqueue(item);
			}
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x00069FC4 File Offset: 0x000681C4
		private void OnAnchorChanged(EntityUid uid, IconSmoothComponent component, ref AnchorStateChangedEvent args)
		{
			if (!args.Detaching)
			{
				this._anchorChangedEntities.Enqueue(uid);
			}
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x00069FDC File Offset: 0x000681DC
		[NullableContext(2)]
		private void CalculateNewSprite(EntityUid uid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<SpriteComponent> spriteQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<IconSmoothComponent> smoothQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, IconSmoothComponent smooth = null)
		{
			MapGridComponent mapGridComponent = null;
			TransformComponent component2;
			if (!smoothQuery.Resolve(uid, ref smooth, false) || smooth.Mode == IconSmoothingMode.NoSprite || smooth.UpdateGeneration == this._generation)
			{
				SmoothEdgeComponent component;
				if (smooth != null && base.TryComp<SmoothEdgeComponent>(uid, ref component) && xformQuery.TryGetComponent(uid, ref component2))
				{
					DirectionFlag directionFlag = 0;
					if (this._mapManager.TryGetGrid(component2.GridUid, ref mapGridComponent))
					{
						Vector2i vector2i = mapGridComponent.TileIndicesFor(component2.Coordinates);
						if (this.MatchingEntity(smooth, mapGridComponent.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 4)), smoothQuery))
						{
							directionFlag |= 4;
						}
						if (this.MatchingEntity(smooth, mapGridComponent.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 0)), smoothQuery))
						{
							directionFlag |= 1;
						}
						if (this.MatchingEntity(smooth, mapGridComponent.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 2)), smoothQuery))
						{
							directionFlag |= 2;
						}
						if (this.MatchingEntity(smooth, mapGridComponent.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 6)), smoothQuery))
						{
							directionFlag |= 8;
						}
					}
					this.CalculateEdge(uid, directionFlag, null, component);
				}
				return;
			}
			component2 = xformQuery.GetComponent(uid);
			smooth.UpdateGeneration = this._generation;
			SpriteComponent sprite;
			if (!spriteQuery.TryGetComponent(uid, ref sprite))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Encountered a icon-smoothing entity without a sprite: ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				base.RemComp(uid, smooth);
				return;
			}
			if (component2.Anchored && !this._mapManager.TryGetGrid(component2.GridUid, ref mapGridComponent))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(77, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Failed to calculate IconSmoothComponent sprite in ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" because grid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(component2.GridUid);
				defaultInterpolatedStringHandler.AppendLiteral(" was missing.");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			switch (smooth.Mode)
			{
			case IconSmoothingMode.Corners:
				this.CalculateNewSpriteCorners(mapGridComponent, smooth, sprite, component2, smoothQuery);
				return;
			case IconSmoothingMode.CardinalFlags:
				this.CalculateNewSpriteCardinal(mapGridComponent, smooth, sprite, component2, smoothQuery);
				return;
			case IconSmoothingMode.Diagonal:
				this.CalculateNewSpriteDiagonal(mapGridComponent, smooth, sprite, component2, smoothQuery);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x0006A1FC File Offset: 0x000683FC
		private void CalculateNewSpriteDiagonal([Nullable(2)] MapGridComponent grid, IconSmoothComponent smooth, SpriteComponent sprite, TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<IconSmoothComponent> smoothQuery)
		{
			if (grid == null)
			{
				sprite.LayerSetState(0, smooth.StateBase + "0");
				return;
			}
			Vector2[] array = new Vector2[]
			{
				new Vector2(1f, 0f),
				new Vector2(1f, -1f),
				new Vector2(0f, -1f)
			};
			Vector2i vector2i = grid.TileIndicesFor(xform.Coordinates);
			Angle localRotation = xform.LocalRotation;
			bool flag = true;
			for (int i = 0; i < array.Length; i++)
			{
				Vector2i vector2i2 = (Vector2i)localRotation.RotateVec(ref array[i]);
				flag = (flag && this.MatchingEntity(smooth, grid.GetAnchoredEntities(vector2i + vector2i2), smoothQuery));
			}
			if (flag)
			{
				sprite.LayerSetState(0, smooth.StateBase + "1");
				return;
			}
			sprite.LayerSetState(0, smooth.StateBase + "0");
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x0006A30C File Offset: 0x0006850C
		private void CalculateNewSpriteCardinal([Nullable(2)] MapGridComponent grid, IconSmoothComponent smooth, SpriteComponent sprite, TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<IconSmoothComponent> smoothQuery)
		{
			IconSmoothSystem.CardinalConnectDirs cardinalConnectDirs = IconSmoothSystem.CardinalConnectDirs.None;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (grid == null)
			{
				int num = 0;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
				defaultInterpolatedStringHandler.AppendFormatted(smooth.StateBase);
				defaultInterpolatedStringHandler.AppendFormatted<int>((int)cardinalConnectDirs);
				sprite.LayerSetState(num, defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			Vector2i vector2i = grid.TileIndicesFor(xform.Coordinates);
			if (this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 4)), smoothQuery))
			{
				cardinalConnectDirs |= IconSmoothSystem.CardinalConnectDirs.North;
			}
			if (this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 0)), smoothQuery))
			{
				cardinalConnectDirs |= IconSmoothSystem.CardinalConnectDirs.South;
			}
			if (this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 2)), smoothQuery))
			{
				cardinalConnectDirs |= IconSmoothSystem.CardinalConnectDirs.East;
			}
			if (this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 6)), smoothQuery))
			{
				cardinalConnectDirs |= IconSmoothSystem.CardinalConnectDirs.West;
			}
			int num2 = 0;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(smooth.StateBase);
			defaultInterpolatedStringHandler.AppendFormatted<int>((int)cardinalConnectDirs);
			sprite.LayerSetState(num2, defaultInterpolatedStringHandler.ToStringAndClear());
			DirectionFlag directionFlag = 0;
			if ((cardinalConnectDirs & IconSmoothSystem.CardinalConnectDirs.South) != IconSmoothSystem.CardinalConnectDirs.None)
			{
				directionFlag |= 1;
			}
			if ((cardinalConnectDirs & IconSmoothSystem.CardinalConnectDirs.East) != IconSmoothSystem.CardinalConnectDirs.None)
			{
				directionFlag |= 2;
			}
			if ((cardinalConnectDirs & IconSmoothSystem.CardinalConnectDirs.North) != IconSmoothSystem.CardinalConnectDirs.None)
			{
				directionFlag |= 4;
			}
			if ((cardinalConnectDirs & IconSmoothSystem.CardinalConnectDirs.West) != IconSmoothSystem.CardinalConnectDirs.None)
			{
				directionFlag |= 8;
			}
			this.CalculateEdge(sprite.Owner, directionFlag, sprite, null);
		}

		// Token: 0x060011E8 RID: 4584 RVA: 0x0006A434 File Offset: 0x00068634
		private bool MatchingEntity(IconSmoothComponent smooth, IEnumerable<EntityUid> candidates, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<IconSmoothComponent> smoothQuery)
		{
			foreach (EntityUid entityUid in candidates)
			{
				IconSmoothComponent iconSmoothComponent;
				if (smoothQuery.TryGetComponent(entityUid, ref iconSmoothComponent) && iconSmoothComponent.SmoothKey == smooth.SmoothKey)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x0006A49C File Offset: 0x0006869C
		private void CalculateNewSpriteCorners([Nullable(2)] MapGridComponent grid, IconSmoothComponent smooth, SpriteComponent sprite, TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<IconSmoothComponent> smoothQuery)
		{
			IconSmoothSystem.CornerFill cornerFill;
			IconSmoothSystem.CornerFill cornerFill2;
			IconSmoothSystem.CornerFill cornerFill3;
			IconSmoothSystem.CornerFill cornerFill4;
			if (grid != null)
			{
				ValueTuple<IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill> valueTuple = this.CalculateCornerFill(grid, smooth, xform, smoothQuery);
				cornerFill = valueTuple.Item1;
				cornerFill2 = valueTuple.Item2;
				cornerFill3 = valueTuple.Item3;
				cornerFill4 = valueTuple.Item4;
			}
			else
			{
				cornerFill = IconSmoothSystem.CornerFill.None;
				cornerFill2 = IconSmoothSystem.CornerFill.None;
				cornerFill3 = IconSmoothSystem.CornerFill.None;
				cornerFill4 = IconSmoothSystem.CornerFill.None;
			}
			object obj = IconSmoothSystem.CornerLayers.NE;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(smooth.StateBase);
			defaultInterpolatedStringHandler.AppendFormatted<int>((int)cornerFill);
			sprite.LayerSetState(obj, defaultInterpolatedStringHandler.ToStringAndClear());
			object obj2 = IconSmoothSystem.CornerLayers.SE;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(smooth.StateBase);
			defaultInterpolatedStringHandler.AppendFormatted<int>((int)cornerFill4);
			sprite.LayerSetState(obj2, defaultInterpolatedStringHandler.ToStringAndClear());
			object obj3 = IconSmoothSystem.CornerLayers.SW;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(smooth.StateBase);
			defaultInterpolatedStringHandler.AppendFormatted<int>((int)cornerFill3);
			sprite.LayerSetState(obj3, defaultInterpolatedStringHandler.ToStringAndClear());
			object obj4 = IconSmoothSystem.CornerLayers.NW;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(smooth.StateBase);
			defaultInterpolatedStringHandler.AppendFormatted<int>((int)cornerFill2);
			sprite.LayerSetState(obj4, defaultInterpolatedStringHandler.ToStringAndClear());
			DirectionFlag directionFlag = 0;
			if ((cornerFill4 & cornerFill3) != IconSmoothSystem.CornerFill.None)
			{
				directionFlag |= 1;
			}
			if ((cornerFill4 & cornerFill) != IconSmoothSystem.CornerFill.None)
			{
				directionFlag |= 2;
			}
			if ((cornerFill & cornerFill2) != IconSmoothSystem.CornerFill.None)
			{
				directionFlag |= 4;
			}
			if ((cornerFill2 & cornerFill3) != IconSmoothSystem.CornerFill.None)
			{
				directionFlag |= 8;
			}
			this.CalculateEdge(sprite.Owner, directionFlag, sprite, null);
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x0006A5FC File Offset: 0x000687FC
		[return: TupleElementNames(new string[]
		{
			"ne",
			"nw",
			"sw",
			"se"
		})]
		[return: Nullable(0)]
		private ValueTuple<IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill> CalculateCornerFill(MapGridComponent grid, IconSmoothComponent smooth, TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<IconSmoothComponent> smoothQuery)
		{
			Vector2i vector2i = grid.TileIndicesFor(xform.Coordinates);
			bool flag = this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 4)), smoothQuery);
			bool flag2 = this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 3)), smoothQuery);
			bool flag3 = this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 2)), smoothQuery);
			bool flag4 = this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 1)), smoothQuery);
			bool flag5 = this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 0)), smoothQuery);
			bool flag6 = this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 7)), smoothQuery);
			bool flag7 = this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 6)), smoothQuery);
			bool flag8 = this.MatchingEntity(smooth, grid.GetAnchoredEntities(DirectionExtensions.Offset(vector2i, 5)), smoothQuery);
			IconSmoothSystem.CornerFill cornerFill = IconSmoothSystem.CornerFill.None;
			IconSmoothSystem.CornerFill cornerFill2 = IconSmoothSystem.CornerFill.None;
			IconSmoothSystem.CornerFill cornerFill3 = IconSmoothSystem.CornerFill.None;
			IconSmoothSystem.CornerFill cornerFill4 = IconSmoothSystem.CornerFill.None;
			if (flag)
			{
				cornerFill |= IconSmoothSystem.CornerFill.CounterClockwise;
				cornerFill4 |= IconSmoothSystem.CornerFill.Clockwise;
			}
			if (flag2)
			{
				cornerFill |= IconSmoothSystem.CornerFill.Diagonal;
			}
			if (flag3)
			{
				cornerFill |= IconSmoothSystem.CornerFill.Clockwise;
				cornerFill2 |= IconSmoothSystem.CornerFill.CounterClockwise;
			}
			if (flag4)
			{
				cornerFill2 |= IconSmoothSystem.CornerFill.Diagonal;
			}
			if (flag5)
			{
				cornerFill2 |= IconSmoothSystem.CornerFill.Clockwise;
				cornerFill3 |= IconSmoothSystem.CornerFill.CounterClockwise;
			}
			if (flag6)
			{
				cornerFill3 |= IconSmoothSystem.CornerFill.Diagonal;
			}
			if (flag7)
			{
				cornerFill3 |= IconSmoothSystem.CornerFill.Clockwise;
				cornerFill4 |= IconSmoothSystem.CornerFill.CounterClockwise;
			}
			if (flag8)
			{
				cornerFill4 |= IconSmoothSystem.CornerFill.Diagonal;
			}
			Direction cardinalDir = xform.LocalRotation.GetCardinalDir();
			if (cardinalDir == null)
			{
				return new ValueTuple<IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill>(cornerFill, cornerFill4, cornerFill3, cornerFill2);
			}
			if (cardinalDir == 4)
			{
				return new ValueTuple<IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill>(cornerFill3, cornerFill2, cornerFill, cornerFill4);
			}
			if (cardinalDir != 6)
			{
				return new ValueTuple<IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill>(cornerFill4, cornerFill3, cornerFill2, cornerFill);
			}
			return new ValueTuple<IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill, IconSmoothSystem.CornerFill>(cornerFill2, cornerFill, cornerFill4, cornerFill3);
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x0006A798 File Offset: 0x00068998
		private void InitializeEdge()
		{
			base.SubscribeLocalEvent<SmoothEdgeComponent, ComponentStartup>(new ComponentEventHandler<SmoothEdgeComponent, ComponentStartup>(this.OnEdgeStartup), null, null);
			base.SubscribeLocalEvent<SmoothEdgeComponent, ComponentShutdown>(new ComponentEventHandler<SmoothEdgeComponent, ComponentShutdown>(this.OnEdgeShutdown), null, null);
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0006A7C4 File Offset: 0x000689C4
		private void OnEdgeStartup(EntityUid uid, SmoothEdgeComponent component, ComponentStartup args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			spriteComponent.LayerSetOffset(IconSmoothSystem.EdgeLayer.South, new Vector2(0f, -1f));
			spriteComponent.LayerSetOffset(IconSmoothSystem.EdgeLayer.East, new Vector2(1f, 0f));
			spriteComponent.LayerSetOffset(IconSmoothSystem.EdgeLayer.North, new Vector2(0f, 1f));
			spriteComponent.LayerSetOffset(IconSmoothSystem.EdgeLayer.West, new Vector2(-1f, 0f));
			spriteComponent.LayerSetVisible(IconSmoothSystem.EdgeLayer.South, false);
			spriteComponent.LayerSetVisible(IconSmoothSystem.EdgeLayer.East, false);
			spriteComponent.LayerSetVisible(IconSmoothSystem.EdgeLayer.North, false);
			spriteComponent.LayerSetVisible(IconSmoothSystem.EdgeLayer.West, false);
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x0006A880 File Offset: 0x00068A80
		private void OnEdgeShutdown(EntityUid uid, SmoothEdgeComponent component, ComponentShutdown args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			spriteComponent.LayerMapRemove(IconSmoothSystem.EdgeLayer.South);
			spriteComponent.LayerMapRemove(IconSmoothSystem.EdgeLayer.East);
			spriteComponent.LayerMapRemove(IconSmoothSystem.EdgeLayer.North);
			spriteComponent.LayerMapRemove(IconSmoothSystem.EdgeLayer.West);
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x0006A8CC File Offset: 0x00068ACC
		[NullableContext(2)]
		private void CalculateEdge(EntityUid uid, DirectionFlag directions, SpriteComponent sprite = null, SmoothEdgeComponent component = null)
		{
			if (!base.Resolve<SpriteComponent, SmoothEdgeComponent>(uid, ref sprite, ref component, false))
			{
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				DirectionFlag directionFlag = (sbyte)Math.Pow(2.0, (double)i);
				IconSmoothSystem.EdgeLayer edge = this.GetEdge(directionFlag);
				if ((directionFlag & directions) != null)
				{
					sprite.LayerSetVisible(edge, false);
				}
				else
				{
					sprite.LayerSetVisible(edge, true);
				}
			}
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x0006A92F File Offset: 0x00068B2F
		private IconSmoothSystem.EdgeLayer GetEdge(DirectionFlag direction)
		{
			switch (direction)
			{
			case 1:
				return IconSmoothSystem.EdgeLayer.South;
			case 2:
				return IconSmoothSystem.EdgeLayer.East;
			case 3:
				break;
			case 4:
				return IconSmoothSystem.EdgeLayer.North;
			default:
				if (direction == 8)
				{
					return IconSmoothSystem.EdgeLayer.West;
				}
				break;
			}
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x040008CC RID: 2252
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040008CD RID: 2253
		private readonly Queue<EntityUid> _dirtyEntities = new Queue<EntityUid>();

		// Token: 0x040008CE RID: 2254
		private readonly Queue<EntityUid> _anchorChangedEntities = new Queue<EntityUid>();

		// Token: 0x040008CF RID: 2255
		private int _generation;

		// Token: 0x020002CA RID: 714
		[NullableContext(0)]
		[Flags]
		private enum CardinalConnectDirs : byte
		{
			// Token: 0x040008D1 RID: 2257
			None = 0,
			// Token: 0x040008D2 RID: 2258
			North = 1,
			// Token: 0x040008D3 RID: 2259
			South = 2,
			// Token: 0x040008D4 RID: 2260
			East = 4,
			// Token: 0x040008D5 RID: 2261
			West = 8
		}

		// Token: 0x020002CB RID: 715
		[NullableContext(0)]
		[Flags]
		private enum CornerFill : byte
		{
			// Token: 0x040008D7 RID: 2263
			None = 0,
			// Token: 0x040008D8 RID: 2264
			CounterClockwise = 1,
			// Token: 0x040008D9 RID: 2265
			Diagonal = 2,
			// Token: 0x040008DA RID: 2266
			Clockwise = 4
		}

		// Token: 0x020002CC RID: 716
		[NullableContext(0)]
		private enum CornerLayers : byte
		{
			// Token: 0x040008DC RID: 2268
			SE,
			// Token: 0x040008DD RID: 2269
			NE,
			// Token: 0x040008DE RID: 2270
			NW,
			// Token: 0x040008DF RID: 2271
			SW
		}

		// Token: 0x020002CD RID: 717
		[NullableContext(0)]
		private enum EdgeLayer : byte
		{
			// Token: 0x040008E1 RID: 2273
			South,
			// Token: 0x040008E2 RID: 2274
			East,
			// Token: 0x040008E3 RID: 2275
			North,
			// Token: 0x040008E4 RID: 2276
			West
		}
	}
}
