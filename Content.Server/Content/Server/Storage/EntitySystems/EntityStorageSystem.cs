using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Construction;
using Content.Server.Construction.Components;
using Content.Server.Popups;
using Content.Server.Storage.Components;
using Content.Server.Tools.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Destructible;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Lock;
using Content.Shared.Placeable;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Wall;
using Content.Shared.Whitelist;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x02000160 RID: 352
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntityStorageSystem : EntitySystem
	{
		// Token: 0x060006BD RID: 1725 RVA: 0x00021344 File Offset: 0x0001F544
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EntityStorageComponent, ComponentInit>(new ComponentEventHandler<EntityStorageComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<EntityStorageComponent, ActivateInWorldEvent>(new ComponentEventHandler<EntityStorageComponent, ActivateInWorldEvent>(this.OnInteract), null, null);
			base.SubscribeLocalEvent<EntityStorageComponent, WeldableAttemptEvent>(new ComponentEventHandler<EntityStorageComponent, WeldableAttemptEvent>(this.OnWeldableAttempt), null, null);
			base.SubscribeLocalEvent<EntityStorageComponent, WeldableChangedEvent>(new ComponentEventHandler<EntityStorageComponent, WeldableChangedEvent>(this.OnWelded), null, null);
			base.SubscribeLocalEvent<EntityStorageComponent, LockToggleAttemptEvent>(new ComponentEventRefHandler<EntityStorageComponent, LockToggleAttemptEvent>(this.OnLockToggleAttempt), null, null);
			base.SubscribeLocalEvent<EntityStorageComponent, DestructionEventArgs>(new ComponentEventHandler<EntityStorageComponent, DestructionEventArgs>(this.OnDestruction), null, null);
			base.SubscribeLocalEvent<InsideEntityStorageComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<InsideEntityStorageComponent, EntGotRemovedFromContainerMessage>(this.OnRemoved), null, null);
			base.SubscribeLocalEvent<InsideEntityStorageComponent, InhaleLocationEvent>(new ComponentEventHandler<InsideEntityStorageComponent, InhaleLocationEvent>(this.OnInsideInhale), null, null);
			base.SubscribeLocalEvent<InsideEntityStorageComponent, ExhaleLocationEvent>(new ComponentEventHandler<InsideEntityStorageComponent, ExhaleLocationEvent>(this.OnInsideExhale), null, null);
			base.SubscribeLocalEvent<InsideEntityStorageComponent, AtmosExposedGetAirEvent>(new ComponentEventRefHandler<InsideEntityStorageComponent, AtmosExposedGetAirEvent>(this.OnInsideExposed), null, null);
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00021420 File Offset: 0x0001F620
		private void OnInit(EntityUid uid, EntityStorageComponent component, ComponentInit args)
		{
			component.Contents = this._container.EnsureContainer<Container>(uid, "entity_storage", null);
			component.Contents.ShowContents = component.ShowContents;
			component.Contents.OccludesLight = component.OccludesLight;
			ConstructionComponent construction;
			if (base.TryComp<ConstructionComponent>(uid, ref construction))
			{
				this._construction.AddContainer(uid, "entity_storage", construction);
			}
			PlaceableSurfaceComponent placeable;
			if (base.TryComp<PlaceableSurfaceComponent>(uid, ref placeable))
			{
				this._placeableSurface.SetPlaceable(uid, component.Open, placeable);
			}
			if (!component.Open)
			{
				this.TakeGas(uid, component);
			}
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x000214B3 File Offset: 0x0001F6B3
		private void OnInteract(EntityUid uid, EntityStorageComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			this.ToggleOpen(args.User, uid, component);
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x000214D4 File Offset: 0x0001F6D4
		private void OnWeldableAttempt(EntityUid uid, EntityStorageComponent component, WeldableAttemptEvent args)
		{
			if (component.Open)
			{
				args.Cancel();
				return;
			}
			if (component.Contents.Contains(args.User))
			{
				string msg = Loc.GetString("entity-storage-component-already-contains-user-message");
				this._popupSystem.PopupEntity(msg, args.User, args.User, PopupType.Small);
				args.Cancel();
			}
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x0002152D File Offset: 0x0001F72D
		private void OnWelded(EntityUid uid, EntityStorageComponent component, WeldableChangedEvent args)
		{
			component.IsWeldedShut = args.IsWelded;
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x0002153B File Offset: 0x0001F73B
		private void OnLockToggleAttempt(EntityUid uid, EntityStorageComponent target, ref LockToggleAttemptEvent args)
		{
			if (target.Open)
			{
				args.Cancelled = true;
			}
			if (target.Contents.Contains(args.User))
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x00021568 File Offset: 0x0001F768
		private void OnDestruction(EntityUid uid, EntityStorageComponent component, DestructionEventArgs args)
		{
			component.Open = true;
			if (!component.DeleteContentsOnDestruction)
			{
				this.EmptyContents(uid, component);
				return;
			}
			foreach (EntityUid ent in new List<EntityUid>(component.Contents.ContainedEntities))
			{
				this.EntityManager.DeleteEntity(ent);
			}
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x000215E4 File Offset: 0x0001F7E4
		[NullableContext(2)]
		public void ToggleOpen(EntityUid user, EntityUid target, EntityStorageComponent component = null)
		{
			if (!base.Resolve<EntityStorageComponent>(target, ref component, true))
			{
				return;
			}
			if (component.Open)
			{
				this.TryCloseStorage(target);
				return;
			}
			this.TryOpenStorage(user, target, false);
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00021610 File Offset: 0x0001F810
		[NullableContext(2)]
		public void EmptyContents(EntityUid uid, EntityStorageComponent component = null)
		{
			if (!base.Resolve<EntityStorageComponent>(uid, ref component, true))
			{
				return;
			}
			TransformComponent uidXform = base.Transform(uid);
			foreach (EntityUid contained in component.Contents.ContainedEntities.ToArray<EntityUid>())
			{
				this.Remove(contained, uid, component, uidXform);
			}
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x00021664 File Offset: 0x0001F864
		[NullableContext(2)]
		public void OpenStorage(EntityUid uid, EntityStorageComponent component = null)
		{
			if (!base.Resolve<EntityStorageComponent>(uid, ref component, true))
			{
				return;
			}
			StorageBeforeOpenEvent beforeev = default(StorageBeforeOpenEvent);
			base.RaiseLocalEvent<StorageBeforeOpenEvent>(uid, ref beforeev, false);
			component.Open = true;
			this.EmptyContents(uid, component);
			this.ModifyComponents(uid, component);
			this._audio.PlayPvs(component.OpenSound, uid, null);
			this.ReleaseGas(uid, component);
			StorageAfterOpenEvent afterev = default(StorageAfterOpenEvent);
			base.RaiseLocalEvent<StorageAfterOpenEvent>(uid, ref afterev, false);
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x000216E0 File Offset: 0x0001F8E0
		[NullableContext(2)]
		public void CloseStorage(EntityUid uid, EntityStorageComponent component = null)
		{
			if (!base.Resolve<EntityStorageComponent>(uid, ref component, true))
			{
				return;
			}
			component.Open = false;
			EntityCoordinates targetCoordinates;
			targetCoordinates..ctor(uid, component.EnteringOffset);
			HashSet<EntityUid> entities = this._lookup.GetEntitiesInRange(targetCoordinates, component.EnteringRange, 11);
			StorageBeforeCloseEvent ev = new StorageBeforeCloseEvent(entities, new HashSet<EntityUid>());
			base.RaiseLocalEvent<StorageBeforeCloseEvent>(uid, ref ev, false);
			int count = 0;
			foreach (EntityUid entity in ev.Contents)
			{
				if ((ev.BypassChecks.Contains(entity) || this.CanFit(entity, uid, component.Whitelist)) && this.AddToContents(entity, uid, component))
				{
					count++;
					if (count >= component.Capacity)
					{
						break;
					}
				}
			}
			this.TakeGas(uid, component);
			this.ModifyComponents(uid, component);
			this._audio.PlayPvs(component.CloseSound, uid, null);
			component.LastInternalOpenAttempt = default(TimeSpan);
			StorageAfterCloseEvent afterev = default(StorageAfterCloseEvent);
			base.RaiseLocalEvent<StorageAfterCloseEvent>(uid, ref afterev, false);
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00021808 File Offset: 0x0001FA08
		[NullableContext(2)]
		public bool Insert(EntityUid toInsert, EntityUid container, EntityStorageComponent component = null)
		{
			if (!base.Resolve<EntityStorageComponent>(container, ref component, true))
			{
				return false;
			}
			if (component.Open)
			{
				base.Transform(toInsert).WorldPosition = base.Transform(container).WorldPosition;
				return true;
			}
			base.EnsureComp<InsideEntityStorageComponent>(toInsert).Storage = container;
			return component.Contents.Insert(toInsert, this.EntityManager, null, null, null, null);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x00021868 File Offset: 0x0001FA68
		[NullableContext(2)]
		public bool Remove(EntityUid toRemove, EntityUid container, EntityStorageComponent component = null, TransformComponent xform = null)
		{
			if (!base.Resolve<EntityStorageComponent, TransformComponent>(container, ref component, ref xform, false))
			{
				return false;
			}
			base.RemComp<InsideEntityStorageComponent>(toRemove);
			component.Contents.Remove(toRemove, this.EntityManager, null, null, true, false, null, null);
			base.Transform(toRemove).WorldPosition = xform.WorldPosition + xform.WorldRotation.RotateVec(ref component.EnteringOffset);
			return true;
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x000218E4 File Offset: 0x0001FAE4
		[NullableContext(2)]
		public bool CanInsert(EntityUid container, EntityStorageComponent component = null)
		{
			return base.Resolve<EntityStorageComponent>(container, ref component, true) && (component.Open || component.Contents.ContainedEntities.Count < component.Capacity);
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x00021919 File Offset: 0x0001FB19
		public bool TryOpenStorage(EntityUid user, EntityUid target, bool silent = false)
		{
			if (!this.CanOpen(user, target, silent, null))
			{
				return false;
			}
			this.OpenStorage(target, null);
			return true;
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x00021932 File Offset: 0x0001FB32
		public bool TryCloseStorage(EntityUid target)
		{
			if (!this.CanClose(target, false))
			{
				return false;
			}
			this.CloseStorage(target, null);
			return true;
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0002194C File Offset: 0x0001FB4C
		[NullableContext(2)]
		public bool CanOpen(EntityUid user, EntityUid target, bool silent = false, EntityStorageComponent component = null)
		{
			if (!base.Resolve<EntityStorageComponent>(target, ref component, true))
			{
				return false;
			}
			if (!base.HasComp<SharedHandsComponent>(user))
			{
				return false;
			}
			if (component.IsWeldedShut)
			{
				if (!silent && !component.Contents.Contains(user))
				{
					this._popupSystem.PopupEntity(Loc.GetString("entity-storage-component-welded-shut-message"), target, PopupType.Small);
				}
				return false;
			}
			if (component.EnteringOffset != new ValueTuple<float, float>(0f, 0f) && !base.HasComp<WallMountComponent>(target))
			{
				EntityCoordinates newCoords;
				newCoords..ctor(target, component.EnteringOffset);
				if (!this._interactionSystem.InRangeUnobstructed(target, newCoords, 0f, component.EnteringOffsetCollisionFlags, null, false))
				{
					if (!silent)
					{
						this._popupSystem.PopupEntity(Loc.GetString("entity-storage-component-cannot-open-no-space"), target, PopupType.Small);
					}
					return false;
				}
			}
			StorageOpenAttemptEvent ev = new StorageOpenAttemptEvent(silent, false);
			base.RaiseLocalEvent<StorageOpenAttemptEvent>(target, ref ev, true);
			return !ev.Cancelled;
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x00021A38 File Offset: 0x0001FC38
		public bool CanClose(EntityUid target, bool silent = false)
		{
			StorageCloseAttemptEvent ev = default(StorageCloseAttemptEvent);
			base.RaiseLocalEvent<StorageCloseAttemptEvent>(target, ref ev, silent);
			return !ev.Cancelled;
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00021A64 File Offset: 0x0001FC64
		[NullableContext(2)]
		public bool AddToContents(EntityUid toAdd, EntityUid container, EntityStorageComponent component = null)
		{
			if (!base.Resolve<EntityStorageComponent>(container, ref component, true))
			{
				return false;
			}
			if (toAdd == container)
			{
				return false;
			}
			PhysicsComponent phys;
			if (base.TryComp<PhysicsComponent>(toAdd, ref phys))
			{
				Box2 aabb = this._physics.GetWorldAABB(toAdd, null, phys, null);
				if (component.MaxSize < aabb.Size.X || component.MaxSize < aabb.Size.Y)
				{
					return false;
				}
			}
			return this.Insert(toAdd, container, component);
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x00021AD8 File Offset: 0x0001FCD8
		[NullableContext(2)]
		public bool CanFit(EntityUid toInsert, EntityUid container, EntityWhitelist whitelist)
		{
			InsertIntoEntityStorageAttemptEvent attemptEvent = default(InsertIntoEntityStorageAttemptEvent);
			base.RaiseLocalEvent<InsertIntoEntityStorageAttemptEvent>(toInsert, ref attemptEvent, false);
			if (attemptEvent.Cancelled)
			{
				return false;
			}
			bool flag = base.HasComp<BodyComponent>(toInsert);
			bool storageIsItem = base.HasComp<ItemComponent>(container);
			bool allowedToEat = (whitelist != null) ? whitelist.IsValid(toInsert, null) : base.HasComp<ItemComponent>(toInsert);
			if (flag)
			{
				if (!storageIsItem)
				{
					allowedToEat = true;
				}
				else
				{
					StoreMobInItemContainerAttemptEvent storeEv = default(StoreMobInItemContainerAttemptEvent);
					base.RaiseLocalEvent<StoreMobInItemContainerAttemptEvent>(container, ref storeEv, false);
					allowedToEat = (storeEv.Handled && !storeEv.Cancelled);
				}
			}
			return allowedToEat;
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00021B5C File Offset: 0x0001FD5C
		[NullableContext(2)]
		public void ModifyComponents(EntityUid uid, EntityStorageComponent component = null)
		{
			if (!base.Resolve<EntityStorageComponent>(uid, ref component, true))
			{
				return;
			}
			FixturesComponent fixtures;
			if (!component.IsCollidableWhenOpen && base.TryComp<FixturesComponent>(uid, ref fixtures) && fixtures.Fixtures.Count > 0)
			{
				Fixture fixture = fixtures.Fixtures.Values.First<Fixture>();
				if (component.Open)
				{
					component.RemovedMasks = (fixture.CollisionLayer & component.MasksToRemove);
					this._physics.SetCollisionLayer(uid, fixture, fixture.CollisionLayer & ~component.MasksToRemove, fixtures, null);
				}
				else
				{
					this._physics.SetCollisionLayer(uid, fixture, fixture.CollisionLayer | component.RemovedMasks, fixtures, null);
					component.RemovedMasks = 0;
				}
			}
			PlaceableSurfaceComponent surface;
			if (base.TryComp<PlaceableSurfaceComponent>(uid, ref surface))
			{
				this._placeableSurface.SetPlaceable(uid, component.Open, surface);
			}
			this._appearance.SetData(uid, StorageVisuals.Open, component.Open, null);
			this._appearance.SetData(uid, StorageVisuals.HasContents, component.Contents.ContainedEntities.Count > 0, null);
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x00021C70 File Offset: 0x0001FE70
		private void TakeGas(EntityUid uid, EntityStorageComponent component)
		{
			if (!component.Airtight)
			{
				return;
			}
			TileRef? tile = this.GetOffsetTileRef(uid, component);
			if (tile != null)
			{
				GasMixture environment = this._atmos.GetTileMixture(new EntityUid?(tile.Value.GridUid), null, tile.Value.GridIndices, true);
				if (environment != null)
				{
					this._atmos.Merge(component.Air, environment.RemoveVolume(70f));
				}
			}
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00021CEC File Offset: 0x0001FEEC
		public void ReleaseGas(EntityUid uid, EntityStorageComponent component)
		{
			if (!component.Airtight)
			{
				return;
			}
			TileRef? tile = this.GetOffsetTileRef(uid, component);
			if (tile != null)
			{
				GasMixture environment = this._atmos.GetTileMixture(new EntityUid?(tile.Value.GridUid), null, tile.Value.GridIndices, true);
				if (environment != null)
				{
					this._atmos.Merge(environment, component.Air);
					component.Air.Clear();
				}
			}
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x00021D68 File Offset: 0x0001FF68
		private TileRef? GetOffsetTileRef(EntityUid uid, EntityStorageComponent component)
		{
			MapCoordinates targetCoordinates = new EntityCoordinates(uid, component.EnteringOffset).ToMap(this.EntityManager);
			MapGridComponent grid;
			if (this._map.TryFindGridAt(targetCoordinates, ref grid))
			{
				return new TileRef?(grid.GetTileRef(targetCoordinates));
			}
			return null;
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x00021DB6 File Offset: 0x0001FFB6
		private void OnRemoved(EntityUid uid, InsideEntityStorageComponent component, EntGotRemovedFromContainerMessage args)
		{
			if (args.Container.Owner != component.Storage)
			{
				return;
			}
			base.RemComp(uid, component);
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x00021DDC File Offset: 0x0001FFDC
		private void OnInsideInhale(EntityUid uid, InsideEntityStorageComponent component, InhaleLocationEvent args)
		{
			EntityStorageComponent storage;
			if (base.TryComp<EntityStorageComponent>(component.Storage, ref storage) && storage.Airtight)
			{
				args.Gas = storage.Air;
			}
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00021E10 File Offset: 0x00020010
		private void OnInsideExhale(EntityUid uid, InsideEntityStorageComponent component, ExhaleLocationEvent args)
		{
			EntityStorageComponent storage;
			if (base.TryComp<EntityStorageComponent>(component.Storage, ref storage) && storage.Airtight)
			{
				args.Gas = storage.Air;
			}
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x00021E44 File Offset: 0x00020044
		private void OnInsideExposed(EntityUid uid, InsideEntityStorageComponent component, ref AtmosExposedGetAirEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			EntityStorageComponent storage;
			if (base.TryComp<EntityStorageComponent>(component.Storage, ref storage))
			{
				if (!storage.Airtight)
				{
					return;
				}
				args.Gas = storage.Air;
			}
			args.Handled = true;
		}

		// Token: 0x040003E6 RID: 998
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040003E7 RID: 999
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040003E8 RID: 1000
		[Dependency]
		private readonly ConstructionSystem _construction;

		// Token: 0x040003E9 RID: 1001
		[Dependency]
		private readonly ContainerSystem _container;

		// Token: 0x040003EA RID: 1002
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040003EB RID: 1003
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x040003EC RID: 1004
		[Dependency]
		private readonly PlaceableSurfaceSystem _placeableSurface;

		// Token: 0x040003ED RID: 1005
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040003EE RID: 1006
		[Dependency]
		private readonly AtmosphereSystem _atmos;

		// Token: 0x040003EF RID: 1007
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040003F0 RID: 1008
		[Dependency]
		private readonly IMapManager _map;

		// Token: 0x040003F1 RID: 1009
		public const string ContainerName = "entity_storage";
	}
}
