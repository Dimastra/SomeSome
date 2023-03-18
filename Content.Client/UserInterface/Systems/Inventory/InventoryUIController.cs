using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Gameplay;
using Content.Client.Hands;
using Content.Client.Hands.Systems;
using Content.Client.Inventory;
using Content.Client.Storage;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Content.Client.UserInterface.Systems.Inventory.Windows;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Players;

namespace Content.Client.UserInterface.Systems.Inventory
{
	// Token: 0x02000077 RID: 119
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InventoryUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<ClientInventorySystem>, IOnSystemLoaded<ClientInventorySystem>, IOnSystemUnloaded<ClientInventorySystem>, IOnSystemChanged<HandsSystem>, IOnSystemLoaded<HandsSystem>, IOnSystemUnloaded<HandsSystem>
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000262 RID: 610 RVA: 0x00010235 File Offset: 0x0000E435
		[Nullable(2)]
		private MenuButton InventoryButton
		{
			[NullableContext(2)]
			get
			{
				UIScreen activeScreen = this.UIManager.ActiveScreen;
				if (activeScreen == null)
				{
					return null;
				}
				GameTopMenuBar widget = activeScreen.GetWidget<GameTopMenuBar>();
				if (widget == null)
				{
					return null;
				}
				return widget.InventoryButton;
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00010258 File Offset: 0x0000E458
		public void OnStateEntered(GameplayState state)
		{
			this._strippingWindow = this.UIManager.CreateWindow<StrippingWindow>();
			LayoutContainer.SetAnchorPreset(this._strippingWindow, 8, false);
			CommandBinds.Builder.Bind(ContentKeyFunctions.OpenInventoryMenu, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.ToggleInventoryBar();
			}, null, true, true)).Register<ClientInventorySystem>();
		}

		// Token: 0x06000264 RID: 612 RVA: 0x000102AC File Offset: 0x0000E4AC
		public void OnStateExited(GameplayState state)
		{
			if (this._strippingWindow != null)
			{
				this._strippingWindow.Dispose();
				this._strippingWindow = null;
			}
			if (this._inventoryHotbar != null)
			{
				this._inventoryHotbar.Visible = false;
			}
			CommandBinds.Unregister<ClientInventorySystem>();
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000102E1 File Offset: 0x0000E4E1
		public void UnloadButton()
		{
			if (this.InventoryButton == null)
			{
				return;
			}
			this.InventoryButton.OnPressed -= this.InventoryButtonPressed;
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00010303 File Offset: 0x0000E503
		public void LoadButton()
		{
			if (this.InventoryButton == null)
			{
				return;
			}
			this.InventoryButton.OnPressed += this.InventoryButtonPressed;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00010325 File Offset: 0x0000E525
		private SlotButton CreateSlotButton(ClientInventorySystem.SlotData data)
		{
			SlotButton slotButton = new SlotButton(data);
			slotButton.Pressed += this.ItemPressed;
			slotButton.StoragePressed += this.StoragePressed;
			slotButton.Hover += this.SlotButtonHovered;
			return slotButton;
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00010363 File Offset: 0x0000E563
		public void RegisterInventoryBarContainer(ItemSlotButtonContainer inventoryHotbar)
		{
			this._inventoryHotbar = inventoryHotbar;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0001036C File Offset: 0x0000E56C
		private void InventoryButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this.ToggleInventoryBar();
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00010374 File Offset: 0x0000E574
		[NullableContext(2)]
		private void UpdateInventoryHotbar(ClientInventoryComponent clientInv)
		{
			if (clientInv != null)
			{
				foreach (KeyValuePair<string, ClientInventorySystem.SlotData> keyValuePair in clientInv.SlotData)
				{
					string text;
					ClientInventorySystem.SlotData slotData;
					keyValuePair.Deconstruct(out text, out slotData);
					ClientInventorySystem.SlotData slotData2 = slotData;
					ItemSlotButtonContainer itemSlotButtonContainer;
					if (slotData2.ShowInWindow && this._slotGroups.TryGetValue(slotData2.SlotGroup, out itemSlotButtonContainer))
					{
						SlotControl newButton;
						if (!itemSlotButtonContainer.TryGetButton(slotData2.SlotName, out newButton))
						{
							newButton = this.CreateSlotButton(slotData2);
							itemSlotButtonContainer.AddButton(newButton);
						}
						SpriteComponent componentOrNull = EntityManagerExt.GetComponentOrNull<SpriteComponent>(this._entities, slotData2.HeldEntity);
						bool showStorage = this._entities.HasComponent<ClientStorageComponent>(slotData2.HeldEntity);
						ClientInventorySystem.SlotSpriteUpdate update = new ClientInventorySystem.SlotSpriteUpdate(slotData2.SlotGroup, slotData2.SlotName, componentOrNull, showStorage);
						this.SpriteUpdated(update);
					}
				}
				return;
			}
			ItemSlotButtonContainer inventoryHotbar = this._inventoryHotbar;
			if (inventoryHotbar == null)
			{
				return;
			}
			inventoryHotbar.ClearButtons();
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00010470 File Offset: 0x0000E670
		[NullableContext(2)]
		private void UpdateStrippingWindow(ClientInventoryComponent clientInv)
		{
			if (clientInv == null)
			{
				this._strippingWindow.InventoryButtons.ClearButtons();
				return;
			}
			foreach (KeyValuePair<string, ClientInventorySystem.SlotData> keyValuePair in clientInv.SlotData)
			{
				string text;
				ClientInventorySystem.SlotData slotData;
				keyValuePair.Deconstruct(out text, out slotData);
				ClientInventorySystem.SlotData slotData2 = slotData;
				if (slotData2.ShowInWindow)
				{
					SlotControl newButton;
					if (!this._strippingWindow.InventoryButtons.TryGetButton(slotData2.SlotName, out newButton))
					{
						newButton = this.CreateSlotButton(slotData2);
						this._strippingWindow.InventoryButtons.AddButton(newButton, slotData2.ButtonOffset);
					}
					SpriteComponent componentOrNull = EntityManagerExt.GetComponentOrNull<SpriteComponent>(this._entities, slotData2.HeldEntity);
					bool showStorage = this._entities.HasComponent<ClientStorageComponent>(slotData2.HeldEntity);
					ClientInventorySystem.SlotSpriteUpdate update = new ClientInventorySystem.SlotSpriteUpdate(slotData2.SlotGroup, slotData2.SlotName, componentOrNull, showStorage);
					this.SpriteUpdated(update);
				}
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00010570 File Offset: 0x0000E770
		public void ToggleStrippingMenu()
		{
			this.UpdateStrippingWindow(this._playerInventory);
			if (this._strippingWindow.IsOpen)
			{
				this._strippingWindow.Close();
				return;
			}
			this._strippingWindow.Open();
		}

		// Token: 0x0600026D RID: 621 RVA: 0x000105A4 File Offset: 0x0000E7A4
		public void ToggleInventoryBar()
		{
			if (this._inventoryHotbar == null)
			{
				Logger.Warning("Tried to toggle inventory bar when none are assigned");
				return;
			}
			this.UpdateInventoryHotbar(this._playerInventory);
			if (this._inventoryHotbar.Visible)
			{
				this._inventoryHotbar.Visible = false;
				if (this.InventoryButton != null)
				{
					this.InventoryButton.Pressed = false;
					return;
				}
			}
			else
			{
				this._inventoryHotbar.Visible = true;
				if (this.InventoryButton != null)
				{
					this.InventoryButton.Pressed = true;
				}
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00010620 File Offset: 0x0000E820
		public void OnSystemLoaded(ClientInventorySystem system)
		{
			ClientInventorySystem inventorySystem = this._inventorySystem;
			inventorySystem.OnSlotAdded = (Action<ClientInventorySystem.SlotData>)Delegate.Combine(inventorySystem.OnSlotAdded, new Action<ClientInventorySystem.SlotData>(this.AddSlot));
			ClientInventorySystem inventorySystem2 = this._inventorySystem;
			inventorySystem2.OnSlotRemoved = (Action<ClientInventorySystem.SlotData>)Delegate.Combine(inventorySystem2.OnSlotRemoved, new Action<ClientInventorySystem.SlotData>(this.RemoveSlot));
			ClientInventorySystem inventorySystem3 = this._inventorySystem;
			inventorySystem3.OnLinkInventory = (Action<ClientInventoryComponent>)Delegate.Combine(inventorySystem3.OnLinkInventory, new Action<ClientInventoryComponent>(this.LoadSlots));
			ClientInventorySystem inventorySystem4 = this._inventorySystem;
			inventorySystem4.OnUnlinkInventory = (Action)Delegate.Combine(inventorySystem4.OnUnlinkInventory, new Action(this.UnloadSlots));
			ClientInventorySystem inventorySystem5 = this._inventorySystem;
			inventorySystem5.OnSpriteUpdate = (Action<ClientInventorySystem.SlotSpriteUpdate>)Delegate.Combine(inventorySystem5.OnSpriteUpdate, new Action<ClientInventorySystem.SlotSpriteUpdate>(this.SpriteUpdated));
		}

		// Token: 0x0600026F RID: 623 RVA: 0x000106F0 File Offset: 0x0000E8F0
		public void OnSystemUnloaded(ClientInventorySystem system)
		{
			ClientInventorySystem inventorySystem = this._inventorySystem;
			inventorySystem.OnSlotAdded = (Action<ClientInventorySystem.SlotData>)Delegate.Remove(inventorySystem.OnSlotAdded, new Action<ClientInventorySystem.SlotData>(this.AddSlot));
			ClientInventorySystem inventorySystem2 = this._inventorySystem;
			inventorySystem2.OnSlotRemoved = (Action<ClientInventorySystem.SlotData>)Delegate.Remove(inventorySystem2.OnSlotRemoved, new Action<ClientInventorySystem.SlotData>(this.RemoveSlot));
			ClientInventorySystem inventorySystem3 = this._inventorySystem;
			inventorySystem3.OnLinkInventory = (Action<ClientInventoryComponent>)Delegate.Remove(inventorySystem3.OnLinkInventory, new Action<ClientInventoryComponent>(this.LoadSlots));
			ClientInventorySystem inventorySystem4 = this._inventorySystem;
			inventorySystem4.OnUnlinkInventory = (Action)Delegate.Remove(inventorySystem4.OnUnlinkInventory, new Action(this.UnloadSlots));
			ClientInventorySystem inventorySystem5 = this._inventorySystem;
			inventorySystem5.OnSpriteUpdate = (Action<ClientInventorySystem.SlotSpriteUpdate>)Delegate.Remove(inventorySystem5.OnSpriteUpdate, new Action<ClientInventorySystem.SlotSpriteUpdate>(this.SpriteUpdated));
		}

		// Token: 0x06000270 RID: 624 RVA: 0x000107C0 File Offset: 0x0000E9C0
		private void ItemPressed(GUIBoundKeyEventArgs args, SlotControl control)
		{
			string slotName = control.SlotName;
			if (args.Function == EngineKeyFunctions.UIClick)
			{
				this._inventorySystem.UIInventoryActivate(control.SlotName);
				return;
			}
			if (this._playerInventory == null)
			{
				return;
			}
			if (args.Function == ContentKeyFunctions.ExamineEntity)
			{
				this._inventorySystem.UIInventoryExamine(slotName, this._playerInventory.Owner);
				return;
			}
			if (args.Function == EngineKeyFunctions.UseSecondary)
			{
				this._inventorySystem.UIInventoryOpenContextMenu(slotName, this._playerInventory.Owner);
				return;
			}
			if (args.Function == ContentKeyFunctions.ActivateItemInWorld)
			{
				this._inventorySystem.UIInventoryActivateItem(slotName, this._playerInventory.Owner);
				return;
			}
			if (args.Function == ContentKeyFunctions.AltActivateItemInWorld)
			{
				this._inventorySystem.UIInventoryAltActivateItem(slotName, this._playerInventory.Owner);
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x000108A8 File Offset: 0x0000EAA8
		private void StoragePressed(GUIBoundKeyEventArgs args, SlotControl control)
		{
			this._inventorySystem.UIInventoryStorageActivate(control.SlotName);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x000108BB File Offset: 0x0000EABB
		private void SlotButtonHovered(GUIMouseHoverEventArgs args, SlotControl control)
		{
			this.UpdateHover(control);
			this._lastHovered = control;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x000108CC File Offset: 0x0000EACC
		public void UpdateHover(SlotControl control)
		{
			ClientInventoryComponent playerInventory = this._playerInventory;
			EntityUid? entityUid = (playerInventory != null) ? new EntityUid?(playerInventory.Owner) : null;
			HandsComponent handsComponent;
			if (control.MouseIsHovering && this._playerInventory != null && this._entities.TryGetComponent<HandsComponent>(entityUid, ref handsComponent))
			{
				EntityUid? activeHandEntity = handsComponent.ActiveHandEntity;
				if (activeHandEntity != null)
				{
					EntityUid valueOrDefault = activeHandEntity.GetValueOrDefault();
					SpriteComponent spriteComponent;
					ContainerSlot containerSlot;
					SlotDefinition slotDefinition;
					if (this._entities.TryGetComponent<SpriteComponent>(valueOrDefault, ref spriteComponent) && this._inventorySystem.TryGetSlotContainer(entityUid.Value, control.SlotName, out containerSlot, out slotDefinition, this._playerInventory, null))
					{
						EntityUid entityUid2 = this._entities.SpawnEntity("hoverentity", MapCoordinates.Nullspace);
						SpriteComponent component = this._entities.GetComponent<SpriteComponent>(entityUid2);
						string text;
						bool flag = this._inventorySystem.CanEquip(entityUid.Value, valueOrDefault, control.SlotName, out text, slotDefinition, this._playerInventory, null, null) && containerSlot.CanInsert(valueOrDefault, this._entities);
						component.CopyFrom(spriteComponent);
						component.Color = (flag ? new Color(0, byte.MaxValue, 0, 127) : new Color(byte.MaxValue, 0, 0, 127));
						control.HoverSpriteView.Sprite = component;
						return;
					}
				}
			}
			control.ClearHover();
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00010A10 File Offset: 0x0000EC10
		private void AddSlot(ClientInventorySystem.SlotData data)
		{
			ItemSlotButtonContainer itemSlotButtonContainer;
			if (!this._slotGroups.TryGetValue(data.SlotGroup, out itemSlotButtonContainer))
			{
				return;
			}
			SlotButton newButton = this.CreateSlotButton(data);
			itemSlotButtonContainer.AddButton(newButton);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00010A44 File Offset: 0x0000EC44
		private void RemoveSlot(ClientInventorySystem.SlotData data)
		{
			ItemSlotButtonContainer itemSlotButtonContainer;
			if (!this._slotGroups.TryGetValue(data.SlotGroup, out itemSlotButtonContainer))
			{
				return;
			}
			itemSlotButtonContainer.RemoveButton(data.SlotName);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00010A73 File Offset: 0x0000EC73
		public void ReloadSlots()
		{
			this._inventorySystem.ReloadInventory(null);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00010A84 File Offset: 0x0000EC84
		private void LoadSlots(ClientInventoryComponent clientInv)
		{
			this.UnloadSlots();
			this._playerInventory = clientInv;
			foreach (ClientInventorySystem.SlotData data in clientInv.SlotData.Values)
			{
				this.AddSlot(data);
			}
			this.UpdateInventoryHotbar(this._playerInventory);
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00010AF8 File Offset: 0x0000ECF8
		private void UnloadSlots()
		{
			this._playerInventory = null;
			foreach (ItemSlotButtonContainer itemSlotButtonContainer in this._slotGroups.Values)
			{
				itemSlotButtonContainer.ClearButtons();
			}
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00010B54 File Offset: 0x0000ED54
		private void SpriteUpdated(ClientInventorySystem.SlotSpriteUpdate update)
		{
			ClientInventorySystem.SlotSpriteUpdate slotSpriteUpdate = update;
			string text;
			string text2;
			SpriteComponent spriteComponent;
			bool flag;
			slotSpriteUpdate.Deconstruct(out text, out text2, out spriteComponent, out flag);
			string key = text;
			string slotName = text2;
			SpriteComponent sprite = spriteComponent;
			bool visible = flag;
			StrippingWindow strippingWindow = this._strippingWindow;
			SlotControl slotControl = (strippingWindow != null) ? strippingWindow.InventoryButtons.GetButton(update.Name) : null;
			if (slotControl != null)
			{
				slotControl.SpriteView.Sprite = sprite;
				slotControl.StorageButton.Visible = visible;
			}
			ItemSlotButtonContainer valueOrDefault = this._slotGroups.GetValueOrDefault(key);
			SlotControl slotControl2 = (valueOrDefault != null) ? valueOrDefault.GetButton(slotName) : null;
			if (slotControl2 == null)
			{
				return;
			}
			slotControl2.SpriteView.Sprite = sprite;
			slotControl2.StorageButton.Visible = visible;
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00010BF7 File Offset: 0x0000EDF7
		public bool RegisterSlotGroupContainer(ItemSlotButtonContainer slotContainer)
		{
			return this._slotGroups.TryAdd(slotContainer.SlotGroup, slotContainer);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00010C10 File Offset: 0x0000EE10
		public void RemoveSlotGroup(string slotGroupName)
		{
			this._slotGroups.Remove(slotGroupName);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00010C20 File Offset: 0x0000EE20
		public void OnSystemLoaded(HandsSystem system)
		{
			this._handsSystem.OnPlayerItemAdded += this.OnItemAdded;
			this._handsSystem.OnPlayerItemRemoved += this.OnItemRemoved;
			this._handsSystem.OnPlayerSetActiveHand += this.SetActiveHand;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00010C74 File Offset: 0x0000EE74
		public void OnSystemUnloaded(HandsSystem system)
		{
			this._handsSystem.OnPlayerItemAdded -= this.OnItemAdded;
			this._handsSystem.OnPlayerItemRemoved -= this.OnItemRemoved;
			this._handsSystem.OnPlayerSetActiveHand -= this.SetActiveHand;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00010CC6 File Offset: 0x0000EEC6
		private void OnItemAdded(string name, EntityUid entity)
		{
			if (this._lastHovered != null)
			{
				this.UpdateHover(this._lastHovered);
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00010CC6 File Offset: 0x0000EEC6
		private void OnItemRemoved(string name, EntityUid entity)
		{
			if (this._lastHovered != null)
			{
				this.UpdateHover(this._lastHovered);
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00010CC6 File Offset: 0x0000EEC6
		[NullableContext(2)]
		private void SetActiveHand(string handName)
		{
			if (this._lastHovered != null)
			{
				this.UpdateHover(this._lastHovered);
			}
		}

		// Token: 0x0400015F RID: 351
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x04000160 RID: 352
		[UISystemDependency]
		private readonly ClientInventorySystem _inventorySystem;

		// Token: 0x04000161 RID: 353
		[UISystemDependency]
		private readonly HandsSystem _handsSystem;

		// Token: 0x04000162 RID: 354
		[Nullable(2)]
		private ClientInventoryComponent _playerInventory;

		// Token: 0x04000163 RID: 355
		private readonly Dictionary<string, ItemSlotButtonContainer> _slotGroups = new Dictionary<string, ItemSlotButtonContainer>();

		// Token: 0x04000164 RID: 356
		[Nullable(2)]
		private StrippingWindow _strippingWindow;

		// Token: 0x04000165 RID: 357
		[Nullable(2)]
		private ItemSlotButtonContainer _inventoryHotbar;

		// Token: 0x04000166 RID: 358
		[Nullable(2)]
		private SlotControl _lastHovered;
	}
}
