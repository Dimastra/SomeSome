using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.Components;
using Content.Server.Chat.Managers;
using Content.Server.Destructible;
using Content.Server.Explosion.Components;
using Content.Server.Mind.Components;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NPC.Pathfinding;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Audio;
using Content.Shared.Camera;
using Content.Shared.CCVar;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Content.Shared.Maps;
using Content.Shared.Spawners.Components;
using Content.Shared.Throwing;
using Robust.Server.GameObjects;
using Robust.Server.GameStates;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x0200050C RID: 1292
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExplosionSystem : EntitySystem
	{
		// Token: 0x06001AAB RID: 6827 RVA: 0x0008D090 File Offset: 0x0008B290
		private void InitAirtightMap()
		{
			int index = 0;
			foreach (ExplosionPrototype prototype in this._prototypeManager.EnumeratePrototypes<ExplosionPrototype>())
			{
				this._explosionTypes.Add(prototype.ID, index);
				index++;
			}
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x0008D0F4 File Offset: 0x0008B2F4
		[NullableContext(2)]
		public void UpdateAirtightMap(EntityUid gridId, Vector2i tile, MapGridComponent grid = null, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<AirtightComponent>? query = null)
		{
			if (base.Resolve<MapGridComponent>(gridId, ref grid, false))
			{
				this.UpdateAirtightMap(gridId, grid, tile, query);
			}
		}

		// Token: 0x06001AAD RID: 6829 RVA: 0x0008D110 File Offset: 0x0008B310
		public void UpdateAirtightMap(EntityUid gridId, MapGridComponent grid, Vector2i tile, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<AirtightComponent>? query = null)
		{
			float[] tolerance = new float[this._explosionTypes.Count];
			AtmosDirection blockedDirections = AtmosDirection.Invalid;
			if (!this._airtightMap.ContainsKey(gridId))
			{
				this._airtightMap[gridId] = new Dictionary<Vector2i, ExplosionSystem.TileData>();
			}
			EntityQuery<AirtightComponent> value = query.GetValueOrDefault();
			if (query == null)
			{
				value = this.EntityManager.GetEntityQuery<AirtightComponent>();
				query = new EntityQuery<AirtightComponent>?(value);
			}
			EntityQuery<DamageableComponent> damageQuery = this.EntityManager.GetEntityQuery<DamageableComponent>();
			EntityQuery<DestructibleComponent> destructibleQuery = this.EntityManager.GetEntityQuery<DestructibleComponent>();
			EntityUid? uid;
			while (grid.GetAnchoredEntitiesEnumerator(tile).MoveNext(ref uid))
			{
				AirtightComponent airtight;
				if (query.Value.TryGetComponent(uid, ref airtight) && airtight.AirBlocked)
				{
					blockedDirections |= airtight.AirBlockedDirection;
					float[] entityTolerances = this.GetExplosionTolerance(uid.Value, damageQuery, destructibleQuery);
					for (int i = 0; i < tolerance.Length; i++)
					{
						tolerance[i] = Math.Max(tolerance[i], entityTolerances[i]);
					}
				}
			}
			if (blockedDirections != AtmosDirection.Invalid)
			{
				this._airtightMap[gridId][tile] = new ExplosionSystem.TileData(tolerance, blockedDirections);
				return;
			}
			this._airtightMap[gridId].Remove(tile);
		}

		// Token: 0x06001AAE RID: 6830 RVA: 0x0008D234 File Offset: 0x0008B434
		private void OnAirtightDamaged(EntityUid uid, AirtightComponent airtight, DamageChangedEvent args)
		{
			if (!airtight.AirBlocked)
			{
				return;
			}
			TransformComponent transform;
			if (!this.EntityManager.TryGetComponent<TransformComponent>(uid, ref transform) || !transform.Anchored)
			{
				return;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(transform.GridUid, ref grid))
			{
				return;
			}
			this.UpdateAirtightMap(transform.GridUid.Value, grid, grid.CoordinatesToTile(transform.Coordinates), null);
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x0008D2A4 File Offset: 0x0008B4A4
		public float[] GetExplosionTolerance(EntityUid uid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<DamageableComponent> damageQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<DestructibleComponent> destructibleQuery)
		{
			FixedPoint2 totalDamageTarget = FixedPoint2.MaxValue;
			DestructibleComponent destructible;
			if (destructibleQuery.TryGetComponent(uid, ref destructible))
			{
				totalDamageTarget = this._destructibleSystem.DestroyedAt(uid, destructible);
			}
			float[] explosionTolerance = new float[this._explosionTypes.Count];
			DamageableComponent damageable;
			if (totalDamageTarget == FixedPoint2.MaxValue || !damageQuery.TryGetComponent(uid, ref damageable))
			{
				for (int i = 0; i < explosionTolerance.Length; i++)
				{
					explosionTolerance[i] = float.MaxValue;
				}
				return explosionTolerance;
			}
			foreach (KeyValuePair<string, int> keyValuePair in this._explosionTypes)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string id = text;
				int index = num;
				ExplosionPrototype explosionType;
				if (this._prototypeManager.TryIndex<ExplosionPrototype>(id, ref explosionType))
				{
					FixedPoint2 damagePerIntensity = FixedPoint2.Zero;
					foreach (KeyValuePair<string, FixedPoint2> keyValuePair2 in explosionType.DamagePerIntensity.DamageDict)
					{
						FixedPoint2 fixedPoint;
						keyValuePair2.Deconstruct(out text, out fixedPoint);
						string type = text;
						FixedPoint2 value = fixedPoint;
						if (damageable.Damage.DamageDict.ContainsKey(type))
						{
							GetExplosionResistanceEvent ev = new GetExplosionResistanceEvent(explosionType.ID);
							base.RaiseLocalEvent<GetExplosionResistanceEvent>(uid, ev, false);
							damagePerIntensity += value * Math.Max(0f, ev.DamageCoefficient);
						}
					}
					explosionTolerance[index] = ((damagePerIntensity > 0) ? ((float)((totalDamageTarget - damageable.TotalDamage) / damagePerIntensity)) : float.MaxValue);
				}
			}
			return explosionTolerance;
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x0008D478 File Offset: 0x0008B678
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GridRemovalEvent>(new EntityEventHandler<GridRemovalEvent>(this.OnGridRemoved), null, null);
			base.SubscribeLocalEvent<GridStartupEvent>(new EntityEventHandler<GridStartupEvent>(this.OnGridStartup), null, null);
			base.SubscribeLocalEvent<ExplosionResistanceComponent, GetExplosionResistanceEvent>(new ComponentEventHandler<ExplosionResistanceComponent, GetExplosionResistanceEvent>(this.OnGetResistance), null, null);
			base.SubscribeLocalEvent<ExplosionResistanceComponent, InventoryRelayedEvent<GetExplosionResistanceEvent>>(delegate(EntityUid e, ExplosionResistanceComponent c, InventoryRelayedEvent<GetExplosionResistanceEvent> ev)
			{
				this.OnGetResistance(e, c, ev.Args);
			}, null, null);
			base.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.OnTileChanged), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnReset), null, null);
			base.SubscribeLocalEvent<MapChangedEvent>(new EntityEventHandler<MapChangedEvent>(this.OnMapChanged), null, null);
			base.SubscribeLocalEvent<AirtightComponent, DamageChangedEvent>(new ComponentEventHandler<AirtightComponent, DamageChangedEvent>(this.OnAirtightDamaged), null, null);
			this.SubscribeCvars();
			this.InitAirtightMap();
			this.InitVisuals();
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x0008D540 File Offset: 0x0008B740
		private void OnReset(RoundRestartCleanupEvent ev)
		{
			this._explosionQueue.Clear();
			if (this._activeExplosion != null)
			{
				base.QueueDel(this._activeExplosion.VisualEnt);
			}
			this._activeExplosion = null;
			this._nodeGroupSystem.PauseUpdating = false;
			this._pathfindingSystem.PauseUpdating = false;
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x0008D590 File Offset: 0x0008B790
		public override void Shutdown()
		{
			base.Shutdown();
			this.UnsubscribeCvars();
			this._nodeGroupSystem.PauseUpdating = false;
			this._pathfindingSystem.PauseUpdating = false;
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x0008D5B8 File Offset: 0x0008B7B8
		private void OnGetResistance(EntityUid uid, ExplosionResistanceComponent component, GetExplosionResistanceEvent args)
		{
			args.DamageCoefficient *= component.DamageCoefficient;
			float resistance;
			if (component.Resistances.TryGetValue(args.ExplotionPrototype, out resistance))
			{
				args.DamageCoefficient *= resistance;
			}
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x0008D5FC File Offset: 0x0008B7FC
		[NullableContext(2)]
		public void TriggerExplosive(EntityUid uid, ExplosiveComponent explosive = null, bool delete = true, float? totalIntensity = null, float? radius = null, EntityUid? user = null)
		{
			if (!base.Resolve<ExplosiveComponent>(uid, ref explosive, false))
			{
				return;
			}
			if (explosive.Exploded)
			{
				return;
			}
			explosive.Exploded = true;
			float value;
			if (radius != null)
			{
				value = totalIntensity.GetValueOrDefault();
				if (totalIntensity == null)
				{
					value = this.RadiusToIntensity(radius.Value, explosive.IntensitySlope, explosive.MaxIntensity);
					totalIntensity = new float?(value);
				}
			}
			value = totalIntensity.GetValueOrDefault();
			if (totalIntensity == null)
			{
				value = explosive.TotalIntensity;
				totalIntensity = new float?(value);
			}
			this.QueueExplosion(uid, explosive.ExplosionType, totalIntensity.Value, explosive.IntensitySlope, explosive.MaxIntensity, explosive.TileBreakScale, explosive.MaxTileBreak, explosive.CanCreateVacuum, explosive.CanShakeGrid, user, true);
			if (delete)
			{
				this.EntityManager.QueueDeleteEntity(uid);
			}
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x0008D6D0 File Offset: 0x0008B8D0
		public float RadiusToIntensity(float radius, float slope, float maxIntensity = 0f)
		{
			float coneVolume = slope * 3.1415927f / 3f * MathF.Pow(radius, 3f);
			if (maxIntensity <= 0f || slope * radius < maxIntensity)
			{
				return coneVolume;
			}
			float h = slope * radius - maxIntensity;
			return coneVolume - h * 3.1415927f / 3f * MathF.Pow(h / slope, 2f);
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x0008D72C File Offset: 0x0008B92C
		public float IntensityToRadius(float totalIntensity, float slope, float maxIntensity)
		{
			float r0 = maxIntensity / slope;
			float v0 = this.RadiusToIntensity(r0, slope, 0f);
			if (totalIntensity <= v0)
			{
				return MathF.Cbrt(3f * totalIntensity / (slope * 3.1415927f));
			}
			return r0 * (MathF.Sqrt(12f * totalIntensity / v0 - 3f) / 6f + 0.5f);
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x0008D788 File Offset: 0x0008B988
		public void QueueExplosion(EntityUid uid, string typeId, float totalIntensity, float slope, float maxTileIntensity, float tileBreakScale = 1f, int maxTileBreak = 2147483647, bool canCreateVacuum = true, bool canShakeGrid = false, EntityUid? user = null, bool addLog = true)
		{
			MapCoordinates pos = base.Transform(uid).MapPosition;
			this.QueueExplosion(pos, typeId, totalIntensity, slope, maxTileIntensity, tileBreakScale, maxTileBreak, canCreateVacuum, canShakeGrid, false);
			if (!addLog)
			{
				return;
			}
			LogStringHandler logStringHandler;
			if (user == null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Explosion;
				LogImpact impact = LogImpact.High;
				logStringHandler = new LogStringHandler(36, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" exploded at ");
				logStringHandler.AppendFormatted<MapCoordinates>(pos, "coordinates", "pos");
				logStringHandler.AppendLiteral(" with intensity ");
				logStringHandler.AppendFormatted<float>(totalIntensity, "totalIntensity");
				logStringHandler.AppendLiteral(" slope ");
				logStringHandler.AppendFormatted<float>(slope, "slope");
				adminLogger.Add(type, impact, ref logStringHandler);
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-explosion-no-player", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entity", base.ToPrettyString(uid)),
					new ValueTuple<string, object>("coordinates", pos),
					new ValueTuple<string, object>("intensity", totalIntensity),
					new ValueTuple<string, object>("slope", slope)
				}));
				return;
			}
			ISharedAdminLogManager adminLogger2 = this._adminLogger;
			LogType type2 = LogType.Explosion;
			LogImpact impact2 = LogImpact.High;
			logStringHandler = new LogStringHandler(46, 5);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
			logStringHandler.AppendLiteral(" caused ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to explode at ");
			logStringHandler.AppendFormatted<MapCoordinates>(pos, "coordinates", "pos");
			logStringHandler.AppendLiteral(" with intensity ");
			logStringHandler.AppendFormatted<float>(totalIntensity, "totalIntensity");
			logStringHandler.AppendLiteral(" slope ");
			logStringHandler.AppendFormatted<float>(slope, "slope");
			adminLogger2.Add(type2, impact2, ref logStringHandler);
			this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-explosion-player", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("player", base.ToPrettyString(user.Value)),
				new ValueTuple<string, object>("entity", base.ToPrettyString(uid)),
				new ValueTuple<string, object>("coordinates", pos),
				new ValueTuple<string, object>("intensity", totalIntensity),
				new ValueTuple<string, object>("slope", slope)
			}));
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x0008DA1C File Offset: 0x0008BC1C
		public void QueueExplosion(MapCoordinates epicenter, string typeId, float totalIntensity, float slope, float maxTileIntensity, float tileBreakScale = 1f, int maxTileBreak = 2147483647, bool canCreateVacuum = true, bool canShakeGrid = false, bool addLog = false)
		{
			if (totalIntensity <= 0f || slope <= 0f)
			{
				return;
			}
			ExplosionPrototype type;
			if (!this._prototypeManager.TryIndex<ExplosionPrototype>(typeId, ref type))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Attempted to spawn unknown explosion prototype: ");
				defaultInterpolatedStringHandler.AppendFormatted<ExplosionPrototype>(type);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (addLog)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type2 = LogType.Explosion;
				LogImpact impact = LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(44, 3);
				logStringHandler.AppendLiteral("Explosion spawned at ");
				logStringHandler.AppendFormatted<MapCoordinates>(epicenter, "coordinates", "epicenter");
				logStringHandler.AppendLiteral(" with intensity ");
				logStringHandler.AppendFormatted<float>(totalIntensity, "totalIntensity");
				logStringHandler.AppendLiteral(" slope ");
				logStringHandler.AppendFormatted<float>(slope, "slope");
				adminLogger.Add(type2, impact, ref logStringHandler);
			}
			this._explosionQueue.Enqueue(() => this.SpawnExplosion(epicenter, type, totalIntensity, slope, maxTileIntensity, tileBreakScale, maxTileBreak, canCreateVacuum, canShakeGrid));
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x0008DB68 File Offset: 0x0008BD68
		[return: Nullable(2)]
		private Explosion SpawnExplosion(MapCoordinates epicenter, ExplosionPrototype type, float totalIntensity, float slope, float maxTileIntensity, float tileBreakScale, int maxTileBreak, bool canCreateVacuum, bool canShakeGrid)
		{
			if (!this._mapManager.MapExists(epicenter.MapId))
			{
				return null;
			}
			ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3>? results = this.GetExplosionTiles(epicenter, type.ID, totalIntensity, slope, maxTileIntensity);
			if (results == null)
			{
				return null;
			}
			ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3> value = results.Value;
			int area = value.Item1;
			List<float> iterationIntensity = value.Item2;
			ExplosionSpaceTileFlood spaceData = value.Item3;
			Dictionary<EntityUid, ExplosionGridTileFlood> gridData = value.Item4;
			Matrix3 spaceMatrix = value.Item5;
			EntityUid visualEnt = this.CreateExplosionVisualEntity(epicenter, type.ID, spaceMatrix, spaceData, gridData.Values, iterationIntensity);
			if (canShakeGrid)
			{
				this.ShakeGrid(epicenter, totalIntensity);
			}
			this.CameraShake((float)iterationIntensity.Count * 2.5f, epicenter, totalIntensity);
			EntityCoordinates mapEntityCoords = EntityCoordinates.FromMap(this.EntityManager, this._mapManager.GetMapEntityId(epicenter.MapId), epicenter);
			int audioRange = iterationIntensity.Count * 5;
			Filter filter = Filter.Pvs(epicenter, 2f).AddInRange(epicenter, (float)audioRange, null, null);
			SoundSystem.Play(type.Sound.GetSound(null, null), filter, mapEntityCoords, new AudioParams?(this._audioParams));
			if (canShakeGrid)
			{
				this.ShakeGrid(epicenter, totalIntensity);
			}
			return new Explosion(this, type, spaceData, gridData.Values.ToList<ExplosionGridTileFlood>(), iterationIntensity, epicenter, spaceMatrix, area, tileBreakScale, maxTileBreak, canCreateVacuum, this.EntityManager, this._mapManager, visualEnt);
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x0008DCAC File Offset: 0x0008BEAC
		private void CameraShake(float range, MapCoordinates epicenter, float totalIntensity)
		{
			Filter filter = Filter.Empty();
			filter.AddInRange(epicenter, range, this._playerManager, this.EntityManager);
			foreach (ICommonSession player in filter.Recipients)
			{
				EntityUid? attachedEntity = player.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid uid = attachedEntity.GetValueOrDefault();
					Vector2 playerPos = base.Transform(player.AttachedEntity.Value).WorldPosition;
					Vector2 delta = epicenter.Position - playerPos;
					if (delta.EqualsApprox(Vector2.Zero))
					{
						delta..ctor(0.01f, 0f);
					}
					float distance = delta.Length;
					float effect = 5f * MathF.Pow(totalIntensity, 0.5f) * (1f - distance / range);
					if (effect > 0.01f)
					{
						this._recoilSystem.KickCamera(uid, -delta.Normalized * effect, null);
					}
				}
			}
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x0008DDC8 File Offset: 0x0008BFC8
		private void ShakeGrid(MapCoordinates epicenter, float totalIntensity)
		{
			foreach (MapGridComponent grid in this.MapManager.GetAllMapGrids(epicenter.MapId))
			{
				if (base.HasComp<MapGridComponent>(grid.Owner))
				{
					this.CameraShake(1000f, epicenter, totalIntensity);
					this.PlayShakeSound(grid.Owner, epicenter);
				}
			}
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x0008DE44 File Offset: 0x0008C044
		private void PlayShakeSound(EntityUid uid, MapCoordinates epicenter)
		{
			EntityCoordinates.FromMap(this.EntityManager, this._mapManager.GetMapEntityId(epicenter.MapId), epicenter);
			Filter filter = Filter.Pvs(epicenter, 2f).AddPlayersByPvs(epicenter, 1000f, null, null, null);
			this._audio.PlayGlobal(this._meteorsHit, filter, false, new AudioParams?(AudioHelpers.WithVariation(1f).WithVolume(-10f)));
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06001ABD RID: 6845 RVA: 0x0008DEB9 File Offset: 0x0008C0B9
		// (set) Token: 0x06001ABE RID: 6846 RVA: 0x0008DEC1 File Offset: 0x0008C0C1
		public int MaxIterations { get; private set; }

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06001ABF RID: 6847 RVA: 0x0008DECA File Offset: 0x0008C0CA
		// (set) Token: 0x06001AC0 RID: 6848 RVA: 0x0008DED2 File Offset: 0x0008C0D2
		public int MaxArea { get; private set; }

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06001AC1 RID: 6849 RVA: 0x0008DEDB File Offset: 0x0008C0DB
		// (set) Token: 0x06001AC2 RID: 6850 RVA: 0x0008DEE3 File Offset: 0x0008C0E3
		public float MaxProcessingTime { get; private set; }

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06001AC3 RID: 6851 RVA: 0x0008DEEC File Offset: 0x0008C0EC
		// (set) Token: 0x06001AC4 RID: 6852 RVA: 0x0008DEF4 File Offset: 0x0008C0F4
		public int TilesPerTick { get; private set; }

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06001AC5 RID: 6853 RVA: 0x0008DEFD File Offset: 0x0008C0FD
		// (set) Token: 0x06001AC6 RID: 6854 RVA: 0x0008DF05 File Offset: 0x0008C105
		public int ThrowLimit { get; private set; }

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06001AC7 RID: 6855 RVA: 0x0008DF0E File Offset: 0x0008C10E
		// (set) Token: 0x06001AC8 RID: 6856 RVA: 0x0008DF16 File Offset: 0x0008C116
		public bool SleepNodeSys { get; private set; }

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06001AC9 RID: 6857 RVA: 0x0008DF1F File Offset: 0x0008C11F
		// (set) Token: 0x06001ACA RID: 6858 RVA: 0x0008DF27 File Offset: 0x0008C127
		public bool IncrementalTileBreaking { get; private set; }

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06001ACB RID: 6859 RVA: 0x0008DF30 File Offset: 0x0008C130
		// (set) Token: 0x06001ACC RID: 6860 RVA: 0x0008DF38 File Offset: 0x0008C138
		public int SingleTickAreaLimit { get; private set; }

		// Token: 0x06001ACD RID: 6861 RVA: 0x0008DF44 File Offset: 0x0008C144
		private void SubscribeCvars()
		{
			this._cfg.OnValueChanged<int>(CCVars.ExplosionTilesPerTick, new Action<int>(this.SetTilesPerTick), true);
			this._cfg.OnValueChanged<int>(CCVars.ExplosionThrowLimit, new Action<int>(this.SetThrowLimit), true);
			this._cfg.OnValueChanged<bool>(CCVars.ExplosionSleepNodeSys, new Action<bool>(this.SetSleepNodeSys), true);
			this._cfg.OnValueChanged<int>(CCVars.ExplosionMaxArea, new Action<int>(this.SetMaxArea), true);
			this._cfg.OnValueChanged<int>(CCVars.ExplosionMaxIterations, new Action<int>(this.SetMaxIterations), true);
			this._cfg.OnValueChanged<float>(CCVars.ExplosionMaxProcessingTime, new Action<float>(this.SetMaxProcessingTime), true);
			this._cfg.OnValueChanged<bool>(CCVars.ExplosionIncrementalTileBreaking, new Action<bool>(this.SetIncrementalTileBreaking), true);
			this._cfg.OnValueChanged<int>(CCVars.ExplosionSingleTickAreaLimit, new Action<int>(this.SetSingleTickAreaLimit), true);
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x0008E03C File Offset: 0x0008C23C
		private void UnsubscribeCvars()
		{
			this._cfg.UnsubValueChanged<int>(CCVars.ExplosionTilesPerTick, new Action<int>(this.SetTilesPerTick));
			this._cfg.UnsubValueChanged<int>(CCVars.ExplosionThrowLimit, new Action<int>(this.SetThrowLimit));
			this._cfg.UnsubValueChanged<bool>(CCVars.ExplosionSleepNodeSys, new Action<bool>(this.SetSleepNodeSys));
			this._cfg.UnsubValueChanged<int>(CCVars.ExplosionMaxArea, new Action<int>(this.SetMaxArea));
			this._cfg.UnsubValueChanged<int>(CCVars.ExplosionMaxIterations, new Action<int>(this.SetMaxIterations));
			this._cfg.UnsubValueChanged<float>(CCVars.ExplosionMaxProcessingTime, new Action<float>(this.SetMaxProcessingTime));
			this._cfg.UnsubValueChanged<bool>(CCVars.ExplosionIncrementalTileBreaking, new Action<bool>(this.SetIncrementalTileBreaking));
			this._cfg.UnsubValueChanged<int>(CCVars.ExplosionSingleTickAreaLimit, new Action<int>(this.SetSingleTickAreaLimit));
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x0008E129 File Offset: 0x0008C329
		private void SetTilesPerTick(int value)
		{
			this.TilesPerTick = value;
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x0008E132 File Offset: 0x0008C332
		private void SetThrowLimit(int value)
		{
			this.ThrowLimit = value;
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x0008E13B File Offset: 0x0008C33B
		private void SetSleepNodeSys(bool value)
		{
			this.SleepNodeSys = value;
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x0008E144 File Offset: 0x0008C344
		private void SetMaxArea(int value)
		{
			this.MaxArea = value;
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x0008E14D File Offset: 0x0008C34D
		private void SetMaxIterations(int value)
		{
			this.MaxIterations = value;
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x0008E156 File Offset: 0x0008C356
		private void SetMaxProcessingTime(float value)
		{
			this.MaxProcessingTime = value;
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x0008E15F File Offset: 0x0008C35F
		private void SetIncrementalTileBreaking(bool value)
		{
			this.IncrementalTileBreaking = value;
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x0008E168 File Offset: 0x0008C368
		private void SetSingleTickAreaLimit(int value)
		{
			this.SingleTickAreaLimit = value;
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x0008E174 File Offset: 0x0008C374
		private void OnGridStartup(GridStartupEvent ev)
		{
			MapGridComponent grid = this._mapManager.GetGrid(ev.EntityUid);
			Dictionary<Vector2i, ExplosionSystem.NeighborFlag> edges = new Dictionary<Vector2i, ExplosionSystem.NeighborFlag>();
			this._gridEdges[ev.EntityUid] = edges;
			foreach (TileRef tileRef in grid.GetAllTiles(true))
			{
				ExplosionSystem.NeighborFlag dir;
				if (this.IsEdge(grid, tileRef.GridIndices, out dir))
				{
					edges.Add(tileRef.GridIndices, dir);
				}
			}
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x0008E204 File Offset: 0x0008C404
		private void OnGridRemoved(GridRemovalEvent ev)
		{
			this._airtightMap.Remove(ev.EntityUid);
			this._gridEdges.Remove(ev.EntityUid);
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x0008E22C File Offset: 0x0008C42C
		[return: Nullable(new byte[]
		{
			0,
			1,
			1
		})]
		public ValueTuple<Dictionary<Vector2i, BlockedSpaceTile>, ushort> TransformGridEdges(MapCoordinates epicentre, EntityUid? referenceGrid, List<EntityUid> localGrids, float maxDistance)
		{
			Dictionary<Vector2i, BlockedSpaceTile> transformedEdges = new Dictionary<Vector2i, BlockedSpaceTile>();
			Matrix3 targetMatrix = Matrix3.Identity;
			Angle targetAngle = default(Angle);
			ushort tileSize = 1;
			int maxDistanceSq = (int)(maxDistance * maxDistance);
			if (referenceGrid != null)
			{
				MapGridComponent targetGrid = this._mapManager.GetGrid(referenceGrid.Value);
				TransformComponent transformComponent = base.Transform(targetGrid.Owner);
				targetAngle = transformComponent.WorldRotation;
				targetMatrix = transformComponent.InvWorldMatrix;
				tileSize = targetGrid.TileSize;
			}
			Matrix3 offsetMatrix = Matrix3.Identity;
			offsetMatrix.R0C2 = (float)tileSize / 2f;
			offsetMatrix.R1C2 = (float)tileSize / 2f;
			foreach (EntityUid gridToTransform in localGrids)
			{
				Dictionary<Vector2i, ExplosionSystem.NeighborFlag> edges;
				MapGridComponent grid;
				if (!(gridToTransform == referenceGrid) && this._gridEdges.TryGetValue(gridToTransform, out edges) && this._mapManager.TryGetGrid(new EntityUid?(gridToTransform), ref grid))
				{
					if (grid.TileSize != tileSize)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Explosions do not support grids with different grid sizes. GridIds: ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(gridToTransform);
						defaultInterpolatedStringHandler.AppendLiteral(" and ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(referenceGrid);
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						EntityQuery<TransformComponent> xforms = this.EntityManager.GetEntityQuery<TransformComponent>();
						ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = xforms.GetComponent(grid.Owner).GetWorldPositionRotationMatrixWithInv(xforms);
						Angle gridWorldRotation = worldPositionRotationMatrixWithInv.Item2;
						Matrix3 gridWorldMatrix = worldPositionRotationMatrixWithInv.Item3;
						Matrix3 invGridWorldMatrid = worldPositionRotationMatrixWithInv.Item4;
						Vector2i localEpicentre = (Vector2i)invGridWorldMatrid.Transform(epicentre.Position);
						Matrix3 matrix2 = ref offsetMatrix * ref gridWorldMatrix;
						Matrix3 matrix = ref matrix2 * ref targetMatrix;
						Angle angle = gridWorldRotation - targetAngle;
						Vector2 vector = new ValueTuple<float, float>((float)tileSize / 4f, (float)tileSize / 4f);
						float num;
						float num2;
						angle.RotateVec(ref vector).Deconstruct(ref num, ref num2);
						float x = num;
						float y = num2;
						foreach (KeyValuePair<Vector2i, ExplosionSystem.NeighborFlag> keyValuePair in edges)
						{
							Vector2i vector2i;
							ExplosionSystem.NeighborFlag neighborFlag;
							keyValuePair.Deconstruct(out vector2i, out neighborFlag);
							Vector2i tile = vector2i;
							ExplosionSystem.NeighborFlag dir = neighborFlag;
							Vector2i delta = tile - localEpicentre;
							if (delta.X * delta.X + delta.Y * delta.Y <= maxDistanceSq)
							{
								Vector2 center = matrix.Transform(tile);
								if ((dir & ExplosionSystem.NeighborFlag.Cardinal) == ExplosionSystem.NeighborFlag.Invalid)
								{
									Vector2i newIndex;
									newIndex..ctor((int)MathF.Floor(center.X), (int)MathF.Floor(center.Y));
									BlockedSpaceTile data;
									if (!transformedEdges.TryGetValue(newIndex, out data))
									{
										data = new BlockedSpaceTile();
										transformedEdges[newIndex] = data;
									}
									List<BlockedSpaceTile.GridEdgeData> blockingGridEdges = data.BlockingGridEdges;
									vector2i = default(Vector2i);
									blockingGridEdges.Add(new BlockedSpaceTile.GridEdgeData(vector2i, null, center, angle, (float)tileSize));
								}
								else
								{
									foreach (Vector2i newIndices in new HashSet<Vector2i>
									{
										new Vector2i((int)MathF.Floor(center.X + x), (int)MathF.Floor(center.Y + x)),
										new Vector2i((int)MathF.Floor(center.X - y), (int)MathF.Floor(center.Y - y)),
										new Vector2i((int)MathF.Floor(center.X - x), (int)MathF.Floor(center.Y + y)),
										new Vector2i((int)MathF.Floor(center.X + y), (int)MathF.Floor(center.Y - x))
									})
									{
										BlockedSpaceTile data2;
										if (!transformedEdges.TryGetValue(newIndices, out data2))
										{
											data2 = new BlockedSpaceTile();
											transformedEdges[newIndices] = data2;
										}
										data2.BlockingGridEdges.Add(new BlockedSpaceTile.GridEdgeData(tile, new EntityUid?(gridToTransform), center, angle, (float)tileSize));
									}
								}
							}
						}
					}
				}
			}
			if (referenceGrid == null)
			{
				return new ValueTuple<Dictionary<Vector2i, BlockedSpaceTile>, ushort>(transformedEdges, tileSize);
			}
			Dictionary<Vector2i, ExplosionSystem.NeighborFlag> localEdges;
			if (this._gridEdges.TryGetValue(referenceGrid.Value, out localEdges))
			{
				foreach (KeyValuePair<Vector2i, ExplosionSystem.NeighborFlag> keyValuePair in localEdges)
				{
					Vector2i vector2i;
					ExplosionSystem.NeighborFlag neighborFlag;
					keyValuePair.Deconstruct(out vector2i, out neighborFlag);
					Vector2i tile2 = vector2i;
					bool flag = neighborFlag != ExplosionSystem.NeighborFlag.Invalid;
					BlockedSpaceTile data3 = new BlockedSpaceTile();
					transformedEdges[tile2] = data3;
					data3.UnblockedDirections = AtmosDirection.Invalid;
					if (((flag ? 1 : 0) & 85) == 0)
					{
						List<BlockedSpaceTile.GridEdgeData> blockingGridEdges2 = data3.BlockingGridEdges;
						vector2i = default(Vector2i);
						blockingGridEdges2.Add(new BlockedSpaceTile.GridEdgeData(vector2i, null, (tile2 + 0.5f) * (float)tileSize, 0f, (float)tileSize));
					}
					else
					{
						data3.BlockingGridEdges.Add(new BlockedSpaceTile.GridEdgeData(tile2, new EntityUid?(referenceGrid.Value), (tile2 + 0.5f) * (float)tileSize, 0f, (float)tileSize));
					}
				}
			}
			return new ValueTuple<Dictionary<Vector2i, BlockedSpaceTile>, ushort>(transformedEdges, tileSize);
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x0008E7D4 File Offset: 0x0008C9D4
		public void GetUnblockedDirections(Dictionary<Vector2i, BlockedSpaceTile> transformedEdges, float tileSize)
		{
			foreach (KeyValuePair<Vector2i, BlockedSpaceTile> keyValuePair in transformedEdges)
			{
				Vector2i vector2i;
				BlockedSpaceTile blockedSpaceTile;
				keyValuePair.Deconstruct(out vector2i, out blockedSpaceTile);
				Vector2i tile = vector2i;
				BlockedSpaceTile data = blockedSpaceTile;
				if (data.UnblockedDirections != AtmosDirection.Invalid)
				{
					Vector2 tileCenter = (tile + 0.5f) * tileSize;
					foreach (BlockedSpaceTile.GridEdgeData edge in data.BlockingGridEdges)
					{
						if (edge.Box.Contains(tileCenter))
						{
							data.UnblockedDirections = AtmosDirection.Invalid;
							break;
						}
						if (edge.Box.Contains(tileCenter + new ValueTuple<float, float>(0f, tileSize / 2f)))
						{
							data.UnblockedDirections &= ~AtmosDirection.North;
						}
						if (edge.Box.Contains(tileCenter + new ValueTuple<float, float>(0f, -tileSize / 2f)))
						{
							data.UnblockedDirections &= ~AtmosDirection.South;
						}
						if (edge.Box.Contains(tileCenter + new ValueTuple<float, float>(tileSize / 2f, 0f)))
						{
							data.UnblockedDirections &= ~AtmosDirection.East;
						}
						if (edge.Box.Contains(tileCenter + new ValueTuple<float, float>(-tileSize / 2f, 0f)))
						{
							data.UnblockedDirections &= ~AtmosDirection.West;
						}
					}
				}
			}
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x0008E9B8 File Offset: 0x0008CBB8
		private void OnTileChanged(ref TileChangedEvent ev)
		{
			if (!ev.NewTile.Tile.IsEmpty && !ev.OldTile.IsEmpty)
			{
				return;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(new EntityUid?(ev.Entity), ref grid))
			{
				return;
			}
			TileRef tileRef = ev.NewTile;
			Dictionary<Vector2i, ExplosionSystem.NeighborFlag> edges;
			if (!this._gridEdges.TryGetValue(tileRef.GridUid, out edges))
			{
				edges = new Dictionary<Vector2i, ExplosionSystem.NeighborFlag>();
				this._gridEdges[tileRef.GridUid] = edges;
			}
			if (tileRef.Tile.IsEmpty)
			{
				edges.Remove(tileRef.GridIndices);
				for (int i = 0; i < ExplosionSystem.NeighbourVectors.Length; i++)
				{
					Vector2i neighbourIndex = tileRef.GridIndices + ExplosionSystem.NeighbourVectors[i];
					TileRef neighbourTile;
					if (grid.TryGetTileRef(neighbourIndex, ref neighbourTile) && !neighbourTile.Tile.IsEmpty)
					{
						ExplosionSystem.NeighborFlag oppositeDirection = (ExplosionSystem.NeighborFlag)(1 << (i + 4) % 8);
						edges[neighbourIndex] = (edges.GetValueOrDefault(neighbourIndex) | oppositeDirection);
					}
				}
				return;
			}
			for (int j = 0; j < ExplosionSystem.NeighbourVectors.Length; j++)
			{
				Vector2i neighbourIndex2 = tileRef.GridIndices + ExplosionSystem.NeighbourVectors[j];
				ExplosionSystem.NeighborFlag neighborSpaceDir;
				if (edges.TryGetValue(neighbourIndex2, out neighborSpaceDir))
				{
					ExplosionSystem.NeighborFlag oppositeDirection2 = (ExplosionSystem.NeighborFlag)(1 << (j + 4) % 8);
					neighborSpaceDir &= ~oppositeDirection2;
					if (neighborSpaceDir == ExplosionSystem.NeighborFlag.Invalid)
					{
						edges.Remove(neighbourIndex2);
					}
					else
					{
						edges[neighbourIndex2] = neighborSpaceDir;
					}
				}
			}
			ExplosionSystem.NeighborFlag spaceDir;
			if (this.IsEdge(grid, tileRef.GridIndices, out spaceDir))
			{
				edges.Add(tileRef.GridIndices, spaceDir);
			}
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x0008EB44 File Offset: 0x0008CD44
		private bool IsEdge(MapGridComponent grid, Vector2i index, out ExplosionSystem.NeighborFlag spaceDirections)
		{
			spaceDirections = ExplosionSystem.NeighborFlag.Invalid;
			for (int i = 0; i < ExplosionSystem.NeighbourVectors.Length; i++)
			{
				TileRef neighborTile;
				if (!grid.TryGetTileRef(index + ExplosionSystem.NeighbourVectors[i], ref neighborTile) || neighborTile.Tile.IsEmpty)
				{
					spaceDirections |= (ExplosionSystem.NeighborFlag)(1 << i);
				}
			}
			return spaceDirections > ExplosionSystem.NeighborFlag.Invalid;
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x0008EBA0 File Offset: 0x0008CDA0
		public static bool AnyNeighborBlocked(ExplosionSystem.NeighborFlag neighbors, AtmosDirection blockedDirs)
		{
			return ((neighbors & ExplosionSystem.NeighborFlag.North) == ExplosionSystem.NeighborFlag.North && (blockedDirs & AtmosDirection.North) == AtmosDirection.North) || ((neighbors & ExplosionSystem.NeighborFlag.South) == ExplosionSystem.NeighborFlag.South && (blockedDirs & AtmosDirection.South) == AtmosDirection.South) || ((neighbors & ExplosionSystem.NeighborFlag.East) == ExplosionSystem.NeighborFlag.East && (blockedDirs & AtmosDirection.East) == AtmosDirection.East) || ((neighbors & ExplosionSystem.NeighborFlag.West) == ExplosionSystem.NeighborFlag.West && (blockedDirs & AtmosDirection.West) == AtmosDirection.West);
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x0008EBE0 File Offset: 0x0008CDE0
		private void OnMapChanged(MapChangedEvent ev)
		{
			if (ev.Created)
			{
				return;
			}
			Explosion activeExplosion = this._activeExplosion;
			MapId? mapId = (activeExplosion != null) ? new MapId?(activeExplosion.Epicenter.MapId) : null;
			MapId map = ev.Map;
			if (mapId == null || (mapId != null && mapId.GetValueOrDefault() != map))
			{
				return;
			}
			base.QueueDel(this._activeExplosion.VisualEnt);
			this._activeExplosion = null;
			this._nodeGroupSystem.PauseUpdating = false;
			this._pathfindingSystem.PauseUpdating = false;
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x0008EC7C File Offset: 0x0008CE7C
		public override void Update(float frameTime)
		{
			if (this._activeExplosion == null && this._explosionQueue.Count == 0)
			{
				return;
			}
			this.Stopwatch.Restart();
			double totalMilliseconds = this.Stopwatch.Elapsed.TotalMilliseconds;
			float maxProcessingTime = this.MaxProcessingTime;
			int tilesRemaining = this.TilesPerTick;
			while (tilesRemaining > 0 && (double)this.MaxProcessingTime > this.Stopwatch.Elapsed.TotalMilliseconds)
			{
				if (this._activeExplosion == null)
				{
					Func<Explosion> spawnNextExplosion;
					if ((double)MathF.Max(this.MaxProcessingTime - 1f, 0.1f) < this.Stopwatch.Elapsed.TotalMilliseconds || !this._explosionQueue.TryDequeue(out spawnNextExplosion))
					{
						break;
					}
					this._activeExplosion = spawnNextExplosion();
					if (this._activeExplosion == null)
					{
						continue;
					}
					this._previousTileIteration = 0;
					if (this.SleepNodeSys)
					{
						this._nodeGroupSystem.PauseUpdating = true;
						this._pathfindingSystem.PauseUpdating = true;
					}
					if (this._activeExplosion.Area > this.SingleTickAreaLimit)
					{
						break;
					}
				}
				try
				{
					int processed = this._activeExplosion.Process(tilesRemaining);
					tilesRemaining -= processed;
					if (this._activeExplosion.FinishedProcessing)
					{
						base.EnsureComp<TimedDespawnComponent>(this._activeExplosion.VisualEnt).Lifetime = this._cfg.GetCVar<float>(CCVars.ExplosionPersistence);
						this._appearance.SetData(this._activeExplosion.VisualEnt, ExplosionAppearanceData.Progress, int.MaxValue, null);
						this._activeExplosion = null;
					}
				}
				catch (Exception)
				{
					if (this._activeExplosion != null)
					{
						base.QueueDel(this._activeExplosion.VisualEnt);
					}
					this._activeExplosion = null;
					this._nodeGroupSystem.PauseUpdating = false;
					this._pathfindingSystem.PauseUpdating = false;
					throw;
				}
			}
			string text = "Explosion";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Processed ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.TilesPerTick - tilesRemaining);
			defaultInterpolatedStringHandler.AppendLiteral(" tiles in ");
			defaultInterpolatedStringHandler.AppendFormatted<double>(this.Stopwatch.Elapsed.TotalMilliseconds);
			defaultInterpolatedStringHandler.AppendLiteral("ms");
			Logger.InfoS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			if (this._activeExplosion != null)
			{
				this._appearance.SetData(this._activeExplosion.VisualEnt, ExplosionAppearanceData.Progress, this._activeExplosion.CurrentIteration + 1, null);
				return;
			}
			if (this._explosionQueue.Count > 0)
			{
				return;
			}
			this._nodeGroupSystem.PauseUpdating = false;
			this._pathfindingSystem.PauseUpdating = false;
		}

		// Token: 0x06001AE0 RID: 6880 RVA: 0x0008EF1C File Offset: 0x0008D11C
		public bool IsBlockingTurf(EntityUid uid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> physicsQuery)
		{
			PhysicsComponent physics;
			return !this.EntityManager.IsQueuedForDeletion(uid) && physicsQuery.TryGetComponent(uid, ref physics) && (physics.CanCollide && physics.Hard) && (physics.CollisionLayer & 2) != 0;
		}

		// Token: 0x06001AE1 RID: 6881 RVA: 0x0008EF64 File Offset: 0x0008D164
		internal bool ExplodeTile(BroadphaseComponent lookup, MapGridComponent grid, Vector2i tile, float throwForce, DamageSpecifier damage, MapCoordinates epicenter, HashSet<EntityUid> processed, string id, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<DamageableComponent> damageQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> physicsQuery, LookupFlags flags)
		{
			Box2 gridBox;
			gridBox..ctor(tile * (int)grid.TileSize, (tile + 1) * (int)grid.TileSize);
			List<TransformComponent> list = new List<TransformComponent>();
			ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>> state = new ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>(list, processed, xformQuery);
			lookup.DynamicTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(this.GridQueryCallback), gridBox, true);
			lookup.StaticTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(this.GridQueryCallback), gridBox, true);
			lookup.SundriesTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(this.GridQueryCallback), gridBox, true);
			lookup.StaticSundriesTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(this.GridQueryCallback), gridBox, true);
			foreach (TransformComponent xform in list)
			{
				this.ProcessEntity(xform.Owner, epicenter, damage, throwForce, id, damageQuery, physicsQuery, xform);
			}
			bool tileBlocked = false;
			List<EntityUid> anchoredList = grid.GetAnchoredEntities(tile).ToList<EntityUid>();
			foreach (EntityUid entity in anchoredList)
			{
				processed.Add(entity);
				this.ProcessEntity(entity, epicenter, damage, throwForce, id, damageQuery, physicsQuery, null);
			}
			if (anchoredList.Count > 0)
			{
				foreach (EntityUid entity2 in grid.GetAnchoredEntities(tile))
				{
					tileBlocked |= this.IsBlockingTurf(entity2, physicsQuery);
				}
			}
			if (throwForce <= 0f)
			{
				return !tileBlocked;
			}
			list.Clear();
			lookup.DynamicTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(this.GridQueryCallback), gridBox, true);
			lookup.SundriesTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>>>(this.GridQueryCallback), gridBox, true);
			foreach (TransformComponent xform2 in list)
			{
				this.ProcessEntity(xform2.Owner, epicenter, null, throwForce, id, damageQuery, physicsQuery, xform2);
			}
			return !tileBlocked;
		}

		// Token: 0x06001AE2 RID: 6882 RVA: 0x0008F1D0 File Offset: 0x0008D3D0
		private bool GridQueryCallback([TupleElementNames(new string[]
		{
			"List",
			"Processed",
			"XformQuery"
		})] [Nullable(new byte[]
		{
			0,
			1,
			1,
			1,
			0,
			1
		})] ref ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>> state, in EntityUid uid)
		{
			TransformComponent xform;
			if (state.Item2.Add(uid) && state.Item3.TryGetComponent(uid, ref xform))
			{
				state.Item1.Add(xform);
			}
			return true;
		}

		// Token: 0x06001AE3 RID: 6883 RVA: 0x0008F214 File Offset: 0x0008D414
		private bool GridQueryCallback([TupleElementNames(new string[]
		{
			"List",
			"Processed",
			"XformQuery"
		})] [Nullable(new byte[]
		{
			0,
			1,
			1,
			1,
			0,
			1
		})] ref ValueTuple<List<TransformComponent>, HashSet<EntityUid>, EntityQuery<TransformComponent>> state, in FixtureProxy proxy)
		{
			EntityUid owner = proxy.Fixture.Body.Owner;
			return this.GridQueryCallback(ref state, owner);
		}

		// Token: 0x06001AE4 RID: 6884 RVA: 0x0008F23C File Offset: 0x0008D43C
		internal void ExplodeSpace(BroadphaseComponent lookup, Matrix3 spaceMatrix, Matrix3 invSpaceMatrix, Vector2i tile, float throwForce, DamageSpecifier damage, MapCoordinates epicenter, HashSet<EntityUid> processed, string id, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<DamageableComponent> damageQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> physicsQuery, LookupFlags flags)
		{
			Box2 gridBox = Box2.FromDimensions(tile * 1, new ValueTuple<float, float>(1f, 1f));
			Box2 worldBox = spaceMatrix.TransformBox(ref gridBox);
			List<TransformComponent> list = new List<TransformComponent>();
			ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2> state = new ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>(list, processed, invSpaceMatrix, lookup.Owner, xformQuery, gridBox);
			lookup.DynamicTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(this.SpaceQueryCallback), worldBox, true);
			lookup.StaticTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(this.SpaceQueryCallback), worldBox, true);
			lookup.SundriesTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(this.SpaceQueryCallback), worldBox, true);
			lookup.StaticSundriesTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(this.SpaceQueryCallback), worldBox, true);
			foreach (TransformComponent xform in state.Item1)
			{
				processed.Add(xform.Owner);
				this.ProcessEntity(xform.Owner, epicenter, damage, throwForce, id, damageQuery, physicsQuery, xform);
			}
			if (throwForce <= 0f)
			{
				return;
			}
			list.Clear();
			lookup.DynamicTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(ref state, new DynamicTree<FixtureProxy>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(this.SpaceQueryCallback), worldBox, true);
			lookup.SundriesTree.QueryAabb<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(ref state, new DynamicTree<EntityUid>.QueryCallbackDelegate<ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2>>(this.SpaceQueryCallback), worldBox, true);
			foreach (TransformComponent xform2 in list)
			{
				this.ProcessEntity(xform2.Owner, epicenter, null, throwForce, id, damageQuery, physicsQuery, xform2);
			}
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x0008F404 File Offset: 0x0008D604
		private bool SpaceQueryCallback([TupleElementNames(new string[]
		{
			"List",
			"Processed",
			"InvSpaceMatrix",
			"LookupOwner",
			"XformQuery",
			"GridBox"
		})] [Nullable(new byte[]
		{
			0,
			1,
			1,
			1,
			0,
			1
		})] ref ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2> state, in EntityUid uid)
		{
			if (state.Item2.Contains(uid))
			{
				return true;
			}
			TransformComponent xform = state.Item5.GetComponent(uid);
			if (xform.ParentUid == state.Item4)
			{
				if (state.Item6.Contains(state.Item3.Transform(xform.LocalPosition), true))
				{
					state.Item1.Add(xform);
				}
				return true;
			}
			if (state.Item6.Contains(state.Item3.Transform(this._transformSystem.GetWorldPosition(xform, state.Item5)), true))
			{
				state.Item1.Add(xform);
			}
			return true;
		}

		// Token: 0x06001AE6 RID: 6886 RVA: 0x0008F4B0 File Offset: 0x0008D6B0
		private bool SpaceQueryCallback([TupleElementNames(new string[]
		{
			"List",
			"Processed",
			"InvSpaceMatrix",
			"LookupOwner",
			"XformQuery",
			"GridBox"
		})] [Nullable(new byte[]
		{
			0,
			1,
			1,
			1,
			0,
			1
		})] ref ValueTuple<List<TransformComponent>, HashSet<EntityUid>, Matrix3, EntityUid, EntityQuery<TransformComponent>, Box2> state, in FixtureProxy proxy)
		{
			EntityUid owner = proxy.Fixture.Body.Owner;
			return this.SpaceQueryCallback(ref state, owner);
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x0008F4D8 File Offset: 0x0008D6D8
		[NullableContext(2)]
		private void ProcessEntity(EntityUid uid, MapCoordinates epicenter, DamageSpecifier damage, float throwForce, [Nullable(1)] string id, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<DamageableComponent> damageQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> physicsQuery, TransformComponent xform = null)
		{
			DamageableComponent damageable;
			if (damage != null && damageQuery.TryGetComponent(uid, ref damageable))
			{
				GetExplosionResistanceEvent ev = new GetExplosionResistanceEvent(id);
				base.RaiseLocalEvent<GetExplosionResistanceEvent>(uid, ev, false);
				ev.DamageCoefficient = Math.Max(0f, ev.DamageCoefficient);
				if (ev.DamageCoefficient == 1f)
				{
					this._damageableSystem.TryChangeDamage(new EntityUid?(uid), damage, true, true, damageable, null);
					if (base.HasComp<MindComponent>(uid) || base.HasComp<ExplosiveComponent>(uid))
					{
						string damageStr = string.Join(", ", damage.DamageDict.Select(delegate(KeyValuePair<string, FixedPoint2> entry)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
							defaultInterpolatedStringHandler.AppendFormatted(entry.Key);
							defaultInterpolatedStringHandler.AppendLiteral(": ");
							defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(entry.Value);
							return defaultInterpolatedStringHandler.ToStringAndClear();
						}));
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.Explosion;
						LogImpact impact = LogImpact.Medium;
						LogStringHandler logStringHandler = new LogStringHandler(27, 3);
						logStringHandler.AppendLiteral("Explosion caused [");
						logStringHandler.AppendFormatted(damageStr);
						logStringHandler.AppendLiteral("] to ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
						logStringHandler.AppendLiteral(" at ");
						logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(uid).Coordinates, "Transform(uid).Coordinates");
						adminLogger.Add(type, impact, ref logStringHandler);
					}
				}
				else
				{
					DamageSpecifier appliedDamage = damage * ev.DamageCoefficient;
					this._damageableSystem.TryChangeDamage(new EntityUid?(uid), appliedDamage, true, true, damageable, null);
					if (base.HasComp<MindComponent>(uid) || base.HasComp<ExplosiveComponent>(uid))
					{
						string damageStr2 = string.Join(", ", appliedDamage.DamageDict.Select(delegate(KeyValuePair<string, FixedPoint2> entry)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
							defaultInterpolatedStringHandler.AppendFormatted(entry.Key);
							defaultInterpolatedStringHandler.AppendLiteral(": ");
							defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(entry.Value);
							return defaultInterpolatedStringHandler.ToStringAndClear();
						}));
						ISharedAdminLogManager adminLogger2 = this._adminLogger;
						LogType type2 = LogType.Explosion;
						LogImpact impact2 = LogImpact.Medium;
						LogStringHandler logStringHandler = new LogStringHandler(27, 3);
						logStringHandler.AppendLiteral("Explosion caused [");
						logStringHandler.AppendFormatted(damageStr2);
						logStringHandler.AppendLiteral("] to ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
						logStringHandler.AppendLiteral(" at ");
						logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(uid).Coordinates, "Transform(uid).Coordinates");
						adminLogger2.Add(type2, impact2, ref logStringHandler);
					}
				}
			}
			PhysicsComponent physics;
			if (xform != null && !xform.Anchored && throwForce > 0f && !this.EntityManager.IsQueuedForDeletion(uid) && physicsQuery.TryGetComponent(uid, ref physics) && physics.BodyType == 8)
			{
				this._throwingSystem.TryThrow(uid, xform.WorldPosition - epicenter.Position, throwForce, null, 5f, null, null, null, null);
			}
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x0008F788 File Offset: 0x0008D988
		public void DamageFloorTile(TileRef tileRef, float effectiveIntensity, int maxTileBreak, bool canCreateVacuum, [TupleElementNames(new string[]
		{
			"GridIndices",
			"Tile"
		})] [Nullable(new byte[]
		{
			1,
			0
		})] List<ValueTuple<Vector2i, Tile>> damagedTiles, ExplosionPrototype type)
		{
			ContentTileDefinition tileDef = this._tileDefinitionManager[(int)tileRef.Tile.TypeId] as ContentTileDefinition;
			if (tileDef == null)
			{
				return;
			}
			if (tileDef.IsSpace)
			{
				canCreateVacuum = true;
			}
			int tileBreakages = 0;
			while (maxTileBreak > tileBreakages && RandomExtensions.Prob(this._robustRandom, type.TileBreakChance(effectiveIntensity)))
			{
				tileBreakages++;
				effectiveIntensity -= type.TileBreakRerollReduction;
				if (tileDef.BaseTurfs.Count == 0)
				{
					break;
				}
				ITileDefinitionManager tileDefinitionManager = this._tileDefinitionManager;
				List<string> baseTurfs = tileDef.BaseTurfs;
				ContentTileDefinition newDef = tileDefinitionManager[baseTurfs[baseTurfs.Count - 1]] as ContentTileDefinition;
				if (newDef == null || (newDef.IsSpace && !canCreateVacuum))
				{
					break;
				}
				tileDef = newDef;
			}
			if (tileDef.TileId == tileRef.Tile.TypeId)
			{
				return;
			}
			damagedTiles.Add(new ValueTuple<Vector2i, Tile>(tileRef.GridIndices, new Tile(tileDef.TileId, 0, 0)));
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x0008F864 File Offset: 0x0008DA64
		[return: Nullable(new byte[]
		{
			0,
			1,
			2,
			1,
			1
		})]
		private ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3>? GetExplosionTiles(MapCoordinates epicenter, string typeID, float totalIntensity, float slope, float maxIntensity)
		{
			if (totalIntensity <= 0f || slope <= 0f)
			{
				return null;
			}
			int typeIndex;
			if (!this._explosionTypes.TryGetValue(typeID, out typeIndex))
			{
				Logger.Error("Attempted to spawn explosion using a prototype that was not defined during initialization. Explosion prototype hot-reload is not currently supported.");
				return null;
			}
			EntityUid? epicentreGrid = null;
			ValueTuple<List<EntityUid>, EntityUid?, float> localGrids2 = this.GetLocalGrids(epicenter, totalIntensity, slope, maxIntensity);
			List<EntityUid> localGrids = localGrids2.Item1;
			EntityUid? referenceGrid = localGrids2.Item2;
			float maxDistance = localGrids2.Item3;
			MapGridComponent candidateGrid;
			TileRef tileRef;
			Vector2i initialTile;
			if (this._mapManager.TryFindGridAt(epicenter, ref candidateGrid) && candidateGrid.TryGetTileRef(candidateGrid.WorldToTile(epicenter.Position), ref tileRef) && !tileRef.Tile.IsEmpty)
			{
				epicentreGrid = new EntityUid?(candidateGrid.Owner);
				initialTile = tileRef.GridIndices;
			}
			else if (referenceGrid != null)
			{
				initialTile = this._mapManager.GetGrid(referenceGrid.Value).WorldToTile(epicenter.Position);
			}
			else
			{
				initialTile..ctor((int)Math.Floor((double)(epicenter.Position.X / 1f)), (int)Math.Floor((double)(epicenter.Position.Y / 1f)));
			}
			Dictionary<EntityUid, ExplosionGridTileFlood> gridData = new Dictionary<EntityUid, ExplosionGridTileFlood>();
			ExplosionSpaceTileFlood spaceData = null;
			float stepSize = slope / 2f;
			HashSet<Vector2i> spaceJump = new HashSet<Vector2i>();
			HashSet<EntityUid> encounteredGrids = new HashSet<EntityUid>();
			Matrix3 spaceMatrix = Matrix3.Identity;
			Angle spaceAngle = Angle.Zero;
			if (referenceGrid != null)
			{
				TransformComponent transformComponent = base.Transform(this._mapManager.GetGrid(referenceGrid.Value).Owner);
				spaceMatrix = transformComponent.WorldMatrix;
				spaceAngle = transformComponent.WorldRotation;
			}
			if (epicentreGrid != null)
			{
				encounteredGrids.Add(epicentreGrid.Value);
				Dictionary<Vector2i, ExplosionSystem.TileData> airtightMap;
				if (!this._airtightMap.TryGetValue(epicentreGrid.Value, out airtightMap))
				{
					airtightMap = new Dictionary<Vector2i, ExplosionSystem.TileData>();
				}
				ExplosionGridTileFlood initialGridData = new ExplosionGridTileFlood(this._mapManager.GetGrid(epicentreGrid.Value), airtightMap, maxIntensity, stepSize, typeIndex, this._gridEdges[epicentreGrid.Value], referenceGrid, spaceMatrix, spaceAngle);
				gridData[epicentreGrid.Value] = initialGridData;
				initialGridData.InitTile(initialTile);
			}
			else
			{
				spaceData = new ExplosionSpaceTileFlood(this, epicenter, referenceGrid, localGrids, maxDistance);
				spaceData.InitTile(initialTile);
			}
			if (totalIntensity < stepSize)
			{
				return new ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3>?(new ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3>(1, new List<float>
				{
					totalIntensity
				}, spaceData, gridData, spaceMatrix));
			}
			List<int> tilesInIteration = new List<int>
			{
				1
			};
			List<float> iterationIntensity = new List<float>
			{
				stepSize
			};
			int totalTiles = 1;
			float remainingIntensity = totalIntensity - stepSize;
			int iteration = 1;
			int maxIntensityIndex = 0;
			bool intensityUnchangedLastLoop = false;
			while (remainingIntensity > 0f && iteration <= this.MaxIterations && totalTiles < this.MaxArea)
			{
				float previousIntensity = remainingIntensity;
				for (int i = maxIntensityIndex; i < iteration; i++)
				{
					float intensityIncrease = MathF.Min(stepSize, maxIntensity - iterationIntensity[i]);
					List<float> list;
					int index;
					if ((float)tilesInIteration[i] * intensityIncrease >= remainingIntensity)
					{
						list = iterationIntensity;
						index = i;
						list[index] += remainingIntensity / (float)tilesInIteration[i];
						remainingIntensity = 0f;
						break;
					}
					list = iterationIntensity;
					index = i;
					list[index] += intensityIncrease;
					remainingIntensity -= (float)tilesInIteration[i] * intensityIncrease;
					if (intensityIncrease < stepSize)
					{
						maxIntensityIndex++;
					}
				}
				if (remainingIntensity <= 0f)
				{
					break;
				}
				HashSet<Vector2i> previousSpaceJump = spaceJump;
				Dictionary<EntityUid, HashSet<Vector2i>> previousGridJump = (spaceData != null) ? spaceData.GridJump : null;
				spaceJump = new HashSet<Vector2i>();
				int newTileCount = 0;
				if (previousGridJump != null)
				{
					encounteredGrids.UnionWith(previousGridJump.Keys);
				}
				foreach (EntityUid grid in encounteredGrids)
				{
					ExplosionGridTileFlood data;
					if (!gridData.TryGetValue(grid, out data))
					{
						Dictionary<Vector2i, ExplosionSystem.TileData> airtightMap2;
						if (!this._airtightMap.TryGetValue(grid, out airtightMap2))
						{
							airtightMap2 = new Dictionary<Vector2i, ExplosionSystem.TileData>();
						}
						data = new ExplosionGridTileFlood(this._mapManager.GetGrid(grid), airtightMap2, maxIntensity, stepSize, typeIndex, this._gridEdges[grid], referenceGrid, spaceMatrix, spaceAngle);
						gridData[grid] = data;
					}
					newTileCount += data.AddNewTiles(iteration, (previousGridJump != null) ? previousGridJump.GetValueOrDefault(grid) : null);
					spaceJump.UnionWith(data.SpaceJump);
				}
				if (spaceData == null && previousSpaceJump.Count != 0)
				{
					spaceData = new ExplosionSpaceTileFlood(this, epicenter, referenceGrid, localGrids, maxDistance);
				}
				if (spaceData != null)
				{
					newTileCount += spaceData.AddNewTiles(iteration, previousSpaceJump);
				}
				tilesInIteration.Add(newTileCount);
				if ((float)newTileCount * stepSize >= remainingIntensity)
				{
					iterationIntensity.Add(remainingIntensity / (float)newTileCount);
					break;
				}
				remainingIntensity -= (float)newTileCount * stepSize;
				iterationIntensity.Add(stepSize);
				totalTiles += newTileCount;
				if (intensityUnchangedLastLoop && remainingIntensity == previousIntensity)
				{
					break;
				}
				intensityUnchangedLastLoop = (remainingIntensity == previousIntensity);
				iteration++;
			}
			foreach (ExplosionGridTileFlood explosionGridTileFlood in gridData.Values)
			{
				explosionGridTileFlood.CleanUp();
			}
			if (spaceData != null)
			{
				spaceData.CleanUp();
			}
			return new ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3>?(new ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3>(totalTiles, iterationIntensity, spaceData, gridData, spaceMatrix));
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x0008FDA0 File Offset: 0x0008DFA0
		[return: Nullable(new byte[]
		{
			0,
			1
		})]
		public ValueTuple<List<EntityUid>, EntityUid?, float> GetLocalGrids(MapCoordinates epicenter, float totalIntensity, float slope, float maxIntensity)
		{
			float radius = 0.5f + this.IntensityToRadius(totalIntensity, slope, maxIntensity);
			radius = Math.Min(radius, (float)(this.MaxIterations / 4));
			EntityUid? referenceGrid = null;
			float mass = 0f;
			Box2 box = Box2.CenteredAround(epicenter.Position, new ValueTuple<float, float>(radius, radius));
			foreach (MapGridComponent grid in this._mapManager.FindGridsIntersecting(epicenter.MapId, box, false))
			{
				PhysicsComponent physics;
				if (base.TryComp<PhysicsComponent>(grid.Owner, ref physics) && physics.Mass > mass)
				{
					mass = physics.Mass;
					referenceGrid = new EntityUid?(grid.Owner);
				}
			}
			radius *= 4f;
			box = Box2.CenteredAround(epicenter.Position, new ValueTuple<float, float>(radius, radius));
			List<MapGridComponent> mapGrids = this._mapManager.FindGridsIntersecting(epicenter.MapId, box, false).ToList<MapGridComponent>();
			List<EntityUid> grids = (from x in mapGrids
			select x.Owner).ToList<EntityUid>();
			if (referenceGrid != null)
			{
				return new ValueTuple<List<EntityUid>, EntityUid?, float>(grids, referenceGrid, radius);
			}
			foreach (MapGridComponent grid2 in mapGrids)
			{
				PhysicsComponent physics2;
				if (base.TryComp<PhysicsComponent>(grid2.Owner, ref physics2) && physics2.Mass > mass)
				{
					mass = physics2.Mass;
					referenceGrid = new EntityUid?(grid2.Owner);
				}
			}
			return new ValueTuple<List<EntityUid>, EntityUid?, float>(grids, referenceGrid, radius);
		}

		// Token: 0x06001AEB RID: 6891 RVA: 0x0008FF5C File Offset: 0x0008E15C
		[return: Nullable(2)]
		public ExplosionVisualsState GenerateExplosionPreview(SpawnExplosionEuiMsg.PreviewRequest request)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3>? results = this.GetExplosionTiles(request.Epicenter, request.TypeId, request.TotalIntensity, request.IntensitySlope, request.MaxIntensity);
			if (results == null)
			{
				return null;
			}
			ValueTuple<int, List<float>, ExplosionSpaceTileFlood, Dictionary<EntityUid, ExplosionGridTileFlood>, Matrix3> value = results.Value;
			int area = value.Item1;
			List<float> iterationIntensity = value.Item2;
			ExplosionSpaceTileFlood spaceData = value.Item3;
			Dictionary<EntityUid, ExplosionGridTileFlood> gridData = value.Item4;
			Matrix3 spaceMatrix = value.Item5;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Generated explosion preview with ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(area);
			defaultInterpolatedStringHandler.AppendLiteral(" tiles in ");
			defaultInterpolatedStringHandler.AppendFormatted<double>(stopwatch.Elapsed.TotalMilliseconds);
			defaultInterpolatedStringHandler.AppendLiteral("ms");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> tileLists = new Dictionary<EntityUid, Dictionary<int, List<Vector2i>>>();
			foreach (KeyValuePair<EntityUid, ExplosionGridTileFlood> keyValuePair in gridData)
			{
				EntityUid entityUid;
				ExplosionGridTileFlood explosionGridTileFlood;
				keyValuePair.Deconstruct(out entityUid, out explosionGridTileFlood);
				EntityUid grid = entityUid;
				ExplosionGridTileFlood data = explosionGridTileFlood;
				tileLists.Add(grid, data.TileLists);
			}
			return new ExplosionVisualsState(request.Epicenter, request.TypeId, iterationIntensity, (spaceData != null) ? spaceData.TileLists : null, tileLists, spaceMatrix, (spaceData != null) ? spaceData.TileSize : 1);
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x000900C4 File Offset: 0x0008E2C4
		public void InitVisuals()
		{
			base.SubscribeLocalEvent<ExplosionVisualsComponent, ComponentGetState>(new ComponentEventRefHandler<ExplosionVisualsComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x06001AED RID: 6893 RVA: 0x000900DA File Offset: 0x0008E2DA
		private void OnGetState(EntityUid uid, ExplosionVisualsComponent component, ref ComponentGetState args)
		{
			args.State = new ExplosionVisualsState(component.Epicenter, component.ExplosionType, component.Intensity, component.SpaceTiles, component.Tiles, component.SpaceMatrix, component.SpaceTileSize);
		}

		// Token: 0x06001AEE RID: 6894 RVA: 0x00090114 File Offset: 0x0008E314
		private EntityUid CreateExplosionVisualEntity(MapCoordinates epicenter, string prototype, Matrix3 spaceMatrix, [Nullable(2)] ExplosionSpaceTileFlood spaceData, IEnumerable<ExplosionGridTileFlood> gridData, List<float> iterationIntensity)
		{
			EntityUid explosionEntity = base.Spawn(null, MapCoordinates.Nullspace);
			ExplosionVisualsComponent comp = base.AddComp<ExplosionVisualsComponent>(explosionEntity);
			foreach (ExplosionGridTileFlood grid in gridData)
			{
				comp.Tiles.Add(grid.Grid.Owner, grid.TileLists);
			}
			comp.SpaceTiles = ((spaceData != null) ? spaceData.TileLists : null);
			comp.Epicenter = epicenter;
			comp.ExplosionType = prototype;
			comp.Intensity = iterationIntensity;
			comp.SpaceMatrix = spaceMatrix;
			comp.SpaceTileSize = ((spaceData != null) ? spaceData.TileSize : 1);
			base.Dirty(comp, null);
			this._pvsSys.AddGlobalOverride(explosionEntity);
			ServerAppearanceComponent appearance = base.AddComp<ServerAppearanceComponent>(explosionEntity);
			this._appearance.SetData(explosionEntity, ExplosionAppearanceData.Progress, 1, appearance);
			return explosionEntity;
		}

		// Token: 0x040010FD RID: 4349
		[Dependency]
		private readonly DestructibleSystem _destructibleSystem;

		// Token: 0x040010FE RID: 4350
		private readonly Dictionary<string, int> _explosionTypes = new Dictionary<string, int>();

		// Token: 0x040010FF RID: 4351
		private Dictionary<EntityUid, Dictionary<Vector2i, ExplosionSystem.TileData>> _airtightMap = new Dictionary<EntityUid, Dictionary<Vector2i, ExplosionSystem.TileData>>();

		// Token: 0x04001100 RID: 4352
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001101 RID: 4353
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04001102 RID: 4354
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x04001103 RID: 4355
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001104 RID: 4356
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04001105 RID: 4357
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001106 RID: 4358
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04001107 RID: 4359
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04001108 RID: 4360
		[Dependency]
		private readonly NodeGroupSystem _nodeGroupSystem;

		// Token: 0x04001109 RID: 4361
		[Dependency]
		private readonly PathfindingSystem _pathfindingSystem;

		// Token: 0x0400110A RID: 4362
		[Dependency]
		private readonly SharedCameraRecoilSystem _recoilSystem;

		// Token: 0x0400110B RID: 4363
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400110C RID: 4364
		[Dependency]
		private readonly ThrowingSystem _throwingSystem;

		// Token: 0x0400110D RID: 4365
		[Dependency]
		private readonly PVSOverrideSystem _pvsSys;

		// Token: 0x0400110E RID: 4366
		[Dependency]
		private readonly SharedTransformSystem _transformSystem;

		// Token: 0x0400110F RID: 4367
		[Dependency]
		protected readonly IMapManager MapManager;

		// Token: 0x04001110 RID: 4368
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04001111 RID: 4369
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04001112 RID: 4370
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04001113 RID: 4371
		public const ushort DefaultTileSize = 1;

		// Token: 0x04001114 RID: 4372
		private readonly SoundCollectionSpecifier _meteorsHit = new SoundCollectionSpecifier("ShuttleImpactSound", null);

		// Token: 0x04001115 RID: 4373
		private AudioParams _audioParams = AudioParams.Default.WithVolume(-3f);

		// Token: 0x04001116 RID: 4374
		public const string DefaultExplosionPrototypeId = "Default";

		// Token: 0x0400111F RID: 4383
		private Dictionary<EntityUid, Dictionary<Vector2i, ExplosionSystem.NeighborFlag>> _gridEdges = new Dictionary<EntityUid, Dictionary<Vector2i, ExplosionSystem.NeighborFlag>>();

		// Token: 0x04001120 RID: 4384
		public static readonly Vector2i[] NeighbourVectors = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 1),
			new Vector2i(1, 0),
			new Vector2i(1, -1),
			new Vector2i(0, -1),
			new Vector2i(-1, -1),
			new Vector2i(-1, 0),
			new Vector2i(-1, 1)
		};

		// Token: 0x04001121 RID: 4385
		internal readonly Stopwatch Stopwatch = new Stopwatch();

		// Token: 0x04001122 RID: 4386
		internal static int TileCheckIteration = 1;

		// Token: 0x04001123 RID: 4387
		[Nullable(new byte[]
		{
			1,
			1,
			2
		})]
		private Queue<Func<Explosion>> _explosionQueue = new Queue<Func<Explosion>>();

		// Token: 0x04001124 RID: 4388
		[Nullable(2)]
		private Explosion _activeExplosion;

		// Token: 0x04001125 RID: 4389
		private int _previousTileIteration;

		// Token: 0x02000A03 RID: 2563
		[Nullable(0)]
		public struct TileData
		{
			// Token: 0x06003418 RID: 13336 RVA: 0x00109E31 File Offset: 0x00108031
			public TileData(float[] explosionTolerance, AtmosDirection blockedDirections)
			{
				this.BlockedDirections = AtmosDirection.Invalid;
				this.ExplosionTolerance = explosionTolerance;
				this.BlockedDirections = blockedDirections;
			}

			// Token: 0x040022FD RID: 8957
			public float[] ExplosionTolerance;

			// Token: 0x040022FE RID: 8958
			public AtmosDirection BlockedDirections;
		}

		// Token: 0x02000A04 RID: 2564
		[NullableContext(0)]
		[Flags]
		public enum NeighborFlag : byte
		{
			// Token: 0x04002300 RID: 8960
			Invalid = 0,
			// Token: 0x04002301 RID: 8961
			North = 1,
			// Token: 0x04002302 RID: 8962
			NorthEast = 2,
			// Token: 0x04002303 RID: 8963
			East = 4,
			// Token: 0x04002304 RID: 8964
			SouthEast = 8,
			// Token: 0x04002305 RID: 8965
			South = 16,
			// Token: 0x04002306 RID: 8966
			SouthWest = 32,
			// Token: 0x04002307 RID: 8967
			West = 64,
			// Token: 0x04002308 RID: 8968
			NorthWest = 128,
			// Token: 0x04002309 RID: 8969
			Cardinal = 85,
			// Token: 0x0400230A RID: 8970
			Diagonal = 170,
			// Token: 0x0400230B RID: 8971
			Any = 255
		}
	}
}
