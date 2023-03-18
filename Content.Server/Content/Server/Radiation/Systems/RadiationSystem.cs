using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Radiation.Components;
using Content.Server.Radiation.Events;
using Content.Shared.CCVar;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Radiation.Components;
using Content.Shared.Radiation.Events;
using Content.Shared.Radiation.Systems;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Players;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Radiation.Systems
{
	// Token: 0x02000264 RID: 612
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadiationSystem : EntitySystem
	{
		// Token: 0x06000C16 RID: 3094 RVA: 0x0003FB84 File Offset: 0x0003DD84
		private void InitRadBlocking()
		{
			base.SubscribeLocalEvent<RadiationBlockerComponent, ComponentInit>(new ComponentEventHandler<RadiationBlockerComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<RadiationBlockerComponent, ComponentShutdown>(new ComponentEventHandler<RadiationBlockerComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<RadiationBlockerComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<RadiationBlockerComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<RadiationBlockerComponent, ReAnchorEvent>(new ComponentEventRefHandler<RadiationBlockerComponent, ReAnchorEvent>(this.OnReAnchor), null, null);
			base.SubscribeLocalEvent<RadiationBlockerComponent, DoorStateChangedEvent>(new ComponentEventHandler<RadiationBlockerComponent, DoorStateChangedEvent>(this.OnDoorChanged), null, null);
			base.SubscribeLocalEvent<RadiationGridResistanceComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<RadiationGridResistanceComponent, EntityTerminatingEvent>(this.OnGridRemoved), null, null);
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0003FC09 File Offset: 0x0003DE09
		private void OnInit(EntityUid uid, RadiationBlockerComponent component, ComponentInit args)
		{
			if (!component.Enabled)
			{
				return;
			}
			this.AddTile(uid, component);
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0003FC1C File Offset: 0x0003DE1C
		private void OnShutdown(EntityUid uid, RadiationBlockerComponent component, ComponentShutdown args)
		{
			if (component.Enabled)
			{
				return;
			}
			this.RemoveTile(uid, component);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0003FC2F File Offset: 0x0003DE2F
		private void OnAnchorChanged(EntityUid uid, RadiationBlockerComponent component, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				this.AddTile(uid, component);
				return;
			}
			this.RemoveTile(uid, component);
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0003FC4A File Offset: 0x0003DE4A
		private void OnReAnchor(EntityUid uid, RadiationBlockerComponent component, ref ReAnchorEvent args)
		{
			this.RemoveTile(uid, component);
			this.AddTile(uid, component);
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x0003FC5C File Offset: 0x0003DE5C
		private void OnDoorChanged(EntityUid uid, RadiationBlockerComponent component, DoorStateChangedEvent args)
		{
			DoorState state = args.State;
			if (state != DoorState.Closed)
			{
				if (state == DoorState.Open)
				{
					this.SetEnabled(uid, false, component);
					return;
				}
			}
			else
			{
				this.SetEnabled(uid, true, component);
			}
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x0003FC8A File Offset: 0x0003DE8A
		private void OnGridRemoved(EntityUid uid, RadiationGridResistanceComponent component, ref EntityTerminatingEvent args)
		{
			base.RemComp(uid, component);
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0003FC94 File Offset: 0x0003DE94
		[NullableContext(2)]
		public void SetEnabled(EntityUid uid, bool isEnabled, RadiationBlockerComponent component = null)
		{
			if (!base.Resolve<RadiationBlockerComponent>(uid, ref component, true))
			{
				return;
			}
			if (isEnabled == component.Enabled)
			{
				return;
			}
			component.Enabled = isEnabled;
			if (!component.Enabled)
			{
				this.RemoveTile(uid, component);
				return;
			}
			this.AddTile(uid, component);
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x0003FCD0 File Offset: 0x0003DED0
		private void AddTile(EntityUid uid, RadiationBlockerComponent component)
		{
			if (component.CurrentPosition != null)
			{
				this.RemoveTile(uid, component);
			}
			if (!component.Enabled || component.RadResistance <= 0f)
			{
				return;
			}
			TransformComponent trs = base.Transform(uid);
			MapGridComponent grid;
			if (!trs.Anchored || !base.TryComp<MapGridComponent>(trs.GridUid, ref grid))
			{
				return;
			}
			EntityUid gridId = trs.GridUid.Value;
			Vector2i tilePos = grid.TileIndicesFor(trs.Coordinates);
			this.AddToTile(gridId, tilePos, component.RadResistance);
			component.CurrentPosition = new ValueTuple<EntityUid, Vector2i>?(new ValueTuple<EntityUid, Vector2i>(gridId, tilePos));
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x0003FD68 File Offset: 0x0003DF68
		private void RemoveTile(EntityUid uid, RadiationBlockerComponent component)
		{
			if (component.CurrentPosition == null)
			{
				return;
			}
			ValueTuple<EntityUid, Vector2i> value = component.CurrentPosition.Value;
			EntityUid gridId = value.Item1;
			Vector2i tilePos = value.Item2;
			this.RemoveFromTile(gridId, tilePos, component.RadResistance);
			component.CurrentPosition = null;
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x0003FDB8 File Offset: 0x0003DFB8
		private void AddToTile(EntityUid gridUid, Vector2i tilePos, float radResistance)
		{
			Dictionary<Vector2i, float> resistancePerTile = base.EnsureComp<RadiationGridResistanceComponent>(gridUid).ResistancePerTile;
			float newResistance = radResistance;
			float existingResistance;
			if (resistancePerTile.TryGetValue(tilePos, out existingResistance))
			{
				newResistance += existingResistance;
			}
			resistancePerTile[tilePos] = newResistance;
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0003FDEC File Offset: 0x0003DFEC
		private void RemoveFromTile(EntityUid gridUid, Vector2i tilePos, float radResistance)
		{
			RadiationGridResistanceComponent resistance;
			if (!base.TryComp<RadiationGridResistanceComponent>(gridUid, ref resistance))
			{
				return;
			}
			Dictionary<Vector2i, float> grid = resistance.ResistancePerTile;
			float existingResistance;
			if (!grid.TryGetValue(tilePos, out existingResistance))
			{
				return;
			}
			existingResistance -= radResistance;
			if (existingResistance > 0f)
			{
				grid[tilePos] = existingResistance;
				return;
			}
			grid.Remove(tilePos);
			if (grid.Count == 0)
			{
				base.RemComp(gridUid, resistance);
			}
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x0003FE45 File Offset: 0x0003E045
		public override void Initialize()
		{
			base.Initialize();
			this.SubscribeCvars();
			this.InitRadBlocking();
		}

		// Token: 0x06000C23 RID: 3107 RVA: 0x0003FE59 File Offset: 0x0003E059
		public override void Shutdown()
		{
			base.Shutdown();
			this.UnsubscribeCvars();
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x0003FE67 File Offset: 0x0003E067
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._accumulator += frameTime;
			if (this._accumulator < this.GridcastUpdateRate)
			{
				return;
			}
			this.UpdateGridcast();
			this.UpdateResistanceDebugOverlay();
			this._accumulator = 0f;
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x0003FEA4 File Offset: 0x0003E0A4
		public void IrradiateEntity(EntityUid uid, float radsPerSecond, float time)
		{
			OnIrradiatedEvent msg = new OnIrradiatedEvent(time, radsPerSecond);
			base.RaiseLocalEvent<OnIrradiatedEvent>(uid, msg, false);
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x0003FEC2 File Offset: 0x0003E0C2
		public void SetCanReceive(EntityUid uid, bool canReceive)
		{
			if (canReceive)
			{
				base.EnsureComp<RadiationReceiverComponent>(uid);
				return;
			}
			base.RemComp<RadiationReceiverComponent>(uid);
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000C27 RID: 3111 RVA: 0x0003FED8 File Offset: 0x0003E0D8
		// (set) Token: 0x06000C28 RID: 3112 RVA: 0x0003FEE0 File Offset: 0x0003E0E0
		public float MinIntensity { get; private set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000C29 RID: 3113 RVA: 0x0003FEE9 File Offset: 0x0003E0E9
		// (set) Token: 0x06000C2A RID: 3114 RVA: 0x0003FEF1 File Offset: 0x0003E0F1
		public float GridcastUpdateRate { get; private set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000C2B RID: 3115 RVA: 0x0003FEFA File Offset: 0x0003E0FA
		// (set) Token: 0x06000C2C RID: 3116 RVA: 0x0003FF02 File Offset: 0x0003E102
		public bool GridcastSimplifiedSameGrid { get; private set; }

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000C2D RID: 3117 RVA: 0x0003FF0B File Offset: 0x0003E10B
		// (set) Token: 0x06000C2E RID: 3118 RVA: 0x0003FF13 File Offset: 0x0003E113
		public float GridcastMaxDistance { get; private set; }

		// Token: 0x06000C2F RID: 3119 RVA: 0x0003FF1C File Offset: 0x0003E11C
		private void SubscribeCvars()
		{
			this._cfg.OnValueChanged<float>(CCVars.RadiationMinIntensity, new Action<float>(this.SetMinRadiationIntensity), true);
			this._cfg.OnValueChanged<float>(CCVars.RadiationGridcastUpdateRate, new Action<float>(this.SetGridcastUpdateRate), true);
			this._cfg.OnValueChanged<bool>(CCVars.RadiationGridcastSimplifiedSameGrid, new Action<bool>(this.SetGridcastSimplifiedSameGrid), true);
			this._cfg.OnValueChanged<float>(CCVars.RadiationGridcastMaxDistance, new Action<float>(this.SetGridcastMaxDistance), true);
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x0003FFA0 File Offset: 0x0003E1A0
		private void UnsubscribeCvars()
		{
			this._cfg.UnsubValueChanged<float>(CCVars.RadiationMinIntensity, new Action<float>(this.SetMinRadiationIntensity));
			this._cfg.UnsubValueChanged<float>(CCVars.RadiationGridcastUpdateRate, new Action<float>(this.SetGridcastUpdateRate));
			this._cfg.UnsubValueChanged<bool>(CCVars.RadiationGridcastSimplifiedSameGrid, new Action<bool>(this.SetGridcastSimplifiedSameGrid));
			this._cfg.UnsubValueChanged<float>(CCVars.RadiationGridcastMaxDistance, new Action<float>(this.SetGridcastMaxDistance));
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x0004001D File Offset: 0x0003E21D
		private void SetMinRadiationIntensity(float radiationMinIntensity)
		{
			this.MinIntensity = radiationMinIntensity;
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x00040026 File Offset: 0x0003E226
		private void SetGridcastUpdateRate(float updateRate)
		{
			this.GridcastUpdateRate = updateRate;
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x0004002F File Offset: 0x0003E22F
		private void SetGridcastSimplifiedSameGrid(bool simplifiedSameGrid)
		{
			this.GridcastSimplifiedSameGrid = simplifiedSameGrid;
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x00040038 File Offset: 0x0003E238
		private void SetGridcastMaxDistance(float maxDistance)
		{
			this.GridcastMaxDistance = maxDistance;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x00040044 File Offset: 0x0003E244
		public void ToggleDebugView(ICommonSession session)
		{
			bool isEnabled;
			if (this._debugSessions.Add(session))
			{
				isEnabled = true;
			}
			else
			{
				this._debugSessions.Remove(session);
				isEnabled = false;
			}
			OnRadiationOverlayToggledEvent ev = new OnRadiationOverlayToggledEvent(isEnabled);
			base.RaiseNetworkEvent(ev, session.ConnectedClient);
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x00040088 File Offset: 0x0003E288
		private void UpdateDebugOverlay(EntityEventArgs ev)
		{
			foreach (ICommonSession session in this._debugSessions.ToArray<ICommonSession>())
			{
				if (session.Status != 3)
				{
					this._debugSessions.Remove(session);
				}
				base.RaiseNetworkEvent(ev, session.ConnectedClient);
			}
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x000400D8 File Offset: 0x0003E2D8
		private void UpdateResistanceDebugOverlay()
		{
			if (this._debugSessions.Count == 0)
			{
				return;
			}
			EntityQuery<RadiationGridResistanceComponent> query = base.GetEntityQuery<RadiationGridResistanceComponent>();
			Dictionary<EntityUid, Dictionary<Vector2i, float>> dict = new Dictionary<EntityUid, Dictionary<Vector2i, float>>();
			foreach (MapGridComponent mapGridComponent in this._mapManager.GetAllGrids())
			{
				EntityUid gridUid = mapGridComponent.Owner;
				RadiationGridResistanceComponent resistance;
				if (query.TryGetComponent(gridUid, ref resistance))
				{
					Dictionary<Vector2i, float> resMap = resistance.ResistancePerTile;
					dict.Add(gridUid, resMap);
				}
			}
			OnRadiationOverlayResistanceUpdateEvent ev = new OnRadiationOverlayResistanceUpdateEvent(dict);
			this.UpdateDebugOverlay(ev);
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x00040174 File Offset: 0x0003E374
		private void UpdateGridcastDebugOverlay(double elapsedTime, int totalSources, int totalReceivers, List<RadiationRay> rays)
		{
			if (this._debugSessions.Count == 0)
			{
				return;
			}
			OnRadiationOverlayUpdateEvent ev = new OnRadiationOverlayUpdateEvent(elapsedTime, totalSources, totalReceivers, rays);
			this.UpdateDebugOverlay(ev);
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x000401A4 File Offset: 0x0003E3A4
		private void UpdateGridcast()
		{
			bool saveVisitedTiles = this._debugSessions.Count > 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			IEnumerable<ValueTuple<RadiationSourceComponent, TransformComponent>> enumerable = base.EntityQuery<RadiationSourceComponent, TransformComponent>(false);
			IEnumerable<ValueTuple<RadiationReceiverComponent, TransformComponent>> destinations = base.EntityQuery<RadiationReceiverComponent, TransformComponent>(false);
			EntityQuery<RadiationGridResistanceComponent> resistanceQuery = base.GetEntityQuery<RadiationGridResistanceComponent>();
			EntityQuery<TransformComponent> transformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<MapGridComponent> gridQuery = base.GetEntityQuery<MapGridComponent>();
			ValueList<ValueTuple<RadiationSourceComponent, TransformComponent, Vector2>> sourcesData = default(ValueList<ValueTuple<RadiationSourceComponent, TransformComponent, Vector2>>);
			foreach (ValueTuple<RadiationSourceComponent, TransformComponent> valueTuple in enumerable)
			{
				RadiationSourceComponent source = valueTuple.Item1;
				TransformComponent sourceTrs = valueTuple.Item2;
				Vector2 worldPos = this._transform.GetWorldPosition(sourceTrs, transformQuery);
				ValueTuple<RadiationSourceComponent, TransformComponent, Vector2> data = new ValueTuple<RadiationSourceComponent, TransformComponent, Vector2>(source, sourceTrs, worldPos);
				sourcesData.Add(data);
			}
			List<RadiationRay> rays = new List<RadiationRay>();
			ValueList<ValueTuple<RadiationReceiverComponent, float>> receiversTotalRads = default(ValueList<ValueTuple<RadiationReceiverComponent, float>>);
			foreach (ValueTuple<RadiationReceiverComponent, TransformComponent> valueTuple2 in destinations)
			{
				RadiationReceiverComponent dest = valueTuple2.Item1;
				TransformComponent destTrs = valueTuple2.Item2;
				Vector2 destWorld = this._transform.GetWorldPosition(destTrs, transformQuery);
				float rads = 0f;
				foreach (ValueTuple<RadiationSourceComponent, TransformComponent, Vector2> valueTuple3 in sourcesData)
				{
					RadiationSourceComponent source2 = valueTuple3.Item1;
					TransformComponent sourceTrs2 = valueTuple3.Item2;
					Vector2 sourceWorld = valueTuple3.Item3;
					RadiationRay ray = this.Irradiate(sourceTrs2.Owner, sourceTrs2, sourceWorld, destTrs.Owner, destTrs, destWorld, source2.Intensity, source2.Slope, saveVisitedTiles, resistanceQuery, transformQuery, gridQuery);
					if (ray != null)
					{
						rays.Add(ray);
						if (ray.ReachedDestination)
						{
							rads += ray.Rads;
						}
					}
				}
				receiversTotalRads.Add(new ValueTuple<RadiationReceiverComponent, float>(dest, rads));
			}
			double elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
			int totalSources = sourcesData.Count;
			int totalReceivers = receiversTotalRads.Count;
			this.UpdateGridcastDebugOverlay(elapsedTime, totalSources, totalReceivers, rays);
			foreach (ValueTuple<RadiationReceiverComponent, float> valueTuple4 in receiversTotalRads)
			{
				RadiationReceiverComponent receiver = valueTuple4.Item1;
				float rads2 = valueTuple4.Item2;
				receiver.CurrentRadiation = rads2;
				if (rads2 > 0f)
				{
					this.IrradiateEntity(receiver.Owner, rads2, this.GridcastUpdateRate);
				}
			}
			base.RaiseLocalEvent<RadiationSystemUpdatedEvent>(default(RadiationSystemUpdatedEvent));
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x00040438 File Offset: 0x0003E638
		[return: Nullable(2)]
		private RadiationRay Irradiate(EntityUid sourceUid, TransformComponent sourceTrs, Vector2 sourceWorld, EntityUid destUid, TransformComponent destTrs, Vector2 destWorld, float incomingRads, float slope, bool saveVisitedTiles, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<RadiationGridResistanceComponent> resistanceQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> transformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<MapGridComponent> gridQuery)
		{
			if (sourceTrs.MapID != destTrs.MapID)
			{
				return null;
			}
			MapId mapId = sourceTrs.MapID;
			float dist = (destWorld - sourceWorld).Length;
			if (dist > this.GridcastMaxDistance)
			{
				return null;
			}
			float rads = incomingRads - slope * dist;
			if (rads <= this.MinIntensity)
			{
				return null;
			}
			RadiationRay ray = new RadiationRay(mapId, sourceUid, sourceWorld, destUid, destWorld, rads);
			if (!this.GridcastSimplifiedSameGrid || sourceTrs.GridUid == null || !(sourceTrs.GridUid == destTrs.GridUid))
			{
				Box2 box = Box2.FromTwoPoints(sourceWorld, destWorld);
				foreach (MapGridComponent grid in this._mapManager.FindGridsIntersecting(mapId, box, true))
				{
					ray = this.Gridcast(grid, ray, saveVisitedTiles, resistanceQuery, sourceTrs, destTrs, transformQuery.GetComponent(grid.Owner));
					if (ray.Rads <= 0f)
					{
						return ray;
					}
				}
				return ray;
			}
			MapGridComponent gridComponent;
			if (!gridQuery.TryGetComponent(sourceTrs.GridUid.Value, ref gridComponent))
			{
				return ray;
			}
			return this.Gridcast(gridComponent, ray, saveVisitedTiles, resistanceQuery, sourceTrs, destTrs, transformQuery.GetComponent(sourceTrs.GridUid.Value));
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x000405D8 File Offset: 0x0003E7D8
		private RadiationRay Gridcast(MapGridComponent grid, RadiationRay ray, bool saveVisitedTiles, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<RadiationGridResistanceComponent> resistanceQuery, TransformComponent sourceTrs, TransformComponent destTrs, TransformComponent gridTrs)
		{
			List<ValueTuple<Vector2i, float>> blockers = new List<ValueTuple<Vector2i, float>>();
			EntityUid gridUid = grid.Owner;
			RadiationGridResistanceComponent resistance;
			if (!resistanceQuery.TryGetComponent(gridUid, ref resistance))
			{
				return ray;
			}
			Dictionary<Vector2i, float> resistanceMap = resistance.ResistancePerTile;
			Vector2 srcLocal = (sourceTrs.ParentUid == grid.Owner) ? sourceTrs.LocalPosition : gridTrs.InvLocalMatrix.Transform(ray.Source);
			Vector2 dstLocal = (destTrs.ParentUid == grid.Owner) ? destTrs.LocalPosition : gridTrs.InvLocalMatrix.Transform(ray.Destination);
			Vector2i sourceGrid;
			sourceGrid..ctor((int)Math.Floor((double)(srcLocal.X / (float)grid.TileSize)), (int)Math.Floor((double)(srcLocal.Y / (float)grid.TileSize)));
			Vector2i destGrid;
			destGrid..ctor((int)Math.Floor((double)(dstLocal.X / (float)grid.TileSize)), (int)Math.Floor((double)(dstLocal.Y / (float)grid.TileSize)));
			GridLineEnumerator line;
			line..ctor(sourceGrid, destGrid);
			while (line.MoveNext())
			{
				Vector2i point = line.Current;
				float resData;
				if (resistanceMap.TryGetValue(point, out resData))
				{
					ray.Rads -= resData;
					if (saveVisitedTiles)
					{
						blockers.Add(new ValueTuple<Vector2i, float>(point, ray.Rads));
					}
					if (ray.Rads <= this.MinIntensity)
					{
						ray.Rads = 0f;
						break;
					}
				}
			}
			if (saveVisitedTiles && blockers.Count > 0)
			{
				ray.Blockers.Add(gridUid, blockers);
			}
			return ray;
		}

		// Token: 0x04000792 RID: 1938
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000793 RID: 1939
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000794 RID: 1940
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000795 RID: 1941
		private float _accumulator;

		// Token: 0x0400079A RID: 1946
		private readonly HashSet<ICommonSession> _debugSessions = new HashSet<ICommonSession>();
	}
}
