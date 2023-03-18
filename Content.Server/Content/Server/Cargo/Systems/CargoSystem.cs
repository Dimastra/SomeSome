using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Access.Systems;
using Content.Server.Cargo.Components;
using Content.Server.Labels.Components;
using Content.Server.MachineLinking.System;
using Content.Server.Paper;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.UserInterface;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Cargo;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Events;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.CCVar;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Dataset;
using Content.Shared.GameTicking;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Cargo.Systems
{
	// Token: 0x020006E0 RID: 1760
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CargoSystem : SharedCargoSystem
	{
		// Token: 0x060024BC RID: 9404 RVA: 0x000BF3E6 File Offset: 0x000BD5E6
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("cargo");
			this.InitializeConsole();
			this.InitializeShuttle();
			this.InitializeTelepad();
			base.SubscribeLocalEvent<StationInitializedEvent>(new EntityEventHandler<StationInitializedEvent>(this.OnStationInit), null, null);
		}

		// Token: 0x060024BD RID: 9405 RVA: 0x000BF424 File Offset: 0x000BD624
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownShuttle();
			this.CleanupShuttle();
		}

		// Token: 0x060024BE RID: 9406 RVA: 0x000BF438 File Offset: 0x000BD638
		private void OnStationInit(StationInitializedEvent ev)
		{
			base.EnsureComp<StationBankAccountComponent>(ev.Station);
			base.EnsureComp<StationCargoOrderDatabaseComponent>(ev.Station);
		}

		// Token: 0x060024BF RID: 9407 RVA: 0x000BF454 File Offset: 0x000BD654
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateConsole(frameTime);
			this.UpdateTelepad(frameTime);
		}

		// Token: 0x060024C0 RID: 9408 RVA: 0x000BF46B File Offset: 0x000BD66B
		public void UpdateBankAccount(StationBankAccountComponent component, int balanceAdded)
		{
			component.Balance += balanceAdded;
		}

		// Token: 0x060024C1 RID: 9409 RVA: 0x000BF47C File Offset: 0x000BD67C
		private void InitializeConsole()
		{
			base.SubscribeLocalEvent<CargoOrderConsoleComponent, CargoConsoleAddOrderMessage>(new ComponentEventHandler<CargoOrderConsoleComponent, CargoConsoleAddOrderMessage>(this.OnAddOrderMessage), null, null);
			base.SubscribeLocalEvent<CargoOrderConsoleComponent, CargoConsoleRemoveOrderMessage>(new ComponentEventHandler<CargoOrderConsoleComponent, CargoConsoleRemoveOrderMessage>(this.OnRemoveOrderMessage), null, null);
			base.SubscribeLocalEvent<CargoOrderConsoleComponent, CargoConsoleApproveOrderMessage>(new ComponentEventHandler<CargoOrderConsoleComponent, CargoConsoleApproveOrderMessage>(this.OnApproveOrderMessage), null, null);
			base.SubscribeLocalEvent<CargoOrderConsoleComponent, BoundUIOpenedEvent>(new ComponentEventHandler<CargoOrderConsoleComponent, BoundUIOpenedEvent>(this.OnOrderUIOpened), null, null);
			base.SubscribeLocalEvent<CargoOrderConsoleComponent, ComponentInit>(new ComponentEventHandler<CargoOrderConsoleComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			this.Reset();
		}

		// Token: 0x060024C2 RID: 9410 RVA: 0x000BF508 File Offset: 0x000BD708
		private void OnInit(EntityUid uid, CargoOrderConsoleComponent orderConsole, ComponentInit args)
		{
			EntityUid? station = this._station.GetOwningStation(uid, null);
			this.UpdateOrderState(orderConsole, station);
		}

		// Token: 0x060024C3 RID: 9411 RVA: 0x000BF52B File Offset: 0x000BD72B
		private void Reset(RoundRestartCleanupEvent ev)
		{
			this.Reset();
		}

		// Token: 0x060024C4 RID: 9412 RVA: 0x000BF533 File Offset: 0x000BD733
		private void Reset()
		{
			this._timer = 0f;
		}

		// Token: 0x060024C5 RID: 9413 RVA: 0x000BF540 File Offset: 0x000BD740
		private void UpdateConsole(float frameTime)
		{
			this._timer += frameTime;
			while (this._timer > 10f)
			{
				this._timer -= 10f;
				foreach (StationBankAccountComponent account in base.EntityQuery<StationBankAccountComponent>(false))
				{
					account.Balance += account.IncreasePerSecond * 10;
				}
				foreach (CargoOrderConsoleComponent comp in base.EntityQuery<CargoOrderConsoleComponent>(false))
				{
					if (this._uiSystem.IsUiOpen(comp.Owner, CargoConsoleUiKey.Orders, null))
					{
						EntityUid? station = this._station.GetOwningStation(comp.Owner, null);
						this.UpdateOrderState(comp, station);
					}
				}
			}
		}

		// Token: 0x060024C6 RID: 9414 RVA: 0x000BF640 File Offset: 0x000BD840
		private void OnApproveOrderMessage(EntityUid uid, CargoOrderConsoleComponent component, CargoConsoleApproveOrderMessage args)
		{
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid player = attachedEntity.GetValueOrDefault();
				if (player.Valid)
				{
					if (!this._accessReaderSystem.IsAllowed(player, uid, null))
					{
						this.ConsolePopup(args.Session, Loc.GetString("cargo-console-order-not-allowed"));
						this.PlayDenySound(uid, component);
						return;
					}
					StationCargoOrderDatabaseComponent orderDatabase = this.GetOrderDatabase(component);
					StationBankAccountComponent bankAccount = this.GetBankAccount(component);
					if (orderDatabase == null || bankAccount == null)
					{
						this.ConsolePopup(args.Session, Loc.GetString("cargo-console-station-not-found"));
						this.PlayDenySound(uid, component);
						return;
					}
					CargoOrderData order;
					if (!orderDatabase.Orders.TryGetValue(args.OrderIndex, out order) || order.Approved)
					{
						return;
					}
					CargoProductPrototype product;
					if (!this._protoMan.TryIndex<CargoProductPrototype>(order.ProductId, ref product))
					{
						this.ConsolePopup(args.Session, Loc.GetString("cargo-console-invalid-product"));
						this.PlayDenySound(uid, component);
						return;
					}
					int amount = this.GetOrderCount(orderDatabase);
					int capacity = orderDatabase.Capacity;
					if (amount >= capacity)
					{
						this.ConsolePopup(args.Session, Loc.GetString("cargo-console-too-many"));
						this.PlayDenySound(uid, component);
						return;
					}
					int orderAmount = Math.Min(capacity - amount, order.Amount);
					if (orderAmount != order.Amount)
					{
						order.Amount = orderAmount;
						this.ConsolePopup(args.Session, Loc.GetString("cargo-console-snip-snip"));
						this.PlayDenySound(uid, component);
					}
					int cost = product.PointCost * order.Amount;
					if (cost > bankAccount.Balance)
					{
						this.ConsolePopup(args.Session, Loc.GetString("cargo-console-insufficient-funds", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("cost", cost)
						}));
						this.PlayDenySound(uid, component);
						return;
					}
					IdCardComponent idCard;
					this._idCardSystem.TryFindIdCard(player, out idCard);
					order.SetApproverData(idCard);
					this._audio.PlayPvs(this._audio.GetSound(component.ConfirmSound), uid, null);
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(84, 7);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "user", "ToPrettyString(player)");
					logStringHandler.AppendLiteral(" approved order [orderIdx:");
					logStringHandler.AppendFormatted<int>(order.OrderIndex, "order.OrderIndex");
					logStringHandler.AppendLiteral(", amount:");
					logStringHandler.AppendFormatted<int>(order.Amount, "order.Amount");
					logStringHandler.AppendLiteral(", product:");
					logStringHandler.AppendFormatted(order.ProductId);
					logStringHandler.AppendLiteral(", requester:");
					logStringHandler.AppendFormatted(order.Requester);
					logStringHandler.AppendLiteral(", reason:");
					logStringHandler.AppendFormatted(order.Reason);
					logStringHandler.AppendLiteral("] with balance at ");
					logStringHandler.AppendFormatted<int>(bankAccount.Balance, "bankAccount.Balance");
					adminLogger.Add(type, impact, ref logStringHandler);
					this.DeductFunds(bankAccount, cost);
					this.UpdateOrders(orderDatabase);
					return;
				}
			}
		}

		// Token: 0x060024C7 RID: 9415 RVA: 0x000BF920 File Offset: 0x000BDB20
		private void OnRemoveOrderMessage(EntityUid uid, CargoOrderConsoleComponent component, CargoConsoleRemoveOrderMessage args)
		{
			StationCargoOrderDatabaseComponent orderDatabase = this.GetOrderDatabase(component);
			if (orderDatabase == null)
			{
				return;
			}
			this.RemoveOrder(orderDatabase, args.OrderIndex);
		}

		// Token: 0x060024C8 RID: 9416 RVA: 0x000BF948 File Offset: 0x000BDB48
		private void OnAddOrderMessage(EntityUid uid, CargoOrderConsoleComponent component, CargoConsoleAddOrderMessage args)
		{
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid player = attachedEntity.GetValueOrDefault();
				if (player.Valid)
				{
					if (args.Amount <= 0)
					{
						return;
					}
					if (this.GetBankAccount(component) == null)
					{
						return;
					}
					StationCargoOrderDatabaseComponent orderDatabase = this.GetOrderDatabase(component);
					if (orderDatabase == null)
					{
						return;
					}
					CargoOrderData data = this.GetOrderData(args, this.GetNextIndex(orderDatabase));
					if (!this.TryAddOrder(orderDatabase, data))
					{
						this.PlayDenySound(uid, component);
						return;
					}
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(64, 6);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "user", "ToPrettyString(player)");
					logStringHandler.AppendLiteral(" added order [orderIdx:");
					logStringHandler.AppendFormatted<int>(data.OrderIndex, "data.OrderIndex");
					logStringHandler.AppendLiteral(", amount:");
					logStringHandler.AppendFormatted<int>(data.Amount, "data.Amount");
					logStringHandler.AppendLiteral(", product:");
					logStringHandler.AppendFormatted(data.ProductId);
					logStringHandler.AppendLiteral(", requester:");
					logStringHandler.AppendFormatted(data.Requester);
					logStringHandler.AppendLiteral(", reason:");
					logStringHandler.AppendFormatted(data.Reason);
					logStringHandler.AppendLiteral("]");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
			}
		}

		// Token: 0x060024C9 RID: 9417 RVA: 0x000BFA84 File Offset: 0x000BDC84
		private void OnOrderUIOpened(EntityUid uid, CargoOrderConsoleComponent component, BoundUIOpenedEvent args)
		{
			EntityUid? station = this._station.GetOwningStation(uid, null);
			this.UpdateOrderState(component, station);
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x000BFAA8 File Offset: 0x000BDCA8
		private void UpdateOrderState(CargoOrderConsoleComponent component, EntityUid? station)
		{
			StationCargoOrderDatabaseComponent orderDatabase;
			StationBankAccountComponent bankAccount;
			if (station == null || !base.TryComp<StationCargoOrderDatabaseComponent>(station, ref orderDatabase) || !base.TryComp<StationBankAccountComponent>(station, ref bankAccount))
			{
				return;
			}
			CargoConsoleInterfaceState state = new CargoConsoleInterfaceState(base.MetaData(station.Value).EntityName, this.GetOrderCount(orderDatabase), orderDatabase.Capacity, bankAccount.Balance, orderDatabase.Orders.Values.ToList<CargoOrderData>());
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(component.Owner, CargoConsoleUiKey.Orders, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.SetState(state, null, true);
		}

		// Token: 0x060024CB RID: 9419 RVA: 0x000BFB35 File Offset: 0x000BDD35
		private void ConsolePopup(ICommonSession session, string text)
		{
			this._popup.PopupCursor(text, session, PopupType.Small);
		}

		// Token: 0x060024CC RID: 9420 RVA: 0x000BFB48 File Offset: 0x000BDD48
		private void PlayDenySound(EntityUid uid, CargoOrderConsoleComponent component)
		{
			this._audio.PlayPvs(this._audio.GetSound(component.ErrorSound), uid, null);
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x000BFB7C File Offset: 0x000BDD7C
		private CargoOrderData GetOrderData(CargoConsoleAddOrderMessage args, int index)
		{
			return new CargoOrderData(index, args.ProductId, args.Amount, args.Requester, args.Reason);
		}

		// Token: 0x060024CE RID: 9422 RVA: 0x000BFB9C File Offset: 0x000BDD9C
		private int GetOrderCount(StationCargoOrderDatabaseComponent component)
		{
			int amount = 0;
			foreach (KeyValuePair<int, CargoOrderData> keyValuePair in component.Orders)
			{
				int num;
				CargoOrderData cargoOrderData;
				keyValuePair.Deconstruct(out num, out cargoOrderData);
				CargoOrderData order = cargoOrderData;
				if (order.Approved)
				{
					amount += order.Amount;
				}
			}
			return amount;
		}

		// Token: 0x060024CF RID: 9423 RVA: 0x000BFC0C File Offset: 0x000BDE0C
		private void UpdateOrders(StationCargoOrderDatabaseComponent component)
		{
			foreach (CargoOrderConsoleComponent comp in base.EntityQuery<CargoOrderConsoleComponent>(true))
			{
				EntityUid? station = this._station.GetOwningStation(component.Owner, null);
				EntityUid? entityUid = station;
				EntityUid owner = component.Owner;
				if (entityUid != null && (entityUid == null || !(entityUid.GetValueOrDefault() != owner)))
				{
					this.UpdateOrderState(comp, station);
				}
			}
			foreach (CargoShuttleConsoleComponent comp2 in base.EntityQuery<CargoShuttleConsoleComponent>(true))
			{
				EntityUid? station2 = this._station.GetOwningStation(component.Owner, null);
				EntityUid? entityUid = station2;
				EntityUid owner = component.Owner;
				if (entityUid != null && (entityUid == null || !(entityUid.GetValueOrDefault() != owner)))
				{
					this.UpdateShuttleState(comp2, station2);
				}
			}
		}

		// Token: 0x060024D0 RID: 9424 RVA: 0x000BFD30 File Offset: 0x000BDF30
		public bool TryAddOrder(StationCargoOrderDatabaseComponent component, CargoOrderData data)
		{
			component.Orders.Add(data.OrderIndex, data);
			this.UpdateOrders(component);
			return true;
		}

		// Token: 0x060024D1 RID: 9425 RVA: 0x000BFD4C File Offset: 0x000BDF4C
		private int GetNextIndex(StationCargoOrderDatabaseComponent component)
		{
			int index = component.Index;
			component.Index++;
			return index;
		}

		// Token: 0x060024D2 RID: 9426 RVA: 0x000BFD62 File Offset: 0x000BDF62
		public void RemoveOrder(StationCargoOrderDatabaseComponent component, int index)
		{
			if (!component.Orders.Remove(index))
			{
				return;
			}
			this.UpdateOrders(component);
		}

		// Token: 0x060024D3 RID: 9427 RVA: 0x000BFD7A File Offset: 0x000BDF7A
		public void ClearOrders(StationCargoOrderDatabaseComponent component)
		{
			if (component.Orders.Count == 0)
			{
				return;
			}
			component.Orders.Clear();
			base.Dirty(component, null);
		}

		// Token: 0x060024D4 RID: 9428 RVA: 0x000BFD9D File Offset: 0x000BDF9D
		private void DeductFunds(StationBankAccountComponent component, int amount)
		{
			component.Balance = Math.Max(0, component.Balance - amount);
			base.Dirty(component, null);
		}

		// Token: 0x060024D5 RID: 9429 RVA: 0x000BFDBC File Offset: 0x000BDFBC
		[return: Nullable(2)]
		private StationBankAccountComponent GetBankAccount(CargoOrderConsoleComponent component)
		{
			EntityUid? station = this._station.GetOwningStation(component.Owner, null);
			StationBankAccountComponent bankComponent;
			base.TryComp<StationBankAccountComponent>(station, ref bankComponent);
			return bankComponent;
		}

		// Token: 0x060024D6 RID: 9430 RVA: 0x000BFDE8 File Offset: 0x000BDFE8
		[return: Nullable(2)]
		private StationCargoOrderDatabaseComponent GetOrderDatabase(CargoOrderConsoleComponent component)
		{
			EntityUid? station = this._station.GetOwningStation(component.Owner, null);
			StationCargoOrderDatabaseComponent orderComponent;
			base.TryComp<StationCargoOrderDatabaseComponent>(station, ref orderComponent);
			return orderComponent;
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x060024D7 RID: 9431 RVA: 0x000BFE13 File Offset: 0x000BE013
		// (set) Token: 0x060024D8 RID: 9432 RVA: 0x000BFE1B File Offset: 0x000BE01B
		public MapId? CargoMap { get; private set; }

		// Token: 0x060024D9 RID: 9433 RVA: 0x000BFE24 File Offset: 0x000BE024
		private void InitializeShuttle()
		{
			this._enabled = this._configManager.GetCVar<bool>(CCVars.CargoShuttles);
			this._configManager.OnValueChanged<bool>(CCVars.CargoShuttles, new Action<bool>(this.SetCargoShuttleEnabled), false);
			base.SubscribeLocalEvent<CargoShuttleComponent, MoveEvent>(new ComponentEventRefHandler<CargoShuttleComponent, MoveEvent>(this.OnCargoShuttleMove), null, null);
			base.SubscribeLocalEvent<CargoShuttleConsoleComponent, ComponentStartup>(new ComponentEventHandler<CargoShuttleConsoleComponent, ComponentStartup>(this.OnCargoShuttleConsoleStartup), null, null);
			base.SubscribeLocalEvent<CargoShuttleConsoleComponent, CargoCallShuttleMessage>(new ComponentEventHandler<CargoShuttleConsoleComponent, CargoCallShuttleMessage>(this.OnCargoShuttleCall), null, null);
			base.SubscribeLocalEvent<CargoShuttleConsoleComponent, CargoRecallShuttleMessage>(new ComponentEventHandler<CargoShuttleConsoleComponent, CargoRecallShuttleMessage>(this.RecallCargoShuttle), null, null);
			base.SubscribeLocalEvent<CargoPilotConsoleComponent, ConsoleShuttleEvent>(new ComponentEventRefHandler<CargoPilotConsoleComponent, ConsoleShuttleEvent>(this.OnCargoGetConsole), null, null);
			base.SubscribeLocalEvent<CargoPilotConsoleComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<CargoPilotConsoleComponent, AfterActivatableUIOpenEvent>(this.OnCargoPilotConsoleOpen), null, null);
			base.SubscribeLocalEvent<CargoPilotConsoleComponent, BoundUIClosedEvent>(new ComponentEventHandler<CargoPilotConsoleComponent, BoundUIClosedEvent>(this.OnCargoPilotConsoleClose), null, null);
			base.SubscribeLocalEvent<StationCargoOrderDatabaseComponent, ComponentStartup>(new ComponentEventHandler<StationCargoOrderDatabaseComponent, ComponentStartup>(this.OnCargoOrderStartup), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
		}

		// Token: 0x060024DA RID: 9434 RVA: 0x000BFF18 File Offset: 0x000BE118
		private void ShutdownShuttle()
		{
			this._configManager.UnsubValueChanged<bool>(CCVars.CargoShuttles, new Action<bool>(this.SetCargoShuttleEnabled));
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x000BFF38 File Offset: 0x000BE138
		private void SetCargoShuttleEnabled(bool value)
		{
			if (this._enabled == value)
			{
				return;
			}
			this._enabled = value;
			if (value)
			{
				this.Setup();
				using (IEnumerator<StationCargoOrderDatabaseComponent> enumerator = base.EntityQuery<StationCargoOrderDatabaseComponent>(true).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StationCargoOrderDatabaseComponent station = enumerator.Current;
						this.AddShuttle(station);
					}
					return;
				}
			}
			this.CleanupShuttle();
		}

		// Token: 0x060024DC RID: 9436 RVA: 0x000BFFA8 File Offset: 0x000BE1A8
		private void OnCargoPilotConsoleOpen(EntityUid uid, CargoPilotConsoleComponent component, AfterActivatableUIOpenEvent args)
		{
			component.Entity = this.GetShuttleConsole(component);
		}

		// Token: 0x060024DD RID: 9437 RVA: 0x000BFFB7 File Offset: 0x000BE1B7
		private void OnCargoPilotConsoleClose(EntityUid uid, CargoPilotConsoleComponent component, BoundUIClosedEvent args)
		{
			component.Entity = null;
		}

		// Token: 0x060024DE RID: 9438 RVA: 0x000BFFC5 File Offset: 0x000BE1C5
		private void OnCargoGetConsole(EntityUid uid, CargoPilotConsoleComponent component, ref ConsoleShuttleEvent args)
		{
			args.Console = this.GetShuttleConsole(component);
		}

		// Token: 0x060024DF RID: 9439 RVA: 0x000BFFD4 File Offset: 0x000BE1D4
		private EntityUid? GetShuttleConsole(CargoPilotConsoleComponent component)
		{
			EntityUid? stationUid = this._station.GetOwningStation(component.Owner, null);
			StationCargoOrderDatabaseComponent orderDatabase;
			CargoShuttleComponent shuttle;
			if (!base.TryComp<StationCargoOrderDatabaseComponent>(stationUid, ref orderDatabase) || !base.TryComp<CargoShuttleComponent>(orderDatabase.Shuttle, ref shuttle))
			{
				return null;
			}
			return this.GetShuttleConsole(shuttle);
		}

		// Token: 0x060024E0 RID: 9440 RVA: 0x000C0020 File Offset: 0x000BE220
		private void UpdateShuttleCargoConsoles(CargoShuttleComponent component)
		{
			foreach (CargoShuttleConsoleComponent console in base.EntityQuery<CargoShuttleConsoleComponent>(true))
			{
				EntityUid? stationUid = this._station.GetOwningStation(console.Owner, null);
				if (!(stationUid != component.Station))
				{
					this.UpdateShuttleState(console, stationUid);
				}
			}
		}

		// Token: 0x060024E1 RID: 9441 RVA: 0x000C00C0 File Offset: 0x000BE2C0
		private void OnCargoShuttleConsoleStartup(EntityUid uid, CargoShuttleConsoleComponent component, ComponentStartup args)
		{
			EntityUid? station = this._station.GetOwningStation(uid, null);
			this.UpdateShuttleState(component, station);
		}

		// Token: 0x060024E2 RID: 9442 RVA: 0x000C00E4 File Offset: 0x000BE2E4
		private void UpdateShuttleState(CargoShuttleConsoleComponent component, EntityUid? station = null)
		{
			StationCargoOrderDatabaseComponent orderDatabase;
			base.TryComp<StationCargoOrderDatabaseComponent>(station, ref orderDatabase);
			CargoShuttleComponent shuttle;
			base.TryComp<CargoShuttleComponent>((orderDatabase != null) ? orderDatabase.Shuttle : null, ref shuttle);
			List<CargoOrderData> orders = this.GetProjectedOrders(orderDatabase, shuttle);
			string shuttleName = (orderDatabase != null && orderDatabase.Shuttle != null) ? base.MetaData(orderDatabase.Shuttle.Value).EntityName : string.Empty;
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(component.Owner, CargoConsoleUiKey.Shuttle, null);
			if (uiOrNull == null)
			{
				return;
			}
			string text;
			uiOrNull.SetState(new CargoShuttleConsoleBoundUserInterfaceState((station != null) ? base.MetaData(station.Value).EntityName : Loc.GetString("cargo-shuttle-console-station-unknown"), string.IsNullOrEmpty(shuttleName) ? Loc.GetString("cargo-shuttle-console-shuttle-not-found") : shuttleName, this._shuttle.CanFTL((shuttle != null) ? new EntityUid?(shuttle.Owner) : null, out text, null), (shuttle != null) ? shuttle.NextCall : null, orders), null, true);
		}

		// Token: 0x060024E3 RID: 9443 RVA: 0x000C01F4 File Offset: 0x000BE3F4
		public EntityUid? GetShuttleConsole(CargoShuttleComponent component)
		{
			foreach (ValueTuple<ShuttleConsoleComponent, TransformComponent> valueTuple in base.EntityQuery<ShuttleConsoleComponent, TransformComponent>(true))
			{
				ShuttleConsoleComponent comp = valueTuple.Item1;
				if (!(valueTuple.Item2.ParentUid != component.Owner))
				{
					return new EntityUid?(comp.Owner);
				}
			}
			return null;
		}

		// Token: 0x060024E4 RID: 9444 RVA: 0x000C0274 File Offset: 0x000BE474
		private void OnCargoShuttleMove(EntityUid uid, CargoShuttleComponent component, ref MoveEvent args)
		{
			if (component.Station == null)
			{
				return;
			}
			bool canRecall2 = component.CanRecall;
			string text;
			bool canRecall = this._shuttle.CanFTL(new EntityUid?(uid), out text, args.Component);
			if (canRecall2 == canRecall)
			{
				return;
			}
			component.CanRecall = canRecall;
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Updated CanRecall for ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			this.UpdateShuttleCargoConsoles(component);
		}

		// Token: 0x060024E5 RID: 9445 RVA: 0x000C02F8 File Offset: 0x000BE4F8
		[NullableContext(2)]
		[return: Nullable(1)]
		private List<CargoOrderData> GetProjectedOrders(StationCargoOrderDatabaseComponent component = null, CargoShuttleComponent shuttle = null)
		{
			List<CargoOrderData> orders = new List<CargoOrderData>();
			if (component == null || shuttle == null || component.Orders.Count == 0)
			{
				return orders;
			}
			int space = this.GetCargoSpace(shuttle);
			if (space == 0)
			{
				return orders;
			}
			List<int> list = component.Orders.Keys.ToList<int>();
			list.Sort();
			int amount = 0;
			foreach (int index in list)
			{
				CargoOrderData order = component.Orders[index];
				if (order.Approved)
				{
					int cappedAmount = Math.Min(space - amount, order.Amount);
					amount += cappedAmount;
					if (cappedAmount < order.Amount)
					{
						CargoOrderData reducedOrder = new CargoOrderData(order.OrderIndex, order.ProductId, cappedAmount, order.Requester, order.Reason);
						orders.Add(reducedOrder);
						break;
					}
					orders.Add(order);
					if (amount == space)
					{
						break;
					}
				}
			}
			return orders;
		}

		// Token: 0x060024E6 RID: 9446 RVA: 0x000C03FC File Offset: 0x000BE5FC
		private int GetCargoSpace(CargoShuttleComponent component)
		{
			return this.GetCargoPallets(component).Count;
		}

		// Token: 0x060024E7 RID: 9447 RVA: 0x000C040C File Offset: 0x000BE60C
		private List<CargoPalletComponent> GetCargoPallets(CargoShuttleComponent component)
		{
			List<CargoPalletComponent> pads = new List<CargoPalletComponent>();
			foreach (ValueTuple<CargoPalletComponent, TransformComponent> valueTuple in base.EntityQuery<CargoPalletComponent, TransformComponent>(true))
			{
				CargoPalletComponent comp = valueTuple.Item1;
				TransformComponent compXform = valueTuple.Item2;
				if (!(compXform.ParentUid != component.Owner) && compXform.Anchored)
				{
					pads.Add(comp);
				}
			}
			return pads;
		}

		// Token: 0x060024E8 RID: 9448 RVA: 0x000C0488 File Offset: 0x000BE688
		private void OnCargoOrderStartup(EntityUid uid, StationCargoOrderDatabaseComponent component, ComponentStartup args)
		{
			this.AddShuttle(component);
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x000C0494 File Offset: 0x000BE694
		private void AddShuttle(StationCargoOrderDatabaseComponent component)
		{
			this.Setup();
			if (this.CargoMap == null || component.Shuttle != null || component.CargoShuttleProto == null)
			{
				return;
			}
			CargoShuttlePrototype prototype = this._protoMan.Index<CargoShuttlePrototype>(component.CargoShuttleProto);
			IReadOnlyList<string> possibleNames = this._protoMan.Index<DatasetPrototype>(prototype.NameDataset).Values;
			string name = RandomExtensions.Pick<string>(this._random, possibleNames);
			IReadOnlyList<EntityUid> gridList;
			if (!this._map.TryLoad(this.CargoMap.Value, prototype.Path.ToString(), ref gridList, null))
			{
				this._sawmill.Error("Could not load the cargo shuttle!");
				return;
			}
			EntityUid shuttleUid = gridList[0];
			TransformComponent xform = base.Transform(shuttleUid);
			base.MetaData(shuttleUid).EntityName = name;
			xform.LocalPosition += (float)(100 * this._index);
			CargoShuttleComponent comp = base.EnsureComp<CargoShuttleComponent>(shuttleUid);
			comp.Station = new EntityUid?(component.Owner);
			comp.Coordinates = xform.Coordinates;
			component.Shuttle = new EntityUid?(shuttleUid);
			comp.NextCall = new TimeSpan?(this._timing.CurTime + TimeSpan.FromSeconds((double)comp.Cooldown));
			this.UpdateShuttleCargoConsoles(comp);
			this._index++;
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Added cargo shuttle to ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(shuttleUid));
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x060024EA RID: 9450 RVA: 0x000C062C File Offset: 0x000BE82C
		private void SellPallets(CargoShuttleComponent component, StationBankAccountComponent bank)
		{
			double amount = 0.0;
			HashSet<EntityUid> toSell = new HashSet<EntityUid>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<CargoSellBlacklistComponent> blacklistQuery = base.GetEntityQuery<CargoSellBlacklistComponent>();
			foreach (CargoPalletComponent pallet in this.GetCargoPallets(component))
			{
				foreach (EntityUid ent in this._lookup.GetEntitiesIntersecting(pallet.Owner, 11))
				{
					TransformComponent xform;
					if (!toSell.Contains(ent) && (!xformQuery.TryGetComponent(ent, ref xform) || !xform.Anchored) && !blacklistQuery.HasComponent(ent))
					{
						double price = this._pricing.GetPrice(ent);
						if (price != 0.0)
						{
							toSell.Add(ent);
							amount += price;
						}
					}
				}
			}
			bank.Balance += (int)amount;
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Cargo sold ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(toSell.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" entities for ");
			defaultInterpolatedStringHandler.AppendFormatted<double>(amount);
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			foreach (EntityUid ent2 in toSell)
			{
				base.Del(ent2);
			}
		}

		// Token: 0x060024EB RID: 9451 RVA: 0x000C07D8 File Offset: 0x000BE9D8
		[NullableContext(2)]
		private void SendToCargoMap(EntityUid uid, CargoShuttleComponent component = null)
		{
			if (!base.Resolve<CargoShuttleComponent>(uid, ref component, true))
			{
				return;
			}
			component.NextCall = new TimeSpan?(this._timing.CurTime + TimeSpan.FromSeconds((double)component.Cooldown));
			base.Transform(uid).Coordinates = component.Coordinates;
			this.UpdateShuttleCargoConsoles(component);
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Stashed cargo shuttle ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x060024EC RID: 9452 RVA: 0x000C0868 File Offset: 0x000BEA68
		public void CallShuttle(StationCargoOrderDatabaseComponent orderDatabase)
		{
			CargoShuttleComponent shuttle;
			if (!base.TryComp<CargoShuttleComponent>(orderDatabase.Shuttle, ref shuttle))
			{
				return;
			}
			if (shuttle.NextCall == null || this._timing.CurTime < shuttle.NextCall)
			{
				return;
			}
			StationDataComponent stationData;
			if (!base.TryComp<StationDataComponent>(orderDatabase.Owner, ref stationData))
			{
				return;
			}
			EntityUid? targetGrid = this._station.GetLargestGrid(stationData);
			TransformComponent xform;
			if (!base.TryComp<TransformComponent>(targetGrid, ref xform))
			{
				return;
			}
			shuttle.NextCall = null;
			Vector2 center = default(Vector2);
			float minRadius = 0f;
			Box2? aabb = null;
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			foreach (MapGridComponent grid in this._mapManager.GetAllMapGrids(xform.MapID))
			{
				Matrix3 worldMatrix = xformQuery.GetComponent(grid.Owner).WorldMatrix;
				Box2 box = grid.LocalAABB;
				Box2 worldAABB = worldMatrix.TransformBox(ref box);
				Box2 value;
				if (aabb == null)
				{
					value = worldAABB;
				}
				else
				{
					box = aabb.GetValueOrDefault();
					value = box.Union(ref worldAABB);
				}
				aabb = new Box2?(value);
			}
			if (aabb != null)
			{
				Box2 box = aabb.Value;
				center = box.Center;
				box = aabb.Value;
				float width = box.Width;
				box = aabb.Value;
				minRadius = MathF.Max(width, box.Height);
			}
			float offset = 0f;
			MapGridComponent shuttleGrid;
			if (base.TryComp<MapGridComponent>(orderDatabase.Shuttle, ref shuttleGrid))
			{
				Box2 bounds = shuttleGrid.LocalAABB;
				offset = MathF.Max(bounds.Width, bounds.Height) / 2f;
			}
			base.Transform(shuttle.Owner).Coordinates = new EntityCoordinates(xform.ParentUid, center + this._random.NextVector2(minRadius + offset, minRadius + 50f + offset));
			this.AddCargoContents(shuttle, orderDatabase);
			this.UpdateOrders(orderDatabase);
			this.UpdateShuttleCargoConsoles(shuttle);
			this._console.RefreshShuttleConsoles();
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Retrieved cargo shuttle ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(shuttle.Owner));
			defaultInterpolatedStringHandler.AppendLiteral(" from ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(orderDatabase.Owner));
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x060024ED RID: 9453 RVA: 0x000C0AEC File Offset: 0x000BECEC
		private void AddCargoContents(CargoShuttleComponent component, StationCargoOrderDatabaseComponent orderDatabase)
		{
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			List<CargoOrderData> orders = this.GetProjectedOrders(orderDatabase, component);
			List<CargoPalletComponent> pads = this.GetCargoPallets(component);
			int i = 0;
			while (i < pads.Count && orders.Count != 0)
			{
				CargoOrderData order = orders[0];
				EntityCoordinates coordinates;
				coordinates..ctor(component.Owner, xformQuery.GetComponent(RandomExtensions.PickAndTake<CargoPalletComponent>(this._random, pads).Owner).LocalPosition);
				EntityUid item = base.Spawn(this._protoMan.Index<CargoProductPrototype>(order.ProductId).Product, coordinates);
				this.SpawnAndAttachOrderManifest(item, order, coordinates, component);
				order.Amount--;
				if (order.Amount == 0)
				{
					Extensions.RemoveSwap<CargoOrderData>(orders, 0);
					orderDatabase.Orders.Remove(order.OrderIndex);
				}
				else
				{
					orderDatabase.Orders[order.OrderIndex] = order;
				}
				i++;
			}
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x000C0BE0 File Offset: 0x000BEDE0
		private void SpawnAndAttachOrderManifest(EntityUid item, CargoOrderData order, EntityCoordinates coordinates, CargoShuttleComponent component)
		{
			CargoProductPrototype prototype;
			if (!this._protoMan.TryIndex<CargoProductPrototype>(order.ProductId, ref prototype))
			{
				return;
			}
			EntityUid printed = this.EntityManager.SpawnEntity(component.PrinterOutput, coordinates);
			PaperComponent paper;
			if (!base.TryComp<PaperComponent>(printed, ref paper))
			{
				return;
			}
			string val = Loc.GetString("cargo-console-paper-print-name", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("orderNumber", order.PrintableOrderNumber)
			});
			base.MetaData(printed).EntityName = val;
			this._paperSystem.SetContent(printed, Loc.GetString("cargo-console-paper-print-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("orderNumber", order.PrintableOrderNumber),
				new ValueTuple<string, object>("itemName", prototype.Name),
				new ValueTuple<string, object>("requester", order.Requester),
				new ValueTuple<string, object>("reason", order.Reason),
				new ValueTuple<string, object>("approver", order.Approver ?? string.Empty)
			}), paper);
			PaperLabelComponent label;
			if (base.TryComp<PaperLabelComponent>(item, ref label))
			{
				this._slots.TryInsert(item, label.LabelSlot, printed, null);
			}
		}

		// Token: 0x060024EF RID: 9455 RVA: 0x000C0D28 File Offset: 0x000BEF28
		private void RecallCargoShuttle(EntityUid uid, CargoShuttleConsoleComponent component, CargoRecallShuttleMessage args)
		{
			EntityUid? player = args.Session.AttachedEntity;
			if (player == null)
			{
				return;
			}
			EntityUid? stationUid = this._station.GetOwningStation(component.Owner, null);
			StationCargoOrderDatabaseComponent orderDatabase;
			StationBankAccountComponent bank;
			if (!base.TryComp<StationCargoOrderDatabaseComponent>(stationUid, ref orderDatabase) || !base.TryComp<StationBankAccountComponent>(stationUid, ref bank))
			{
				return;
			}
			CargoShuttleComponent shuttle;
			if (!base.TryComp<CargoShuttleComponent>(orderDatabase.Shuttle, ref shuttle))
			{
				this._popup.PopupEntity(Loc.GetString("cargo-no-shuttle"), args.Entity, args.Entity, PopupType.Small);
				return;
			}
			string reason;
			if (!this._shuttle.CanFTL(new EntityUid?(shuttle.Owner), out reason, null))
			{
				this._popup.PopupEntity(reason, args.Entity, args.Entity, PopupType.Small);
				return;
			}
			if (this.IsBlocked(shuttle))
			{
				this._popup.PopupEntity(Loc.GetString("cargo-shuttle-console-organics"), player.Value, player.Value, PopupType.Small);
				this._audio.PlayPvs(this._audio.GetSound(component.DenySound), uid, null);
				return;
			}
			this.SellPallets(shuttle, bank);
			this._console.RefreshShuttleConsoles();
			this.SendToCargoMap(orderDatabase.Shuttle.Value, null);
		}

		// Token: 0x060024F0 RID: 9456 RVA: 0x000C0E5C File Offset: 0x000BF05C
		private bool IsBlocked(CargoShuttleComponent component)
		{
			EntityQuery<MobStateComponent> mobQuery = base.GetEntityQuery<MobStateComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			return this.FoundOrganics(component.Owner, mobQuery, xformQuery);
		}

		// Token: 0x060024F1 RID: 9457 RVA: 0x000C0E88 File Offset: 0x000BF088
		public bool FoundOrganics(EntityUid uid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<MobStateComponent> mobQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			EntityUid? child;
			while (xformQuery.GetComponent(uid).ChildEnumerator.MoveNext(ref child))
			{
				MobStateComponent mobState;
				if ((mobQuery.TryGetComponent(child.Value, ref mobState) && !this._mobState.IsDead(child.Value, mobState)) || this.FoundOrganics(child.Value, mobQuery, xformQuery))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060024F2 RID: 9458 RVA: 0x000C0EEC File Offset: 0x000BF0EC
		private void OnCargoShuttleCall(EntityUid uid, CargoShuttleConsoleComponent component, CargoCallShuttleMessage args)
		{
			EntityUid? stationUid = this._station.GetOwningStation(args.Entity, null);
			StationCargoOrderDatabaseComponent orderDatabase;
			if (!base.TryComp<StationCargoOrderDatabaseComponent>(stationUid, ref orderDatabase))
			{
				return;
			}
			this.CallShuttle(orderDatabase);
		}

		// Token: 0x060024F3 RID: 9459 RVA: 0x000C0F1F File Offset: 0x000BF11F
		private void OnRoundRestart(RoundRestartCleanupEvent ev)
		{
			this.CleanupShuttle();
		}

		// Token: 0x060024F4 RID: 9460 RVA: 0x000C0F28 File Offset: 0x000BF128
		private void CleanupShuttle()
		{
			if (this.CargoMap == null || !this._mapManager.MapExists(this.CargoMap.Value))
			{
				this.CargoMap = null;
				return;
			}
			this._mapManager.DeleteMap(this.CargoMap.Value);
			this.CargoMap = null;
			foreach (CargoShuttleComponent comp in base.EntityQuery<CargoShuttleComponent>(false))
			{
				StationCargoOrderDatabaseComponent station;
				if (base.TryComp<StationCargoOrderDatabaseComponent>(comp.Station, ref station))
				{
					station.Shuttle = null;
				}
				base.QueueDel(comp.Owner);
			}
		}

		// Token: 0x060024F5 RID: 9461 RVA: 0x000C0FFC File Offset: 0x000BF1FC
		private void Setup()
		{
			if (!this._enabled || (this.CargoMap != null && this._mapManager.MapExists(this.CargoMap.Value)))
			{
				return;
			}
			this.CargoMap = new MapId?(this._mapManager.CreateMap(null));
			this._mapManager.SetMapPaused(this.CargoMap.Value, true);
			foreach (StationCargoOrderDatabaseComponent comp in base.EntityQuery<StationCargoOrderDatabaseComponent>(true))
			{
				this.AddShuttle(comp);
			}
		}

		// Token: 0x060024F6 RID: 9462 RVA: 0x000C10B8 File Offset: 0x000BF2B8
		private void InitializeTelepad()
		{
			base.SubscribeLocalEvent<CargoTelepadComponent, ComponentInit>(new ComponentEventHandler<CargoTelepadComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<CargoTelepadComponent, PowerChangedEvent>(new ComponentEventRefHandler<CargoTelepadComponent, PowerChangedEvent>(this.OnTelepadPowerChange), null, null);
			base.SubscribeLocalEvent<CargoTelepadComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<CargoTelepadComponent, AnchorStateChangedEvent>(this.OnTelepadAnchorChange), null, null);
		}

		// Token: 0x060024F7 RID: 9463 RVA: 0x000C10F8 File Offset: 0x000BF2F8
		private unsafe void UpdateTelepad(float frameTime)
		{
			foreach (CargoTelepadComponent comp in this.EntityManager.EntityQuery<CargoTelepadComponent>(false))
			{
				AppearanceComponent appearance;
				base.TryComp<AppearanceComponent>(comp.Owner, ref appearance);
				if (comp.CurrentState == CargoTelepadState.Unpowered)
				{
					comp.CurrentState = CargoTelepadState.Idle;
					this._appearance.SetData(comp.Owner, CargoTelepadVisuals.State, CargoTelepadState.Idle, appearance);
					comp.Accumulator = comp.Delay;
				}
				else
				{
					comp.Accumulator -= frameTime;
					if (comp.Accumulator > 0f)
					{
						comp.CurrentState = CargoTelepadState.Idle;
						this._appearance.SetData(comp.Owner, CargoTelepadVisuals.State, CargoTelepadState.Idle, appearance);
					}
					else
					{
						EntityUid? station = this._station.GetOwningStation(comp.Owner, null);
						StationCargoOrderDatabaseComponent orderDatabase;
						if (!base.TryComp<StationCargoOrderDatabaseComponent>(station, ref orderDatabase) || orderDatabase.Orders.Count == 0)
						{
							comp.Accumulator += comp.Delay;
						}
						else
						{
							ValueList<int> orderIndices = default(ValueList<int>);
							foreach (KeyValuePair<int, CargoOrderData> keyValuePair in orderDatabase.Orders)
							{
								int num;
								CargoOrderData cargoOrderData;
								keyValuePair.Deconstruct(out num, out cargoOrderData);
								int oIndex = num;
								if (cargoOrderData.Approved)
								{
									orderIndices.Add(oIndex);
								}
							}
							if (orderIndices.Count == 0)
							{
								comp.Accumulator += comp.Delay;
							}
							else
							{
								orderIndices.Sort();
								int index = *orderIndices[0];
								CargoOrderData order = orderDatabase.Orders[index];
								order.Amount--;
								if (order.Amount <= 0)
								{
									orderDatabase.Orders.Remove(index);
								}
								this._audio.PlayPvs(this._audio.GetSound(comp.TeleportSound), comp.Owner, new AudioParams?(AudioParams.Default.WithVolume(-8f)));
								this.SpawnProduct(comp, order);
								this.UpdateOrders(orderDatabase);
								comp.CurrentState = CargoTelepadState.Teleporting;
								this._appearance.SetData(comp.Owner, CargoTelepadVisuals.State, CargoTelepadState.Teleporting, appearance);
								comp.Accumulator += comp.Delay;
							}
						}
					}
				}
			}
		}

		// Token: 0x060024F8 RID: 9464 RVA: 0x000C138C File Offset: 0x000BF58C
		private void OnInit(EntityUid uid, CargoTelepadComponent telepad, ComponentInit args)
		{
			this._linker.EnsureReceiverPorts(uid, new string[]
			{
				telepad.ReceiverPort
			});
		}

		// Token: 0x060024F9 RID: 9465 RVA: 0x000C13AC File Offset: 0x000BF5AC
		[NullableContext(2)]
		private void SetEnabled([Nullable(1)] CargoTelepadComponent component, ApcPowerReceiverComponent receiver = null, TransformComponent xform = null)
		{
			if (!base.Resolve<ApcPowerReceiverComponent, TransformComponent>(component.Owner, ref receiver, ref xform, false))
			{
				return;
			}
			if (!receiver.Powered || !xform.Anchored)
			{
				return;
			}
			AppearanceComponent appearance;
			base.TryComp<AppearanceComponent>(component.Owner, ref appearance);
			component.CurrentState = CargoTelepadState.Unpowered;
			this._appearance.SetData(component.Owner, CargoTelepadVisuals.State, CargoTelepadState.Unpowered, appearance);
		}

		// Token: 0x060024FA RID: 9466 RVA: 0x000C1418 File Offset: 0x000BF618
		private void OnTelepadPowerChange(EntityUid uid, CargoTelepadComponent component, ref PowerChangedEvent args)
		{
			this.SetEnabled(component, null, null);
		}

		// Token: 0x060024FB RID: 9467 RVA: 0x000C1423 File Offset: 0x000BF623
		private void OnTelepadAnchorChange(EntityUid uid, CargoTelepadComponent component, ref AnchorStateChangedEvent args)
		{
			this.SetEnabled(component, null, null);
		}

		// Token: 0x060024FC RID: 9468 RVA: 0x000C1430 File Offset: 0x000BF630
		private void SpawnProduct(CargoTelepadComponent component, CargoOrderData data)
		{
			CargoProductPrototype prototype;
			if (!this._protoMan.TryIndex<CargoProductPrototype>(data.ProductId, ref prototype))
			{
				return;
			}
			TransformComponent xform = base.Transform(component.Owner);
			EntityUid product = this.EntityManager.SpawnEntity(prototype.Product, xform.Coordinates);
			base.Transform(product).Anchored = false;
			EntityUid printed = this.EntityManager.SpawnEntity(component.PrinterOutput, xform.Coordinates);
			PaperComponent paper;
			if (!base.TryComp<PaperComponent>(printed, ref paper))
			{
				return;
			}
			string val = Loc.GetString("cargo-console-paper-print-name", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("orderNumber", data.PrintableOrderNumber)
			});
			base.MetaData(printed).EntityName = val;
			this._paperSystem.SetContent(printed, Loc.GetString("cargo-console-paper-print-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("orderNumber", data.PrintableOrderNumber),
				new ValueTuple<string, object>("itemName", prototype.Name),
				new ValueTuple<string, object>("requester", data.Requester),
				new ValueTuple<string, object>("reason", data.Reason),
				new ValueTuple<string, object>("approver", data.Approver ?? string.Empty)
			}), paper);
			PaperLabelComponent label;
			if (base.TryComp<PaperLabelComponent>(product, ref label))
			{
				this._slots.TryInsert(product, label.LabelSlot, printed, null);
			}
		}

		// Token: 0x04001691 RID: 5777
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04001692 RID: 5778
		[Dependency]
		private readonly ItemSlotsSystem _slots;

		// Token: 0x04001693 RID: 5779
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04001694 RID: 5780
		private ISawmill _sawmill;

		// Token: 0x04001695 RID: 5781
		private const int Delay = 10;

		// Token: 0x04001696 RID: 5782
		private float _timer;

		// Token: 0x04001697 RID: 5783
		[Dependency]
		private readonly IdCardSystem _idCardSystem;

		// Token: 0x04001698 RID: 5784
		[Dependency]
		private readonly AccessReaderSystem _accessReaderSystem;

		// Token: 0x04001699 RID: 5785
		[Dependency]
		private readonly SignalLinkerSystem _linker;

		// Token: 0x0400169A RID: 5786
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x0400169B RID: 5787
		[Dependency]
		private readonly StationSystem _station;

		// Token: 0x0400169C RID: 5788
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x0400169D RID: 5789
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x0400169E RID: 5790
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x0400169F RID: 5791
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040016A0 RID: 5792
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040016A1 RID: 5793
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040016A2 RID: 5794
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040016A3 RID: 5795
		[Dependency]
		private readonly MapLoaderSystem _map;

		// Token: 0x040016A4 RID: 5796
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x040016A5 RID: 5797
		[Dependency]
		private readonly PricingSystem _pricing;

		// Token: 0x040016A6 RID: 5798
		[Dependency]
		private readonly ShuttleConsoleSystem _console;

		// Token: 0x040016A7 RID: 5799
		[Dependency]
		private readonly ShuttleSystem _shuttle;

		// Token: 0x040016A9 RID: 5801
		private const float CallOffset = 50f;

		// Token: 0x040016AA RID: 5802
		private int _index;

		// Token: 0x040016AB RID: 5803
		private bool _enabled;

		// Token: 0x040016AC RID: 5804
		[Dependency]
		private readonly PaperSystem _paperSystem;

		// Token: 0x040016AD RID: 5805
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
