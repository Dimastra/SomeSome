using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Players;

namespace Content.Shared.Hands.EntitySystems
{
	// Token: 0x0200043C RID: 1084
	[NullableContext(2)]
	[Nullable(0)]
	public abstract class SharedHandsSystem : EntitySystem
	{
		// Token: 0x06000CF4 RID: 3316 RVA: 0x0002A6A8 File Offset: 0x000288A8
		public bool TrySelect(EntityUid uid, EntityUid? entity, SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			Hand hand;
			if (!this.IsHolding(uid, entity, out hand, handsComp))
			{
				return false;
			}
			this.SetActiveHand(uid, hand, handsComp);
			return true;
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0002A6E0 File Offset: 0x000288E0
		public bool TrySelect<[Nullable(0)] TComponent>(EntityUid uid, [NotNullWhen(true)] out TComponent component, SharedHandsComponent handsComp = null) where TComponent : Component
		{
			component = default(TComponent);
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			foreach (Hand hand in handsComp.Hands.Values)
			{
				if (base.TryComp<TComponent>(hand.HeldEntity, ref component))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x0002A75C File Offset: 0x0002895C
		public bool TrySelectEmptyHand(EntityUid uid, SharedHandsComponent handsComp = null)
		{
			return this.TrySelect(uid, null, handsComp);
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000CF7 RID: 3319 RVA: 0x0002A77C File Offset: 0x0002897C
		// (remove) Token: 0x06000CF8 RID: 3320 RVA: 0x0002A7B4 File Offset: 0x000289B4
		protected event Action<SharedHandsComponent> OnHandSetActive;

		// Token: 0x06000CF9 RID: 3321 RVA: 0x0002A7E9 File Offset: 0x000289E9
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeInteractions();
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x0002A7F7 File Offset: 0x000289F7
		public override void Shutdown()
		{
			base.Shutdown();
			CommandBinds.Unregister<SharedHandsSystem>();
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x0002A804 File Offset: 0x00028A04
		[NullableContext(1)]
		public virtual void AddHand(EntityUid uid, string handName, HandLocation handLocation, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return;
			}
			if (handsComp.Hands.ContainsKey(handName))
			{
				return;
			}
			ContainerSlot container = this._containerSystem.EnsureContainer<ContainerSlot>(uid, handName, null);
			container.OccludesLight = false;
			Hand newHand = new Hand(handName, handLocation, container);
			handsComp.Hands.Add(handName, newHand);
			handsComp.SortedHands.Add(handName);
			if (handsComp.ActiveHand == null)
			{
				this.SetActiveHand(uid, newHand, handsComp);
			}
			base.RaiseLocalEvent<HandCountChangedEvent>(uid, new HandCountChangedEvent(uid), false);
			base.Dirty(handsComp, null);
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x0002A894 File Offset: 0x00028A94
		[NullableContext(1)]
		public virtual void RemoveHand(EntityUid uid, string handName, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return;
			}
			Hand hand;
			if (!handsComp.Hands.Remove(handName, out hand))
			{
				return;
			}
			this.TryDrop(uid, hand, null, false, true, handsComp);
			ContainerSlot container = hand.Container;
			if (container != null)
			{
				container.Shutdown(null, null);
			}
			handsComp.SortedHands.Remove(hand.Name);
			if (handsComp.ActiveHand == hand)
			{
				this.TrySetActiveHand(uid, handsComp.SortedHands.FirstOrDefault<string>(), handsComp);
			}
			base.RaiseLocalEvent<HandCountChangedEvent>(uid, new HandCountChangedEvent(uid), false);
			base.Dirty(handsComp, null);
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0002A92C File Offset: 0x00028B2C
		[NullableContext(1)]
		private void HandleSetHand(RequestSetHandEvent msg, EntitySessionEventArgs eventArgs)
		{
			if (eventArgs.SenderSession.AttachedEntity == null)
			{
				return;
			}
			this.TrySetActiveHand(eventArgs.SenderSession.AttachedEntity.Value, msg.HandName, null);
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x0002A974 File Offset: 0x00028B74
		public bool TryGetEmptyHand(EntityUid uid, [NotNullWhen(true)] out Hand emptyHand, SharedHandsComponent handComp = null)
		{
			emptyHand = null;
			if (!base.Resolve<SharedHandsComponent>(uid, ref handComp, false))
			{
				return false;
			}
			foreach (Hand hand in this.EnumerateHands(uid, handComp))
			{
				if (hand.IsEmpty)
				{
					emptyHand = hand;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x0002A9E0 File Offset: 0x00028BE0
		[NullableContext(1)]
		public IEnumerable<Hand> EnumerateHands(EntityUid uid, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				yield break;
			}
			if (handsComp.ActiveHand != null)
			{
				yield return handsComp.ActiveHand;
			}
			foreach (string name in handsComp.SortedHands)
			{
				string a = name;
				Hand activeHand = handsComp.ActiveHand;
				if (a != ((activeHand != null) ? activeHand.Name : null))
				{
					yield return handsComp.Hands[name];
				}
			}
			List<string>.Enumerator enumerator = default(List<string>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x0002A9FE File Offset: 0x00028BFE
		[NullableContext(1)]
		public IEnumerable<EntityUid> EnumerateHeld(EntityUid uid, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				yield break;
			}
			if (handsComp.ActiveHandEntity != null)
			{
				yield return handsComp.ActiveHandEntity.Value;
			}
			foreach (string name in handsComp.SortedHands)
			{
				string a = name;
				Hand activeHand = handsComp.ActiveHand;
				if (!(a == ((activeHand != null) ? activeHand.Name : null)))
				{
					EntityUid? heldEntity = handsComp.Hands[name].HeldEntity;
					if (heldEntity != null)
					{
						EntityUid held = heldEntity.GetValueOrDefault();
						yield return held;
					}
				}
			}
			List<string>.Enumerator enumerator = default(List<string>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x0002AA1C File Offset: 0x00028C1C
		public virtual bool TrySetActiveHand(EntityUid uid, string name, SharedHandsComponent handComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handComp, true))
			{
				return false;
			}
			Hand activeHand = handComp.ActiveHand;
			if (name == ((activeHand != null) ? activeHand.Name : null))
			{
				return false;
			}
			Hand hand = null;
			return (name == null || handComp.Hands.TryGetValue(name, out hand)) && this.SetActiveHand(uid, hand, handComp);
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x0002AA74 File Offset: 0x00028C74
		public bool SetActiveHand(EntityUid uid, Hand hand, SharedHandsComponent handComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handComp, true))
			{
				return false;
			}
			if (hand == handComp.ActiveHand)
			{
				return false;
			}
			Hand activeHand = handComp.ActiveHand;
			EntityUid? entityUid = (activeHand != null) ? activeHand.HeldEntity : null;
			if (entityUid != null)
			{
				EntityUid held = entityUid.GetValueOrDefault();
				base.RaiseLocalEvent<HandDeselectedEvent>(held, new HandDeselectedEvent(uid), false);
			}
			if (hand == null)
			{
				handComp.ActiveHand = null;
				return true;
			}
			handComp.ActiveHand = hand;
			Action<SharedHandsComponent> onHandSetActive = this.OnHandSetActive;
			if (onHandSetActive != null)
			{
				onHandSetActive(handComp);
			}
			if (hand.HeldEntity != null)
			{
				base.RaiseLocalEvent<HandSelectedEvent>(hand.HeldEntity.Value, new HandSelectedEvent(uid), false);
			}
			base.Dirty(handComp, null);
			return true;
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x0002AB30 File Offset: 0x00028D30
		public bool IsHolding(EntityUid uid, EntityUid? entity, [NotNullWhen(true)] out Hand inHand, SharedHandsComponent handsComp = null)
		{
			inHand = null;
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			foreach (Hand hand in handsComp.Hands.Values)
			{
				if (hand.HeldEntity == entity)
				{
					inHand = hand;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x0002ABDC File Offset: 0x00028DDC
		[NullableContext(1)]
		public bool CanDropHeld(EntityUid uid, Hand hand, bool checkActionBlocker = true)
		{
			return hand.HeldEntity != null && hand.Container.CanRemove(hand.HeldEntity.Value, this.EntityManager) && (!checkActionBlocker || this._actionBlocker.CanDrop(uid));
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x0002AC32 File Offset: 0x00028E32
		public bool TryDrop(EntityUid uid, EntityCoordinates? targetDropLocation = null, bool checkActionBlocker = true, bool doDropInteraction = true, SharedHandsComponent handsComp = null)
		{
			return base.Resolve<SharedHandsComponent>(uid, ref handsComp, true) && handsComp.ActiveHand != null && this.TryDrop(uid, handsComp.ActiveHand, targetDropLocation, checkActionBlocker, doDropInteraction, handsComp);
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0002AC64 File Offset: 0x00028E64
		public bool TryDrop(EntityUid uid, EntityUid entity, EntityCoordinates? targetDropLocation = null, bool checkActionBlocker = true, bool doDropInteraction = true, SharedHandsComponent handsComp = null)
		{
			Hand hand;
			return base.Resolve<SharedHandsComponent>(uid, ref handsComp, true) && this.IsHolding(uid, new EntityUid?(entity), out hand, handsComp) && this.TryDrop(uid, hand, targetDropLocation, checkActionBlocker, doDropInteraction, handsComp);
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x0002ACA4 File Offset: 0x00028EA4
		[NullableContext(1)]
		public bool TryDrop(EntityUid uid, Hand hand, EntityCoordinates? targetDropLocation = null, bool checkActionBlocker = true, bool doDropInteraction = true, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, true))
			{
				return false;
			}
			if (!this.CanDropHeld(uid, hand, checkActionBlocker))
			{
				return false;
			}
			EntityUid entity = hand.HeldEntity.Value;
			this.DoDrop(uid, hand, doDropInteraction, handsComp);
			TransformComponent userXform = base.Transform(uid);
			TransformComponent itemXform = base.Transform(entity);
			bool isInContainer = this._containerSystem.IsEntityInContainer(uid, null);
			if (targetDropLocation == null || isInContainer)
			{
				IContainer container;
				if (!isInContainer || !this._containerSystem.TryGetContainingContainer(userXform.ParentUid, uid, ref container, null, true) || !container.Insert(entity, this.EntityManager, itemXform, null, null, null))
				{
					itemXform.AttachToGridOrMap();
				}
				return true;
			}
			MapCoordinates target = targetDropLocation.Value.ToMap(this.EntityManager);
			itemXform.WorldPosition = this.GetFinalDropCoordinates(uid, userXform.MapPosition, target);
			return true;
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0002AD7C File Offset: 0x00028F7C
		[NullableContext(1)]
		public bool TryDropIntoContainer(EntityUid uid, EntityUid entity, IContainer targetContainer, bool checkActionBlocker = true, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, true))
			{
				return false;
			}
			Hand hand;
			if (!this.IsHolding(uid, new EntityUid?(entity), out hand, handsComp))
			{
				return false;
			}
			if (!this.CanDropHeld(uid, hand, checkActionBlocker))
			{
				return false;
			}
			if (!targetContainer.CanInsert(entity, this.EntityManager))
			{
				return false;
			}
			this.DoDrop(uid, hand, false, handsComp);
			targetContainer.Insert(entity, null, null, null, null, null);
			return true;
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x0002ADE4 File Offset: 0x00028FE4
		private Vector2 GetFinalDropCoordinates(EntityUid user, MapCoordinates origin, MapCoordinates target)
		{
			Vector2 dropVector = target.Position - origin.Position;
			float requestedDropDistance = dropVector.Length;
			if (dropVector.Length > 1.5f)
			{
				dropVector = dropVector.Normalized * 1.5f;
				target..ctor(origin.Position + dropVector, target.MapId);
			}
			float dropLength = this._interactionSystem.UnobstructedDistance(origin, target, 130, (EntityUid e) => e == user);
			if (dropLength < requestedDropDistance)
			{
				return origin.Position + dropVector.Normalized * dropLength;
			}
			return target.Position;
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x0002AE94 File Offset: 0x00029094
		[NullableContext(1)]
		public virtual void DoDrop(EntityUid uid, Hand hand, bool doDropInteraction = true, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, true))
			{
				return;
			}
			ContainerSlot container = hand.Container;
			if (container == null || container.ContainedEntity == null)
			{
				return;
			}
			EntityUid entity = hand.Container.ContainedEntity.Value;
			if (!hand.Container.Remove(entity, this.EntityManager, null, null, true, false, null, null))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(74, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Failed to remove ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity));
				defaultInterpolatedStringHandler.AppendLiteral(" from users hand container when dropping. User: ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(". Hand: ");
				defaultInterpolatedStringHandler.AppendFormatted(hand.Name);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			base.Dirty(handsComp, null);
			if (doDropInteraction)
			{
				this._interactionSystem.DroppedInteraction(uid, entity);
			}
			GotUnequippedHandEvent gotUnequipped = new GotUnequippedHandEvent(uid, entity, hand);
			base.RaiseLocalEvent<GotUnequippedHandEvent>(entity, gotUnequipped, false);
			DidUnequipHandEvent didUnequip = new DidUnequipHandEvent(uid, entity, hand);
			base.RaiseLocalEvent<DidUnequipHandEvent>(uid, didUnequip, true);
			if (hand == handsComp.ActiveHand)
			{
				base.RaiseLocalEvent<HandDeselectedEvent>(entity, new HandDeselectedEvent(uid), false);
			}
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x0002AFD4 File Offset: 0x000291D4
		private void InitializeInteractions()
		{
			base.SubscribeAllEvent<RequestSetHandEvent>(new EntitySessionEventHandler<RequestSetHandEvent>(this.HandleSetHand), null, null);
			base.SubscribeAllEvent<RequestActivateInHandEvent>(new EntitySessionEventHandler<RequestActivateInHandEvent>(this.HandleActivateItemInHand), null, null);
			base.SubscribeAllEvent<RequestHandInteractUsingEvent>(new EntitySessionEventHandler<RequestHandInteractUsingEvent>(this.HandleInteractUsingInHand), null, null);
			base.SubscribeAllEvent<RequestUseInHandEvent>(new EntitySessionEventHandler<RequestUseInHandEvent>(this.HandleUseInHand), null, null);
			base.SubscribeAllEvent<RequestMoveHandItemEvent>(new EntitySessionEventHandler<RequestMoveHandItemEvent>(this.HandleMoveItemFromHand), null, null);
			base.SubscribeAllEvent<RequestHandAltInteractEvent>(new EntitySessionEventHandler<RequestHandAltInteractEvent>(this.HandleHandAltInteract), null, null);
			base.SubscribeLocalEvent<SharedHandsComponent, ExaminedEvent>(new ComponentEventHandler<SharedHandsComponent, ExaminedEvent>(this.HandleExamined), null, null);
			CommandBinds.Builder.Bind(ContentKeyFunctions.UseItemInHand, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleUseItem), null, false, false)).Bind(ContentKeyFunctions.AltUseItemInHand, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleAltUseInHand), null, false, false)).Bind(ContentKeyFunctions.SwapHands, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.SwapHandsPressed), null, false, false)).Bind(ContentKeyFunctions.Drop, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.DropPressed), true, false)).Register<SharedHandsSystem>();
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x0002B0F0 File Offset: 0x000292F0
		private void HandleAltUseInHand(ICommonSession session)
		{
			if (session != null && session.AttachedEntity != null)
			{
				this.TryUseItemInHand(session.AttachedEntity.Value, true, null, null);
			}
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x0002B128 File Offset: 0x00029328
		private void HandleUseItem(ICommonSession session)
		{
			if (session != null && session.AttachedEntity != null)
			{
				this.TryUseItemInHand(session.AttachedEntity.Value, false, null, null);
			}
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x0002B160 File Offset: 0x00029360
		[NullableContext(1)]
		private void HandleMoveItemFromHand(RequestMoveHandItemEvent msg, EntitySessionEventArgs args)
		{
			if (args.SenderSession.AttachedEntity != null)
			{
				this.TryMoveHeldEntityToActiveHand(args.SenderSession.AttachedEntity.Value, msg.HandName, true, null);
			}
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x0002B1A8 File Offset: 0x000293A8
		[NullableContext(1)]
		private void HandleUseInHand(RequestUseInHandEvent msg, EntitySessionEventArgs args)
		{
			if (args.SenderSession.AttachedEntity != null)
			{
				this.TryUseItemInHand(args.SenderSession.AttachedEntity.Value, false, null, null);
			}
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x0002B1EC File Offset: 0x000293EC
		[NullableContext(1)]
		private void HandleActivateItemInHand(RequestActivateInHandEvent msg, EntitySessionEventArgs args)
		{
			if (args.SenderSession.AttachedEntity != null)
			{
				this.TryActivateItemInHand(args.SenderSession.AttachedEntity.Value, null, msg.HandName);
			}
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x0002B234 File Offset: 0x00029434
		[NullableContext(1)]
		private void HandleInteractUsingInHand(RequestHandInteractUsingEvent msg, EntitySessionEventArgs args)
		{
			if (args.SenderSession.AttachedEntity != null)
			{
				this.TryInteractHandWithActiveHand(args.SenderSession.AttachedEntity.Value, msg.HandName, null);
			}
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x0002B27C File Offset: 0x0002947C
		[NullableContext(1)]
		private void HandleHandAltInteract(RequestHandAltInteractEvent msg, EntitySessionEventArgs args)
		{
			if (args.SenderSession.AttachedEntity != null)
			{
				this.TryUseItemInHand(args.SenderSession.AttachedEntity.Value, true, null, msg.HandName);
			}
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x0002B2C4 File Offset: 0x000294C4
		private void SwapHandsPressed(ICommonSession session)
		{
			SharedHandsComponent component;
			if (!base.TryComp<SharedHandsComponent>((session != null) ? session.AttachedEntity : null, ref component))
			{
				return;
			}
			if (!this._actionBlocker.CanInteract(session.AttachedEntity.Value, null))
			{
				return;
			}
			if (component.ActiveHand == null || component.Hands.Count < 2)
			{
				return;
			}
			int newActiveIndex = component.SortedHands.IndexOf(component.ActiveHand.Name) + 1;
			string nextHand = component.SortedHands[newActiveIndex % component.Hands.Count];
			this.TrySetActiveHand(component.Owner, nextHand, component);
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x0002B370 File Offset: 0x00029570
		private bool DropPressed(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			SharedHandsComponent hands;
			if (base.TryComp<SharedHandsComponent>((session != null) ? session.AttachedEntity : null, ref hands) && hands.ActiveHand != null)
			{
				this.TryDrop(session.AttachedEntity.Value, hands.ActiveHand, new EntityCoordinates?(coords), true, true, hands);
			}
			return false;
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x0002B3C8 File Offset: 0x000295C8
		public bool TryActivateItemInHand(EntityUid uid, SharedHandsComponent handsComp = null, string handName = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			Hand hand;
			if (handName == null || !handsComp.Hands.TryGetValue(handName, out hand))
			{
				hand = handsComp.ActiveHand;
			}
			EntityUid? entityUid = (hand != null) ? hand.HeldEntity : null;
			if (entityUid != null)
			{
				EntityUid held = entityUid.GetValueOrDefault();
				return this._interactionSystem.InteractionActivate(uid, held, true, true, true);
			}
			return false;
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0002B438 File Offset: 0x00029638
		[NullableContext(1)]
		public bool TryInteractHandWithActiveHand(EntityUid uid, string handName, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			if (handsComp.ActiveHandEntity == null)
			{
				return false;
			}
			Hand hand;
			if (!handsComp.Hands.TryGetValue(handName, out hand))
			{
				return false;
			}
			if (hand.HeldEntity == null)
			{
				return false;
			}
			this._interactionSystem.InteractUsing(uid, handsComp.ActiveHandEntity.Value, hand.HeldEntity.Value, base.Transform(hand.HeldEntity.Value).Coordinates, true, true);
			return true;
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0002B4D0 File Offset: 0x000296D0
		public bool TryUseItemInHand(EntityUid uid, bool altInteract = false, SharedHandsComponent handsComp = null, string handName = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			Hand hand;
			if (handName == null || !handsComp.Hands.TryGetValue(handName, out hand))
			{
				hand = handsComp.ActiveHand;
			}
			EntityUid? entityUid = (hand != null) ? hand.HeldEntity : null;
			if (entityUid == null)
			{
				return false;
			}
			EntityUid held = entityUid.GetValueOrDefault();
			if (altInteract)
			{
				return this._interactionSystem.AltInteract(uid, held);
			}
			return this._interactionSystem.UseInHandInteraction(uid, held, true, true, true);
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0002B554 File Offset: 0x00029754
		[NullableContext(1)]
		public bool TryMoveHeldEntityToActiveHand(EntityUid uid, string handName, bool checkActionBlocker = true, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, true))
			{
				return false;
			}
			if (handsComp.ActiveHand == null || !handsComp.ActiveHand.IsEmpty)
			{
				return false;
			}
			Hand hand;
			if (!handsComp.Hands.TryGetValue(handName, out hand))
			{
				return false;
			}
			if (!this.CanDropHeld(uid, hand, checkActionBlocker))
			{
				return false;
			}
			EntityUid entity = hand.HeldEntity.Value;
			if (!this.CanPickupToHand(uid, entity, handsComp.ActiveHand, checkActionBlocker, handsComp, null))
			{
				return false;
			}
			this.DoDrop(uid, hand, false, handsComp);
			this.DoPickup(uid, handsComp.ActiveHand, entity, handsComp);
			return true;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0002B5EC File Offset: 0x000297EC
		[NullableContext(1)]
		private void HandleExamined(EntityUid uid, SharedHandsComponent handsComp, ExaminedEvent args)
		{
			foreach (EntityUid inhand in this.EnumerateHeld(uid, handsComp))
			{
				if (!base.HasComp<HandVirtualItemComponent>(inhand))
				{
					args.PushText(Loc.GetString("comp-hands-examine", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("user", Identity.Entity(handsComp.Owner, this.EntityManager)),
						new ValueTuple<string, object>("item", inhand)
					}));
				}
			}
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0002B694 File Offset: 0x00029894
		public bool TryPickup(EntityUid uid, EntityUid entity, string handName = null, bool checkActionBlocker = true, bool animateUser = false, SharedHandsComponent handsComp = null, ItemComponent item = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			Hand hand = handsComp.ActiveHand;
			return (handName == null || handsComp.Hands.TryGetValue(handName, out hand)) && hand != null && this.TryPickup(uid, entity, hand, checkActionBlocker, animateUser, handsComp, item);
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x0002B6E4 File Offset: 0x000298E4
		public bool TryPickupAnyHand(EntityUid uid, EntityUid entity, bool checkActionBlocker = true, bool animateUser = false, SharedHandsComponent handsComp = null, ItemComponent item = null)
		{
			Hand hand;
			return base.Resolve<SharedHandsComponent>(uid, ref handsComp, false) && this.TryGetEmptyHand(uid, out hand, handsComp) && this.TryPickup(uid, entity, hand, checkActionBlocker, animateUser, handsComp, item);
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0002B720 File Offset: 0x00029920
		public bool TryPickup(EntityUid uid, EntityUid entity, [Nullable(1)] Hand hand, bool checkActionBlocker = true, bool animateUser = false, SharedHandsComponent handsComp = null, ItemComponent item = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			if (!base.Resolve<ItemComponent>(entity, ref item, false))
			{
				return false;
			}
			if (!this.CanPickupToHand(uid, entity, hand, checkActionBlocker, handsComp, item))
			{
				return false;
			}
			TransformComponent xform = base.Transform(uid);
			EntityUid coordinateEntity = xform.ParentUid.IsValid() ? xform.ParentUid : uid;
			MapCoordinates itemPos = base.Transform(entity).MapPosition;
			if (itemPos.MapId == xform.MapID)
			{
				EntityCoordinates initialPosition = EntityCoordinates.FromMap(coordinateEntity, itemPos, this.EntityManager);
				this.PickupAnimation(entity, initialPosition, xform.LocalPosition, animateUser ? null : new EntityUid?(uid));
			}
			this.DoPickup(uid, hand, entity, handsComp);
			return true;
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0002B7E0 File Offset: 0x000299E0
		public bool CanPickupAnyHand(EntityUid uid, EntityUid entity, bool checkActionBlocker = true, SharedHandsComponent handsComp = null, ItemComponent item = null)
		{
			Hand hand;
			return base.Resolve<SharedHandsComponent>(uid, ref handsComp, false) && this.TryGetEmptyHand(uid, out hand, handsComp) && this.CanPickupToHand(uid, entity, hand, checkActionBlocker, handsComp, item);
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x0002B818 File Offset: 0x00029A18
		public bool CanPickupToHand(EntityUid uid, EntityUid entity, [Nullable(1)] Hand hand, bool checkActionBlocker = true, SharedHandsComponent handsComp = null, ItemComponent item = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref handsComp, false))
			{
				return false;
			}
			ContainerSlot handContainer = hand.Container;
			PhysicsComponent physics;
			return handContainer != null && handContainer.ContainedEntity == null && base.Resolve<ItemComponent>(entity, ref item, false) && (!base.TryComp<PhysicsComponent>(entity, ref physics) || physics.BodyType != 4) && (!checkActionBlocker || this._actionBlocker.CanPickup(uid, entity)) && handContainer.CanInsert(entity, this.EntityManager);
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x0002B898 File Offset: 0x00029A98
		public void PickupOrDrop(EntityUid? uid, EntityUid entity, bool checkActionBlocker = true, bool animateUser = false, SharedHandsComponent handsComp = null, ItemComponent item = null)
		{
			Hand hand;
			if (uid == null || !base.Resolve<SharedHandsComponent>(uid.Value, ref handsComp, false) || !this.TryGetEmptyHand(uid.Value, out hand, handsComp) || !this.TryPickup(uid.Value, entity, hand, checkActionBlocker, animateUser, handsComp, item))
			{
				ContainerHelpers.AttachParentToContainerOrGrid(base.Transform(entity), this.EntityManager);
			}
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0002B900 File Offset: 0x00029B00
		[NullableContext(1)]
		public virtual void DoPickup(EntityUid uid, Hand hand, EntityUid entity, [Nullable(2)] SharedHandsComponent hands = null)
		{
			if (!base.Resolve<SharedHandsComponent>(uid, ref hands, true))
			{
				return;
			}
			ContainerSlot handContainer = hand.Container;
			if (handContainer == null || handContainer.ContainedEntity != null)
			{
				return;
			}
			if (!handContainer.Insert(entity, this.EntityManager, null, null, null, null))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(76, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Failed to insert ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity));
				defaultInterpolatedStringHandler.AppendLiteral(" into users hand container when picking up. User: ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(". Hand: ");
				defaultInterpolatedStringHandler.AppendFormatted(hand.Name);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Pickup;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(11, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "user", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" picked up ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity), "entity", "ToPrettyString(entity)");
			adminLogger.Add(type, impact, ref logStringHandler);
			base.Dirty(hands, null);
			DidEquipHandEvent didEquip = new DidEquipHandEvent(uid, entity, hand);
			base.RaiseLocalEvent<DidEquipHandEvent>(uid, didEquip, false);
			GotEquippedHandEvent gotEquipped = new GotEquippedHandEvent(uid, entity, hand);
			base.RaiseLocalEvent<GotEquippedHandEvent>(entity, gotEquipped, true);
			if (gotEquipped.Handled)
			{
				return;
			}
			if (hand == hands.ActiveHand)
			{
				base.RaiseLocalEvent<HandSelectedEvent>(entity, new HandSelectedEvent(uid), false);
			}
		}

		// Token: 0x06000D21 RID: 3361
		public abstract void PickupAnimation(EntityUid item, EntityCoordinates initialPosition, Vector2 finalPosition, EntityUid? exclude);

		// Token: 0x04000CB0 RID: 3248
		[Nullable(1)]
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000CB1 RID: 3249
		[Nullable(1)]
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000CB2 RID: 3250
		[Nullable(1)]
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x04000CB3 RID: 3251
		[Nullable(1)]
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;
	}
}
