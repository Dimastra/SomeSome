using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Administration.Logs;
using Content.Server.Stack;
using Content.Server.Store.Components;
using Content.Server.UserInterface;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Store;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.Store.Systems
{
	// Token: 0x02000151 RID: 337
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StoreSystem : EntitySystem
	{
		// Token: 0x0600066E RID: 1646 RVA: 0x0001F010 File Offset: 0x0001D210
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CurrencyComponent, AfterInteractEvent>(new ComponentEventHandler<CurrencyComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<StoreComponent, BeforeActivatableUIOpenEvent>(new ComponentEventHandler<StoreComponent, BeforeActivatableUIOpenEvent>(this.BeforeActivatableUiOpen), null, null);
			base.SubscribeLocalEvent<StoreComponent, MapInitEvent>(new ComponentEventHandler<StoreComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<StoreComponent, ComponentStartup>(new ComponentEventHandler<StoreComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<StoreComponent, ComponentShutdown>(new ComponentEventHandler<StoreComponent, ComponentShutdown>(this.OnShutdown), null, null);
			this.InitializeUi();
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0001F08D File Offset: 0x0001D28D
		private void OnMapInit(EntityUid uid, StoreComponent component, MapInitEvent args)
		{
			this.RefreshAllListings(component);
			this.InitializeFromPreset(component.Preset, uid, component);
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0001F0A4 File Offset: 0x0001D2A4
		private void OnStartup(EntityUid uid, StoreComponent component, ComponentStartup args)
		{
			if (base.MetaData(uid).EntityLifeStage == 3)
			{
				this.RefreshAllListings(component);
				this.InitializeFromPreset(component.Preset, uid, component);
			}
			StoreAddedEvent ev = default(StoreAddedEvent);
			base.RaiseLocalEvent<StoreAddedEvent>(uid, ref ev, true);
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0001F0E8 File Offset: 0x0001D2E8
		private void OnShutdown(EntityUid uid, StoreComponent component, ComponentShutdown args)
		{
			StoreRemovedEvent ev = default(StoreRemovedEvent);
			base.RaiseLocalEvent<StoreRemovedEvent>(uid, ref ev, true);
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0001F108 File Offset: 0x0001D308
		private void OnAfterInteract(EntityUid uid, CurrencyComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			StoreComponent store;
			if (args.Target == null || !base.TryComp<StoreComponent>(args.Target, ref store))
			{
				return;
			}
			args.Handled = this.TryAddCurrency(this.GetCurrencyValue(uid, component), args.Target.Value, store);
			if (args.Handled)
			{
				string msg = Loc.GetString("store-currency-inserted", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("used", args.Used),
					new ValueTuple<string, object>("target", args.Target)
				});
				this._popup.PopupEntity(msg, args.Target.Value, PopupType.Small);
				base.QueueDel(args.Used);
			}
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0001F1E4 File Offset: 0x0001D3E4
		public Dictionary<string, FixedPoint2> GetCurrencyValue(EntityUid uid, CurrencyComponent component)
		{
			StoreSystem.<>c__DisplayClass7_0 CS$<>8__locals1 = new StoreSystem.<>c__DisplayClass7_0();
			StoreSystem.<>c__DisplayClass7_0 CS$<>8__locals2 = CS$<>8__locals1;
			StackComponent componentOrNull = EntityManagerExt.GetComponentOrNull<StackComponent>(this.EntityManager, uid);
			CS$<>8__locals2.amount = ((componentOrNull != null) ? componentOrNull.Count : 1);
			return component.Price.ToDictionary((KeyValuePair<string, FixedPoint2> v) => v.Key, (KeyValuePair<string, FixedPoint2> p) => p.Value * CS$<>8__locals1.amount);
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x0001F24B File Offset: 0x0001D44B
		[NullableContext(2)]
		public bool TryAddCurrency(EntityUid currencyEnt, EntityUid storeEnt, StoreComponent store = null, CurrencyComponent currency = null)
		{
			return base.Resolve<CurrencyComponent>(currencyEnt, ref currency, true) && base.Resolve<StoreComponent>(storeEnt, ref store, true) && this.TryAddCurrency(this.GetCurrencyValue(currencyEnt, currency), storeEnt, store);
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x0001F278 File Offset: 0x0001D478
		public bool TryAddCurrency(Dictionary<string, FixedPoint2> currency, EntityUid uid, [Nullable(2)] StoreComponent store = null)
		{
			if (!base.Resolve<StoreComponent>(uid, ref store, true))
			{
				return false;
			}
			foreach (KeyValuePair<string, FixedPoint2> type in currency)
			{
				if (!store.CurrencyWhitelist.Contains(type.Key))
				{
					return false;
				}
			}
			foreach (KeyValuePair<string, FixedPoint2> type2 in currency)
			{
				if (!store.Balance.TryAdd(type2.Key, type2.Value))
				{
					Dictionary<string, FixedPoint2> balance = store.Balance;
					string key = type2.Key;
					balance[key] += type2.Value;
				}
			}
			this.UpdateUserInterface(null, uid, store, null);
			return true;
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x0001F380 File Offset: 0x0001D580
		public void InitializeFromPreset([Nullable(2)] string preset, EntityUid uid, StoreComponent component)
		{
			if (preset == null)
			{
				return;
			}
			StorePresetPrototype proto;
			if (!this._proto.TryIndex<StorePresetPrototype>(preset, ref proto))
			{
				return;
			}
			this.InitializeFromPreset(proto, uid, component);
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0001F3AC File Offset: 0x0001D5AC
		public void InitializeFromPreset(StorePresetPrototype preset, EntityUid uid, StoreComponent component)
		{
			component.Preset = preset.ID;
			component.CurrencyWhitelist.UnionWith(preset.CurrencyWhitelist);
			component.Categories.UnionWith(preset.Categories);
			if (component.Balance == new Dictionary<string, FixedPoint2>() && preset.InitialBalance != null)
			{
				this.TryAddCurrency(preset.InitialBalance, uid, component);
			}
			BoundUserInterface ui = this._ui.GetUiOrNull(uid, StoreUiKey.Key, null);
			if (ui != null)
			{
				this._ui.SetUiState(ui, new StoreInitializeState(preset.StoreName), null, true);
			}
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x0001F43B File Offset: 0x0001D63B
		public void RefreshAllListings(StoreComponent component)
		{
			component.Listings = this.GetAllListings();
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0001F44C File Offset: 0x0001D64C
		public HashSet<ListingData> GetAllListings()
		{
			IEnumerable<ListingPrototype> enumerable = this._proto.EnumeratePrototypes<ListingPrototype>();
			HashSet<ListingData> allData = new HashSet<ListingData>();
			foreach (ListingPrototype listing in enumerable)
			{
				allData.Add((ListingData)listing.Clone());
			}
			return allData;
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x0001F4B0 File Offset: 0x0001D6B0
		public bool TryAddListing(StoreComponent component, string listingId)
		{
			ListingPrototype proto;
			if (!this._proto.TryIndex<ListingPrototype>(listingId, ref proto))
			{
				Logger.Error("Attempted to add invalid listing.");
				return false;
			}
			return this.TryAddListing(component, proto);
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x0001F4E1 File Offset: 0x0001D6E1
		public bool TryAddListing(StoreComponent component, ListingData listing)
		{
			return component.Listings.Add(listing);
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x0001F4EF File Offset: 0x0001D6EF
		public IEnumerable<ListingData> GetAvailableListings(EntityUid buyer, EntityUid store, StoreComponent component)
		{
			return this.GetAvailableListings(buyer, component.Listings, component.Categories, new EntityUid?(store));
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x0001F50A File Offset: 0x0001D70A
		public IEnumerable<ListingData> GetAvailableListings(EntityUid buyer, [Nullable(new byte[]
		{
			2,
			1
		})] HashSet<ListingData> listings, HashSet<string> categories, EntityUid? storeEntity = null)
		{
			if (listings == null)
			{
				listings = this.GetAllListings();
			}
			foreach (ListingData listing in listings)
			{
				if (this.ListingHasCategory(listing, categories))
				{
					if (listing.Conditions != null)
					{
						ListingConditionArgs args = new ListingConditionArgs(buyer, storeEntity, listing, this.EntityManager);
						bool conditionsMet = true;
						using (List<ListingCondition>.Enumerator enumerator2 = listing.Conditions.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (!enumerator2.Current.Condition(args))
								{
									conditionsMet = false;
									break;
								}
							}
						}
						if (!conditionsMet)
						{
							continue;
						}
					}
					yield return listing;
				}
			}
			HashSet<ListingData>.Enumerator enumerator = default(HashSet<ListingData>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x0001F538 File Offset: 0x0001D738
		public bool ListingHasCategory(ListingData listing, HashSet<string> categories)
		{
			foreach (string cat in categories)
			{
				if (listing.Categories.Contains(cat))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0001F594 File Offset: 0x0001D794
		private void InitializeUi()
		{
			base.SubscribeLocalEvent<StoreComponent, StoreRequestUpdateInterfaceMessage>(new ComponentEventHandler<StoreComponent, StoreRequestUpdateInterfaceMessage>(this.OnRequestUpdate), null, null);
			base.SubscribeLocalEvent<StoreComponent, StoreBuyListingMessage>(new ComponentEventHandler<StoreComponent, StoreBuyListingMessage>(this.OnBuyRequest), null, null);
			base.SubscribeLocalEvent<StoreComponent, StoreRequestWithdrawMessage>(new ComponentEventHandler<StoreComponent, StoreRequestWithdrawMessage>(this.OnRequestWithdraw), null, null);
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x0001F5D4 File Offset: 0x0001D7D4
		[NullableContext(2)]
		public void ToggleUi(EntityUid user, EntityUid storeEnt, StoreComponent component = null)
		{
			if (!base.Resolve<StoreComponent>(storeEnt, ref component, true))
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(user, ref actor))
			{
				return;
			}
			if (!this._ui.TryToggleUi(storeEnt, StoreUiKey.Key, actor.PlayerSession, null))
			{
				return;
			}
			this.UpdateUserInterface(new EntityUid?(user), storeEnt, component, null);
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0001F628 File Offset: 0x0001D828
		public void CloseUi(EntityUid user, StoreComponent component)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(user, ref actor))
			{
				return;
			}
			this._ui.TryClose(component.Owner, StoreUiKey.Key, actor.PlayerSession, null);
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0001F660 File Offset: 0x0001D860
		[NullableContext(2)]
		public void UpdateUserInterface(EntityUid? user, EntityUid store, StoreComponent component = null, BoundUserInterface ui = null)
		{
			if (!base.Resolve<StoreComponent>(store, ref component, true))
			{
				return;
			}
			if (ui == null && !this._ui.TryGetUi(store, StoreUiKey.Key, ref ui, null))
			{
				return;
			}
			if (user != null)
			{
				component.LastAvailableListings = this.GetAvailableListings(component.AccountOwner ?? user.Value, store, component).ToHashSet<ListingData>();
			}
			Dictionary<string, FixedPoint2> allCurrency = new Dictionary<string, FixedPoint2>();
			foreach (string supported in component.CurrencyWhitelist)
			{
				allCurrency.Add(supported, FixedPoint2.Zero);
				if (component.Balance.ContainsKey(supported))
				{
					allCurrency[supported] = component.Balance[supported];
				}
			}
			StoreUpdateState state = new StoreUpdateState(component.LastAvailableListings, allCurrency);
			this._ui.SetUiState(ui, state, null, true);
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x0001F768 File Offset: 0x0001D968
		private void OnRequestUpdate(EntityUid uid, StoreComponent component, StoreRequestUpdateInterfaceMessage args)
		{
			this.UpdateUserInterface(args.Session.AttachedEntity, args.Entity, component, null);
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0001F783 File Offset: 0x0001D983
		private void BeforeActivatableUiOpen(EntityUid uid, StoreComponent component, BeforeActivatableUIOpenEvent args)
		{
			this.UpdateUserInterface(new EntityUid?(args.User), uid, component, null);
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0001F79C File Offset: 0x0001D99C
		private void OnBuyRequest(EntityUid uid, StoreComponent component, StoreBuyListingMessage msg)
		{
			ListingData listing = component.Listings.FirstOrDefault((ListingData x) => x.Equals(msg.Listing));
			if (listing == null)
			{
				Logger.Debug("listing does not exist");
				return;
			}
			EntityUid? attachedEntity = msg.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid buyer = attachedEntity.GetValueOrDefault();
				if (buyer.Valid)
				{
					if (!this.ListingHasCategory(listing, component.Categories))
					{
						return;
					}
					if (listing.Conditions != null)
					{
						ListingConditionArgs args = new ListingConditionArgs(component.AccountOwner ?? buyer, new EntityUid?(uid), listing, this.EntityManager);
						if (!listing.Conditions.All((ListingCondition condition) => condition.Condition(args)))
						{
							return;
						}
					}
					foreach (KeyValuePair<string, FixedPoint2> currency in listing.Cost)
					{
						FixedPoint2 balance;
						if (!component.Balance.TryGetValue(currency.Key, out balance) || balance < currency.Value)
						{
							return;
						}
					}
					foreach (KeyValuePair<string, FixedPoint2> currency2 in listing.Cost)
					{
						Dictionary<string, FixedPoint2> balance2 = component.Balance;
						string key = currency2.Key;
						balance2[key] -= currency2.Value;
					}
					if (listing.ProductEntity != null)
					{
						EntityUid product = base.Spawn(listing.ProductEntity, base.Transform(buyer).Coordinates);
						this._hands.PickupOrDrop(new EntityUid?(buyer), product, true, false, null, null);
					}
					if (listing.ProductAction != null)
					{
						InstantAction action = new InstantAction(this._proto.Index<InstantActionPrototype>(listing.ProductAction));
						this._actions.AddAction(buyer, action, null, null, true);
					}
					if (listing.ProductEvent != null)
					{
						base.RaiseLocalEvent(buyer, listing.ProductEvent, false);
					}
					ISharedAdminLogManager admin = this._admin;
					LogType type = LogType.StorePurchase;
					LogImpact impact = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(27, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(buyer), "player", "ToPrettyString(buyer)");
					logStringHandler.AppendLiteral(" purchased listing \"");
					logStringHandler.AppendFormatted(Loc.GetString(listing.Name));
					logStringHandler.AppendLiteral("\" from ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
					admin.Add(type, impact, ref logStringHandler);
					listing.PurchaseAmount++;
					this._audio.PlayEntity(component.BuySuccessSound, msg.Session, uid, null);
					this.UpdateUserInterface(new EntityUid?(buyer), uid, component, null);
					return;
				}
			}
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x0001FA8C File Offset: 0x0001DC8C
		private void OnRequestWithdraw(EntityUid uid, StoreComponent component, StoreRequestWithdrawMessage msg)
		{
			FixedPoint2 currentAmount;
			if (!component.Balance.TryGetValue(msg.Currency, out currentAmount) || currentAmount < msg.Amount)
			{
				return;
			}
			CurrencyPrototype proto;
			if (!this._proto.TryIndex<CurrencyPrototype>(msg.Currency, ref proto))
			{
				return;
			}
			if (proto.Cash == null || !proto.CanWithdraw)
			{
				return;
			}
			EntityUid? attachedEntity = msg.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid buyer = attachedEntity.GetValueOrDefault();
				if (buyer.Valid)
				{
					FixedPoint2 amountRemaining = msg.Amount;
					EntityCoordinates coordinates = base.Transform(buyer).Coordinates;
					foreach (FixedPoint2 value in (from x in proto.Cash.Keys
					orderby x descending
					select x).ToList<FixedPoint2>())
					{
						string cashId = proto.Cash[value];
						int amountToSpawn = (int)MathF.Floor((float)(amountRemaining / value));
						List<EntityUid> ents = this._stack.SpawnMultiple(cashId, amountToSpawn, coordinates);
						this._hands.PickupOrDrop(new EntityUid?(buyer), ents.First<EntityUid>(), true, false, null, null);
						amountRemaining -= value * amountToSpawn;
					}
					Dictionary<string, FixedPoint2> balance = component.Balance;
					string currency = msg.Currency;
					balance[currency] -= msg.Amount;
					this.UpdateUserInterface(new EntityUid?(buyer), uid, component, null);
					return;
				}
			}
		}

		// Token: 0x040003BF RID: 959
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x040003C0 RID: 960
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x040003C1 RID: 961
		[Dependency]
		private readonly IAdminLogManager _admin;

		// Token: 0x040003C2 RID: 962
		[Dependency]
		private readonly SharedHandsSystem _hands;

		// Token: 0x040003C3 RID: 963
		[Dependency]
		private readonly ActionsSystem _actions;

		// Token: 0x040003C4 RID: 964
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040003C5 RID: 965
		[Dependency]
		private readonly StackSystem _stack;

		// Token: 0x040003C6 RID: 966
		[Dependency]
		private readonly UserInterfaceSystem _ui;
	}
}
