using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Hands.Components;
using Content.Server.Popups;
using Content.Server.Pulling;
using Content.Server.Stack;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Server.Stunnable;
using Content.Shared.ActionBlocker;
using Content.Shared.Body.Part;
using Content.Shared.CombatMode;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Physics.Pull;
using Content.Shared.Popups;
using Content.Shared.Pulling.Components;
using Content.Shared.Stacks;
using Content.Shared.Throwing;
using Robust.Server.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Players;

namespace Content.Server.Hands.Systems
{
	// Token: 0x02000478 RID: 1144
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class HandsSystem : SharedHandsSystem
	{
		// Token: 0x060016D8 RID: 5848 RVA: 0x00078528 File Offset: 0x00076728
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HandsComponent, DisarmedEvent>(new ComponentEventHandler<HandsComponent, DisarmedEvent>(this.OnDisarmed), new Type[]
			{
				typeof(StunSystem)
			}, null);
			base.SubscribeLocalEvent<HandsComponent, PullStartedMessage>(new ComponentEventHandler<HandsComponent, PullStartedMessage>(this.HandlePullStarted), null, null);
			base.SubscribeLocalEvent<HandsComponent, PullStoppedMessage>(new ComponentEventHandler<HandsComponent, PullStoppedMessage>(this.HandlePullStopped), null, null);
			base.SubscribeLocalEvent<HandsComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<HandsComponent, EntRemovedFromContainerMessage>(this.HandleEntityRemoved), null, null);
			base.SubscribeLocalEvent<HandsComponent, BodyPartAddedEvent>(new ComponentEventRefHandler<HandsComponent, BodyPartAddedEvent>(this.HandleBodyPartAdded), null, null);
			base.SubscribeLocalEvent<HandsComponent, BodyPartRemovedEvent>(new ComponentEventRefHandler<HandsComponent, BodyPartRemovedEvent>(this.HandleBodyPartRemoved), null, null);
			base.SubscribeLocalEvent<HandsComponent, ComponentGetState>(new ComponentEventRefHandler<HandsComponent, ComponentGetState>(this.GetComponentState), null, null);
			CommandBinds.Builder.Bind(ContentKeyFunctions.ThrowItemInHand, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleThrowItem), true, false)).Bind(ContentKeyFunctions.SmartEquipBackpack, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleSmartEquipBackpack), null, true, true)).Bind(ContentKeyFunctions.SmartEquipBelt, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleSmartEquipBelt), null, true, true)).Register<HandsSystem>();
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x0007863D File Offset: 0x0007683D
		public override void Shutdown()
		{
			base.Shutdown();
			CommandBinds.Unregister<HandsSystem>();
		}

		// Token: 0x060016DA RID: 5850 RVA: 0x0007864A File Offset: 0x0007684A
		private void GetComponentState(EntityUid uid, HandsComponent hands, ref ComponentGetState args)
		{
			args.State = new HandsComponentState(hands);
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x00078658 File Offset: 0x00076858
		private void OnDisarmed(EntityUid uid, HandsComponent component, DisarmedEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			SharedPullerComponent puller;
			if (base.TryComp<SharedPullerComponent>(uid, ref puller))
			{
				EntityUid? pulling = puller.Pulling;
				if (pulling != null)
				{
					EntityUid pulled = pulling.GetValueOrDefault();
					SharedPullableComponent pullable;
					if (base.TryComp<SharedPullableComponent>(pulled, ref pullable))
					{
						this._pullingSystem.TryStopPull(pullable, null);
					}
				}
			}
			if (!this._handsSystem.TryDrop(uid, component.ActiveHand, null, false, true, null))
			{
				return;
			}
			args.Handled = true;
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x000786DC File Offset: 0x000768DC
		public override void PickupAnimation(EntityUid item, EntityCoordinates initialPosition, Vector2 finalPosition, EntityUid? exclude)
		{
			if (finalPosition.EqualsApprox(initialPosition.Position, 0.10000000149011612))
			{
				return;
			}
			Filter filter = Filter.Pvs(item, 2f, null, null, null);
			if (exclude != null)
			{
				filter = filter.RemoveWhereAttachedEntity((EntityUid entity) => entity == exclude);
			}
			base.RaiseNetworkEvent(new PickupAnimationEvent(item, initialPosition, finalPosition), filter, true);
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x00078750 File Offset: 0x00076950
		private void HandleEntityRemoved(EntityUid uid, SharedHandsComponent component, EntRemovedFromContainerMessage args)
		{
			HandVirtualItemComponent @virtual;
			if (!base.Deleted(args.Entity, null) && base.TryComp<HandVirtualItemComponent>(args.Entity, ref @virtual))
			{
				this._virtualSystem.Delete(@virtual, uid);
			}
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x0007878C File Offset: 0x0007698C
		private void HandleBodyPartAdded(EntityUid uid, HandsComponent component, ref BodyPartAddedEvent args)
		{
			if (args.Part.PartType != BodyPartType.Hand)
			{
				return;
			}
			HandLocation handLocation;
			switch (args.Part.Symmetry)
			{
			case BodyPartSymmetry.None:
				handLocation = HandLocation.Middle;
				break;
			case BodyPartSymmetry.Left:
				handLocation = HandLocation.Left;
				break;
			case BodyPartSymmetry.Right:
				handLocation = HandLocation.Right;
				break;
			default:
				throw new ArgumentOutOfRangeException("Symmetry");
			}
			HandLocation location = handLocation;
			this.AddHand(uid, args.Slot, location, null);
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x000787F0 File Offset: 0x000769F0
		private void HandleBodyPartRemoved(EntityUid uid, HandsComponent component, ref BodyPartRemovedEvent args)
		{
			if (args.Part.PartType != BodyPartType.Hand)
			{
				return;
			}
			this.RemoveHand(uid, args.Slot, null);
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x00078810 File Offset: 0x00076A10
		private void HandlePullStarted(EntityUid uid, HandsComponent component, PullStartedMessage args)
		{
			if (args.Puller.Owner != uid)
			{
				return;
			}
			SharedPullerComponent pullerComp;
			if (base.TryComp<SharedPullerComponent>(args.Puller.Owner, ref pullerComp) && !pullerComp.NeedsHands)
			{
				return;
			}
			this._virtualItemSystem.TrySpawnVirtualItemInHand(args.Pulled.Owner, uid);
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x00078868 File Offset: 0x00076A68
		private void HandlePullStopped(EntityUid uid, HandsComponent component, PullStoppedMessage args)
		{
			if (args.Puller.Owner != uid)
			{
				return;
			}
			foreach (Hand hand in component.Hands.Values)
			{
				HandVirtualItemComponent virtualItem;
				if (hand.HeldEntity != null && base.TryComp<HandVirtualItemComponent>(hand.HeldEntity, ref virtualItem) && !(virtualItem.BlockingEntity != args.Pulled.Owner))
				{
					base.QueueDel(hand.HeldEntity.Value);
					break;
				}
			}
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x0007891C File Offset: 0x00076B1C
		[NullableContext(2)]
		private bool HandleThrowItem(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			IPlayerSession playerSession = session as IPlayerSession;
			if (playerSession == null)
			{
				return false;
			}
			EntityUid? entityUid = playerSession.AttachedEntity;
			if (entityUid != null)
			{
				EntityUid player = entityUid.GetValueOrDefault();
				SharedHandsComponent hands;
				if (player.Valid && base.Exists(player) && !ContainerHelpers.IsInContainer(player, null) && base.TryComp<SharedHandsComponent>(player, ref hands))
				{
					entityUid = hands.ActiveHandEntity;
					if (entityUid != null)
					{
						EntityUid throwEnt = entityUid.GetValueOrDefault();
						if (this._actionBlockerSystem.CanThrow(player, throwEnt))
						{
							StackComponent stack;
							if (this.EntityManager.TryGetComponent<StackComponent>(throwEnt, ref stack) && stack.Count > 1 && stack.ThrowIndividually)
							{
								EntityUid? splitStack = this._stackSystem.Split(throwEnt, 1, this.EntityManager.GetComponent<TransformComponent>(player).Coordinates, stack);
								if (splitStack == null || !splitStack.GetValueOrDefault().Valid)
								{
									return false;
								}
								throwEnt = splitStack.Value;
							}
							Vector2 direction = coords.ToMapPos(this.EntityManager) - base.Transform(player).WorldPosition;
							if (direction == Vector2.Zero)
							{
								return true;
							}
							direction = direction.Normalized * Math.Min(direction.Length, hands.ThrowRange);
							float throwStrength = hands.ThrowForceMultiplier;
							BeforeThrowEvent ev = new BeforeThrowEvent(throwEnt, direction, throwStrength, player);
							base.RaiseLocalEvent<BeforeThrowEvent>(player, ev, false);
							if (ev.Handled)
							{
								return true;
							}
							EntityUid uid2 = player;
							EntityUid entity = throwEnt;
							SharedHandsComponent handsComp = hands;
							if (!base.TryDrop(uid2, entity, null, true, true, handsComp))
							{
								return false;
							}
							this._throwingSystem.TryThrow(ev.ItemUid, ev.Direction, ev.ThrowStrength, new EntityUid?(ev.PlayerUid), 5f, null, null, null, null);
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x00078AE7 File Offset: 0x00076CE7
		[NullableContext(2)]
		private void HandleSmartEquipBackpack(ICommonSession session)
		{
			this.HandleSmartEquip(session, "back");
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x00078AF5 File Offset: 0x00076CF5
		[NullableContext(2)]
		private void HandleSmartEquipBelt(ICommonSession session)
		{
			this.HandleSmartEquip(session, "belt");
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x00078B04 File Offset: 0x00076D04
		private void HandleSmartEquip([Nullable(2)] ICommonSession session, string equipmentSlot)
		{
			IPlayerSession playerSession = session as IPlayerSession;
			if (playerSession == null)
			{
				return;
			}
			EntityUid? attachedEntity = playerSession.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid plyEnt = attachedEntity.GetValueOrDefault();
				if (plyEnt.Valid && base.Exists(plyEnt))
				{
					if (!this._actionBlockerSystem.CanInteract(plyEnt, null))
					{
						return;
					}
					SharedHandsComponent hands;
					if (!base.TryComp<SharedHandsComponent>(plyEnt, ref hands) || hands.ActiveHand == null)
					{
						return;
					}
					EntityUid? slotEntity;
					ServerStorageComponent storageComponent;
					if (!this._inventorySystem.TryGetSlotEntity(plyEnt, equipmentSlot, out slotEntity, null, null) || !base.TryComp<ServerStorageComponent>(slotEntity, ref storageComponent))
					{
						this._popupSystem.PopupEntity(Loc.GetString("hands-system-missing-equipment-slot", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("slotName", equipmentSlot)
						}), plyEnt, session, PopupType.Small);
						return;
					}
					if (hands.ActiveHand.HeldEntity != null)
					{
						this._storageSystem.PlayerInsertHeldEntity(slotEntity.Value, plyEnt, storageComponent);
						return;
					}
					if (storageComponent.StoredEntities != null)
					{
						if (storageComponent.StoredEntities.Count == 0)
						{
							this._popupSystem.PopupEntity(Loc.GetString("hands-system-empty-equipment-slot", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("slotName", equipmentSlot)
							}), plyEnt, session, PopupType.Small);
							return;
						}
						IReadOnlyList<EntityUid> storedEntities = storageComponent.StoredEntities;
						EntityUid lastStoredEntity = storedEntities[storedEntities.Count - 1];
						if (storageComponent.Remove(lastStoredEntity))
						{
							base.PickupOrDrop(new EntityUid?(plyEnt), lastStoredEntity, true, true, hands, null);
						}
					}
					return;
				}
			}
		}

		// Token: 0x04000E57 RID: 3671
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x04000E58 RID: 3672
		[Dependency]
		private readonly StackSystem _stackSystem;

		// Token: 0x04000E59 RID: 3673
		[Dependency]
		private readonly HandVirtualItemSystem _virtualItemSystem;

		// Token: 0x04000E5A RID: 3674
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04000E5B RID: 3675
		[Dependency]
		private readonly SharedHandVirtualItemSystem _virtualSystem;

		// Token: 0x04000E5C RID: 3676
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000E5D RID: 3677
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000E5E RID: 3678
		[Dependency]
		private readonly PullingSystem _pullingSystem;

		// Token: 0x04000E5F RID: 3679
		[Dependency]
		private readonly ThrowingSystem _throwingSystem;

		// Token: 0x04000E60 RID: 3680
		[Dependency]
		private readonly StorageSystem _storageSystem;
	}
}
