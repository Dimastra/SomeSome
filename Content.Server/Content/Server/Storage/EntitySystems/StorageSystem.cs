using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.DoAfter;
using Content.Server.Hands.Components;
using Content.Server.Interaction;
using Content.Server.Popups;
using Content.Server.Storage.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.CombatMode;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Lock;
using Content.Shared.Movement.Events;
using Content.Shared.Physics;
using Content.Shared.Placeable;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x02000166 RID: 358
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StorageSystem : EntitySystem
	{
		// Token: 0x060006EE RID: 1774 RVA: 0x000224B4 File Offset: 0x000206B4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ServerStorageComponent, ComponentInit>(new ComponentEventHandler<ServerStorageComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<ServerStorageComponent, GetVerbsEvent<ActivationVerb>>(this.AddOpenUiVerb), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<ServerStorageComponent, GetVerbsEvent<UtilityVerb>>(this.AddTransferVerbs), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, InteractUsingEvent>(new ComponentEventHandler<ServerStorageComponent, InteractUsingEvent>(this.OnInteractUsing), null, new Type[]
			{
				typeof(ItemSlotsSystem)
			});
			base.SubscribeLocalEvent<ServerStorageComponent, ActivateInWorldEvent>(new ComponentEventHandler<ServerStorageComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, OpenStorageImplantEvent>(new ComponentEventHandler<ServerStorageComponent, OpenStorageImplantEvent>(this.OnImplantActivate), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, AfterInteractEvent>(new ComponentEventHandler<ServerStorageComponent, AfterInteractEvent>(this.AfterInteract), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, DestructionEventArgs>(new ComponentEventHandler<ServerStorageComponent, DestructionEventArgs>(this.OnDestroy), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, SharedStorageComponent.StorageInteractWithItemEvent>(new ComponentEventHandler<ServerStorageComponent, SharedStorageComponent.StorageInteractWithItemEvent>(this.OnInteractWithItem), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, SharedStorageComponent.StorageInsertItemMessage>(new ComponentEventHandler<ServerStorageComponent, SharedStorageComponent.StorageInsertItemMessage>(this.OnInsertItemMessage), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, BoundUIOpenedEvent>(new ComponentEventHandler<ServerStorageComponent, BoundUIOpenedEvent>(this.OnBoundUIOpen), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, BoundUIClosedEvent>(new ComponentEventHandler<ServerStorageComponent, BoundUIClosedEvent>(this.OnBoundUIClosed), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ServerStorageComponent, EntRemovedFromContainerMessage>(this.OnStorageItemRemoved), null, null);
			base.SubscribeLocalEvent<ServerStorageComponent, DoAfterEvent<StorageSystem.StorageData>>(new ComponentEventHandler<ServerStorageComponent, DoAfterEvent<StorageSystem.StorageData>>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<EntityStorageComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<EntityStorageComponent, GetVerbsEvent<InteractionVerb>>(this.AddToggleOpenVerb), null, null);
			base.SubscribeLocalEvent<EntityStorageComponent, ContainerRelayMovementEntityEvent>(new ComponentEventRefHandler<EntityStorageComponent, ContainerRelayMovementEntityEvent>(this.OnRelayMovement), null, null);
			base.SubscribeLocalEvent<StorageFillComponent, MapInitEvent>(new ComponentEventHandler<StorageFillComponent, MapInitEvent>(this.OnStorageFillMapInit), null, null);
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x00022630 File Offset: 0x00020830
		private void OnComponentInit(EntityUid uid, ServerStorageComponent storageComp, ComponentInit args)
		{
			base.Initialize();
			storageComp.Storage = this._containerSystem.EnsureContainer<Container>(uid, "storagebase", null);
			storageComp.Storage.OccludesLight = storageComp.OccludesLight;
			this.UpdateStorageVisualization(uid, storageComp);
			this.RecalculateStorageUsed(storageComp);
			this.UpdateStorageUI(uid, storageComp);
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x00022684 File Offset: 0x00020884
		private void OnRelayMovement(EntityUid uid, EntityStorageComponent component, ref ContainerRelayMovementEntityEvent args)
		{
			if (!this.EntityManager.HasComponent<HandsComponent>(args.Entity) || this._gameTiming.CurTime < component.LastInternalOpenAttempt + EntityStorageComponent.InternalOpenAttemptDelay)
			{
				return;
			}
			component.LastInternalOpenAttempt = this._gameTiming.CurTime;
			if (component.OpenOnMove)
			{
				this._entityStorage.TryOpenStorage(args.Entity, component.Owner, false);
			}
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x000226FC File Offset: 0x000208FC
		private void AddToggleOpenVerb(EntityUid uid, EntityStorageComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || !this._entityStorage.CanOpen(args.User, args.Target, true, component))
			{
				return;
			}
			InteractionVerb verb = new InteractionVerb();
			if (component.Open)
			{
				verb.Text = Loc.GetString("verb-common-close");
				verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/close.svg.192dpi.png", "/"));
			}
			else
			{
				verb.Text = Loc.GetString("verb-common-open");
				verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/open.svg.192dpi.png", "/"));
			}
			verb.Act = delegate()
			{
				this._entityStorage.ToggleOpen(args.User, args.Target, component);
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x000227F8 File Offset: 0x000209F8
		private void AddOpenUiVerb(EntityUid uid, ServerStorageComponent component, GetVerbsEvent<ActivationVerb> args)
		{
			LockComponent lockComponent;
			if (!args.CanAccess || !args.CanInteract || (base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked))
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			bool flag = this._uiSystem.SessionHasOpenUi(uid, SharedStorageComponent.StorageUiKey.Key, actor.PlayerSession, null);
			ActivationVerb verb = new ActivationVerb
			{
				Act = delegate()
				{
					this.OpenStorageUI(uid, args.User, component);
				}
			};
			if (flag)
			{
				verb.Text = Loc.GetString("verb-common-close-ui");
				verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/close.svg.192dpi.png", "/"));
			}
			else
			{
				verb.Text = Loc.GetString("verb-common-open-ui");
				verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/open.svg.192dpi.png", "/"));
			}
			args.Verbs.Add(verb);
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x00022910 File Offset: 0x00020B10
		private void AddTransferVerbs(EntityUid uid, ServerStorageComponent component, GetVerbsEvent<UtilityVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			Container storage = component.Storage;
			IReadOnlyList<EntityUid> entities = (storage != null) ? storage.ContainedEntities : null;
			LockComponent lockComponent;
			if (entities == null || entities.Count == 0 || (base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked))
			{
				return;
			}
			ServerStorageComponent targetStorage;
			LockComponent targetLock;
			if (base.TryComp<ServerStorageComponent>(args.Target, ref targetStorage) && (!base.TryComp<LockComponent>(uid, ref targetLock) || !targetLock.Locked))
			{
				UtilityVerb verb = new UtilityVerb
				{
					Text = Loc.GetString("storage-component-transfer-verb"),
					IconEntity = args.Using,
					Act = delegate()
					{
						this.TransferEntities(uid, args.Target, component, lockComponent, targetStorage, targetLock);
					}
				};
				args.Verbs.Add(verb);
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00022A24 File Offset: 0x00020C24
		private void OnInteractUsing(EntityUid uid, ServerStorageComponent storageComp, InteractUsingEvent args)
		{
			LockComponent lockComponent;
			if (args.Handled || !storageComp.ClickInsert || (base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked))
			{
				return;
			}
			string loggerName = storageComp.LoggerName;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Storage (UID ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
			defaultInterpolatedStringHandler.AppendLiteral(") attacked by user (UID ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(args.User);
			defaultInterpolatedStringHandler.AppendLiteral(") with entity (UID ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(args.Used);
			defaultInterpolatedStringHandler.AppendLiteral(").");
			Logger.DebugS(loggerName, defaultInterpolatedStringHandler.ToStringAndClear());
			if (base.HasComp<PlaceableSurfaceComponent>(uid))
			{
				return;
			}
			if (this.PlayerInsertHeldEntity(uid, args.User, storageComp))
			{
				args.Handled = true;
			}
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x00022AE4 File Offset: 0x00020CE4
		private void OnActivate(EntityUid uid, ServerStorageComponent storageComp, ActivateInWorldEvent args)
		{
			LockComponent lockComponent;
			if (args.Handled || this._combatMode.IsInCombatMode(new EntityUid?(args.User), null) || (base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked))
			{
				return;
			}
			this.OpenStorageUI(uid, args.User, storageComp);
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x00022B34 File Offset: 0x00020D34
		private void OnImplantActivate(EntityUid uid, ServerStorageComponent storageComp, OpenStorageImplantEvent args)
		{
			TransformComponent xform;
			if (args.Handled || !base.TryComp<TransformComponent>(uid, ref xform))
			{
				return;
			}
			this.OpenStorageUI(uid, xform.ParentUid, storageComp);
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x00022B64 File Offset: 0x00020D64
		private void AfterInteract(EntityUid uid, ServerStorageComponent storageComp, AfterInteractEvent args)
		{
			StorageSystem.<AfterInteract>d__25 <AfterInteract>d__;
			<AfterInteract>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AfterInteract>d__.<>4__this = this;
			<AfterInteract>d__.uid = uid;
			<AfterInteract>d__.storageComp = storageComp;
			<AfterInteract>d__.args = args;
			<AfterInteract>d__.<>1__state = -1;
			<AfterInteract>d__.<>t__builder.Start<StorageSystem.<AfterInteract>d__25>(ref <AfterInteract>d__);
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00022BB4 File Offset: 0x00020DB4
		private void OnDoAfter(EntityUid uid, ServerStorageComponent component, DoAfterEvent<StorageSystem.StorageData> args)
		{
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			List<EntityUid> successfullyInserted = new List<EntityUid>();
			List<EntityCoordinates> successfullyInsertedPositions = new List<EntityCoordinates>();
			EntityQuery<ItemComponent> itemQuery = base.GetEntityQuery<ItemComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			TransformComponent xform;
			xformQuery.TryGetComponent(uid, ref xform);
			foreach (EntityUid entity in args.AdditionalData.ValidStorables)
			{
				TransformComponent targetXform;
				if (!this._containerSystem.IsEntityInContainer(entity, null) && !(entity == args.Args.User) && itemQuery.HasComponent(entity) && xform != null && xformQuery.TryGetComponent(entity, ref targetXform) && !(targetXform.MapID != xform.MapID))
				{
					EntityCoordinates position = EntityCoordinates.FromMap(xform.ParentUid.IsValid() ? xform.ParentUid : uid, new MapCoordinates(this._transform.GetWorldPosition(targetXform, xformQuery), targetXform.MapID), this.EntityManager);
					if (this.PlayerInsertEntityInWorld(uid, args.Args.User, entity, component))
					{
						successfullyInserted.Add(entity);
						successfullyInsertedPositions.Add(position);
					}
				}
			}
			if (successfullyInserted.Count > 0)
			{
				this._audio.PlayPvs(component.StorageInsertSound, uid, null);
				base.RaiseNetworkEvent(new AnimateInsertingEntitiesEvent(uid, successfullyInserted, successfullyInsertedPositions));
			}
			args.Handled = true;
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00022D4C File Offset: 0x00020F4C
		private void OnDestroy(EntityUid uid, ServerStorageComponent storageComp, DestructionEventArgs args)
		{
			IReadOnlyList<EntityUid> storedEntities2 = storageComp.StoredEntities;
			List<EntityUid> storedEntities = (storedEntities2 != null) ? storedEntities2.ToList<EntityUid>() : null;
			if (storedEntities == null)
			{
				return;
			}
			foreach (EntityUid entity in storedEntities)
			{
				this.RemoveAndDrop(uid, entity, storageComp);
			}
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00022DB4 File Offset: 0x00020FB4
		private void OnInteractWithItem(EntityUid uid, ServerStorageComponent storageComp, SharedStorageComponent.StorageInteractWithItemEvent args)
		{
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity == null)
			{
				return;
			}
			EntityUid player = attachedEntity.GetValueOrDefault();
			if (!base.Exists(args.InteractedItemUID))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Player ");
				defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(args.Session);
				defaultInterpolatedStringHandler.AppendLiteral(" interacted with non-existent item ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(args.InteractedItemUID);
				defaultInterpolatedStringHandler.AppendLiteral(" stored in ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!this._actionBlockerSystem.CanInteract(player, new EntityUid?(args.InteractedItemUID)) || storageComp.Storage == null || !storageComp.Storage.Contains(args.InteractedItemUID))
			{
				return;
			}
			HandsComponent hands;
			if (!base.TryComp<HandsComponent>(player, ref hands) || hands.Count == 0)
			{
				return;
			}
			if (hands.ActiveHandEntity == null)
			{
				if (this._sharedHandsSystem.TryPickupAnyHand(player, args.InteractedItemUID, true, false, hands, null) && storageComp.StorageRemoveSound != null)
				{
					this._audio.Play(storageComp.StorageRemoveSound, Filter.Pvs(uid, 2f, this.EntityManager, null, null), uid, true, new AudioParams?(AudioParams.Default));
				}
				return;
			}
			this._interactionSystem.InteractUsing(player, hands.ActiveHandEntity.Value, args.InteractedItemUID, base.Transform(args.InteractedItemUID).Coordinates, false, true);
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x00022F30 File Offset: 0x00021130
		private void OnInsertItemMessage(EntityUid uid, ServerStorageComponent storageComp, SharedStorageComponent.StorageInsertItemMessage args)
		{
			if (args.Session.AttachedEntity == null)
			{
				return;
			}
			this.PlayerInsertHeldEntity(uid, args.Session.AttachedEntity.Value, storageComp);
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x00022F6F File Offset: 0x0002116F
		private void OnBoundUIOpen(EntityUid uid, ServerStorageComponent storageComp, BoundUIOpenedEvent args)
		{
			if (!storageComp.IsOpen)
			{
				storageComp.IsOpen = true;
				this.UpdateStorageVisualization(uid, storageComp);
			}
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x00022F88 File Offset: 0x00021188
		private void OnBoundUIClosed(EntityUid uid, ServerStorageComponent storageComp, BoundUIClosedEvent args)
		{
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(args.Session.AttachedEntity, ref actor) && ((actor != null) ? actor.PlayerSession : null) != null)
			{
				this.CloseNestedInterfaces(uid, actor.PlayerSession, storageComp);
			}
			if (!this._uiSystem.IsUiOpen(uid, args.UiKey, null))
			{
				storageComp.IsOpen = false;
				this.UpdateStorageVisualization(uid, storageComp);
				if (storageComp.StorageCloseSound != null)
				{
					this._audio.Play(storageComp.StorageCloseSound, Filter.Pvs(uid, 2f, this.EntityManager, null, null), uid, true, new AudioParams?(storageComp.StorageCloseSound.Params));
				}
			}
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00023029 File Offset: 0x00021229
		private void OnStorageItemRemoved(EntityUid uid, ServerStorageComponent storageComp, EntRemovedFromContainerMessage args)
		{
			this.RecalculateStorageUsed(storageComp);
			this.UpdateStorageUI(uid, storageComp);
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0002303C File Offset: 0x0002123C
		private void UpdateStorageVisualization(EntityUid uid, ServerStorageComponent storageComp)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, StorageVisuals.Open, storageComp.IsOpen, appearance);
			this._appearance.SetData(uid, SharedBagOpenVisuals.BagState, storageComp.IsOpen ? SharedBagState.Open : SharedBagState.Closed, null);
			if (base.HasComp<ItemCounterComponent>(uid))
			{
				this._appearance.SetData(uid, StackVisuals.Hide, !storageComp.IsOpen, null);
			}
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x000230C4 File Offset: 0x000212C4
		private void RecalculateStorageUsed(ServerStorageComponent storageComp)
		{
			storageComp.StorageUsed = 0;
			storageComp.SizeCache.Clear();
			if (storageComp.Storage == null)
			{
				return;
			}
			EntityQuery<ItemComponent> itemQuery = base.GetEntityQuery<ItemComponent>();
			foreach (EntityUid entity in storageComp.Storage.ContainedEntities)
			{
				ItemComponent itemComp;
				if (itemQuery.TryGetComponent(entity, ref itemComp))
				{
					storageComp.StorageUsed += itemComp.Size;
					storageComp.SizeCache.Add(entity, itemComp.Size);
				}
			}
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x00023164 File Offset: 0x00021364
		[NullableContext(2)]
		public void TransferEntities(EntityUid source, EntityUid target, ServerStorageComponent sourceComp = null, LockComponent sourceLock = null, ServerStorageComponent targetComp = null, LockComponent targetLock = null)
		{
			if (!base.Resolve<ServerStorageComponent>(source, ref sourceComp, true) || !base.Resolve<ServerStorageComponent>(target, ref targetComp, true))
			{
				return;
			}
			Container storage = sourceComp.Storage;
			IReadOnlyList<EntityUid> entities = (storage != null) ? storage.ContainedEntities : null;
			if (entities == null || entities.Count == 0)
			{
				return;
			}
			if ((base.Resolve<LockComponent>(source, ref sourceLock, false) && sourceLock.Locked) || (base.Resolve<LockComponent>(target, ref targetLock, false) && targetLock.Locked))
			{
				return;
			}
			foreach (EntityUid entity in entities.ToList<EntityUid>())
			{
				this.Insert(target, entity, targetComp, true);
			}
			this.RecalculateStorageUsed(sourceComp);
			this.UpdateStorageUI(source, sourceComp);
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x00023230 File Offset: 0x00021430
		[NullableContext(2)]
		public bool CanInsert(EntityUid uid, EntityUid insertEnt, out string reason, ServerStorageComponent storageComp = null)
		{
			if (!base.Resolve<ServerStorageComponent>(uid, ref storageComp, true))
			{
				reason = null;
				return false;
			}
			TransformComponent transformComp;
			if (base.TryComp<TransformComponent>(insertEnt, ref transformComp) && transformComp.Anchored)
			{
				reason = "comp-storage-anchored-failure";
				return false;
			}
			EntityWhitelist whitelist = storageComp.Whitelist;
			if (whitelist != null && !whitelist.IsValid(insertEnt, this.EntityManager))
			{
				reason = "comp-storage-invalid-container";
				return false;
			}
			EntityWhitelist blacklist = storageComp.Blacklist;
			if (blacklist != null && blacklist.IsValid(insertEnt, this.EntityManager))
			{
				reason = "comp-storage-invalid-container";
				return false;
			}
			ServerStorageComponent storage;
			if (base.TryComp<ServerStorageComponent>(insertEnt, ref storage) && storage.StorageCapacityMax >= storageComp.StorageCapacityMax)
			{
				reason = "comp-storage-insufficient-capacity";
				return false;
			}
			ItemComponent itemComp;
			if (base.TryComp<ItemComponent>(insertEnt, ref itemComp) && itemComp.Size > storageComp.StorageCapacityMax - storageComp.StorageUsed)
			{
				reason = "comp-storage-insufficient-capacity";
				return false;
			}
			reason = null;
			return true;
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x0002330C File Offset: 0x0002150C
		[NullableContext(2)]
		public bool Insert(EntityUid uid, EntityUid insertEnt, ServerStorageComponent storageComp = null, bool playSound = true)
		{
			string text;
			if (base.Resolve<ServerStorageComponent>(uid, ref storageComp, true) && this.CanInsert(uid, insertEnt, out text, storageComp))
			{
				Container storage = storageComp.Storage;
				if (storage == null || storage.Insert(insertEnt, null, null, null, null, null))
				{
					if (playSound && storageComp.StorageInsertSound != null)
					{
						this._audio.PlayPvs(storageComp.StorageInsertSound, uid, null);
					}
					this.RecalculateStorageUsed(storageComp);
					this.UpdateStorageUI(uid, storageComp);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x0002338C File Offset: 0x0002158C
		[NullableContext(2)]
		public bool RemoveAndDrop(EntityUid uid, EntityUid removeEnt, ServerStorageComponent storageComp = null)
		{
			if (!base.Resolve<ServerStorageComponent>(uid, ref storageComp, true))
			{
				return false;
			}
			Container storage = storageComp.Storage;
			bool flag = storage != null && storage.Remove(removeEnt, null, null, null, true, false, null, null);
			if (flag)
			{
				this.RecalculateStorageUsed(storageComp);
			}
			return flag;
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x000233DC File Offset: 0x000215DC
		[NullableContext(2)]
		public bool PlayerInsertHeldEntity(EntityUid uid, EntityUid player, ServerStorageComponent storageComp = null)
		{
			HandsComponent hands;
			if (!base.Resolve<ServerStorageComponent>(uid, ref storageComp, true) || !base.TryComp<HandsComponent>(player, ref hands) || hands.ActiveHandEntity == null)
			{
				return false;
			}
			EntityUid? toInsert = hands.ActiveHandEntity;
			string reason;
			if (this.CanInsert(uid, toInsert.Value, out reason, storageComp))
			{
				SharedHandsSystem sharedHandsSystem = this._sharedHandsSystem;
				EntityUid value = toInsert.Value;
				SharedHandsComponent handsComp = hands;
				if (sharedHandsSystem.TryDrop(player, value, null, true, true, handsComp))
				{
					return this.PlayerInsertEntityInWorld(uid, player, toInsert.Value, storageComp);
				}
			}
			this.Popup(uid, player, reason ?? "comp-storage-cant-insert", storageComp);
			return false;
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00023478 File Offset: 0x00021678
		[NullableContext(2)]
		public bool PlayerInsertEntityInWorld(EntityUid uid, EntityUid player, EntityUid toInsert, ServerStorageComponent storageComp = null)
		{
			if (!base.Resolve<ServerStorageComponent>(uid, ref storageComp, true) || !this._sharedInteractionSystem.InRangeUnobstructed(player, uid, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, storageComp.ShowPopup))
			{
				return false;
			}
			if (!this.Insert(uid, toInsert, storageComp, true))
			{
				this.Popup(uid, player, "comp-storage-cant-insert", storageComp);
				return false;
			}
			return true;
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x000234D4 File Offset: 0x000216D4
		[NullableContext(2)]
		public void OpenStorageUI(EntityUid uid, EntityUid entity, ServerStorageComponent storageComp = null)
		{
			ActorComponent player;
			if (!base.Resolve<ServerStorageComponent>(uid, ref storageComp, true) || !base.TryComp<ActorComponent>(entity, ref player))
			{
				return;
			}
			if (storageComp.StorageOpenSound != null)
			{
				this._audio.Play(storageComp.StorageOpenSound, Filter.Pvs(uid, 2f, this.EntityManager, null, null), uid, true, new AudioParams?(storageComp.StorageOpenSound.Params));
			}
			string loggerName = storageComp.LoggerName;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Storage (UID ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
			defaultInterpolatedStringHandler.AppendLiteral(") \"used\" by player session (UID ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(player.PlayerSession.AttachedEntity);
			defaultInterpolatedStringHandler.AppendLiteral(").");
			Logger.DebugS(loggerName, defaultInterpolatedStringHandler.ToStringAndClear());
			BoundUserInterface bui = this._uiSystem.GetUiOrNull(uid, SharedStorageComponent.StorageUiKey.Key, null);
			if (bui != null)
			{
				this._uiSystem.OpenUi(bui, player.PlayerSession);
			}
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x000235BC File Offset: 0x000217BC
		public void CloseNestedInterfaces(EntityUid uid, IPlayerSession session, [Nullable(2)] ServerStorageComponent storageComp = null)
		{
			if (!base.Resolve<ServerStorageComponent>(uid, ref storageComp, true) || storageComp.StoredEntities == null)
			{
				return;
			}
			foreach (EntityUid entity in storageComp.StoredEntities)
			{
				ServerStorageComponent storedStorageComp;
				base.TryComp<ServerStorageComponent>(entity, ref storedStorageComp);
				ServerUserInterfaceComponent ui;
				if (base.TryComp<ServerUserInterfaceComponent>(entity, ref ui))
				{
					foreach (BoundUserInterface bui in ui.Interfaces.Values)
					{
						this._uiSystem.TryClose(entity, bui.UiKey, session, ui);
					}
				}
			}
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00023684 File Offset: 0x00021884
		private void UpdateStorageUI(EntityUid uid, ServerStorageComponent storageComp)
		{
			if (storageComp.Storage == null)
			{
				return;
			}
			SharedStorageComponent.StorageBoundUserInterfaceState state = new SharedStorageComponent.StorageBoundUserInterfaceState((List<EntityUid>)storageComp.Storage.ContainedEntities, storageComp.StorageUsed, storageComp.StorageCapacityMax);
			BoundUserInterface bui = this._uiSystem.GetUiOrNull(uid, SharedStorageComponent.StorageUiKey.Key, null);
			if (bui != null)
			{
				this._uiSystem.SetUiState(bui, state, null, true);
			}
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x000236E2 File Offset: 0x000218E2
		private void Popup(EntityUid uid, EntityUid player, string message, ServerStorageComponent storageComp)
		{
			if (!storageComp.ShowPopup)
			{
				return;
			}
			this._popupSystem.PopupEntity(Loc.GetString(message), player, player, PopupType.Small);
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x00023704 File Offset: 0x00021904
		private void OnStorageFillMapInit(EntityUid uid, StorageFillComponent component, MapInitEvent args)
		{
			if (component.Contents.Count == 0)
			{
				return;
			}
			ServerStorageComponent serverStorageComp;
			base.TryComp<ServerStorageComponent>(uid, ref serverStorageComp);
			EntityStorageComponent entityStorageComp;
			base.TryComp<EntityStorageComponent>(uid, ref entityStorageComp);
			if (entityStorageComp == null && serverStorageComp == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 1);
				defaultInterpolatedStringHandler.AppendLiteral("StorageFillComponent couldn't find any StorageComponent (");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			EntityCoordinates coordinates = base.Transform(uid).Coordinates;
			foreach (string item in EntitySpawnCollection.GetSpawns(component.Contents, this._random))
			{
				EntityUid ent = this.EntityManager.SpawnEntity(item, coordinates);
				if ((entityStorageComp == null || !this._entityStorage.Insert(ent, uid, null)) && (serverStorageComp == null || !this.Insert(uid, ent, serverStorageComp, false)))
				{
					string text = "storage";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Tried to StorageFill ");
					defaultInterpolatedStringHandler.AppendFormatted(item);
					defaultInterpolatedStringHandler.AppendLiteral(" inside ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					defaultInterpolatedStringHandler.AppendLiteral(" but can't.");
					Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
					this.EntityManager.DeleteEntity(ent);
				}
			}
		}

		// Token: 0x040003F9 RID: 1017
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040003FA RID: 1018
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040003FB RID: 1019
		[Dependency]
		private readonly ContainerSystem _containerSystem;

		// Token: 0x040003FC RID: 1020
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x040003FD RID: 1021
		[Dependency]
		private readonly EntityLookupSystem _entityLookupSystem;

		// Token: 0x040003FE RID: 1022
		[Dependency]
		private readonly EntityStorageSystem _entityStorage;

		// Token: 0x040003FF RID: 1023
		[Dependency]
		private readonly InteractionSystem _interactionSystem;

		// Token: 0x04000400 RID: 1024
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000401 RID: 1025
		[Dependency]
		private readonly SharedHandsSystem _sharedHandsSystem;

		// Token: 0x04000402 RID: 1026
		[Dependency]
		private readonly SharedInteractionSystem _sharedInteractionSystem;

		// Token: 0x04000403 RID: 1027
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04000404 RID: 1028
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04000405 RID: 1029
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000406 RID: 1030
		[Dependency]
		private readonly SharedCombatModeSystem _combatMode;

		// Token: 0x04000407 RID: 1031
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000408 RID: 1032
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x020008F3 RID: 2291
		[Nullable(0)]
		private struct StorageData : IEquatable<StorageSystem.StorageData>
		{
			// Token: 0x060030CF RID: 12495 RVA: 0x000FC558 File Offset: 0x000FA758
			public StorageData(List<EntityUid> validStorables)
			{
				this.validStorables = validStorables;
				this.ValidStorables = validStorables;
			}

			// Token: 0x170007FD RID: 2045
			// (get) Token: 0x060030D0 RID: 12496 RVA: 0x000FC568 File Offset: 0x000FA768
			// (set) Token: 0x060030D1 RID: 12497 RVA: 0x000FC570 File Offset: 0x000FA770
			public List<EntityUid> validStorables { readonly get; set; }

			// Token: 0x060030D2 RID: 12498 RVA: 0x000FC57C File Offset: 0x000FA77C
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("StorageData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060030D3 RID: 12499 RVA: 0x000FC5C8 File Offset: 0x000FA7C8
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("validStorables = ");
				builder.Append(this.validStorables);
				builder.Append(", ValidStorables = ");
				builder.Append(this.ValidStorables);
				return true;
			}

			// Token: 0x060030D4 RID: 12500 RVA: 0x000FC5FD File Offset: 0x000FA7FD
			[CompilerGenerated]
			public static bool operator !=(StorageSystem.StorageData left, StorageSystem.StorageData right)
			{
				return !(left == right);
			}

			// Token: 0x060030D5 RID: 12501 RVA: 0x000FC609 File Offset: 0x000FA809
			[CompilerGenerated]
			public static bool operator ==(StorageSystem.StorageData left, StorageSystem.StorageData right)
			{
				return left.Equals(right);
			}

			// Token: 0x060030D6 RID: 12502 RVA: 0x000FC613 File Offset: 0x000FA813
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return EqualityComparer<List<EntityUid>>.Default.GetHashCode(this.<validStorables>k__BackingField) * -1521134295 + EqualityComparer<List<EntityUid>>.Default.GetHashCode(this.ValidStorables);
			}

			// Token: 0x060030D7 RID: 12503 RVA: 0x000FC63C File Offset: 0x000FA83C
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is StorageSystem.StorageData && this.Equals((StorageSystem.StorageData)obj);
			}

			// Token: 0x060030D8 RID: 12504 RVA: 0x000FC654 File Offset: 0x000FA854
			[CompilerGenerated]
			public readonly bool Equals(StorageSystem.StorageData other)
			{
				return EqualityComparer<List<EntityUid>>.Default.Equals(this.<validStorables>k__BackingField, other.<validStorables>k__BackingField) && EqualityComparer<List<EntityUid>>.Default.Equals(this.ValidStorables, other.ValidStorables);
			}

			// Token: 0x060030D9 RID: 12505 RVA: 0x000FC686 File Offset: 0x000FA886
			[CompilerGenerated]
			public readonly void Deconstruct(out List<EntityUid> validStorables)
			{
				validStorables = this.validStorables;
			}

			// Token: 0x04001E47 RID: 7751
			public List<EntityUid> ValidStorables;
		}
	}
}
