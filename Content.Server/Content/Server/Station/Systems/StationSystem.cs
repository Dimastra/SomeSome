using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.Station.Components;
using Content.Shared.CCVar;
using Content.Shared.Station;
using Robust.Server.Player;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Random;

namespace Content.Server.Station.Systems
{
	// Token: 0x0200019B RID: 411
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StationSystem : EntitySystem
	{
		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000833 RID: 2099 RVA: 0x00029ABD File Offset: 0x00027CBD
		public IReadOnlySet<EntityUid> Stations
		{
			get
			{
				return this._stations;
			}
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x00029AC8 File Offset: 0x00027CC8
		public override void Initialize()
		{
			this._sawmill = this._logManager.GetSawmill("station");
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnRoundEnd), null, null);
			base.SubscribeLocalEvent<PreGameMapLoad>(new EntityEventHandler<PreGameMapLoad>(this.OnPreGameMapLoad), null, null);
			base.SubscribeLocalEvent<PostGameMapLoad>(new EntityEventHandler<PostGameMapLoad>(this.OnPostGameMapLoad), null, null);
			base.SubscribeLocalEvent<StationDataComponent, ComponentAdd>(new ComponentEventHandler<StationDataComponent, ComponentAdd>(this.OnStationAdd), null, null);
			base.SubscribeLocalEvent<StationDataComponent, ComponentShutdown>(new ComponentEventHandler<StationDataComponent, ComponentShutdown>(this.OnStationDeleted), null, null);
			base.SubscribeLocalEvent<StationDataComponent, EntParentChangedMessage>(new ComponentEventRefHandler<StationDataComponent, EntParentChangedMessage>(this.OnParentChanged), null, null);
			base.SubscribeLocalEvent<StationMemberComponent, ComponentShutdown>(new ComponentEventHandler<StationMemberComponent, ComponentShutdown>(this.OnStationGridDeleted), null, null);
			base.SubscribeLocalEvent<StationMemberComponent, PostGridSplitEvent>(new ComponentEventRefHandler<StationMemberComponent, PostGridSplitEvent>(this.OnStationSplitEvent), null, null);
			this._configurationManager.OnValueChanged<bool>(CCVars.StationOffset, delegate(bool x)
			{
				this._randomStationOffset = x;
			}, true);
			this._configurationManager.OnValueChanged<float>(CCVars.MaxStationOffset, delegate(float x)
			{
				this._maxRandomStationOffset = x;
			}, true);
			this._configurationManager.OnValueChanged<bool>(CCVars.StationRotation, delegate(bool x)
			{
				this._randomStationRotation = x;
			}, true);
			this._player.PlayerStatusChanged += this.OnPlayerStatusChanged;
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x00029BF9 File Offset: 0x00027DF9
		private void OnStationSplitEvent(EntityUid uid, StationMemberComponent component, ref PostGridSplitEvent args)
		{
			this.AddGridToStation(component.Station, args.Grid, null, null, null);
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x00029C10 File Offset: 0x00027E10
		private void OnStationGridDeleted(EntityUid uid, StationMemberComponent component, ComponentShutdown args)
		{
			StationDataComponent stationData;
			if (!base.TryComp<StationDataComponent>(component.Station, ref stationData))
			{
				return;
			}
			stationData.Grids.Remove(uid);
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x00029C3B File Offset: 0x00027E3B
		public override void Shutdown()
		{
			base.Shutdown();
			this._player.PlayerStatusChanged -= this.OnPlayerStatusChanged;
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x00029C5A File Offset: 0x00027E5A
		public void OnServerDispose()
		{
			this._stations.Clear();
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x00029C67 File Offset: 0x00027E67
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 2)
			{
				base.RaiseNetworkEvent(new StationsUpdatedEvent(this._stations), e.Session);
			}
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x00029C89 File Offset: 0x00027E89
		private void OnStationAdd(EntityUid uid, StationDataComponent component, ComponentAdd args)
		{
			this._stations.Add(uid);
			base.RaiseNetworkEvent(new StationsUpdatedEvent(this._stations), Filter.Broadcast(), true);
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x00029CB0 File Offset: 0x00027EB0
		private void OnStationDeleted(EntityUid uid, StationDataComponent component, ComponentShutdown args)
		{
			if (this._stations.Contains(uid) && this._gameTicker.RunLevel == GameRunLevel.InRound && this._gameTicker.LobbyEnabled)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Station entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" is getting deleted mid-round. Trace: ");
				defaultInterpolatedStringHandler.AppendFormatted(Environment.StackTrace);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			foreach (EntityUid grid in component.Grids)
			{
				base.RemComp<StationMemberComponent>(grid);
			}
			this._stations.Remove(uid);
			base.RaiseNetworkEvent(new StationsUpdatedEvent(this._stations), Filter.Broadcast(), true);
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x00029D9C File Offset: 0x00027F9C
		private void OnParentChanged(EntityUid uid, StationDataComponent component, ref EntParentChangedMessage args)
		{
			if (this._gameTicker.RunLevel != GameRunLevel.InRound || base.MetaData(uid).EntityLifeStage >= 3 || component.LifeStage <= 3)
			{
				return;
			}
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Station entity ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
			defaultInterpolatedStringHandler.AppendLiteral(" is getting reparented from ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.OldParent ?? EntityUid.Invalid));
			defaultInterpolatedStringHandler.AppendLiteral(" to ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Transform.ParentUid));
			sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x00029E64 File Offset: 0x00028064
		private void OnPreGameMapLoad(PreGameMapLoad ev)
		{
			if (this._gameTicker.RunLevel == GameRunLevel.InRound)
			{
				return;
			}
			if (this._randomStationOffset)
			{
				ev.Options.Offset += this._random.NextVector2(this._maxRandomStationOffset);
			}
			if (this._randomStationRotation)
			{
				ev.Options.Rotation = this._random.NextAngle();
			}
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x00029ED0 File Offset: 0x000280D0
		private void OnPostGameMapLoad(PostGameMapLoad ev)
		{
			StationSystem.<>c__DisplayClass25_0 CS$<>8__locals1;
			CS$<>8__locals1.dict = new Dictionary<string, List<EntityUid>>();
			foreach (EntityUid grid in ev.Grids)
			{
				BecomesStationComponent becomesStation;
				if (base.TryComp<BecomesStationComponent>(grid, ref becomesStation))
				{
					StationSystem.<OnPostGameMapLoad>g__AddGrid|25_0(becomesStation.Id, grid, ref CS$<>8__locals1);
				}
			}
			if (!CS$<>8__locals1.dict.Any<KeyValuePair<string, List<EntityUid>>>())
			{
				this._sawmill.Error("There were no station grids for " + ev.GameMap.ID + "!");
			}
			foreach (EntityUid grid2 in ev.Grids)
			{
				PartOfStationComponent partOfStation;
				if (base.TryComp<PartOfStationComponent>(grid2, ref partOfStation))
				{
					StationSystem.<OnPostGameMapLoad>g__AddGrid|25_0(partOfStation.Id, grid2, ref CS$<>8__locals1);
				}
			}
			foreach (KeyValuePair<string, List<EntityUid>> keyValuePair in CS$<>8__locals1.dict)
			{
				string text;
				List<EntityUid> list;
				keyValuePair.Deconstruct(out text, out list);
				string id = text;
				List<EntityUid> gridIds = list;
				StationConfig stationConfig = null;
				if (ev.GameMap.Stations.ContainsKey(id))
				{
					stationConfig = ev.GameMap.Stations[id];
				}
				else
				{
					ISawmill sawmill = this._sawmill;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(64, 2);
					defaultInterpolatedStringHandler.AppendLiteral("The station ");
					defaultInterpolatedStringHandler.AppendFormatted(id);
					defaultInterpolatedStringHandler.AppendLiteral(" in map ");
					defaultInterpolatedStringHandler.AppendFormatted(ev.GameMap.ID);
					defaultInterpolatedStringHandler.AppendLiteral(" does not have an associated station config!");
					sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				this.InitializeNewStation(stationConfig, gridIds, ev.StationName);
			}
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0002A0B0 File Offset: 0x000282B0
		private void OnRoundEnd(GameRunLevelChangedEvent eventArgs)
		{
			if (eventArgs.New != GameRunLevel.PreRoundLobby)
			{
				return;
			}
			foreach (EntityUid entity in this._stations)
			{
				this.DeleteStation(entity, null);
			}
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0002A110 File Offset: 0x00028310
		public EntityUid? GetLargestGrid(StationDataComponent component)
		{
			EntityUid? largestGrid = null;
			Box2 largestBounds = default(Box2);
			foreach (EntityUid gridUid in component.Grids)
			{
				MapGridComponent grid;
				if (base.TryComp<MapGridComponent>(gridUid, ref grid) && grid.LocalAABB.Size.LengthSquared >= largestBounds.Size.LengthSquared)
				{
					largestBounds = grid.LocalAABB;
					largestGrid = new EntityUid?(gridUid);
				}
			}
			return largestGrid;
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0002A1B4 File Offset: 0x000283B4
		public Filter GetInOwningStation(EntityUid source, float range = 32f)
		{
			EntityUid? station = this.GetOwningStation(source, null);
			StationDataComponent data;
			if (base.TryComp<StationDataComponent>(station, ref data))
			{
				return this.GetInStation(data, 32f);
			}
			return Filter.Empty();
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x0002A1E8 File Offset: 0x000283E8
		public Filter GetInStation(StationDataComponent dataComponent, float range = 32f)
		{
			ValueList<Box2> bounds = new ValueList<Box2>(dataComponent.Grids.Count);
			Filter filter = Filter.Empty();
			ValueList<MapId> mapIds = default(ValueList<MapId>);
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			foreach (EntityUid gridUid in dataComponent.Grids)
			{
				MapGridComponent grid;
				TransformComponent xform;
				if (this._mapManager.TryGetGrid(new EntityUid?(gridUid), ref grid) && xformQuery.TryGetComponent(gridUid, ref xform))
				{
					MapId mapId = xform.MapID;
					Vector2 position = this._transform.GetWorldPosition(xform, xformQuery);
					Box2 bound = grid.LocalAABB.Enlarged(range).Translated(position);
					bounds.Add(bound);
					if (!mapIds.Contains(mapId))
					{
						mapIds.Add(xform.MapID);
					}
				}
			}
			foreach (ICommonSession session in Filter.GetAllPlayers(this._player))
			{
				EntityUid? entity = session.AttachedEntity;
				TransformComponent xform2;
				if (entity != null && xformQuery.TryGetComponent(entity, ref xform2))
				{
					MapId mapId2 = xform2.MapID;
					if (mapIds.Contains(mapId2))
					{
						Vector2 position2 = this._transform.GetWorldPosition(xform2, xformQuery);
						foreach (Box2 bound2 in bounds)
						{
							if (bound2.Contains(position2, true))
							{
								filter.AddPlayer(session);
								break;
							}
						}
					}
				}
			}
			return filter;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x0002A3BC File Offset: 0x000285BC
		public static string GenerateStationName(StationConfig config)
		{
			if (config.NameGenerator == null)
			{
				return config.StationNameTemplate;
			}
			return config.NameGenerator.FormatName(config.StationNameTemplate);
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x0002A3E0 File Offset: 0x000285E0
		[NullableContext(2)]
		public EntityUid InitializeNewStation(StationConfig stationConfig, IEnumerable<EntityUid> gridIds, string name = null)
		{
			EntityUid station = base.Spawn(null, new MapCoordinates(0f, 0f, this._gameTicker.DefaultMap));
			this._transform.DetachParentToNull(station, base.Transform(station));
			StationDataComponent data = base.AddComp<StationDataComponent>(station);
			MetaDataComponent metaData = base.MetaData(station);
			data.StationConfig = stationConfig;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (stationConfig != null && name == null)
			{
				name = StationSystem.GenerateStationName(stationConfig);
			}
			else if (name == null)
			{
				ISawmill sawmill = this._sawmill;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(97, 1);
				defaultInterpolatedStringHandler.AppendLiteral("When setting up station ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(station);
				defaultInterpolatedStringHandler.AppendLiteral(", was unable to find a valid name in the config and no name was provided.");
				sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				name = "unnamed station";
			}
			metaData.EntityName = name;
			base.RaiseLocalEvent<StationInitializedEvent>(new StationInitializedEvent(station));
			ISawmill sawmill2 = this._sawmill;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Set up station ");
			defaultInterpolatedStringHandler.AppendFormatted(metaData.EntityName);
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(station);
			defaultInterpolatedStringHandler.AppendLiteral(").");
			sawmill2.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			foreach (EntityUid grid in (gridIds ?? Array.Empty<EntityUid>()))
			{
				this.AddGridToStation(station, grid, null, data, name);
			}
			return station;
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x0002A548 File Offset: 0x00028748
		[NullableContext(2)]
		public void AddGridToStation(EntityUid station, EntityUid mapGrid, MapGridComponent gridComponent = null, StationDataComponent stationData = null, string name = null)
		{
			if (!base.Resolve<MapGridComponent>(mapGrid, ref gridComponent, true))
			{
				throw new ArgumentException("Tried to initialize a station on a non-grid entity!", "mapGrid");
			}
			if (!base.Resolve<StationDataComponent>(station, ref stationData, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			if (!string.IsNullOrEmpty(name))
			{
				base.MetaData(mapGrid).EntityName = name;
			}
			base.AddComp<StationMemberComponent>(mapGrid).Station = station;
			stationData.Grids.Add(gridComponent.Owner);
			base.RaiseLocalEvent<StationGridAddedEvent>(station, new StationGridAddedEvent(gridComponent.Owner, false), true);
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 4);
			defaultInterpolatedStringHandler.AppendLiteral("Adding grid ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(mapGrid);
			defaultInterpolatedStringHandler.AppendLiteral(":");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(gridComponent.Owner);
			defaultInterpolatedStringHandler.AppendLiteral(" to station ");
			defaultInterpolatedStringHandler.AppendFormatted(base.Name(station, null));
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(station);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x0002A65C File Offset: 0x0002885C
		[NullableContext(2)]
		public void RemoveGridFromStation(EntityUid station, EntityUid mapGrid, MapGridComponent gridComponent = null, StationDataComponent stationData = null)
		{
			if (!base.Resolve<MapGridComponent>(mapGrid, ref gridComponent, true))
			{
				throw new ArgumentException("Tried to initialize a station on a non-grid entity!", "mapGrid");
			}
			if (!base.Resolve<StationDataComponent>(station, ref stationData, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			base.RemComp<StationMemberComponent>(mapGrid);
			stationData.Grids.Remove(gridComponent.Owner);
			base.RaiseLocalEvent<StationGridRemovedEvent>(station, new StationGridRemovedEvent(gridComponent.Owner), true);
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 4);
			defaultInterpolatedStringHandler.AppendLiteral("Removing grid ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(mapGrid);
			defaultInterpolatedStringHandler.AppendLiteral(":");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(gridComponent.Owner);
			defaultInterpolatedStringHandler.AppendLiteral(" from station ");
			defaultInterpolatedStringHandler.AppendFormatted(base.Name(station, null));
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(station);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0002A754 File Offset: 0x00028954
		[NullableContext(2)]
		public void RenameStation(EntityUid station, [Nullable(1)] string name, bool loud = true, StationDataComponent stationData = null, MetaDataComponent metaData = null)
		{
			if (!base.Resolve<StationDataComponent, MetaDataComponent>(station, ref stationData, ref metaData, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			string oldName = metaData.EntityName;
			metaData.EntityName = name;
			if (loud)
			{
				ChatSystem chatSystem = this._chatSystem;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
				defaultInterpolatedStringHandler.AppendLiteral("The station ");
				defaultInterpolatedStringHandler.AppendFormatted(oldName);
				defaultInterpolatedStringHandler.AppendLiteral(" has been renamed to ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				chatSystem.DispatchStationAnnouncement(station, defaultInterpolatedStringHandler.ToStringAndClear(), "Central Command", true, null, null);
			}
			base.RaiseLocalEvent<StationRenamedEvent>(station, new StationRenamedEvent(oldName, name), true);
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0002A802 File Offset: 0x00028A02
		[NullableContext(2)]
		public void DeleteStation(EntityUid station, StationDataComponent stationData = null)
		{
			if (!base.Resolve<StationDataComponent>(station, ref stationData, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			this._stations.Remove(station);
			base.Del(station);
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x0002A834 File Offset: 0x00028A34
		[NullableContext(2)]
		public EntityUid? GetOwningStation(EntityUid entity, TransformComponent xform = null)
		{
			if (!base.Resolve<TransformComponent>(entity, ref xform, true))
			{
				throw new ArgumentException("Tried to use an abstract entity!", "entity");
			}
			StationDataComponent stationDataComponent;
			if (base.TryComp<StationDataComponent>(entity, ref stationDataComponent))
			{
				return new EntityUid?(entity);
			}
			MapGridComponent mapGridComponent;
			if (base.TryComp<MapGridComponent>(entity, ref mapGridComponent))
			{
				StationMemberComponent stationMemberComponent = base.CompOrNull<StationMemberComponent>(entity);
				if (stationMemberComponent == null)
				{
					return null;
				}
				return new EntityUid?(stationMemberComponent.Station);
			}
			else
			{
				EntityUid? gridUid = xform.GridUid;
				EntityUid invalid = EntityUid.Invalid;
				if (gridUid != null && (gridUid == null || gridUid.GetValueOrDefault() == invalid))
				{
					Logger.Debug("A");
					return null;
				}
				StationMemberComponent stationMemberComponent2 = base.CompOrNull<StationMemberComponent>(xform.GridUid);
				if (stationMemberComponent2 == null)
				{
					return null;
				}
				return new EntityUid?(stationMemberComponent2.Station);
			}
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x0002A934 File Offset: 0x00028B34
		[CompilerGenerated]
		internal static void <OnPostGameMapLoad>g__AddGrid|25_0(string station, EntityUid grid, ref StationSystem.<>c__DisplayClass25_0 A_2)
		{
			if (A_2.dict.ContainsKey(station))
			{
				A_2.dict[station].Add(grid);
				return;
			}
			A_2.dict[station] = new List<EntityUid>
			{
				grid
			};
		}

		// Token: 0x04000502 RID: 1282
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000503 RID: 1283
		[Dependency]
		private readonly ILogManager _logManager;

		// Token: 0x04000504 RID: 1284
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000505 RID: 1285
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x04000506 RID: 1286
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000507 RID: 1287
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x04000508 RID: 1288
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x04000509 RID: 1289
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x0400050A RID: 1290
		private ISawmill _sawmill;

		// Token: 0x0400050B RID: 1291
		private readonly HashSet<EntityUid> _stations = new HashSet<EntityUid>();

		// Token: 0x0400050C RID: 1292
		private bool _randomStationOffset;

		// Token: 0x0400050D RID: 1293
		private bool _randomStationRotation;

		// Token: 0x0400050E RID: 1294
		private float _maxRandomStationOffset;
	}
}
