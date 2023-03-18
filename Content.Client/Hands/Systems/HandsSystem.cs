using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Client.Animations;
using Content.Client.Examine;
using Content.Client.Strip;
using Content.Client.Verbs.UI;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Hands.Systems
{
	// Token: 0x020002DF RID: 735
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandsSystem : SharedHandsSystem
	{
		// Token: 0x14000066 RID: 102
		// (add) Token: 0x0600127E RID: 4734 RVA: 0x0006E6A4 File Offset: 0x0006C8A4
		// (remove) Token: 0x0600127F RID: 4735 RVA: 0x0006E6DC File Offset: 0x0006C8DC
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string, HandLocation> OnPlayerAddHand;

		// Token: 0x14000067 RID: 103
		// (add) Token: 0x06001280 RID: 4736 RVA: 0x0006E714 File Offset: 0x0006C914
		// (remove) Token: 0x06001281 RID: 4737 RVA: 0x0006E74C File Offset: 0x0006C94C
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string> OnPlayerRemoveHand;

		// Token: 0x14000068 RID: 104
		// (add) Token: 0x06001282 RID: 4738 RVA: 0x0006E784 File Offset: 0x0006C984
		// (remove) Token: 0x06001283 RID: 4739 RVA: 0x0006E7BC File Offset: 0x0006C9BC
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<string> OnPlayerSetActiveHand;

		// Token: 0x14000069 RID: 105
		// (add) Token: 0x06001284 RID: 4740 RVA: 0x0006E7F4 File Offset: 0x0006C9F4
		// (remove) Token: 0x06001285 RID: 4741 RVA: 0x0006E82C File Offset: 0x0006CA2C
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<HandsComponent> OnPlayerHandsAdded;

		// Token: 0x1400006A RID: 106
		// (add) Token: 0x06001286 RID: 4742 RVA: 0x0006E864 File Offset: 0x0006CA64
		// (remove) Token: 0x06001287 RID: 4743 RVA: 0x0006E89C File Offset: 0x0006CA9C
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action OnPlayerHandsRemoved;

		// Token: 0x1400006B RID: 107
		// (add) Token: 0x06001288 RID: 4744 RVA: 0x0006E8D4 File Offset: 0x0006CAD4
		// (remove) Token: 0x06001289 RID: 4745 RVA: 0x0006E90C File Offset: 0x0006CB0C
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string, EntityUid> OnPlayerItemAdded;

		// Token: 0x1400006C RID: 108
		// (add) Token: 0x0600128A RID: 4746 RVA: 0x0006E944 File Offset: 0x0006CB44
		// (remove) Token: 0x0600128B RID: 4747 RVA: 0x0006E97C File Offset: 0x0006CB7C
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string, EntityUid> OnPlayerItemRemoved;

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x0600128C RID: 4748 RVA: 0x0006E9B4 File Offset: 0x0006CBB4
		// (remove) Token: 0x0600128D RID: 4749 RVA: 0x0006E9EC File Offset: 0x0006CBEC
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string> OnPlayerHandBlocked;

		// Token: 0x1400006E RID: 110
		// (add) Token: 0x0600128E RID: 4750 RVA: 0x0006EA24 File Offset: 0x0006CC24
		// (remove) Token: 0x0600128F RID: 4751 RVA: 0x0006EA5C File Offset: 0x0006CC5C
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<string> OnPlayerHandUnblocked;

		// Token: 0x06001290 RID: 4752 RVA: 0x0006EA94 File Offset: 0x0006CC94
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SharedHandsComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<SharedHandsComponent, EntRemovedFromContainerMessage>(this.HandleItemRemoved), null, null);
			base.SubscribeLocalEvent<SharedHandsComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<SharedHandsComponent, EntInsertedIntoContainerMessage>(this.HandleItemAdded), null, null);
			base.SubscribeLocalEvent<HandsComponent, PlayerAttachedEvent>(new ComponentEventHandler<HandsComponent, PlayerAttachedEvent>(this.HandlePlayerAttached), null, null);
			base.SubscribeLocalEvent<HandsComponent, PlayerDetachedEvent>(new ComponentEventHandler<HandsComponent, PlayerDetachedEvent>(this.HandlePlayerDetached), null, null);
			base.SubscribeLocalEvent<HandsComponent, ComponentAdd>(new ComponentEventHandler<HandsComponent, ComponentAdd>(this.HandleCompAdd), null, null);
			base.SubscribeLocalEvent<HandsComponent, ComponentRemove>(new ComponentEventHandler<HandsComponent, ComponentRemove>(this.HandleCompRemove), null, null);
			base.SubscribeLocalEvent<HandsComponent, ComponentHandleState>(new ComponentEventRefHandler<HandsComponent, ComponentHandleState>(this.HandleComponentState), null, null);
			base.SubscribeLocalEvent<HandsComponent, VisualsChangedEvent>(new ComponentEventHandler<HandsComponent, VisualsChangedEvent>(this.OnVisualsChanged), null, null);
			base.SubscribeNetworkEvent<PickupAnimationEvent>(new EntityEventHandler<PickupAnimationEvent>(this.HandlePickupAnimation), null, null);
			base.OnHandSetActive += this.OnHandActivated;
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x0006EB70 File Offset: 0x0006CD70
		private void HandleComponentState(EntityUid uid, HandsComponent component, ref ComponentHandleState args)
		{
			HandsComponentState handsComponentState = args.Current as HandsComponentState;
			if (handsComponentState == null)
			{
				return;
			}
			bool flag = component.Hands.Count != handsComponentState.Hands.Count;
			ContainerManagerComponent containerManagerComponent = base.EnsureComp<ContainerManagerComponent>(uid);
			if (flag)
			{
				List<Hand> list = new List<Hand>();
				foreach (Hand hand in handsComponentState.Hands)
				{
					if (component.Hands.TryAdd(hand.Name, hand))
					{
						hand.Container = this._containerSystem.EnsureContainer<ContainerSlot>(uid, hand.Name, containerManagerComponent);
						list.Add(hand);
					}
				}
				foreach (string text in component.Hands.Keys)
				{
					if (!handsComponentState.HandNames.Contains(text))
					{
						this.RemoveHand(uid, text, component);
					}
				}
				foreach (Hand newHand in list)
				{
					this.AddHand(uid, newHand, component);
				}
				component.SortedHands = new List<string>(handsComponentState.HandNames);
			}
			this._stripSys.UpdateUi(uid, null, null);
			if (component.ActiveHand == null && handsComponentState.ActiveHand == null)
			{
				return;
			}
			if (component.ActiveHand != null && handsComponentState.ActiveHand != component.ActiveHand.Name)
			{
				base.SetActiveHand(uid, component.Hands[handsComponentState.ActiveHand], component);
			}
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x0006ED3C File Offset: 0x0006CF3C
		private void HandlePickupAnimation(PickupAnimationEvent msg)
		{
			this.PickupAnimation(msg.ItemUid, msg.InitialPosition, msg.FinalPosition);
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x0006ED56 File Offset: 0x0006CF56
		public override void PickupAnimation(EntityUid item, EntityCoordinates initialPosition, Vector2 finalPosition, EntityUid? exclude)
		{
			this.PickupAnimation(item, initialPosition, finalPosition);
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x0006ED61 File Offset: 0x0006CF61
		public void PickupAnimation(EntityUid item, EntityCoordinates initialPosition, Vector2 finalPosition)
		{
			if (!this._gameTiming.IsFirstTimePredicted)
			{
				return;
			}
			if (finalPosition.EqualsApprox(initialPosition.Position, 0.10000000149011612))
			{
				return;
			}
			ReusableAnimations.AnimateEntityPickup(item, initialPosition, finalPosition, null);
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x0006ED94 File Offset: 0x0006CF94
		public void ReloadHandButtons()
		{
			HandsComponent obj;
			if (!this.TryGetPlayerHands(out obj))
			{
				return;
			}
			Action<HandsComponent> onPlayerHandsAdded = this.OnPlayerHandsAdded;
			if (onPlayerHandsAdded == null)
			{
				return;
			}
			onPlayerHandsAdded(obj);
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x0006EDC0 File Offset: 0x0006CFC0
		public override void DoDrop(EntityUid uid, Hand hand, bool doDropInteraction = true, [Nullable(2)] SharedHandsComponent hands = null)
		{
			base.DoDrop(uid, hand, doDropInteraction, hands);
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(hand.HeldEntity, ref spriteComponent))
			{
				spriteComponent.RenderOrder = this.EntityManager.CurrentTick.Value;
			}
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x0006EE00 File Offset: 0x0006D000
		public EntityUid? GetActiveHandEntity()
		{
			HandsComponent handsComponent;
			if (!this.TryGetPlayerHands(out handsComponent))
			{
				return null;
			}
			return handsComponent.ActiveHandEntity;
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x0006EE28 File Offset: 0x0006D028
		[NullableContext(2)]
		public bool TryGetPlayerHands([NotNullWhen(true)] out HandsComponent hands)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			hands = null;
			return entityUid != null && base.TryComp<HandsComponent>(entityUid.Value, ref hands);
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x0006EE74 File Offset: 0x0006D074
		public void UIHandClick(HandsComponent hands, string handName)
		{
			Hand hand;
			if (!hands.Hands.TryGetValue(handName, out hand))
			{
				return;
			}
			if (hands.ActiveHand == null)
			{
				return;
			}
			EntityUid? heldEntity = hand.HeldEntity;
			EntityUid? heldEntity2 = hands.ActiveHand.HeldEntity;
			if (hand == hands.ActiveHand && heldEntity2 != null)
			{
				this.EntityManager.RaisePredictiveEvent<RequestUseInHandEvent>(new RequestUseInHandEvent());
				return;
			}
			if (hand != hands.ActiveHand && heldEntity == null)
			{
				this.EntityManager.RaisePredictiveEvent<RequestSetHandEvent>(new RequestSetHandEvent(handName));
				return;
			}
			if (hand != hands.ActiveHand && heldEntity != null && heldEntity2 != null)
			{
				this.EntityManager.RaisePredictiveEvent<RequestHandInteractUsingEvent>(new RequestHandInteractUsingEvent(hand.Name));
				return;
			}
			if (hand != hands.ActiveHand && heldEntity != null && heldEntity2 == null)
			{
				this.EntityManager.RaisePredictiveEvent<RequestMoveHandItemEvent>(new RequestMoveHandItemEvent(hand.Name));
			}
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x0006EF58 File Offset: 0x0006D158
		public void UIHandActivate(string handName)
		{
			this.EntityManager.RaisePredictiveEvent<RequestActivateInHandEvent>(new RequestActivateInHandEvent(handName));
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x0006EF6C File Offset: 0x0006D16C
		public void UIInventoryExamine(string handName)
		{
			HandsComponent handsComponent;
			Hand hand;
			if (this.TryGetPlayerHands(out handsComponent) && handsComponent.Hands.TryGetValue(handName, out hand))
			{
				EntityUid? heldEntity = hand.HeldEntity;
				if (heldEntity != null)
				{
					EntityUid valueOrDefault = heldEntity.GetValueOrDefault();
					if (valueOrDefault.Valid)
					{
						this._examine.DoExamine(valueOrDefault, true);
						return;
					}
				}
			}
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x0006EFC4 File Offset: 0x0006D1C4
		public void UIHandOpenContextMenu(string handName)
		{
			HandsComponent handsComponent;
			Hand hand;
			if (this.TryGetPlayerHands(out handsComponent) && handsComponent.Hands.TryGetValue(handName, out hand))
			{
				EntityUid? heldEntity = hand.HeldEntity;
				if (heldEntity != null)
				{
					EntityUid valueOrDefault = heldEntity.GetValueOrDefault();
					if (valueOrDefault.Valid)
					{
						this._ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(valueOrDefault, false, null);
						return;
					}
				}
			}
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x0006F020 File Offset: 0x0006D220
		public void UIHandAltActivateItem(string handName)
		{
			base.RaisePredictiveEvent<RequestHandAltInteractEvent>(new RequestHandAltInteractEvent(handName));
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x0006F030 File Offset: 0x0006D230
		private void HandleItemAdded(EntityUid uid, SharedHandsComponent handComp, ContainerModifiedMessage args)
		{
			Hand hand;
			if (!handComp.Hands.TryGetValue(args.Container.ID, out hand))
			{
				return;
			}
			this.UpdateHandVisuals(uid, args.Entity, hand, null, null);
			this._stripSys.UpdateUi(uid, null, null);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			Action<string, EntityUid> onPlayerItemAdded = this.OnPlayerItemAdded;
			if (onPlayerItemAdded != null)
			{
				onPlayerItemAdded(hand.Name, args.Entity);
			}
			if (base.HasComp<HandVirtualItemComponent>(args.Entity))
			{
				Action<string> onPlayerHandBlocked = this.OnPlayerHandBlocked;
				if (onPlayerHandBlocked == null)
				{
					return;
				}
				onPlayerHandBlocked(hand.Name);
			}
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x0006F0F8 File Offset: 0x0006D2F8
		private void HandleItemRemoved(EntityUid uid, SharedHandsComponent handComp, ContainerModifiedMessage args)
		{
			Hand hand;
			if (!handComp.Hands.TryGetValue(args.Container.ID, out hand))
			{
				return;
			}
			this.UpdateHandVisuals(uid, args.Entity, hand, null, null);
			this._stripSys.UpdateUi(uid, null, null);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			Action<string, EntityUid> onPlayerItemRemoved = this.OnPlayerItemRemoved;
			if (onPlayerItemRemoved != null)
			{
				onPlayerItemRemoved(hand.Name, args.Entity);
			}
			if (base.HasComp<HandVirtualItemComponent>(args.Entity))
			{
				Action<string> onPlayerHandUnblocked = this.OnPlayerHandUnblocked;
				if (onPlayerHandUnblocked == null)
				{
					return;
				}
				onPlayerHandUnblocked(hand.Name);
			}
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x0006F1C0 File Offset: 0x0006D3C0
		[NullableContext(2)]
		private void UpdateHandVisuals(EntityUid uid, EntityUid held, [Nullable(1)] Hand hand, HandsComponent handComp = null, SpriteComponent sprite = null)
		{
			if (!base.Resolve<HandsComponent, SpriteComponent>(uid, ref handComp, ref sprite, false))
			{
				return;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid == ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				Action<string, EntityUid> onPlayerItemAdded = this.OnPlayerItemAdded;
				if (onPlayerItemAdded != null)
				{
					onPlayerItemAdded(hand.Name, held);
				}
			}
			if (!handComp.ShowInHands)
			{
				return;
			}
			HashSet<string> hashSet;
			if (handComp.RevealedLayers.TryGetValue(hand.Location, out hashSet))
			{
				foreach (string text in hashSet)
				{
					sprite.RemoveLayer(text);
				}
				hashSet.Clear();
			}
			else
			{
				hashSet = new HashSet<string>();
				handComp.RevealedLayers[hand.Location] = hashSet;
			}
			if (hand.HeldEntity == null)
			{
				base.RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(uid, hashSet), true);
				return;
			}
			GetInhandVisualsEvent getInhandVisualsEvent = new GetInhandVisualsEvent(uid, hand.Location);
			base.RaiseLocalEvent<GetInhandVisualsEvent>(held, getInhandVisualsEvent, false);
			if (getInhandVisualsEvent.Layers.Count == 0)
			{
				base.RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(uid, hashSet), true);
				return;
			}
			foreach (ValueTuple<string, SharedSpriteComponent.PrototypeLayerData> valueTuple in getInhandVisualsEvent.Layers)
			{
				string item = valueTuple.Item1;
				SharedSpriteComponent.PrototypeLayerData item2 = valueTuple.Item2;
				if (!hashSet.Add(item))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(106, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Duplicate key for in-hand visuals: ");
					defaultInterpolatedStringHandler.AppendFormatted(item);
					defaultInterpolatedStringHandler.AppendLiteral(". Are multiple components attempting to modify the same layer? Entity: ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(held));
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					int num = sprite.LayerMapReserveBlank(item);
					SpriteComponent spriteComponent;
					if (item2.RsiPath == null && item2.TexturePath == null && sprite[num].Rsi == null && base.TryComp<SpriteComponent>(held, ref spriteComponent))
					{
						sprite.LayerSetRSI(num, spriteComponent.BaseRSI);
					}
					sprite.LayerSetData(num, item2);
				}
			}
			base.RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(uid, hashSet), true);
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x0006F410 File Offset: 0x0006D610
		private void OnVisualsChanged(EntityUid uid, HandsComponent component, VisualsChangedEvent args)
		{
			Hand hand;
			if (component.Hands.TryGetValue(args.ContainerId, out hand))
			{
				this.UpdateHandVisuals(uid, args.Item, hand, component, null);
			}
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x0006F442 File Offset: 0x0006D642
		private void HandlePlayerAttached(EntityUid uid, HandsComponent component, PlayerAttachedEvent args)
		{
			Action<HandsComponent> onPlayerHandsAdded = this.OnPlayerHandsAdded;
			if (onPlayerHandsAdded == null)
			{
				return;
			}
			onPlayerHandsAdded(component);
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x0006F455 File Offset: 0x0006D655
		private void HandlePlayerDetached(EntityUid uid, HandsComponent component, PlayerDetachedEvent args)
		{
			Action onPlayerHandsRemoved = this.OnPlayerHandsRemoved;
			if (onPlayerHandsRemoved == null)
			{
				return;
			}
			onPlayerHandsRemoved();
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x0006F468 File Offset: 0x0006D668
		private void HandleCompAdd(EntityUid uid, HandsComponent component, ComponentAdd args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == uid));
			}
			if (flag)
			{
				Action<HandsComponent> onPlayerHandsAdded = this.OnPlayerHandsAdded;
				if (onPlayerHandsAdded == null)
				{
					return;
				}
				onPlayerHandsAdded(component);
			}
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x0006F4C8 File Offset: 0x0006D6C8
		private void HandleCompRemove(EntityUid uid, HandsComponent component, ComponentRemove args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == uid));
			}
			if (flag)
			{
				Action onPlayerHandsRemoved = this.OnPlayerHandsRemoved;
				if (onPlayerHandsRemoved == null)
				{
					return;
				}
				onPlayerHandsRemoved();
			}
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x0006F526 File Offset: 0x0006D726
		private void AddHand(EntityUid uid, Hand newHand, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			this.AddHand(uid, newHand.Name, newHand.Location, handsComp);
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x0006F53C File Offset: 0x0006D73C
		public override void AddHand(EntityUid uid, string handName, HandLocation handLocation, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			base.AddHand(uid, handName, handLocation, handsComp);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid == ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				Action<string, HandLocation> onPlayerAddHand = this.OnPlayerAddHand;
				if (onPlayerAddHand != null)
				{
					onPlayerAddHand(handName, handLocation);
				}
			}
			if (handsComp == null)
			{
				return;
			}
			if (handsComp.ActiveHand == null)
			{
				base.SetActiveHand(uid, handsComp.Hands[handName], handsComp);
			}
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x0006F5CC File Offset: 0x0006D7CC
		public override void RemoveHand(EntityUid uid, string handName, [Nullable(2)] SharedHandsComponent handsComp = null)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid == ((localPlayer != null) ? localPlayer.ControlledEntity : null) && handsComp != null && handsComp.Hands.ContainsKey(handName))
			{
				LocalPlayer localPlayer2 = this._playerManager.LocalPlayer;
				if (uid == ((localPlayer2 != null) ? localPlayer2.ControlledEntity : null))
				{
					Action<string> onPlayerRemoveHand = this.OnPlayerRemoveHand;
					if (onPlayerRemoveHand != null)
					{
						onPlayerRemoveHand(handName);
					}
				}
			}
			base.RemoveHand(uid, handName, handsComp);
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x0006F680 File Offset: 0x0006D880
		[NullableContext(2)]
		private void OnHandActivated(SharedHandsComponent handsComponent)
		{
			if (handsComponent == null)
			{
				return;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			EntityUid owner = handsComponent.Owner;
			if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != owner))
			{
				return;
			}
			if (handsComponent.ActiveHand == null)
			{
				Action<string> onPlayerSetActiveHand = this.OnPlayerSetActiveHand;
				if (onPlayerSetActiveHand == null)
				{
					return;
				}
				onPlayerSetActiveHand(null);
				return;
			}
			else
			{
				Action<string> onPlayerSetActiveHand2 = this.OnPlayerSetActiveHand;
				if (onPlayerSetActiveHand2 == null)
				{
					return;
				}
				onPlayerSetActiveHand2(handsComponent.ActiveHand.Name);
				return;
			}
		}

		// Token: 0x0400092E RID: 2350
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400092F RID: 2351
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000930 RID: 2352
		[Dependency]
		private readonly IUserInterfaceManager _ui;

		// Token: 0x04000931 RID: 2353
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04000932 RID: 2354
		[Dependency]
		private readonly StrippableSystem _stripSys;

		// Token: 0x04000933 RID: 2355
		[Dependency]
		private readonly ExamineSystem _examine;
	}
}
