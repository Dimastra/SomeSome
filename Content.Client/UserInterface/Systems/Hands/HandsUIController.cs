using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Gameplay;
using Content.Client.Hands;
using Content.Client.Hands.Systems;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Shared.Cooldown;
using Content.Shared.Hands.Components;
using Content.Shared.Input;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Hands
{
	// Token: 0x02000081 RID: 129
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandsUIController : UIController, IOnStateEntered<GameplayState>, IOnSystemChanged<HandsSystem>, IOnSystemLoaded<HandsSystem>, IOnSystemUnloaded<HandsSystem>
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x00011F16 File Offset: 0x00010116
		[Nullable(2)]
		private HotbarGui HandsGui
		{
			[NullableContext(2)]
			get
			{
				return this.UIManager.GetActiveUIWidgetOrNull<HotbarGui>();
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00011F24 File Offset: 0x00010124
		public void OnSystemLoaded(HandsSystem system)
		{
			this._handsSystem.OnPlayerAddHand += this.OnAddHand;
			this._handsSystem.OnPlayerItemAdded += this.OnItemAdded;
			this._handsSystem.OnPlayerItemRemoved += this.OnItemRemoved;
			this._handsSystem.OnPlayerSetActiveHand += this.SetActiveHand;
			this._handsSystem.OnPlayerRemoveHand += this.RemoveHand;
			this._handsSystem.OnPlayerHandsAdded += this.LoadPlayerHands;
			this._handsSystem.OnPlayerHandsRemoved += this.UnloadPlayerHands;
			this._handsSystem.OnPlayerHandBlocked += this.HandBlocked;
			this._handsSystem.OnPlayerHandUnblocked += this.HandUnblocked;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00012000 File Offset: 0x00010200
		public void OnSystemUnloaded(HandsSystem system)
		{
			this._handsSystem.OnPlayerAddHand -= this.OnAddHand;
			this._handsSystem.OnPlayerItemAdded -= this.OnItemAdded;
			this._handsSystem.OnPlayerItemRemoved -= this.OnItemRemoved;
			this._handsSystem.OnPlayerSetActiveHand -= this.SetActiveHand;
			this._handsSystem.OnPlayerRemoveHand -= this.RemoveHand;
			this._handsSystem.OnPlayerHandsAdded -= this.LoadPlayerHands;
			this._handsSystem.OnPlayerHandsRemoved -= this.UnloadPlayerHands;
			this._handsSystem.OnPlayerHandBlocked -= this.HandBlocked;
			this._handsSystem.OnPlayerHandUnblocked -= this.HandUnblocked;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x000120DC File Offset: 0x000102DC
		private void OnAddHand(string name, HandLocation location)
		{
			this.AddHand(name, location);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x000120E8 File Offset: 0x000102E8
		private void HandPressed(GUIBoundKeyEventArgs args, SlotControl hand)
		{
			if (this._playerHandsComponent == null)
			{
				return;
			}
			if (args.Function == EngineKeyFunctions.UIClick)
			{
				this._handsSystem.UIHandClick(this._playerHandsComponent, hand.SlotName);
				return;
			}
			if (args.Function == EngineKeyFunctions.UseSecondary)
			{
				this._handsSystem.UIHandOpenContextMenu(hand.SlotName);
				return;
			}
			if (args.Function == ContentKeyFunctions.ActivateItemInWorld)
			{
				this._handsSystem.UIHandActivate(hand.SlotName);
				return;
			}
			if (args.Function == ContentKeyFunctions.AltActivateItemInWorld)
			{
				this._handsSystem.UIHandAltActivateItem(hand.SlotName);
				return;
			}
			if (args.Function == ContentKeyFunctions.ExamineEntity)
			{
				this._handsSystem.UIInventoryExamine(hand.SlotName);
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x000121B8 File Offset: 0x000103B8
		private void UnloadPlayerHands()
		{
			if (this.HandsGui != null)
			{
				this.HandsGui.Visible = false;
			}
			this._handContainerIndices.Clear();
			this._handLookup.Clear();
			this._playerHandsComponent = null;
			foreach (HandsContainer handsContainer in this._handsContainers)
			{
				handsContainer.Clear();
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0001223C File Offset: 0x0001043C
		private void LoadPlayerHands(HandsComponent handsComp)
		{
			if (this.HandsGui != null)
			{
				this.HandsGui.Visible = true;
			}
			this._playerHandsComponent = handsComp;
			foreach (KeyValuePair<string, Hand> keyValuePair in handsComp.Hands)
			{
				string text;
				Hand hand;
				keyValuePair.Deconstruct(out text, out hand);
				string handName = text;
				Hand hand2 = hand;
				HandButton handButton = this.AddHand(handName, hand2.Location);
				HandVirtualItemComponent handVirtualItemComponent;
				if (this._entities.TryGetComponent<HandVirtualItemComponent>(hand2.HeldEntity, ref handVirtualItemComponent))
				{
					handButton.SpriteView.Sprite = EntityManagerExt.GetComponentOrNull<SpriteComponent>(this._entities, handVirtualItemComponent.BlockingEntity);
					handButton.Blocked = true;
				}
				else
				{
					handButton.SpriteView.Sprite = EntityManagerExt.GetComponentOrNull<SpriteComponent>(this._entities, hand2.HeldEntity);
					handButton.Blocked = false;
				}
			}
			Hand activeHand = handsComp.ActiveHand;
			if (activeHand == null)
			{
				return;
			}
			this.SetActiveHand(activeHand.Name);
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00012344 File Offset: 0x00010544
		private void HandBlocked(string handName)
		{
			HandButton handButton;
			if (!this._handLookup.TryGetValue(handName, out handButton))
			{
				return;
			}
			handButton.Blocked = true;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0001236C File Offset: 0x0001056C
		private void HandUnblocked(string handName)
		{
			HandButton handButton;
			if (!this._handLookup.TryGetValue(handName, out handButton))
			{
				return;
			}
			handButton.Blocked = false;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00012394 File Offset: 0x00010594
		private int GetHandContainerIndex(string containerName)
		{
			int result;
			if (!this._handContainerIndices.TryGetValue(containerName, out result))
			{
				return -1;
			}
			return result;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x000123B4 File Offset: 0x000105B4
		private void OnItemAdded(string name, EntityUid entity)
		{
			HandButton hand = this.GetHand(name);
			if (hand == null)
			{
				return;
			}
			HandVirtualItemComponent handVirtualItemComponent;
			if (this._entities.TryGetComponent<HandVirtualItemComponent>(entity, ref handVirtualItemComponent))
			{
				hand.SpriteView.Sprite = EntityManagerExt.GetComponentOrNull<SpriteComponent>(this._entities, handVirtualItemComponent.BlockingEntity);
				hand.Blocked = true;
			}
			else
			{
				hand.SpriteView.Sprite = EntityManagerExt.GetComponentOrNull<SpriteComponent>(this._entities, entity);
				hand.Blocked = false;
			}
			HandsComponent playerHandsComponent = this._playerHandsComponent;
			string a;
			if (playerHandsComponent == null)
			{
				a = null;
			}
			else
			{
				Hand activeHand = playerHandsComponent.ActiveHand;
				a = ((activeHand != null) ? activeHand.Name : null);
			}
			if (a == name)
			{
				HotbarGui handsGui = this.HandsGui;
				if (handsGui == null)
				{
					return;
				}
				handsGui.UpdatePanelEntity(new EntityUid?(entity));
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0001245C File Offset: 0x0001065C
		private void OnItemRemoved(string name, EntityUid entity)
		{
			HandButton hand = this.GetHand(name);
			if (hand == null)
			{
				return;
			}
			hand.SpriteView.Sprite = null;
			HandsComponent playerHandsComponent = this._playerHandsComponent;
			string a;
			if (playerHandsComponent == null)
			{
				a = null;
			}
			else
			{
				Hand activeHand = playerHandsComponent.ActiveHand;
				a = ((activeHand != null) ? activeHand.Name : null);
			}
			if (a == name)
			{
				HotbarGui handsGui = this.HandsGui;
				if (handsGui == null)
				{
					return;
				}
				handsGui.UpdatePanelEntity(null);
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x000124C0 File Offset: 0x000106C0
		private HandsContainer GetFirstAvailableContainer()
		{
			if (this._handsContainers.Count == 0)
			{
				throw new Exception("Could not find an attached hand hud container");
			}
			foreach (HandsContainer handsContainer in this._handsContainers)
			{
				if (!handsContainer.IsFull)
				{
					return handsContainer;
				}
			}
			throw new Exception("All attached hand hud containers were full!");
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0001253C File Offset: 0x0001073C
		public bool TryGetHandContainer(string containerName, [Nullable(2)] out HandsContainer container)
		{
			container = null;
			int handContainerIndex = this.GetHandContainerIndex(containerName);
			if (handContainerIndex == -1)
			{
				return false;
			}
			container = this._handsContainers[handContainerIndex];
			return true;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00012569 File Offset: 0x00010769
		private void StorageActivate(GUIBoundKeyEventArgs args, SlotControl handControl)
		{
			this._handsSystem.UIHandActivate(handControl.SlotName);
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0001257C File Offset: 0x0001077C
		[NullableContext(2)]
		private void SetActiveHand(string handName)
		{
			if (handName == null)
			{
				if (this._activeHand != null)
				{
					this._activeHand.Highlight = false;
				}
				HotbarGui handsGui = this.HandsGui;
				if (handsGui == null)
				{
					return;
				}
				handsGui.UpdatePanelEntity(null);
				return;
			}
			else
			{
				HandButton handButton;
				if (!this._handLookup.TryGetValue(handName, out handButton) || handButton == this._activeHand)
				{
					return;
				}
				if (this._activeHand != null)
				{
					this._activeHand.Highlight = false;
				}
				handButton.Highlight = true;
				this._activeHand = handButton;
				Hand hand;
				if (this.HandsGui != null && this._playerHandsComponent != null && this._playerHandsComponent.Hands.TryGetValue(handName, out hand))
				{
					this.HandsGui.UpdatePanelEntity(hand.HeldEntity);
				}
				return;
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0001262C File Offset: 0x0001082C
		[return: Nullable(2)]
		private HandButton GetHand(string handName)
		{
			HandButton result;
			this._handLookup.TryGetValue(handName, out result);
			return result;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001264C File Offset: 0x0001084C
		private HandButton AddHand(string handName, HandLocation location)
		{
			HandButton handButton = new HandButton(handName, location);
			handButton.StoragePressed += this.StorageActivate;
			handButton.Pressed += this.HandPressed;
			if (!this._handLookup.TryAdd(handName, handButton))
			{
				throw new Exception("Tried to add hand with duplicate name to UI. Name:" + handName);
			}
			if (this.HandsGui != null)
			{
				this.HandsGui.HandContainer.AddButton(handButton);
			}
			else
			{
				this.GetFirstAvailableContainer().AddButton(handButton);
			}
			return handButton;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x000126CF File Offset: 0x000108CF
		public void ReloadHands()
		{
			this.UnloadPlayerHands();
			this._handsSystem.ReloadHandButtons();
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000126E4 File Offset: 0x000108E4
		public void SwapHands(HandsContainer other, [Nullable(2)] HandsContainer source = null)
		{
			if (this.HandsGui == null && source == null)
			{
				throw new ArgumentException("Cannot swap hands if no source hand container exists!");
			}
			if (source == null)
			{
				source = this.HandsGui.HandContainer;
			}
			List<Control> list = new List<Control>();
			foreach (Control control in source.Children)
			{
				if (control is HandButton)
				{
					list.Add(control);
				}
			}
			foreach (Control control2 in list)
			{
				source.RemoveChild(control2);
				other.AddChild(control2);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000127B4 File Offset: 0x000109B4
		private void RemoveHand(string handName)
		{
			HandButton handButton;
			this.RemoveHand(handName, out handButton);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x000127CC File Offset: 0x000109CC
		private bool RemoveHand(string handName, [Nullable(2)] out HandButton handButton)
		{
			handButton = null;
			if (!this._handLookup.TryGetValue(handName, out handButton))
			{
				return false;
			}
			HandsContainer handsContainer = handButton.Parent as HandsContainer;
			if (handsContainer != null)
			{
				handsContainer.RemoveButton(handButton);
			}
			this._handLookup.Remove(handName);
			handButton.Dispose();
			return true;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0001281C File Offset: 0x00010A1C
		public string RegisterHandContainer(HandsContainer handContainer)
		{
			string text = "HandContainer_" + this._backupSuffix.ToString();
			if (handContainer.Indexer == null)
			{
				handContainer.Indexer = text;
				this._backupSuffix++;
			}
			else
			{
				text = handContainer.Indexer;
			}
			this._handContainerIndices.Add(text, this._handsContainers.Count);
			this._handsContainers.Add(handContainer);
			return text;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001288C File Offset: 0x00010A8C
		public bool RemoveHandContainer(string handContainerName)
		{
			int handContainerIndex = this.GetHandContainerIndex(handContainerName);
			if (handContainerIndex == -1)
			{
				return false;
			}
			this._handContainerIndices.Remove(handContainerName);
			this._handsContainers.RemoveAt(handContainerIndex);
			return true;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x000128C4 File Offset: 0x00010AC4
		public bool RemoveHandContainer(string handContainerName, [Nullable(2)] out HandsContainer container)
		{
			int index;
			bool result = this._handContainerIndices.TryGetValue(handContainerName, out index);
			container = this._handsContainers[index];
			this._handContainerIndices.Remove(handContainerName);
			this._handsContainers.RemoveAt(index);
			return result;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00012906 File Offset: 0x00010B06
		public void OnStateEntered(GameplayState state)
		{
			if (this.HandsGui != null)
			{
				this.HandsGui.Visible = (this._playerHandsComponent != null);
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00012924 File Offset: 0x00010B24
		public override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			foreach (HandsContainer handsContainer in this._handsContainers)
			{
				foreach (HandButton handButton in handsContainer.GetButtons())
				{
					ItemCooldownComponent itemCooldownComponent;
					if (this._entities.TryGetComponent<ItemCooldownComponent>(handButton.Entity, ref itemCooldownComponent) && itemCooldownComponent != null)
					{
						TimeSpan? cooldownStart = itemCooldownComponent.CooldownStart;
						if (cooldownStart != null)
						{
							TimeSpan valueOrDefault = cooldownStart.GetValueOrDefault();
							TimeSpan? cooldownEnd = itemCooldownComponent.CooldownEnd;
							if (cooldownEnd != null)
							{
								TimeSpan valueOrDefault2 = cooldownEnd.GetValueOrDefault();
								handButton.CooldownDisplay.Visible = true;
								handButton.CooldownDisplay.FromTime(valueOrDefault, valueOrDefault2);
								continue;
							}
						}
					}
					handButton.CooldownDisplay.Visible = false;
					return;
				}
			}
		}

		// Token: 0x04000176 RID: 374
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x04000177 RID: 375
		[UISystemDependency]
		private readonly HandsSystem _handsSystem;

		// Token: 0x04000178 RID: 376
		private readonly List<HandsContainer> _handsContainers = new List<HandsContainer>();

		// Token: 0x04000179 RID: 377
		private readonly Dictionary<string, int> _handContainerIndices = new Dictionary<string, int>();

		// Token: 0x0400017A RID: 378
		private readonly Dictionary<string, HandButton> _handLookup = new Dictionary<string, HandButton>();

		// Token: 0x0400017B RID: 379
		[Nullable(2)]
		private HandsComponent _playerHandsComponent;

		// Token: 0x0400017C RID: 380
		[Nullable(2)]
		private HandButton _activeHand;

		// Token: 0x0400017D RID: 381
		private int _backupSuffix;
	}
}
