using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Client.Clothing;
using Content.Client.Examine;
using Content.Client.Storage;
using Content.Client.UserInterface.Controls;
using Content.Client.Verbs.UI;
using Content.Shared.Clothing.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Inventory
{
	// Token: 0x020002A4 RID: 676
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClientInventorySystem : InventorySystem
	{
		// Token: 0x060010FA RID: 4346 RVA: 0x00065588 File Offset: 0x00063788
		public override void Initialize()
		{
			base.UpdatesOutsidePrediction = true;
			base.Initialize();
			base.SubscribeLocalEvent<ClientInventoryComponent, PlayerAttachedEvent>(new ComponentEventHandler<ClientInventoryComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<ClientInventoryComponent, PlayerDetachedEvent>(new ComponentEventHandler<ClientInventoryComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<ClientInventoryComponent, ComponentShutdown>(new ComponentEventHandler<ClientInventoryComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<ClientInventoryComponent, DidEquipEvent>(delegate(EntityUid _, ClientInventoryComponent comp, DidEquipEvent args)
			{
				this._equipEventsQueue.Enqueue(new ValueTuple<ClientInventoryComponent, EntityEventArgs>(comp, args));
			}, null, null);
			base.SubscribeLocalEvent<ClientInventoryComponent, DidUnequipEvent>(delegate(EntityUid _, ClientInventoryComponent comp, DidUnequipEvent args)
			{
				this._equipEventsQueue.Enqueue(new ValueTuple<ClientInventoryComponent, EntityEventArgs>(comp, args));
			}, null, null);
			base.SubscribeLocalEvent<ClothingComponent, UseInHandEvent>(new ComponentEventHandler<ClothingComponent, UseInHandEvent>(this.OnUseInHand), null, null);
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0006561C File Offset: 0x0006381C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			ValueTuple<ClientInventoryComponent, EntityEventArgs> valueTuple;
			while (this._equipEventsQueue.TryDequeue(out valueTuple))
			{
				ValueTuple<ClientInventoryComponent, EntityEventArgs> valueTuple2 = valueTuple;
				ClientInventoryComponent item = valueTuple2.Item1;
				EntityEventArgs item2 = valueTuple2.Item2;
				DidEquipEvent didEquipEvent = item2 as DidEquipEvent;
				if (didEquipEvent == null)
				{
					DidUnequipEvent didUnequipEvent = item2 as DidUnequipEvent;
					if (didUnequipEvent == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Received queued event of unknown type: ");
						defaultInterpolatedStringHandler.AppendFormatted<Type>(item2.GetType());
						throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					this.OnDidUnequip(item, didUnequipEvent);
				}
				else
				{
					this.OnDidEquip(item, didEquipEvent);
				}
			}
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x000656AA File Offset: 0x000638AA
		private void OnUseInHand(EntityUid uid, ClothingComponent component, UseInHandEvent args)
		{
			if (args.Handled || !component.QuickEquip)
			{
				return;
			}
			base.QuickEquip(uid, component, args);
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x000656C8 File Offset: 0x000638C8
		private void OnDidUnequip(ClientInventoryComponent component, DidUnequipEvent args)
		{
			this.UpdateSlot(args.Equipee, component, args.Slot, null, null);
			EntityUid equipee = args.Equipee;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (equipee != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			ClientInventorySystem.SlotSpriteUpdate obj = new ClientInventorySystem.SlotSpriteUpdate(args.SlotGroup, args.Slot, null, false);
			Action<ClientInventorySystem.SlotSpriteUpdate> onSpriteUpdate = this.OnSpriteUpdate;
			if (onSpriteUpdate == null)
			{
				return;
			}
			onSpriteUpdate(obj);
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x00065764 File Offset: 0x00063964
		private void OnDidEquip(ClientInventoryComponent component, DidEquipEvent args)
		{
			this.UpdateSlot(args.Equipee, component, args.Slot, null, null);
			EntityUid equipee = args.Equipee;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (equipee != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			SpriteComponent componentOrNull = EntityManagerExt.GetComponentOrNull<SpriteComponent>(this.EntityManager, args.Equipment);
			ClientInventorySystem.SlotSpriteUpdate obj = new ClientInventorySystem.SlotSpriteUpdate(args.SlotGroup, args.Slot, componentOrNull, base.HasComp<ClientStorageComponent>(args.Equipment));
			Action<ClientInventorySystem.SlotSpriteUpdate> onSpriteUpdate = this.OnSpriteUpdate;
			if (onSpriteUpdate == null)
			{
				return;
			}
			onSpriteUpdate(obj);
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00065820 File Offset: 0x00063A20
		private void OnShutdown(EntityUid uid, ClientInventoryComponent component, ComponentShutdown args)
		{
			EntityUid owner = component.Owner;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (owner != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			Action onUnlinkInventory = this.OnUnlinkInventory;
			if (onUnlinkInventory == null)
			{
				return;
			}
			onUnlinkInventory();
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x00065880 File Offset: 0x00063A80
		private void OnPlayerDetached(EntityUid uid, ClientInventoryComponent component, PlayerDetachedEvent args)
		{
			Action onUnlinkInventory = this.OnUnlinkInventory;
			if (onUnlinkInventory == null)
			{
				return;
			}
			onUnlinkInventory();
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x00065894 File Offset: 0x00063A94
		private void OnPlayerAttached(EntityUid uid, ClientInventoryComponent component, PlayerAttachedEvent args)
		{
			SlotDefinition[] array;
			if (base.TryGetSlots(uid, out array, null))
			{
				foreach (SlotDefinition slotDefinition in array)
				{
					ContainerSlot container;
					SlotDefinition slotDefinition2;
					if (base.TryGetSlotContainer(uid, slotDefinition.Name, out container, out slotDefinition2, component, null))
					{
						ClientInventorySystem.SlotData slotData;
						if (!component.SlotData.TryGetValue(slotDefinition.Name, out slotData))
						{
							slotData = new ClientInventorySystem.SlotData(slotDefinition, null, false, false);
							component.SlotData[slotDefinition.Name] = slotData;
						}
						slotData.Container = container;
					}
				}
			}
			Action<ClientInventoryComponent> onLinkInventory = this.OnLinkInventory;
			if (onLinkInventory == null)
			{
				return;
			}
			onLinkInventory(component);
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x00065923 File Offset: 0x00063B23
		public override void Shutdown()
		{
			CommandBinds.Unregister<ClientInventorySystem>();
			base.Shutdown();
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x00065930 File Offset: 0x00063B30
		protected override void OnInit(EntityUid uid, InventoryComponent component, ComponentInit args)
		{
			base.OnInit(uid, component, args);
			this._clothingVisualsSystem.InitClothing(uid, (ClientInventoryComponent)component, null);
			InventoryTemplatePrototype inventoryTemplatePrototype;
			if (!this._prototypeManager.TryIndex<InventoryTemplatePrototype>(component.TemplateId, ref inventoryTemplatePrototype))
			{
				return;
			}
			foreach (SlotDefinition newSlotDef in inventoryTemplatePrototype.Slots)
			{
				this.TryAddSlotDef(uid, (ClientInventoryComponent)component, newSlotDef);
			}
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x00065998 File Offset: 0x00063B98
		[NullableContext(2)]
		public void ReloadInventory(ClientInventoryComponent component = null)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null || !base.Resolve<ClientInventoryComponent>(entityUid.Value, ref component, false))
			{
				return;
			}
			Action onUnlinkInventory = this.OnUnlinkInventory;
			if (onUnlinkInventory != null)
			{
				onUnlinkInventory();
			}
			Action<ClientInventoryComponent> onLinkInventory = this.OnLinkInventory;
			if (onLinkInventory == null)
			{
				return;
			}
			onLinkInventory(component);
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x00065A04 File Offset: 0x00063C04
		public void SetSlotHighlight(EntityUid owner, ClientInventoryComponent component, string slotName, bool state)
		{
			ClientInventorySystem.SlotData oldData = component.SlotData[slotName];
			ClientInventorySystem.SlotData obj = component.SlotData[slotName] = new ClientInventorySystem.SlotData(oldData, state, false);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (owner == ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				Action<ClientInventorySystem.SlotData> entitySlotUpdate = this.EntitySlotUpdate;
				if (entitySlotUpdate == null)
				{
					return;
				}
				entitySlotUpdate(obj);
			}
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x00065A88 File Offset: 0x00063C88
		public void UpdateSlot(EntityUid owner, ClientInventoryComponent component, string slotName, bool? blocked = null, bool? highlight = null)
		{
			ClientInventorySystem.SlotData slotData = component.SlotData[slotName];
			bool highlighted = slotData.Highlighted;
			bool blocked2 = slotData.Blocked;
			if (blocked != null)
			{
				blocked2 = blocked.Value;
			}
			if (highlight != null)
			{
				highlighted = highlight.Value;
			}
			ClientInventorySystem.SlotData obj = component.SlotData[slotName] = new ClientInventorySystem.SlotData(component.SlotData[slotName], highlighted, blocked2);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (owner == ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				Action<ClientInventorySystem.SlotData> entitySlotUpdate = this.EntitySlotUpdate;
				if (entitySlotUpdate == null)
				{
					return;
				}
				entitySlotUpdate(obj);
			}
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x00065B48 File Offset: 0x00063D48
		public bool TryAddSlotDef(EntityUid owner, ClientInventoryComponent component, SlotDefinition newSlotDef)
		{
			ClientInventorySystem.SlotData slotData = newSlotDef;
			if (!component.SlotData.TryAdd(newSlotDef.Name, slotData))
			{
				return false;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (owner == ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				Action<ClientInventorySystem.SlotData> onSlotAdded = this.OnSlotAdded;
				if (onSlotAdded != null)
				{
					onSlotAdded(slotData);
				}
			}
			return true;
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x00065BC4 File Offset: 0x00063DC4
		public void RemoveSlotDef(EntityUid owner, ClientInventoryComponent component, ClientInventorySystem.SlotData slotData)
		{
			if (component.SlotData.Remove(slotData.SlotName))
			{
				LocalPlayer localPlayer = this._playerManager.LocalPlayer;
				if (owner == ((localPlayer != null) ? localPlayer.ControlledEntity : null))
				{
					Action<ClientInventorySystem.SlotData> onSlotRemoved = this.OnSlotRemoved;
					if (onSlotRemoved == null)
					{
						return;
					}
					onSlotRemoved(slotData);
				}
			}
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00065C34 File Offset: 0x00063E34
		public void RemoveSlotDef(EntityUid owner, ClientInventoryComponent component, string slotName)
		{
			ClientInventorySystem.SlotData obj;
			if (!component.SlotData.TryGetValue(slotName, out obj))
			{
				return;
			}
			component.SlotData.Remove(slotName);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (owner == ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				Action<ClientInventorySystem.SlotData> onSlotRemoved = this.OnSlotRemoved;
				if (onSlotRemoved == null)
				{
					return;
				}
				onSlotRemoved(obj);
			}
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x00065CB0 File Offset: 0x00063EB0
		private void HoverInSlotButton(EntityUid uid, string slot, SlotControl control, [Nullable(2)] InventoryComponent inventoryComponent = null, [Nullable(2)] SharedHandsComponent hands = null)
		{
			if (!base.Resolve<InventoryComponent>(uid, ref inventoryComponent, true))
			{
				return;
			}
			if (!base.Resolve<SharedHandsComponent>(uid, ref hands, false))
			{
				return;
			}
			EntityUid? activeHandEntity = hands.ActiveHandEntity;
			if (activeHandEntity != null)
			{
				activeHandEntity.GetValueOrDefault();
				ContainerSlot containerSlot;
				SlotDefinition slotDefinition;
				base.TryGetSlotContainer(uid, slot, out containerSlot, out slotDefinition, inventoryComponent, null);
				return;
			}
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00065D03 File Offset: 0x00063F03
		public void UIInventoryActivate(string slot)
		{
			this.EntityManager.RaisePredictiveEvent<UseSlotNetworkMessage>(new UseSlotNetworkMessage(slot));
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x00065D16 File Offset: 0x00063F16
		public void UIInventoryStorageActivate(string slot)
		{
			IEntityNetworkManager entityNetManager = this.EntityManager.EntityNetManager;
			if (entityNetManager == null)
			{
				return;
			}
			entityNetManager.SendSystemNetworkMessage(new OpenSlotStorageNetworkMessage(slot), true);
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x00065D34 File Offset: 0x00063F34
		public void UIInventoryExamine(string slot, EntityUid uid)
		{
			EntityUid? entityUid;
			if (!base.TryGetSlotEntity(uid, slot, out entityUid, null, null))
			{
				return;
			}
			this._examine.DoExamine(entityUid.Value, true);
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x00065D64 File Offset: 0x00063F64
		public void UIInventoryOpenContextMenu(string slot, EntityUid uid)
		{
			EntityUid? entityUid;
			if (!base.TryGetSlotEntity(uid, slot, out entityUid, null, null))
			{
				return;
			}
			this._ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(entityUid.Value, false, null);
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x00065D9C File Offset: 0x00063F9C
		public void UIInventoryActivateItem(string slot, EntityUid uid)
		{
			EntityUid? entityUid;
			if (!base.TryGetSlotEntity(uid, slot, out entityUid, null, null))
			{
				return;
			}
			this.EntityManager.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(entityUid.Value, false));
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x00065DD0 File Offset: 0x00063FD0
		public void UIInventoryAltActivateItem(string slot, EntityUid uid)
		{
			EntityUid? entityUid;
			if (!base.TryGetSlotEntity(uid, slot, out entityUid, null, null))
			{
				return;
			}
			this.EntityManager.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(entityUid.Value, true));
		}

		// Token: 0x04000853 RID: 2131
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000854 RID: 2132
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000855 RID: 2133
		[Dependency]
		private readonly IUserInterfaceManager _ui;

		// Token: 0x04000856 RID: 2134
		[Dependency]
		private readonly ClientClothingSystem _clothingVisualsSystem;

		// Token: 0x04000857 RID: 2135
		[Dependency]
		private readonly ExamineSystem _examine;

		// Token: 0x04000858 RID: 2136
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<ClientInventorySystem.SlotData> EntitySlotUpdate;

		// Token: 0x04000859 RID: 2137
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<ClientInventorySystem.SlotData> OnSlotAdded;

		// Token: 0x0400085A RID: 2138
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<ClientInventorySystem.SlotData> OnSlotRemoved;

		// Token: 0x0400085B RID: 2139
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<ClientInventoryComponent> OnLinkInventory;

		// Token: 0x0400085C RID: 2140
		[Nullable(2)]
		public Action OnUnlinkInventory;

		// Token: 0x0400085D RID: 2141
		[Nullable(2)]
		public Action<ClientInventorySystem.SlotSpriteUpdate> OnSpriteUpdate;

		// Token: 0x0400085E RID: 2142
		[TupleElementNames(new string[]
		{
			"comp",
			"args"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		private readonly Queue<ValueTuple<ClientInventoryComponent, EntityEventArgs>> _equipEventsQueue = new Queue<ValueTuple<ClientInventoryComponent, EntityEventArgs>>();

		// Token: 0x020002A5 RID: 677
		[Nullable(0)]
		public sealed class SlotData
		{
			// Token: 0x170003A8 RID: 936
			// (get) Token: 0x06001114 RID: 4372 RVA: 0x00065E2C File Offset: 0x0006402C
			public EntityUid? HeldEntity
			{
				get
				{
					ContainerSlot container = this.Container;
					if (container == null)
					{
						return null;
					}
					return container.ContainedEntity;
				}
			}

			// Token: 0x170003A9 RID: 937
			// (get) Token: 0x06001115 RID: 4373 RVA: 0x00065E52 File Offset: 0x00064052
			public bool HasSlotGroup
			{
				get
				{
					return this.SlotDef.SlotGroup != "Default";
				}
			}

			// Token: 0x170003AA RID: 938
			// (get) Token: 0x06001116 RID: 4374 RVA: 0x00065E69 File Offset: 0x00064069
			public Vector2i ButtonOffset
			{
				get
				{
					return this.SlotDef.UIWindowPosition;
				}
			}

			// Token: 0x170003AB RID: 939
			// (get) Token: 0x06001117 RID: 4375 RVA: 0x00065E76 File Offset: 0x00064076
			public string SlotName
			{
				get
				{
					return this.SlotDef.Name;
				}
			}

			// Token: 0x170003AC RID: 940
			// (get) Token: 0x06001118 RID: 4376 RVA: 0x00065E83 File Offset: 0x00064083
			public bool ShowInWindow
			{
				get
				{
					return this.SlotDef.ShowInWindow;
				}
			}

			// Token: 0x170003AD RID: 941
			// (get) Token: 0x06001119 RID: 4377 RVA: 0x00065E90 File Offset: 0x00064090
			public string SlotGroup
			{
				get
				{
					return this.SlotDef.SlotGroup;
				}
			}

			// Token: 0x170003AE RID: 942
			// (get) Token: 0x0600111A RID: 4378 RVA: 0x00065E9D File Offset: 0x0006409D
			public string SlotDisplayName
			{
				get
				{
					return this.SlotDef.DisplayName;
				}
			}

			// Token: 0x170003AF RID: 943
			// (get) Token: 0x0600111B RID: 4379 RVA: 0x00065EAA File Offset: 0x000640AA
			public string TextureName
			{
				get
				{
					return "Slots/" + this.SlotDef.TextureName;
				}
			}

			// Token: 0x0600111C RID: 4380 RVA: 0x00065EC1 File Offset: 0x000640C1
			public SlotData(SlotDefinition slotDef, [Nullable(2)] ContainerSlot container = null, bool highlighted = false, bool blocked = false)
			{
				this.SlotDef = slotDef;
				this.Highlighted = highlighted;
				this.Blocked = blocked;
				this.Container = container;
			}

			// Token: 0x0600111D RID: 4381 RVA: 0x00065EE6 File Offset: 0x000640E6
			public SlotData(ClientInventorySystem.SlotData oldData, bool highlighted = false, bool blocked = false)
			{
				this.SlotDef = oldData.SlotDef;
				this.Highlighted = highlighted;
				this.Container = oldData.Container;
				this.Blocked = blocked;
			}

			// Token: 0x0600111E RID: 4382 RVA: 0x00065F14 File Offset: 0x00064114
			public static implicit operator ClientInventorySystem.SlotData(SlotDefinition s)
			{
				return new ClientInventorySystem.SlotData(s, null, false, false);
			}

			// Token: 0x0600111F RID: 4383 RVA: 0x00065F1F File Offset: 0x0006411F
			public static implicit operator SlotDefinition(ClientInventorySystem.SlotData s)
			{
				return s.SlotDef;
			}

			// Token: 0x0400085F RID: 2143
			public readonly SlotDefinition SlotDef;

			// Token: 0x04000860 RID: 2144
			public bool Blocked;

			// Token: 0x04000861 RID: 2145
			public bool Highlighted;

			// Token: 0x04000862 RID: 2146
			[Nullable(2)]
			[ViewVariables]
			public ContainerSlot Container;
		}

		// Token: 0x020002A6 RID: 678
		[Nullable(0)]
		public readonly struct SlotSpriteUpdate : IEquatable<ClientInventorySystem.SlotSpriteUpdate>
		{
			// Token: 0x06001120 RID: 4384 RVA: 0x00065F27 File Offset: 0x00064127
			public SlotSpriteUpdate(string Group, string Name, [Nullable(2)] SpriteComponent Sprite, bool ShowStorage)
			{
				this.Group = Group;
				this.Name = Name;
				this.Sprite = Sprite;
				this.ShowStorage = ShowStorage;
			}

			// Token: 0x170003B0 RID: 944
			// (get) Token: 0x06001121 RID: 4385 RVA: 0x00065F46 File Offset: 0x00064146
			// (set) Token: 0x06001122 RID: 4386 RVA: 0x00065F4E File Offset: 0x0006414E
			public string Group { get; set; }

			// Token: 0x170003B1 RID: 945
			// (get) Token: 0x06001123 RID: 4387 RVA: 0x00065F57 File Offset: 0x00064157
			// (set) Token: 0x06001124 RID: 4388 RVA: 0x00065F5F File Offset: 0x0006415F
			public string Name { get; set; }

			// Token: 0x170003B2 RID: 946
			// (get) Token: 0x06001125 RID: 4389 RVA: 0x00065F68 File Offset: 0x00064168
			// (set) Token: 0x06001126 RID: 4390 RVA: 0x00065F70 File Offset: 0x00064170
			[Nullable(2)]
			public SpriteComponent Sprite { [NullableContext(2)] get; [NullableContext(2)] set; }

			// Token: 0x170003B3 RID: 947
			// (get) Token: 0x06001127 RID: 4391 RVA: 0x00065F79 File Offset: 0x00064179
			// (set) Token: 0x06001128 RID: 4392 RVA: 0x00065F81 File Offset: 0x00064181
			public bool ShowStorage { get; set; }

			// Token: 0x06001129 RID: 4393 RVA: 0x00065F8C File Offset: 0x0006418C
			[NullableContext(0)]
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("SlotSpriteUpdate");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x0600112A RID: 4394 RVA: 0x00065FD8 File Offset: 0x000641D8
			[NullableContext(0)]
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Group = ");
				builder.Append(this.Group);
				builder.Append(", Name = ");
				builder.Append(this.Name);
				builder.Append(", Sprite = ");
				builder.Append(this.Sprite);
				builder.Append(", ShowStorage = ");
				builder.Append(this.ShowStorage.ToString());
				return true;
			}

			// Token: 0x0600112B RID: 4395 RVA: 0x00066058 File Offset: 0x00064258
			[CompilerGenerated]
			public static bool operator !=(ClientInventorySystem.SlotSpriteUpdate left, ClientInventorySystem.SlotSpriteUpdate right)
			{
				return !(left == right);
			}

			// Token: 0x0600112C RID: 4396 RVA: 0x00066064 File Offset: 0x00064264
			[CompilerGenerated]
			public static bool operator ==(ClientInventorySystem.SlotSpriteUpdate left, ClientInventorySystem.SlotSpriteUpdate right)
			{
				return left.Equals(right);
			}

			// Token: 0x0600112D RID: 4397 RVA: 0x00066070 File Offset: 0x00064270
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return ((EqualityComparer<string>.Default.GetHashCode(this.<Group>k__BackingField) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Name>k__BackingField)) * -1521134295 + EqualityComparer<SpriteComponent>.Default.GetHashCode(this.<Sprite>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<ShowStorage>k__BackingField);
			}

			// Token: 0x0600112E RID: 4398 RVA: 0x000660D2 File Offset: 0x000642D2
			[NullableContext(0)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return obj is ClientInventorySystem.SlotSpriteUpdate && this.Equals((ClientInventorySystem.SlotSpriteUpdate)obj);
			}

			// Token: 0x0600112F RID: 4399 RVA: 0x000660EC File Offset: 0x000642EC
			[CompilerGenerated]
			public bool Equals(ClientInventorySystem.SlotSpriteUpdate other)
			{
				return EqualityComparer<string>.Default.Equals(this.<Group>k__BackingField, other.<Group>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Name>k__BackingField, other.<Name>k__BackingField) && EqualityComparer<SpriteComponent>.Default.Equals(this.<Sprite>k__BackingField, other.<Sprite>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<ShowStorage>k__BackingField, other.<ShowStorage>k__BackingField);
			}

			// Token: 0x06001130 RID: 4400 RVA: 0x00066159 File Offset: 0x00064359
			[CompilerGenerated]
			public void Deconstruct(out string Group, out string Name, [Nullable(2)] out SpriteComponent Sprite, out bool ShowStorage)
			{
				Group = this.Group;
				Name = this.Name;
				Sprite = this.Sprite;
				ShowStorage = this.ShowStorage;
			}
		}
	}
}
