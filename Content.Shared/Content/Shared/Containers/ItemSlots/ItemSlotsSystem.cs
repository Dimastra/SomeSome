using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Containers.ItemSlots
{
	// Token: 0x02000565 RID: 1381
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ItemSlotsSystem : EntitySystem
	{
		// Token: 0x060010B6 RID: 4278 RVA: 0x000366B4 File Offset: 0x000348B4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ItemSlotsComponent, MapInitEvent>(new ComponentEventHandler<ItemSlotsComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, ComponentInit>(new ComponentEventHandler<ItemSlotsComponent, ComponentInit>(this.Oninitialize), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, InteractUsingEvent>(new ComponentEventHandler<ItemSlotsComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, InteractHandEvent>(new ComponentEventHandler<ItemSlotsComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, UseInHandEvent>(new ComponentEventHandler<ItemSlotsComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<ItemSlotsComponent, GetVerbsEvent<AlternativeVerb>>(this.AddEjectVerbs), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ItemSlotsComponent, GetVerbsEvent<InteractionVerb>>(this.AddInteractionVerbsVerbs), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, BreakageEventArgs>(new ComponentEventHandler<ItemSlotsComponent, BreakageEventArgs>(this.OnBreak), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, DestructionEventArgs>(new ComponentEventHandler<ItemSlotsComponent, DestructionEventArgs>(this.OnBreak), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, ComponentGetState>(new ComponentEventRefHandler<ItemSlotsComponent, ComponentGetState>(this.GetItemSlotsState), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemSlotsComponent, ComponentHandleState>(this.HandleItemSlotsState), null, null);
			base.SubscribeLocalEvent<ItemSlotsComponent, ItemSlotButtonPressedEvent>(new ComponentEventHandler<ItemSlotsComponent, ItemSlotButtonPressedEvent>(this.HandleButtonPressed), null, null);
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x000367B8 File Offset: 0x000349B8
		private void OnMapInit(EntityUid uid, ItemSlotsComponent itemSlots, MapInitEvent args)
		{
			foreach (ItemSlot slot in itemSlots.Slots.Values)
			{
				if (!slot.HasItem && !string.IsNullOrEmpty(slot.StartingItem))
				{
					EntityUid item = this.EntityManager.SpawnEntity(slot.StartingItem, this.EntityManager.GetComponent<TransformComponent>(itemSlots.Owner).Coordinates);
					ContainerSlot containerSlot = slot.ContainerSlot;
					if (containerSlot != null)
					{
						containerSlot.Insert(item, null, null, null, null, null);
					}
				}
			}
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00036860 File Offset: 0x00034A60
		private void Oninitialize(EntityUid uid, ItemSlotsComponent itemSlots, ComponentInit args)
		{
			foreach (KeyValuePair<string, ItemSlot> keyValuePair in itemSlots.Slots)
			{
				string text;
				ItemSlot itemSlot;
				keyValuePair.Deconstruct(out text, out itemSlot);
				string id = text;
				itemSlot.ContainerSlot = this._containers.EnsureContainer<ContainerSlot>(itemSlots.Owner, id, null);
			}
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x000368D4 File Offset: 0x00034AD4
		public void AddItemSlot(EntityUid uid, string id, ItemSlot slot, [Nullable(2)] ItemSlotsComponent itemSlots = null)
		{
			if (itemSlots == null)
			{
				itemSlots = this.EntityManager.EnsureComponent<ItemSlotsComponent>(uid);
			}
			ItemSlot existing;
			if (itemSlots.Slots.TryGetValue(id, out existing))
			{
				if (existing.Local)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
					defaultInterpolatedStringHandler.AppendLiteral("Duplicate item slot key. Entity: ");
					defaultInterpolatedStringHandler.AppendFormatted(this.EntityManager.GetComponent<MetaDataComponent>(itemSlots.Owner).EntityName);
					defaultInterpolatedStringHandler.AppendLiteral(" (");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
					defaultInterpolatedStringHandler.AppendLiteral("), key: ");
					defaultInterpolatedStringHandler.AppendFormatted(id);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					slot.CopyFrom(existing);
				}
			}
			slot.ContainerSlot = this._containers.EnsureContainer<ContainerSlot>(itemSlots.Owner, id, null);
			itemSlots.Slots[id] = slot;
			base.Dirty(itemSlots, null);
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x000369B0 File Offset: 0x00034BB0
		public void RemoveItemSlot(EntityUid uid, ItemSlot slot, [Nullable(2)] ItemSlotsComponent itemSlots = null)
		{
			if (base.Terminating(uid, null) || slot.ContainerSlot == null)
			{
				return;
			}
			slot.ContainerSlot.Shutdown(null, null);
			if (!base.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
			{
				return;
			}
			itemSlots.Slots.Remove(slot.ContainerSlot.ID);
			if (itemSlots.Slots.Count == 0)
			{
				this.EntityManager.RemoveComponent(uid, itemSlots);
				return;
			}
			base.Dirty(itemSlots, null);
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x00036A24 File Offset: 0x00034C24
		[NullableContext(2)]
		public bool TryGetSlot(EntityUid uid, [Nullable(1)] string slotId, [NotNullWhen(true)] out ItemSlot itemSlot, ItemSlotsComponent component = null)
		{
			itemSlot = null;
			return base.Resolve<ItemSlotsComponent>(uid, ref component, true) && component.Slots.TryGetValue(slotId, out itemSlot);
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x00036A48 File Offset: 0x00034C48
		private void OnInteractHand(EntityUid uid, ItemSlotsComponent itemSlots, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			foreach (ItemSlot slot in itemSlots.Slots.Values)
			{
				if (!slot.Locked && slot.EjectOnInteract && slot.Item != null)
				{
					args.Handled = true;
					this.TryEjectToHands(uid, slot, new EntityUid?(args.User), true);
					break;
				}
			}
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x00036AE0 File Offset: 0x00034CE0
		private void OnUseInHand(EntityUid uid, ItemSlotsComponent itemSlots, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			foreach (ItemSlot slot in itemSlots.Slots.Values)
			{
				if (!slot.Locked && slot.EjectOnUse && slot.Item != null)
				{
					args.Handled = true;
					this.TryEjectToHands(uid, slot, new EntityUid?(args.User), true);
					break;
				}
			}
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x00036B78 File Offset: 0x00034D78
		private void OnInteractUsing(EntityUid uid, ItemSlotsComponent itemSlots, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			SharedHandsComponent hands;
			if (!this.EntityManager.TryGetComponent<SharedHandsComponent>(args.User, ref hands))
			{
				return;
			}
			foreach (ItemSlot slot in itemSlots.Slots.Values)
			{
				if (slot.InsertOnInteract && this.CanInsert(uid, args.Used, slot, slot.Swap, new EntityUid?(args.User)))
				{
					SharedHandsSystem handsSystem = this._handsSystem;
					EntityUid user = args.User;
					EntityUid used = args.Used;
					SharedHandsComponent handsComp = hands;
					if (!handsSystem.TryDrop(user, used, null, true, true, handsComp))
					{
						break;
					}
					if (slot.Item != null)
					{
						this._handsSystem.TryPickupAnyHand(args.User, slot.Item.Value, true, false, hands, null);
					}
					this.Insert(uid, slot, args.Used, new EntityUid?(args.User), true);
					args.Handled = true;
					break;
				}
			}
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x00036CA0 File Offset: 0x00034EA0
		private void Insert(EntityUid uid, ItemSlot slot, EntityUid item, EntityUid? user, bool excludeUserAudio = false)
		{
			ContainerSlot containerSlot = slot.ContainerSlot;
			bool? inserted = (containerSlot != null) ? new bool?(containerSlot.Insert(item, null, null, null, null, null)) : null;
			if (inserted != null && inserted.Value && user != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(16, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "ToPrettyString(user.Value)");
				logStringHandler.AppendLiteral(" inserted ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(item), "ToPrettyString(item)");
				logStringHandler.AppendLiteral(" into ");
				ContainerSlot containerSlot2 = slot.ContainerSlot;
				logStringHandler.AppendFormatted(((containerSlot2 != null) ? containerSlot2.ID : null) + " slot of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this._audioSystem.PlayPredicted(slot.InsertSound, uid, excludeUserAudio ? user : null, null);
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x00036DBC File Offset: 0x00034FBC
		public bool CanInsert(EntityUid uid, EntityUid usedUid, ItemSlot slot, bool swap = false, EntityUid? popup = null)
		{
			if (slot.Locked)
			{
				return false;
			}
			if (!swap && slot.HasItem)
			{
				return false;
			}
			if (slot.Whitelist != null && !slot.Whitelist.IsValid(usedUid, null))
			{
				if (this._netManager.IsClient && this._timing.IsFirstTimePredicted && popup != null && !string.IsNullOrWhiteSpace(slot.WhitelistFailPopup))
				{
					this._popupSystem.PopupEntity(Loc.GetString(slot.WhitelistFailPopup), uid, popup.Value, PopupType.Small);
				}
				return false;
			}
			ContainerSlot containerSlot = slot.ContainerSlot;
			return containerSlot != null && containerSlot.CanInsertIfEmpty(usedUid, this.EntityManager);
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00036E64 File Offset: 0x00035064
		public bool TryInsert(EntityUid uid, string id, EntityUid item, EntityUid? user, [Nullable(2)] ItemSlotsComponent itemSlots = null)
		{
			ItemSlot slot;
			return base.Resolve<ItemSlotsComponent>(uid, ref itemSlots, true) && itemSlots.Slots.TryGetValue(id, out slot) && this.TryInsert(uid, slot, item, user);
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x00036EA0 File Offset: 0x000350A0
		public bool TryInsert(EntityUid uid, ItemSlot slot, EntityUid item, EntityUid? user)
		{
			if (!this.CanInsert(uid, item, slot, false, null))
			{
				return false;
			}
			this.Insert(uid, slot, item, user, false);
			return true;
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x00036ED4 File Offset: 0x000350D4
		public bool TryInsertFromHand(EntityUid uid, ItemSlot slot, EntityUid user, [Nullable(2)] SharedHandsComponent hands = null)
		{
			if (!base.Resolve<SharedHandsComponent>(user, ref hands, false))
			{
				return false;
			}
			Hand activeHand = hands.ActiveHand;
			EntityUid? entityUid = (activeHand != null) ? activeHand.HeldEntity : null;
			if (entityUid == null)
			{
				return false;
			}
			EntityUid held = entityUid.GetValueOrDefault();
			if (!this.CanInsert(uid, held, slot, false, null))
			{
				return false;
			}
			if (!this._handsSystem.TryDrop(user, hands.ActiveHand, null, true, true, null))
			{
				return false;
			}
			this.Insert(uid, slot, held, new EntityUid?(user), false);
			return true;
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x00036F6C File Offset: 0x0003516C
		public bool CanEject(ItemSlot slot)
		{
			if (slot.Locked || slot.Item == null)
			{
				return false;
			}
			ContainerSlot containerSlot = slot.ContainerSlot;
			return containerSlot != null && containerSlot.CanRemove(slot.Item.Value, this.EntityManager);
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x00036FB8 File Offset: 0x000351B8
		private void Eject(EntityUid uid, ItemSlot slot, EntityUid item, EntityUid? user, bool excludeUserAudio = false)
		{
			ContainerSlot containerSlot = slot.ContainerSlot;
			bool? ejected = (containerSlot != null) ? new bool?(containerSlot.Remove(item, null, null, null, true, false, null, null)) : null;
			if (ejected != null && ejected.Value && user != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(15, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "ToPrettyString(user.Value)");
				logStringHandler.AppendLiteral(" ejected ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(item), "ToPrettyString(item)");
				logStringHandler.AppendLiteral(" from ");
				ContainerSlot containerSlot2 = slot.ContainerSlot;
				logStringHandler.AppendFormatted(((containerSlot2 != null) ? containerSlot2.ID : null) + " slot of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this._audioSystem.PlayPredicted(slot.EjectSound, uid, excludeUserAudio ? user : null, new AudioParams?(slot.SoundOptions));
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x000370E8 File Offset: 0x000352E8
		public bool TryEject(EntityUid uid, ItemSlot slot, EntityUid? user, [NotNullWhen(true)] out EntityUid? item, bool excludeUserAudio = false)
		{
			item = null;
			if (!this.CanEject(slot))
			{
				return false;
			}
			item = slot.Item;
			if (user != null && item != null && !this._actionBlockerSystem.CanPickup(user.Value, item.Value))
			{
				return false;
			}
			this.Eject(uid, slot, item.Value, user, excludeUserAudio);
			return true;
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x00037158 File Offset: 0x00035358
		public bool TryEject(EntityUid uid, string id, EntityUid? user, [NotNullWhen(true)] out EntityUid? item, [Nullable(2)] ItemSlotsComponent itemSlots = null, bool excludeUserAudio = false)
		{
			item = null;
			ItemSlot slot;
			return base.Resolve<ItemSlotsComponent>(uid, ref itemSlots, true) && itemSlots.Slots.TryGetValue(id, out slot) && this.TryEject(uid, slot, user, out item, excludeUserAudio);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x0003719C File Offset: 0x0003539C
		public bool TryEjectToHands(EntityUid uid, ItemSlot slot, EntityUid? user, bool excludeUserAudio = false)
		{
			EntityUid? item;
			if (!this.TryEject(uid, slot, user, out item, excludeUserAudio))
			{
				return false;
			}
			if (user != null)
			{
				this._handsSystem.PickupOrDrop(new EntityUid?(user.Value), item.Value, true, false, null, null);
			}
			return true;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x000371E8 File Offset: 0x000353E8
		private void AddEjectVerbs(EntityUid uid, ItemSlotsComponent itemSlots, GetVerbsEvent<AlternativeVerb> args)
		{
			if (args.Hands == null || !args.CanAccess || !args.CanInteract)
			{
				return;
			}
			using (Dictionary<string, ItemSlot>.ValueCollection.Enumerator enumerator = itemSlots.Slots.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ItemSlot slot = enumerator.Current;
					if (!slot.EjectOnInteract && this.CanEject(slot) && this._actionBlockerSystem.CanPickup(args.User, slot.Item.Value))
					{
						string verbSubject = (slot.Name != string.Empty) ? Loc.GetString(slot.Name) : (this.EntityManager.GetComponent<MetaDataComponent>(slot.Item.Value).EntityName ?? string.Empty);
						AlternativeVerb verb = new AlternativeVerb();
						verb.IconEntity = slot.Item;
						verb.Act = delegate()
						{
							this.TryEjectToHands(uid, slot, new EntityUid?(args.User), true);
						};
						if (slot.EjectVerbText == null)
						{
							verb.Text = verbSubject;
							verb.Category = VerbCategory.Eject;
						}
						else
						{
							verb.Text = Loc.GetString(slot.EjectVerbText);
						}
						verb.Priority = slot.Priority;
						args.Verbs.Add(verb);
					}
				}
			}
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x000373DC File Offset: 0x000355DC
		private void AddInteractionVerbsVerbs(EntityUid uid, ItemSlotsComponent itemSlots, GetVerbsEvent<InteractionVerb> args)
		{
			if (args.Hands == null || !args.CanAccess || !args.CanInteract)
			{
				return;
			}
			using (Dictionary<string, ItemSlot>.ValueCollection.Enumerator enumerator = itemSlots.Slots.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ItemSlot slot = enumerator.Current;
					if (slot.EjectOnInteract && this.CanEject(slot) && this._actionBlockerSystem.CanPickup(args.User, slot.Item.Value))
					{
						string verbSubject = (slot.Name != string.Empty) ? Loc.GetString(slot.Name) : (this.EntityManager.GetComponent<MetaDataComponent>(slot.Item.Value).EntityName ?? string.Empty);
						InteractionVerb takeVerb = new InteractionVerb();
						takeVerb.IconEntity = slot.Item;
						takeVerb.Act = delegate()
						{
							this.TryEjectToHands(uid, slot, new EntityUid?(args.User), true);
						};
						if (slot.EjectVerbText == null)
						{
							takeVerb.Text = Loc.GetString("take-item-verb-text", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("subject", verbSubject)
							});
						}
						else
						{
							takeVerb.Text = Loc.GetString(slot.EjectVerbText);
						}
						takeVerb.Priority = slot.Priority;
						args.Verbs.Add(takeVerb);
					}
				}
			}
			if (args.Using == null || !this._actionBlockerSystem.CanDrop(args.User))
			{
				return;
			}
			using (Dictionary<string, ItemSlot>.ValueCollection.Enumerator enumerator = itemSlots.Slots.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ItemSlot slot = enumerator.Current;
					if (this.CanInsert(uid, args.Using.Value, slot, false, null))
					{
						string verbSubject2 = (slot.Name != string.Empty) ? Loc.GetString(slot.Name) : (base.Name(args.Using.Value, null) ?? string.Empty);
						InteractionVerb insertVerb = new InteractionVerb();
						insertVerb.IconEntity = args.Using;
						insertVerb.Act = delegate()
						{
							this.Insert(uid, slot, args.Using.Value, new EntityUid?(args.User), true);
						};
						if (slot.InsertVerbText != null)
						{
							insertVerb.Text = Loc.GetString(slot.InsertVerbText);
							insertVerb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/insert.svg.192dpi.png", "/"));
						}
						else if (slot.EjectOnInteract)
						{
							insertVerb.Text = Loc.GetString("place-item-verb-text", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("subject", verbSubject2)
							});
							insertVerb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/drop.svg.192dpi.png", "/"));
						}
						else
						{
							insertVerb.Category = VerbCategory.Insert;
							insertVerb.Text = verbSubject2;
						}
						insertVerb.Priority = slot.Priority;
						args.Verbs.Add(insertVerb);
					}
				}
			}
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x00037818 File Offset: 0x00035A18
		private void HandleButtonPressed(EntityUid uid, ItemSlotsComponent component, ItemSlotButtonPressedEvent args)
		{
			ItemSlot slot;
			if (!component.Slots.TryGetValue(args.SlotId, out slot))
			{
				return;
			}
			if (args.TryEject && slot.HasItem)
			{
				this.TryEjectToHands(uid, slot, args.Session.AttachedEntity, false);
				return;
			}
			if (args.TryInsert && !slot.HasItem)
			{
				EntityUid? attachedEntity = args.Session.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid user = attachedEntity.GetValueOrDefault();
					this.TryInsertFromHand(uid, slot, user, null);
				}
			}
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0003789C File Offset: 0x00035A9C
		private void OnBreak(EntityUid uid, ItemSlotsComponent component, EntityEventArgs args)
		{
			foreach (ItemSlot slot in component.Slots.Values)
			{
				if (slot.EjectOnBreak && slot.HasItem)
				{
					this.SetLock(uid, slot, false, component);
					EntityUid? entityUid;
					this.TryEject(uid, slot, null, out entityUid, false);
				}
			}
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0003791C File Offset: 0x00035B1C
		public EntityUid? GetItemOrNull(EntityUid uid, string id, [Nullable(2)] ItemSlotsComponent itemSlots = null)
		{
			if (!base.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
			{
				return null;
			}
			ItemSlot valueOrDefault = itemSlots.Slots.GetValueOrDefault(id);
			if (valueOrDefault == null)
			{
				return null;
			}
			return valueOrDefault.Item;
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x00037960 File Offset: 0x00035B60
		public void SetLock(EntityUid uid, string id, bool locked, [Nullable(2)] ItemSlotsComponent itemSlots = null)
		{
			if (!base.Resolve<ItemSlotsComponent>(uid, ref itemSlots, true))
			{
				return;
			}
			ItemSlot slot;
			if (!itemSlots.Slots.TryGetValue(id, out slot))
			{
				return;
			}
			this.SetLock(uid, slot, locked, itemSlots);
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x00037997 File Offset: 0x00035B97
		public void SetLock(EntityUid uid, ItemSlot slot, bool locked, [Nullable(2)] ItemSlotsComponent itemSlots = null)
		{
			if (!base.Resolve<ItemSlotsComponent>(uid, ref itemSlots, true))
			{
				return;
			}
			slot.Locked = locked;
			itemSlots.Dirty(null);
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x000379B8 File Offset: 0x00035BB8
		private void HandleItemSlotsState(EntityUid uid, ItemSlotsComponent component, ref ComponentHandleState args)
		{
			ItemSlotsComponentState state = args.Current as ItemSlotsComponentState;
			if (state == null)
			{
				return;
			}
			foreach (KeyValuePair<string, ItemSlot> keyValuePair in component.Slots)
			{
				string text;
				ItemSlot itemSlot2;
				keyValuePair.Deconstruct(out text, out itemSlot2);
				string key = text;
				ItemSlot slot = itemSlot2;
				if (!state.Slots.ContainsKey(key))
				{
					this.RemoveItemSlot(uid, slot, component);
				}
			}
			foreach (KeyValuePair<string, ItemSlot> keyValuePair in state.Slots)
			{
				string text;
				ItemSlot itemSlot2;
				keyValuePair.Deconstruct(out text, out itemSlot2);
				string serverKey = text;
				ItemSlot serverSlot = itemSlot2;
				ItemSlot itemSlot;
				if (component.Slots.TryGetValue(serverKey, out itemSlot))
				{
					itemSlot.CopyFrom(serverSlot);
					itemSlot.ContainerSlot = this._containers.EnsureContainer<ContainerSlot>(uid, serverKey, null);
				}
				else
				{
					this.AddItemSlot(uid, serverKey, new ItemSlot(serverSlot)
					{
						Local = false
					}, null);
				}
			}
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x00037ADC File Offset: 0x00035CDC
		private void GetItemSlotsState(EntityUid uid, ItemSlotsComponent component, ref ComponentGetState args)
		{
			args.State = new ItemSlotsComponentState(component.Slots);
		}

		// Token: 0x04000FC2 RID: 4034
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000FC3 RID: 4035
		[Dependency]
		private readonly INetManager _netManager;

		// Token: 0x04000FC4 RID: 4036
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000FC5 RID: 4037
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04000FC6 RID: 4038
		[Dependency]
		private readonly SharedContainerSystem _containers;

		// Token: 0x04000FC7 RID: 4039
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000FC8 RID: 4040
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000FC9 RID: 4041
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;
	}
}
