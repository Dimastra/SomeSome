using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.GameTicking;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Maps;
using Content.Shared.Respawn;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Respawn
{
	// Token: 0x02000235 RID: 565
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpecialRespawnSystem : SharedSpecialRespawnSystem
	{
		// Token: 0x06000B47 RID: 2887 RVA: 0x0003B708 File Offset: 0x00039908
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnRunLevelChanged), null, null);
			base.SubscribeLocalEvent<SpecialRespawnSetupEvent>(new EntityEventHandler<SpecialRespawnSetupEvent>(this.OnSpecialRespawnSetup), null, null);
			base.SubscribeLocalEvent<SpecialRespawnComponent, ComponentStartup>(new ComponentEventHandler<SpecialRespawnComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<SpecialRespawnComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<SpecialRespawnComponent, EntityTerminatingEvent>(this.OnTermination), null, null);
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0003B76B File Offset: 0x0003996B
		private void OnRunLevelChanged(GameRunLevelChangedEvent ev)
		{
			if (ev.Old == GameRunLevel.InRound && ev.New == GameRunLevel.PreRoundLobby)
			{
				this.OnRoundEnd();
			}
			if (ev.New == GameRunLevel.PostRound)
			{
				this.OnRoundEnd();
			}
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x0003B794 File Offset: 0x00039994
		private void OnSpecialRespawnSetup(SpecialRespawnSetupEvent ev)
		{
			SpecialRespawnComponent comp;
			if (!base.TryComp<SpecialRespawnComponent>(ev.Entity, ref comp))
			{
				return;
			}
			TransformComponent xform = base.Transform(ev.Entity);
			if (xform.GridUid != null)
			{
				comp.StationMap = new ValueTuple<EntityUid?, EntityUid?>(xform.MapUid, xform.GridUid);
			}
		}

		// Token: 0x06000B4A RID: 2890 RVA: 0x0003B7E8 File Offset: 0x000399E8
		private void OnRoundEnd()
		{
			foreach (SpecialRespawnComponent specialRespawnComponent in base.EntityQuery<SpecialRespawnComponent>(false))
			{
				specialRespawnComponent.Respawn = false;
			}
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x0003B834 File Offset: 0x00039A34
		private void OnStartup(EntityUid uid, SpecialRespawnComponent component, ComponentStartup args)
		{
			SpecialRespawnSetupEvent ev = new SpecialRespawnSetupEvent(uid);
			base.QueueLocalEvent(ev);
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x0003B850 File Offset: 0x00039A50
		private void OnTermination(EntityUid uid, SpecialRespawnComponent component, ref EntityTerminatingEvent args)
		{
			EntityUid? entityMapUid = component.StationMap.Item1;
			EntityUid? entityGridUid = component.StationMap.Item2;
			if (!component.Respawn || !base.HasComp<StationMemberComponent>(entityGridUid) || entityMapUid == null)
			{
				return;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(entityGridUid, ref grid) || base.MetaData(entityGridUid.Value).EntityLifeStage >= 4)
			{
				return;
			}
			EntityCoordinates coords;
			if (this.TryFindRandomTile(entityGridUid.Value, entityMapUid.Value, 10, out coords))
			{
				this.Respawn(component.Prototype, coords);
				return;
			}
			TransformComponent transformComponent = base.Transform(entityGridUid.Value);
			EntityCoordinates pos = transformComponent.Coordinates;
			MapCoordinates mapPos = transformComponent.MapPosition;
			Circle circle;
			circle..ctor(mapPos.Position, 2f);
			foreach (TileRef tile in grid.GetTilesIntersecting(circle, true, null))
			{
				if (!tile.IsSpace(this._tileDefinitionManager) && !tile.IsBlockedTurf(true, null, null) && this._atmosphere.IsTileMixtureProbablySafe(entityGridUid, entityMapUid.Value, grid.TileIndicesFor(mapPos)))
				{
					pos = tile.GridPosition(null);
					if (true)
					{
						break;
					}
				}
			}
			this.Respawn(component.Prototype, pos);
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0003B9A8 File Offset: 0x00039BA8
		private void Respawn(string prototype, EntityCoordinates coords)
		{
			EntityUid entity = base.Spawn(prototype, coords);
			string name = base.MetaData(entity).EntityName;
			ISharedAdminLogManager adminLog = this._adminLog;
			LogType type = LogType.Respawn;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(34, 2);
			logStringHandler.AppendFormatted(name);
			logStringHandler.AppendLiteral(" was deleted and was respawned at ");
			logStringHandler.AppendFormatted<MapCoordinates>(coords.ToMap(this.EntityManager), "coords.ToMap(EntityManager)");
			adminLog.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x0003BA14 File Offset: 0x00039C14
		public bool TryFindRandomTile(EntityUid targetGrid, EntityUid targetMap, int maxAttempts, out EntityCoordinates targetCoords)
		{
			targetCoords = EntityCoordinates.Invalid;
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(new EntityUid?(targetGrid), ref grid))
			{
				return false;
			}
			TransformComponent xform = base.Transform(targetGrid);
			TileRef tileRef;
			if (!grid.TryGetTileRef(xform.Coordinates, ref tileRef))
			{
				return false;
			}
			Vector2i tile = tileRef.GridIndices;
			bool found = false;
			ValueTuple<Vector2, Angle, Matrix3> worldPositionRotationMatrix = xform.GetWorldPositionRotationMatrix();
			Vector2 gridPos = worldPositionRotationMatrix.Item1;
			Matrix3 gridMatrix = worldPositionRotationMatrix.Item3;
			Box2 localAABB = grid.LocalAABB;
			Box2 gridBounds = gridMatrix.TransformBox(ref localAABB);
			for (int i = 0; i < maxAttempts; i++)
			{
				int randomX = this._random.Next((int)gridBounds.Left, (int)gridBounds.Right);
				int randomY = this._random.Next((int)gridBounds.Bottom, (int)gridBounds.Top);
				tile..ctor(randomX - (int)gridPos.X, randomY - (int)gridPos.Y);
				Vector2 mapPos = grid.GridTileToWorldPos(tile);
				Vector2i mapTarget = grid.WorldToTile(mapPos);
				Circle circle;
				circle..ctor(mapPos, 2f);
				foreach (TileRef newTileRef in grid.GetTilesIntersecting(circle, true, null))
				{
					if (!newTileRef.IsSpace(this._tileDefinitionManager) && !newTileRef.IsBlockedTurf(true, null, null) && this._atmosphere.IsTileMixtureProbablySafe(new EntityUid?(targetGrid), targetMap, mapTarget))
					{
						found = true;
						targetCoords = grid.GridTileToLocal(tile);
						break;
					}
				}
				if (found)
				{
					break;
				}
			}
			return found;
		}

		// Token: 0x040006F0 RID: 1776
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040006F1 RID: 1777
		[Dependency]
		private readonly IAdminLogManager _adminLog;

		// Token: 0x040006F2 RID: 1778
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x040006F3 RID: 1779
		[Dependency]
		private readonly AtmosphereSystem _atmosphere;

		// Token: 0x040006F4 RID: 1780
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040006F5 RID: 1781
		[Dependency]
		private readonly StationSystem _stationSystem;
	}
}
