using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.Ghost.Components;
using Content.Server.Popups;
using Content.Server.Singularity.Components;
using Content.Server.Singularity.Events;
using Content.Server.Station.Components;
using Content.Shared.Coordinates;
using Content.Shared.Popups;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.EntitySystems
{
	// Token: 0x020001EA RID: 490
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EventHorizonSystem : SharedEventHorizonSystem
	{
		// Token: 0x06000958 RID: 2392 RVA: 0x0002F614 File Offset: 0x0002D814
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MapGridComponent, EventHorizonAttemptConsumeEntityEvent>(new ComponentEventHandler<MapGridComponent, EventHorizonAttemptConsumeEntityEvent>(this.PreventConsume<MapGridComponent>), null, null);
			base.SubscribeLocalEvent<GhostComponent, EventHorizonAttemptConsumeEntityEvent>(new ComponentEventHandler<GhostComponent, EventHorizonAttemptConsumeEntityEvent>(this.PreventConsume<GhostComponent>), null, null);
			base.SubscribeLocalEvent<StationDataComponent, EventHorizonAttemptConsumeEntityEvent>(new ComponentEventHandler<StationDataComponent, EventHorizonAttemptConsumeEntityEvent>(this.PreventConsume<StationDataComponent>), null, null);
			base.SubscribeLocalEvent<EventHorizonComponent, StartCollideEvent>(new ComponentEventRefHandler<EventHorizonComponent, StartCollideEvent>(this.OnStartCollide), null, null);
			base.SubscribeLocalEvent<EventHorizonComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventHandler<EventHorizonComponent, EntGotInsertedIntoContainerMessage>(this.OnEventHorizonContained), null, null);
			base.SubscribeLocalEvent<EventHorizonContainedEvent>(new EntityEventHandler<EventHorizonContainedEvent>(this.OnEventHorizonContained), null, null);
			base.SubscribeLocalEvent<EventHorizonComponent, EventHorizonAttemptConsumeEntityEvent>(new ComponentEventHandler<EventHorizonComponent, EventHorizonAttemptConsumeEntityEvent>(this.OnAnotherEventHorizonAttemptConsumeThisEventHorizon), null, null);
			base.SubscribeLocalEvent<EventHorizonComponent, EventHorizonConsumedEntityEvent>(new ComponentEventHandler<EventHorizonComponent, EventHorizonConsumedEntityEvent>(this.OnAnotherEventHorizonConsumedThisEventHorizon), null, null);
			base.SubscribeLocalEvent<ContainerManagerComponent, EventHorizonConsumedEntityEvent>(new ComponentEventHandler<ContainerManagerComponent, EventHorizonConsumedEntityEvent>(this.OnContainerConsumed), null, null);
			this.Vvm.GetTypeHandler<EventHorizonComponent>().AddPath<TimeSpan>("TargetConsumePeriod", (EntityUid _, EventHorizonComponent comp) => comp.TargetConsumePeriod, new ComponentPropertySetter<EventHorizonComponent, TimeSpan>(this.SetConsumePeriod));
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x0002F71C File Offset: 0x0002D91C
		public override void Shutdown()
		{
			this.Vvm.GetTypeHandler<EventHorizonComponent>().RemovePath("TargetConsumePeriod");
			base.Shutdown();
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x0002F73C File Offset: 0x0002D93C
		public override void Update(float frameTime)
		{
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			foreach (ValueTuple<EventHorizonComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<EventHorizonComponent, TransformComponent>(false))
			{
				EventHorizonComponent eventHorizon = valueTuple.Item1;
				TransformComponent xform = valueTuple.Item2;
				TimeSpan curTime = this._timing.CurTime;
				if (eventHorizon.NextConsumeWaveTime <= curTime)
				{
					this.Update(eventHorizon.Owner, eventHorizon, xform);
				}
				if (eventHorizon != null && !eventHorizon.WasDetectedInBreach && eventHorizon.CanBreachContainment)
				{
					this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-singularity-can-breach-containment", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("singularity", base.ToPrettyString(eventHorizon.Owner))
					}));
					eventHorizon.WasDetectedInBreach = true;
				}
			}
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0002F828 File Offset: 0x0002DA28
		[NullableContext(2)]
		public void Update(EntityUid uid, EventHorizonComponent eventHorizon = null, TransformComponent xform = null)
		{
			if (!base.Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
			{
				return;
			}
			eventHorizon.LastConsumeWaveTime = this._timing.CurTime;
			eventHorizon.NextConsumeWaveTime = eventHorizon.LastConsumeWaveTime + eventHorizon.TargetConsumePeriod;
			if (eventHorizon.BeingConsumedByAnotherEventHorizon)
			{
				return;
			}
			if (!base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				return;
			}
			IContainer container;
			if (this._containerSystem.TryGetContainingContainer(uid, ref container, null, xform) && !this.AttemptConsumeEntity(container.Owner, eventHorizon, null))
			{
				this.ConsumeEntitiesInContainer(uid, container, eventHorizon, container);
				return;
			}
			if (eventHorizon.Radius > 0f)
			{
				this.ConsumeEverythingInRange(xform.Owner, eventHorizon.Radius, xform, eventHorizon);
			}
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x0002F8D0 File Offset: 0x0002DAD0
		public void ConsumeEntity(EntityUid uid, EventHorizonComponent eventHorizon, [Nullable(2)] IContainer outerContainer = null)
		{
			this.EntityManager.QueueDeleteEntity(uid);
			base.RaiseLocalEvent<EntityConsumedByEventHorizonEvent>(eventHorizon.Owner, new EntityConsumedByEventHorizonEvent(uid, eventHorizon, outerContainer), false);
			base.RaiseLocalEvent<EventHorizonConsumedEntityEvent>(uid, new EventHorizonConsumedEntityEvent(uid, eventHorizon, outerContainer), false);
			SingularityDestroyerComponent destroyerComponent;
			if (base.TryComp<SingularityDestroyerComponent>(uid, ref destroyerComponent) && destroyerComponent.Active)
			{
				this.EntityManager.QueueDeleteEntity(eventHorizon.Owner);
				base.RemComp<EventHorizonComponent>(eventHorizon.Owner);
				Filter filter = Filter.Pvs(eventHorizon.Owner.ToCoordinates(), 2f, null, null).AddPlayersByPvs(eventHorizon.Owner, 40f, null, null, null);
				this._audioSystem.Play("/Audio/Items/Toys/singularity_prolapse.ogg", filter, eventHorizon.Owner, false, null);
				EntityUid toysingulo = base.Spawn("SingularityToy", eventHorizon.Owner.ToCoordinates().ToMap(this._entityManager));
				this._popupSystem.PopupEntity(Loc.GetString("singularity-destroyer-component-trigger"), toysingulo, PopupType.LargeCaution);
			}
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0002F9CF File Offset: 0x0002DBCF
		public bool AttemptConsumeEntity(EntityUid uid, EventHorizonComponent eventHorizon, [Nullable(2)] IContainer outerContainer = null)
		{
			if (!this.CanConsumeEntity(uid, eventHorizon))
			{
				return false;
			}
			this.ConsumeEntity(uid, eventHorizon, outerContainer);
			return true;
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x0002F9E8 File Offset: 0x0002DBE8
		public bool CanConsumeEntity(EntityUid uid, EventHorizonComponent eventHorizon)
		{
			EventHorizonAttemptConsumeEntityEvent ev = new EventHorizonAttemptConsumeEntityEvent(uid, eventHorizon);
			base.RaiseLocalEvent<EventHorizonAttemptConsumeEntityEvent>(uid, ev, false);
			return !ev.Cancelled;
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x0002FA10 File Offset: 0x0002DC10
		[NullableContext(2)]
		public void ConsumeEntitiesInRange(EntityUid uid, float range, TransformComponent xform = null, EventHorizonComponent eventHorizon = null)
		{
			if (!base.Resolve<TransformComponent, EventHorizonComponent>(uid, ref xform, ref eventHorizon, true))
			{
				return;
			}
			foreach (EntityUid entity in this._lookup.GetEntitiesInRange(xform.MapPosition, range, 14))
			{
				if (!(entity == uid))
				{
					this.AttemptConsumeEntity(entity, eventHorizon, null);
				}
			}
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x0002FA90 File Offset: 0x0002DC90
		public void ConsumeEntitiesInContainer(EntityUid uid, IContainer container, EventHorizonComponent eventHorizon, [Nullable(2)] IContainer outerContainer = null)
		{
			List<EntityUid> immune = new List<EntityUid>();
			foreach (EntityUid entity in container.ContainedEntities)
			{
				if (entity == uid || !this.AttemptConsumeEntity(entity, eventHorizon, outerContainer))
				{
					immune.Add(entity);
				}
			}
			if (outerContainer == container)
			{
				return;
			}
			foreach (EntityUid entity2 in immune)
			{
				IContainer target_container = outerContainer;
				while (target_container != null && !target_container.Insert(entity2, null, null, null, null, null))
				{
					this._containerSystem.TryGetContainingContainer(target_container.Owner, ref target_container, null, null);
				}
				if (target_container == null)
				{
					base.Transform(entity2).AttachToGridOrMap();
				}
			}
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0002FB78 File Offset: 0x0002DD78
		public void ConsumeTile(TileRef tile, EventHorizonComponent eventHorizon)
		{
			this.ConsumeTiles(new List<ValueTuple<Vector2i, Tile>>(new ValueTuple<Vector2i, Tile>[]
			{
				new ValueTuple<Vector2i, Tile>(tile.GridIndices, Tile.Empty)
			}), this._mapMan.GetGrid(tile.GridUid), eventHorizon);
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0002FBB4 File Offset: 0x0002DDB4
		public void AttemptConsumeTile(TileRef tile, EventHorizonComponent eventHorizon)
		{
			this.AttemptConsumeTiles(new TileRef[]
			{
				tile
			}, this._mapMan.GetGrid(tile.GridUid), eventHorizon);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x0002FBDD File Offset: 0x0002DDDD
		public void ConsumeTiles([Nullable(new byte[]
		{
			1,
			0
		})] List<ValueTuple<Vector2i, Tile>> tiles, MapGridComponent grid, EventHorizonComponent eventHorizon)
		{
			if (tiles.Count <= 0)
			{
				return;
			}
			base.RaiseLocalEvent<TilesConsumedByEventHorizonEvent>(eventHorizon.Owner, new TilesConsumedByEventHorizonEvent(tiles, grid, eventHorizon), false);
			grid.SetTiles(tiles);
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x0002FC08 File Offset: 0x0002DE08
		public int AttemptConsumeTiles(IEnumerable<TileRef> tiles, MapGridComponent grid, EventHorizonComponent eventHorizon)
		{
			List<ValueTuple<Vector2i, Tile>> toConsume = new List<ValueTuple<Vector2i, Tile>>();
			foreach (TileRef tile in tiles)
			{
				if (this.CanConsumeTile(tile, grid, eventHorizon))
				{
					toConsume.Add(new ValueTuple<Vector2i, Tile>(tile.GridIndices, Tile.Empty));
				}
			}
			int count = toConsume.Count;
			if (toConsume.Count > 0)
			{
				this.ConsumeTiles(toConsume, grid, eventHorizon);
			}
			return count;
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0002FC88 File Offset: 0x0002DE88
		public bool CanConsumeTile(TileRef tile, MapGridComponent grid, EventHorizonComponent eventHorizon)
		{
			foreach (EntityUid blockingEntity in grid.GetAnchoredEntities(tile.GridIndices))
			{
				if (!this.CanConsumeEntity(blockingEntity, eventHorizon))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x0002FCE8 File Offset: 0x0002DEE8
		[NullableContext(2)]
		public void ConsumeTilesInRange(EntityUid uid, float range, TransformComponent xform, EventHorizonComponent eventHorizon)
		{
			if (!base.Resolve<TransformComponent>(uid, ref xform, true) || !base.Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
			{
				return;
			}
			MapCoordinates mapPos = xform.MapPosition;
			Box2 box = Box2.CenteredAround(mapPos.Position, new Vector2(range, range));
			Circle circle;
			circle..ctor(mapPos.Position, range);
			foreach (MapGridComponent grid in this._mapMan.FindGridsIntersecting(mapPos.MapId, box, false))
			{
				this.AttemptConsumeTiles(grid.GetTilesIntersecting(circle, true, null), grid, eventHorizon);
			}
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0002FD94 File Offset: 0x0002DF94
		[NullableContext(2)]
		public void ConsumeEverythingInRange(EntityUid uid, float range, TransformComponent xform = null, EventHorizonComponent eventHorizon = null)
		{
			if (!base.Resolve<TransformComponent, EventHorizonComponent>(uid, ref xform, ref eventHorizon, true))
			{
				return;
			}
			this.ConsumeEntitiesInRange(uid, range, xform, eventHorizon);
			this.ConsumeTilesInRange(uid, range, xform, eventHorizon);
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x0002FDBC File Offset: 0x0002DFBC
		[NullableContext(2)]
		public void SetConsumePeriod(EntityUid uid, TimeSpan value, EventHorizonComponent eventHorizon = null)
		{
			if (!base.Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
			{
				return;
			}
			if (MathHelper.CloseTo(eventHorizon.TargetConsumePeriod.TotalSeconds, value.TotalSeconds, 1E-07))
			{
				return;
			}
			eventHorizon.TargetConsumePeriod = value;
			eventHorizon.NextConsumeWaveTime = eventHorizon.LastConsumeWaveTime + eventHorizon.TargetConsumePeriod;
			TimeSpan curTime = this._timing.CurTime;
			if (eventHorizon.NextConsumeWaveTime < curTime)
			{
				this.Update(uid, eventHorizon, null);
			}
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x0002FE3D File Offset: 0x0002E03D
		protected override bool PreventCollide(EntityUid uid, EventHorizonComponent comp, ref PreventCollideEvent args)
		{
			if (base.PreventCollide(uid, comp, ref args) || args.Cancelled)
			{
				return true;
			}
			args.Cancelled = !this.CanConsumeEntity(args.BodyB.Owner, comp);
			return false;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0002FE70 File Offset: 0x0002E070
		public void PreventConsume<[Nullable(2)] TComp>(EntityUid uid, TComp comp, EventHorizonAttemptConsumeEntityEvent args)
		{
			if (!args.Cancelled)
			{
				args.Cancel();
			}
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0002FE80 File Offset: 0x0002E080
		public void PreventBreach<[Nullable(2)] TComp>(EntityUid uid, TComp comp, EventHorizonAttemptConsumeEntityEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			if (!args.EventHorizon.CanBreachContainment)
			{
				this.PreventConsume<TComp>(uid, comp, args);
			}
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0002FEA1 File Offset: 0x0002E0A1
		private void OnStartCollide(EntityUid uid, EventHorizonComponent comp, ref StartCollideEvent args)
		{
			if (comp.BeingConsumedByAnotherEventHorizon)
			{
				return;
			}
			if (args.OurFixture.ID != comp.HorizonFixtureId)
			{
				return;
			}
			this.AttemptConsumeEntity(args.OtherFixture.Body.Owner, comp, null);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0002FEDE File Offset: 0x0002E0DE
		private void OnAnotherEventHorizonAttemptConsumeThisEventHorizon(EntityUid uid, EventHorizonComponent comp, EventHorizonAttemptConsumeEntityEvent args)
		{
			if (!args.Cancelled && (args.EventHorizon == comp || comp.BeingConsumedByAnotherEventHorizon))
			{
				args.Cancel();
			}
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0002FEFF File Offset: 0x0002E0FF
		private void OnAnotherEventHorizonConsumedThisEventHorizon(EntityUid uid, EventHorizonComponent comp, EventHorizonConsumedEntityEvent args)
		{
			comp.BeingConsumedByAnotherEventHorizon = true;
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x0002FF08 File Offset: 0x0002E108
		private void OnEventHorizonContained(EntityUid uid, EventHorizonComponent comp, EntGotInsertedIntoContainerMessage args)
		{
			base.QueueLocalEvent(new EventHorizonContainedEvent(uid, comp, args));
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0002FF18 File Offset: 0x0002E118
		private void OnEventHorizonContained(EventHorizonContainedEvent args)
		{
			EntityUid uid = args.Entity;
			EventHorizonComponent comp = args.EventHorizon;
			if (!this.EntityManager.EntityExists(uid))
			{
				return;
			}
			if (comp.BeingConsumedByAnotherEventHorizon)
			{
				return;
			}
			EntityUid containerEntity = args.Args.Container.Owner;
			if (!this.EntityManager.EntityExists(containerEntity) || !this.AttemptConsumeEntity(containerEntity, comp, null))
			{
				this.ConsumeEntitiesInContainer(uid, args.Args.Container, comp, args.Args.Container);
			}
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x0002FF94 File Offset: 0x0002E194
		private void OnContainerConsumed(EntityUid uid, ContainerManagerComponent comp, EventHorizonConsumedEntityEvent args)
		{
			IContainer drop_container = args.Container;
			if (drop_container == null)
			{
				this._containerSystem.TryGetContainingContainer(uid, ref drop_container, null, null);
			}
			foreach (IContainer container in comp.GetAllContainers())
			{
				this.ConsumeEntitiesInContainer(args.EventHorizon.Owner, container, args.EventHorizon, drop_container);
			}
		}

		// Token: 0x040005A6 RID: 1446
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040005A7 RID: 1447
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040005A8 RID: 1448
		[Dependency]
		private readonly IMapManager _mapMan;

		// Token: 0x040005A9 RID: 1449
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x040005AA RID: 1450
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x040005AB RID: 1451
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040005AC RID: 1452
		[Dependency]
		private readonly AudioSystem _audioSystem;

		// Token: 0x040005AD RID: 1453
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040005AE RID: 1454
		private const int MaxEventHorizonUnnestingIterations = 100;

		// Token: 0x040005AF RID: 1455
		private const int MaxEventHorizonDumpSearchIterations = 100;
	}
}
