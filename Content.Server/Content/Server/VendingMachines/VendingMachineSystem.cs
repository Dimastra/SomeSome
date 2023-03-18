using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Cargo.Systems;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.UserInterface;
using Content.Server.VendingMachines.Restock;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.VendingMachines;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.VendingMachines
{
	// Token: 0x020000D4 RID: 212
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VendingMachineSystem : SharedVendingMachineSystem
	{
		// Token: 0x060003BD RID: 957 RVA: 0x0001393C File Offset: 0x00011B3C
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("vending");
			base.SubscribeLocalEvent<VendingMachineComponent, PowerChangedEvent>(new ComponentEventRefHandler<VendingMachineComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, BreakageEventArgs>(new ComponentEventHandler<VendingMachineComponent, BreakageEventArgs>(this.OnBreak), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, GotEmaggedEvent>(new ComponentEventRefHandler<VendingMachineComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, DamageChangedEvent>(new ComponentEventHandler<VendingMachineComponent, DamageChangedEvent>(this.OnDamage), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, PriceCalculationEvent>(new ComponentEventRefHandler<VendingMachineComponent, PriceCalculationEvent>(this.OnVendingPrice), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<VendingMachineComponent, ActivatableUIOpenAttemptEvent>(this.OnActivatableUIOpenAttempt), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, BoundUIOpenedEvent>(new ComponentEventHandler<VendingMachineComponent, BoundUIOpenedEvent>(this.OnBoundUIOpened), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, VendingMachineEjectMessage>(new ComponentEventHandler<VendingMachineComponent, VendingMachineEjectMessage>(this.OnInventoryEjectMessage), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, VendingMachineSelfDispenseEvent>(new ComponentEventHandler<VendingMachineComponent, VendingMachineSelfDispenseEvent>(this.OnSelfDispense), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, DoAfterEvent>(new ComponentEventHandler<VendingMachineComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00013A28 File Offset: 0x00011C28
		private void OnVendingPrice(EntityUid uid, VendingMachineComponent component, ref PriceCalculationEvent args)
		{
			double price = 0.0;
			foreach (KeyValuePair<string, VendingMachineInventoryEntry> keyValuePair in component.Inventory)
			{
				string text;
				VendingMachineInventoryEntry vendingMachineInventoryEntry;
				keyValuePair.Deconstruct(out text, out vendingMachineInventoryEntry);
				VendingMachineInventoryEntry entry = vendingMachineInventoryEntry;
				EntityPrototype proto;
				if (!this._prototypeManager.TryIndex<EntityPrototype>(entry.ID, ref proto))
				{
					ISawmill sawmill = this._sawmill;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Unable to find entity prototype ");
					defaultInterpolatedStringHandler.AppendFormatted(entry.ID);
					defaultInterpolatedStringHandler.AppendLiteral(" on ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					defaultInterpolatedStringHandler.AppendLiteral(" vending.");
					sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					price += entry.Amount * this._pricing.GetEstimatedPrice(proto, this._factory);
				}
			}
			args.Price += price;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00013B30 File Offset: 0x00011D30
		protected override void OnComponentInit(EntityUid uid, VendingMachineComponent component, ComponentInit args)
		{
			base.OnComponentInit(uid, component, args);
			if (base.HasComp<ApcPowerReceiverComponent>(component.Owner))
			{
				this.TryUpdateVisualState(uid, component);
			}
			if (component.Action != null)
			{
				InstantAction action = new InstantAction(this._prototypeManager.Index<InstantActionPrototype>(component.Action));
				this._action.AddAction(uid, action, new EntityUid?(uid), null, true);
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00013B90 File Offset: 0x00011D90
		private void OnActivatableUIOpenAttempt(EntityUid uid, VendingMachineComponent component, ActivatableUIOpenAttemptEvent args)
		{
			if (component.Broken)
			{
				args.Cancel();
			}
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x00013BA0 File Offset: 0x00011DA0
		private void OnBoundUIOpened(EntityUid uid, VendingMachineComponent component, BoundUIOpenedEvent args)
		{
			this.UpdateVendingMachineInterfaceState(component);
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00013BAC File Offset: 0x00011DAC
		private void UpdateVendingMachineInterfaceState(VendingMachineComponent component)
		{
			VendingMachineInterfaceState state = new VendingMachineInterfaceState(base.GetAllInventory(component.Owner, component));
			this._userInterfaceSystem.TrySetUiState(component.Owner, VendingMachineUiKey.Key, state, null, null, true);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00013BE8 File Offset: 0x00011DE8
		private void OnInventoryEjectMessage(EntityUid uid, VendingMachineComponent component, VendingMachineEjectMessage args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid entity = attachedEntity.GetValueOrDefault();
				if (entity.Valid && !base.Deleted(entity, null))
				{
					this.AuthorizedVend(uid, entity, args.Type, args.ID, component);
					return;
				}
			}
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00013C4C File Offset: 0x00011E4C
		private void OnPowerChanged(EntityUid uid, VendingMachineComponent component, ref PowerChangedEvent args)
		{
			this.TryUpdateVisualState(uid, component);
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00013C56 File Offset: 0x00011E56
		private void OnBreak(EntityUid uid, VendingMachineComponent vendComponent, BreakageEventArgs eventArgs)
		{
			vendComponent.Broken = true;
			this.TryUpdateVisualState(uid, vendComponent);
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x00013C67 File Offset: 0x00011E67
		private void OnEmagged(EntityUid uid, VendingMachineComponent component, ref GotEmaggedEvent args)
		{
			args.Handled = (component.EmaggedInventory.Count > 0);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00013C80 File Offset: 0x00011E80
		private void OnDamage(EntityUid uid, VendingMachineComponent component, DamageChangedEvent args)
		{
			if (component.Broken || component.DispenseOnHitCoolingDown || component.DispenseOnHitChance == null || args.DamageDelta == null)
			{
				return;
			}
			if (args.DamageIncreased)
			{
				FixedPoint2 total = args.DamageDelta.Total;
				float? num = component.DispenseOnHitThreshold;
				if (total >= ((num != null) ? new FixedPoint2?(num.GetValueOrDefault()) : null) && RandomExtensions.Prob(this._random, component.DispenseOnHitChance.Value))
				{
					num = component.DispenseOnHitCooldown;
					float num2 = 0f;
					if (num.GetValueOrDefault() > num2 & num != null)
					{
						component.DispenseOnHitCoolingDown = true;
					}
					this.EjectRandom(uid, true, true, component);
				}
			}
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00013D5E File Offset: 0x00011F5E
		private void OnSelfDispense(EntityUid uid, VendingMachineComponent component, VendingMachineSelfDispenseEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			this.EjectRandom(uid, true, false, component);
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00013D7C File Offset: 0x00011F7C
		private void OnDoAfter(EntityUid uid, VendingMachineComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || args.Args.Used == null)
			{
				return;
			}
			VendingMachineRestockComponent restockComponent;
			if (!base.TryComp<VendingMachineRestockComponent>(args.Args.Used, ref restockComponent))
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(77, 3);
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.User));
				defaultInterpolatedStringHandler.AppendLiteral(" tried to restock ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" with ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.Used.Value));
				defaultInterpolatedStringHandler.AppendLiteral(" which did not have a VendingMachineRestockComponent.");
				sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			this.TryRestockInventory(uid, component);
			this._popupSystem.PopupEntity(Loc.GetString("vending-machine-restock-done", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("this", args.Args.Used),
				new ValueTuple<string, object>("user", args.Args.User),
				new ValueTuple<string, object>("target", uid)
			}), args.Args.User, PopupType.Medium);
			this._audioSystem.PlayPvs(restockComponent.SoundRestockDone, uid, new AudioParams?(AudioParams.Default.WithVolume(-2f).WithVariation(new float?(0.2f))));
			base.Del(args.Args.Used.Value);
			args.Handled = true;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00013F26 File Offset: 0x00012126
		[NullableContext(2)]
		public void SetShooting(EntityUid uid, bool canShoot, VendingMachineComponent component = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref component, true))
			{
				return;
			}
			component.CanShoot = canShoot;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00013F3C File Offset: 0x0001213C
		[NullableContext(2)]
		public void Deny(EntityUid uid, VendingMachineComponent vendComponent = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref vendComponent, true))
			{
				return;
			}
			if (vendComponent.Denying)
			{
				return;
			}
			vendComponent.Denying = true;
			this._audioSystem.PlayPvs(vendComponent.SoundDeny, vendComponent.Owner, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
			this.TryUpdateVisualState(uid, vendComponent);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x00013F9C File Offset: 0x0001219C
		[NullableContext(2)]
		public bool IsAuthorized(EntityUid uid, EntityUid? sender, VendingMachineComponent vendComponent = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref vendComponent, true) || sender == null)
			{
				return false;
			}
			AccessReaderComponent accessReader;
			if (base.TryComp<AccessReaderComponent>(vendComponent.Owner, ref accessReader) && !this._accessReader.IsAllowed(sender.Value, accessReader) && !base.HasComp<EmaggedComponent>(uid))
			{
				this._popupSystem.PopupEntity(Loc.GetString("vending-machine-component-try-eject-access-denied"), uid, PopupType.Small);
				this.Deny(uid, vendComponent);
				return false;
			}
			return true;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00014010 File Offset: 0x00012210
		public void TryEjectVendorItem(EntityUid uid, InventoryType type, string itemId, bool throwItem, [Nullable(2)] VendingMachineComponent vendComponent = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref vendComponent, true))
			{
				return;
			}
			if (vendComponent.Ejecting || vendComponent.Broken || !this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			VendingMachineInventoryEntry entry = this.GetEntry(itemId, type, vendComponent);
			if (entry == null)
			{
				this._popupSystem.PopupEntity(Loc.GetString("vending-machine-component-try-eject-invalid-item"), uid, PopupType.Small);
				this.Deny(uid, vendComponent);
				return;
			}
			if (entry.Amount <= 0U)
			{
				this._popupSystem.PopupEntity(Loc.GetString("vending-machine-component-try-eject-out-of-stock"), uid, PopupType.Small);
				this.Deny(uid, vendComponent);
				return;
			}
			if (string.IsNullOrEmpty(entry.ID))
			{
				return;
			}
			TransformComponent transformComp;
			if (!base.TryComp<TransformComponent>(vendComponent.Owner, ref transformComp))
			{
				return;
			}
			vendComponent.Ejecting = true;
			vendComponent.NextItemToEject = entry.ID;
			vendComponent.ThrowNextItem = throwItem;
			entry.Amount -= 1U;
			this.UpdateVendingMachineInterfaceState(vendComponent);
			this.TryUpdateVisualState(uid, vendComponent);
			this._audioSystem.PlayPvs(vendComponent.SoundVend, vendComponent.Owner, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00014131 File Offset: 0x00012331
		public void AuthorizedVend(EntityUid uid, EntityUid sender, InventoryType type, string itemId, VendingMachineComponent component)
		{
			if (this.IsAuthorized(uid, new EntityUid?(sender), component))
			{
				this.TryEjectVendorItem(uid, type, itemId, component.CanShoot, component);
			}
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00014158 File Offset: 0x00012358
		[NullableContext(2)]
		public void TryUpdateVisualState(EntityUid uid, VendingMachineComponent vendComponent = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref vendComponent, true))
			{
				return;
			}
			VendingMachineVisualState finalState = VendingMachineVisualState.Normal;
			if (vendComponent.Broken)
			{
				finalState = VendingMachineVisualState.Broken;
			}
			else if (vendComponent.Ejecting)
			{
				finalState = VendingMachineVisualState.Eject;
			}
			else if (vendComponent.Denying)
			{
				finalState = VendingMachineVisualState.Deny;
			}
			else if (!this.IsPowered(uid, this.EntityManager, null))
			{
				finalState = VendingMachineVisualState.Off;
			}
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(vendComponent.Owner, ref appearance))
			{
				this._appearanceSystem.SetData(uid, VendingMachineVisuals.VisualState, finalState, appearance);
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x000141D4 File Offset: 0x000123D4
		[NullableContext(2)]
		public void EjectRandom(EntityUid uid, bool throwItem, bool forceEject = false, VendingMachineComponent vendComponent = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref vendComponent, true))
			{
				return;
			}
			List<VendingMachineInventoryEntry> availableItems = base.GetAvailableInventory(uid, vendComponent);
			if (availableItems.Count <= 0)
			{
				return;
			}
			VendingMachineInventoryEntry item = RandomExtensions.Pick<VendingMachineInventoryEntry>(this._random, availableItems);
			if (forceEject)
			{
				vendComponent.NextItemToEject = item.ID;
				vendComponent.ThrowNextItem = throwItem;
				VendingMachineInventoryEntry entry = this.GetEntry(item.ID, item.Type, vendComponent);
				if (entry != null)
				{
					entry.Amount -= 1U;
				}
				this.EjectItem(vendComponent, forceEject);
				return;
			}
			this.TryEjectVendorItem(uid, item.Type, item.ID, throwItem, vendComponent);
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00014270 File Offset: 0x00012470
		private void EjectItem(VendingMachineComponent vendComponent, bool forceEject = false)
		{
			if (!forceEject)
			{
				this.TryUpdateVisualState(vendComponent.Owner, vendComponent);
			}
			if (string.IsNullOrEmpty(vendComponent.NextItemToEject))
			{
				vendComponent.ThrowNextItem = false;
				return;
			}
			EntityUid ent = this.EntityManager.SpawnEntity(vendComponent.NextItemToEject, base.Transform(vendComponent.Owner).Coordinates);
			if (vendComponent.ThrowNextItem)
			{
				float range = vendComponent.NonLimitedEjectRange;
				Vector2 direction;
				direction..ctor(this._random.NextFloat(-range, range), this._random.NextFloat(-range, range));
				this._throwingSystem.TryThrow(ent, direction, vendComponent.NonLimitedEjectForce, null, 5f, null, null, null, null);
			}
			vendComponent.NextItemToEject = null;
			vendComponent.ThrowNextItem = false;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0001433C File Offset: 0x0001253C
		private void DenyItem(VendingMachineComponent vendComponent)
		{
			this.TryUpdateVisualState(vendComponent.Owner, vendComponent);
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0001434C File Offset: 0x0001254C
		[return: Nullable(2)]
		private VendingMachineInventoryEntry GetEntry(string entryId, InventoryType type, VendingMachineComponent component)
		{
			if (type == InventoryType.Emagged && base.HasComp<EmaggedComponent>(component.Owner))
			{
				return component.EmaggedInventory.GetValueOrDefault(entryId);
			}
			if (type == InventoryType.Contraband && component.Contraband)
			{
				return component.ContrabandInventory.GetValueOrDefault(entryId);
			}
			return component.Inventory.GetValueOrDefault(entryId);
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000143A0 File Offset: 0x000125A0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (VendingMachineComponent comp in base.EntityQuery<VendingMachineComponent>(false))
			{
				if (comp.Ejecting)
				{
					comp.EjectAccumulator += frameTime;
					if (comp.EjectAccumulator >= comp.EjectDelay)
					{
						comp.EjectAccumulator = 0f;
						comp.Ejecting = false;
						this.EjectItem(comp, false);
					}
				}
				if (comp.Denying)
				{
					comp.DenyAccumulator += frameTime;
					if (comp.DenyAccumulator >= comp.DenyDelay)
					{
						comp.DenyAccumulator = 0f;
						comp.Denying = false;
						this.DenyItem(comp);
					}
				}
				if (comp.DispenseOnHitCoolingDown)
				{
					comp.DispenseOnHitAccumulator += frameTime;
					float dispenseOnHitAccumulator = comp.DispenseOnHitAccumulator;
					float? dispenseOnHitCooldown = comp.DispenseOnHitCooldown;
					if (dispenseOnHitAccumulator >= dispenseOnHitCooldown.GetValueOrDefault() & dispenseOnHitCooldown != null)
					{
						comp.DispenseOnHitAccumulator = 0f;
						comp.DispenseOnHitCoolingDown = false;
					}
				}
			}
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x000144BC File Offset: 0x000126BC
		[NullableContext(2)]
		public void TryRestockInventory(EntityUid uid, VendingMachineComponent vendComponent = null)
		{
			if (!base.Resolve<VendingMachineComponent>(uid, ref vendComponent, true))
			{
				return;
			}
			base.RestockInventoryFromPrototype(uid, vendComponent);
			this.UpdateVendingMachineInterfaceState(vendComponent);
			this.TryUpdateVisualState(uid, vendComponent);
		}

		// Token: 0x04000253 RID: 595
		[Dependency]
		private readonly IComponentFactory _factory;

		// Token: 0x04000254 RID: 596
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000255 RID: 597
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000256 RID: 598
		[Dependency]
		private readonly AccessReaderSystem _accessReader;

		// Token: 0x04000257 RID: 599
		[Dependency]
		private readonly AppearanceSystem _appearanceSystem;

		// Token: 0x04000258 RID: 600
		[Dependency]
		private readonly AudioSystem _audioSystem;

		// Token: 0x04000259 RID: 601
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400025A RID: 602
		[Dependency]
		private readonly SharedActionsSystem _action;

		// Token: 0x0400025B RID: 603
		[Dependency]
		private readonly PricingSystem _pricing;

		// Token: 0x0400025C RID: 604
		[Dependency]
		private readonly ThrowingSystem _throwingSystem;

		// Token: 0x0400025D RID: 605
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x0400025E RID: 606
		private ISawmill _sawmill;
	}
}
