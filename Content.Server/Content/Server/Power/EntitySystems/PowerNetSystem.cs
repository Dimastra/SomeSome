using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.Power.Components;
using Content.Server.Power.NodeGroups;
using Content.Server.Power.Pow3r;
using Content.Shared.Power;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Threading;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x02000292 RID: 658
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PowerNetSystem : EntitySystem
	{
		// Token: 0x06000D48 RID: 3400 RVA: 0x00045B58 File Offset: 0x00043D58
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesAfter.Add(typeof(NodeGroupSystem));
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, ComponentInit>(new ComponentEventHandler<ApcPowerReceiverComponent, ComponentInit>(this.ApcPowerReceiverInit), null, null);
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, ComponentShutdown>(new ComponentEventHandler<ApcPowerReceiverComponent, ComponentShutdown>(this.ApcPowerReceiverShutdown), null, null);
			ComponentEventRefHandler<ApcPowerReceiverComponent, EntityPausedEvent> componentEventRefHandler;
			if ((componentEventRefHandler = PowerNetSystem.<>O.<0>__ApcPowerReceiverPaused) == null)
			{
				componentEventRefHandler = (PowerNetSystem.<>O.<0>__ApcPowerReceiverPaused = new ComponentEventRefHandler<ApcPowerReceiverComponent, EntityPausedEvent>(PowerNetSystem.ApcPowerReceiverPaused));
			}
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, EntityPausedEvent>(componentEventRefHandler, null, null);
			ComponentEventRefHandler<ApcPowerReceiverComponent, EntityUnpausedEvent> componentEventRefHandler2;
			if ((componentEventRefHandler2 = PowerNetSystem.<>O.<1>__ApcPowerReceiverUnpaused) == null)
			{
				componentEventRefHandler2 = (PowerNetSystem.<>O.<1>__ApcPowerReceiverUnpaused = new ComponentEventRefHandler<ApcPowerReceiverComponent, EntityUnpausedEvent>(PowerNetSystem.ApcPowerReceiverUnpaused));
			}
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, EntityUnpausedEvent>(componentEventRefHandler2, null, null);
			base.SubscribeLocalEvent<PowerNetworkBatteryComponent, ComponentInit>(new ComponentEventHandler<PowerNetworkBatteryComponent, ComponentInit>(this.BatteryInit), null, null);
			base.SubscribeLocalEvent<PowerNetworkBatteryComponent, ComponentShutdown>(new ComponentEventHandler<PowerNetworkBatteryComponent, ComponentShutdown>(this.BatteryShutdown), null, null);
			ComponentEventRefHandler<PowerNetworkBatteryComponent, EntityPausedEvent> componentEventRefHandler3;
			if ((componentEventRefHandler3 = PowerNetSystem.<>O.<2>__BatteryPaused) == null)
			{
				componentEventRefHandler3 = (PowerNetSystem.<>O.<2>__BatteryPaused = new ComponentEventRefHandler<PowerNetworkBatteryComponent, EntityPausedEvent>(PowerNetSystem.BatteryPaused));
			}
			base.SubscribeLocalEvent<PowerNetworkBatteryComponent, EntityPausedEvent>(componentEventRefHandler3, null, null);
			ComponentEventRefHandler<PowerNetworkBatteryComponent, EntityUnpausedEvent> componentEventRefHandler4;
			if ((componentEventRefHandler4 = PowerNetSystem.<>O.<3>__BatteryUnpaused) == null)
			{
				componentEventRefHandler4 = (PowerNetSystem.<>O.<3>__BatteryUnpaused = new ComponentEventRefHandler<PowerNetworkBatteryComponent, EntityUnpausedEvent>(PowerNetSystem.BatteryUnpaused));
			}
			base.SubscribeLocalEvent<PowerNetworkBatteryComponent, EntityUnpausedEvent>(componentEventRefHandler4, null, null);
			base.SubscribeLocalEvent<PowerConsumerComponent, ComponentInit>(new ComponentEventHandler<PowerConsumerComponent, ComponentInit>(this.PowerConsumerInit), null, null);
			base.SubscribeLocalEvent<PowerConsumerComponent, ComponentShutdown>(new ComponentEventHandler<PowerConsumerComponent, ComponentShutdown>(this.PowerConsumerShutdown), null, null);
			ComponentEventRefHandler<PowerConsumerComponent, EntityPausedEvent> componentEventRefHandler5;
			if ((componentEventRefHandler5 = PowerNetSystem.<>O.<4>__PowerConsumerPaused) == null)
			{
				componentEventRefHandler5 = (PowerNetSystem.<>O.<4>__PowerConsumerPaused = new ComponentEventRefHandler<PowerConsumerComponent, EntityPausedEvent>(PowerNetSystem.PowerConsumerPaused));
			}
			base.SubscribeLocalEvent<PowerConsumerComponent, EntityPausedEvent>(componentEventRefHandler5, null, null);
			ComponentEventRefHandler<PowerConsumerComponent, EntityUnpausedEvent> componentEventRefHandler6;
			if ((componentEventRefHandler6 = PowerNetSystem.<>O.<5>__PowerConsumerUnpaused) == null)
			{
				componentEventRefHandler6 = (PowerNetSystem.<>O.<5>__PowerConsumerUnpaused = new ComponentEventRefHandler<PowerConsumerComponent, EntityUnpausedEvent>(PowerNetSystem.PowerConsumerUnpaused));
			}
			base.SubscribeLocalEvent<PowerConsumerComponent, EntityUnpausedEvent>(componentEventRefHandler6, null, null);
			base.SubscribeLocalEvent<PowerSupplierComponent, ComponentInit>(new ComponentEventHandler<PowerSupplierComponent, ComponentInit>(this.PowerSupplierInit), null, null);
			base.SubscribeLocalEvent<PowerSupplierComponent, ComponentShutdown>(new ComponentEventHandler<PowerSupplierComponent, ComponentShutdown>(this.PowerSupplierShutdown), null, null);
			ComponentEventRefHandler<PowerSupplierComponent, EntityPausedEvent> componentEventRefHandler7;
			if ((componentEventRefHandler7 = PowerNetSystem.<>O.<6>__PowerSupplierPaused) == null)
			{
				componentEventRefHandler7 = (PowerNetSystem.<>O.<6>__PowerSupplierPaused = new ComponentEventRefHandler<PowerSupplierComponent, EntityPausedEvent>(PowerNetSystem.PowerSupplierPaused));
			}
			base.SubscribeLocalEvent<PowerSupplierComponent, EntityPausedEvent>(componentEventRefHandler7, null, null);
			ComponentEventRefHandler<PowerSupplierComponent, EntityUnpausedEvent> componentEventRefHandler8;
			if ((componentEventRefHandler8 = PowerNetSystem.<>O.<7>__PowerSupplierUnpaused) == null)
			{
				componentEventRefHandler8 = (PowerNetSystem.<>O.<7>__PowerSupplierUnpaused = new ComponentEventRefHandler<PowerSupplierComponent, EntityUnpausedEvent>(PowerNetSystem.PowerSupplierUnpaused));
			}
			base.SubscribeLocalEvent<PowerSupplierComponent, EntityUnpausedEvent>(componentEventRefHandler8, null, null);
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x00045D38 File Offset: 0x00043F38
		private void ApcPowerReceiverInit(EntityUid uid, ApcPowerReceiverComponent component, ComponentInit args)
		{
			this.AllocLoad(component.NetworkLoad);
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x00045D46 File Offset: 0x00043F46
		private void ApcPowerReceiverShutdown(EntityUid uid, ApcPowerReceiverComponent component, ComponentShutdown args)
		{
			this._powerState.Loads.Free(component.NetworkLoad.Id);
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x00045D63 File Offset: 0x00043F63
		private static void ApcPowerReceiverPaused(EntityUid uid, ApcPowerReceiverComponent component, ref EntityPausedEvent args)
		{
			component.NetworkLoad.Paused = true;
		}

		// Token: 0x06000D4C RID: 3404 RVA: 0x00045D71 File Offset: 0x00043F71
		private static void ApcPowerReceiverUnpaused(EntityUid uid, ApcPowerReceiverComponent component, ref EntityUnpausedEvent args)
		{
			component.NetworkLoad.Paused = false;
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x00045D7F File Offset: 0x00043F7F
		private void BatteryInit(EntityUid uid, PowerNetworkBatteryComponent component, ComponentInit args)
		{
			this.AllocBattery(component.NetworkBattery);
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x00045D8D File Offset: 0x00043F8D
		private void BatteryShutdown(EntityUid uid, PowerNetworkBatteryComponent component, ComponentShutdown args)
		{
			this._powerState.Batteries.Free(component.NetworkBattery.Id);
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x00045DAA File Offset: 0x00043FAA
		private static void BatteryPaused(EntityUid uid, PowerNetworkBatteryComponent component, ref EntityPausedEvent args)
		{
			component.NetworkBattery.Paused = true;
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x00045DB8 File Offset: 0x00043FB8
		private static void BatteryUnpaused(EntityUid uid, PowerNetworkBatteryComponent component, ref EntityUnpausedEvent args)
		{
			component.NetworkBattery.Paused = false;
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x00045DC6 File Offset: 0x00043FC6
		private void PowerConsumerInit(EntityUid uid, PowerConsumerComponent component, ComponentInit args)
		{
			this.AllocLoad(component.NetworkLoad);
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x00045DD4 File Offset: 0x00043FD4
		private void PowerConsumerShutdown(EntityUid uid, PowerConsumerComponent component, ComponentShutdown args)
		{
			this._powerState.Loads.Free(component.NetworkLoad.Id);
		}

		// Token: 0x06000D53 RID: 3411 RVA: 0x00045DF1 File Offset: 0x00043FF1
		private static void PowerConsumerPaused(EntityUid uid, PowerConsumerComponent component, ref EntityPausedEvent args)
		{
			component.NetworkLoad.Paused = true;
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x00045DFF File Offset: 0x00043FFF
		private static void PowerConsumerUnpaused(EntityUid uid, PowerConsumerComponent component, ref EntityUnpausedEvent args)
		{
			component.NetworkLoad.Paused = false;
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x00045E0D File Offset: 0x0004400D
		private void PowerSupplierInit(EntityUid uid, PowerSupplierComponent component, ComponentInit args)
		{
			this.AllocSupply(component.NetworkSupply);
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x00045E1B File Offset: 0x0004401B
		private void PowerSupplierShutdown(EntityUid uid, PowerSupplierComponent component, ComponentShutdown args)
		{
			this._powerState.Supplies.Free(component.NetworkSupply.Id);
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x00045E38 File Offset: 0x00044038
		private static void PowerSupplierPaused(EntityUid uid, PowerSupplierComponent component, ref EntityPausedEvent args)
		{
			component.NetworkSupply.Paused = true;
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x00045E46 File Offset: 0x00044046
		private static void PowerSupplierUnpaused(EntityUid uid, PowerSupplierComponent component, ref EntityUnpausedEvent args)
		{
			component.NetworkSupply.Paused = false;
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x00045E54 File Offset: 0x00044054
		public void InitPowerNet(PowerNet powerNet)
		{
			this.AllocNetwork(powerNet.NetworkNode);
			this._powerState.GroupedNets = null;
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x00045E6E File Offset: 0x0004406E
		public void DestroyPowerNet(PowerNet powerNet)
		{
			this._powerState.Networks.Free(powerNet.NetworkNode.Id);
			this._powerState.GroupedNets = null;
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x00045E97 File Offset: 0x00044097
		public void QueueReconnectPowerNet(PowerNet powerNet)
		{
			this._powerNetReconnectQueue.Add(powerNet);
			this._powerState.GroupedNets = null;
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x00045EB2 File Offset: 0x000440B2
		public void InitApcNet(ApcNet apcNet)
		{
			this.AllocNetwork(apcNet.NetworkNode);
			this._powerState.GroupedNets = null;
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x00045ECC File Offset: 0x000440CC
		public void DestroyApcNet(ApcNet apcNet)
		{
			this._powerState.Networks.Free(apcNet.NetworkNode.Id);
			this._powerState.GroupedNets = null;
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x00045EF5 File Offset: 0x000440F5
		public void QueueReconnectApcNet(ApcNet apcNet)
		{
			this._apcNetReconnectQueue.Add(apcNet);
			this._powerState.GroupedNets = null;
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x00045F10 File Offset: 0x00044110
		public PowerStatistics GetStatistics()
		{
			return new PowerStatistics
			{
				CountBatteries = this._powerState.Batteries.Count,
				CountLoads = this._powerState.Loads.Count,
				CountNetworks = this._powerState.Networks.Count,
				CountSupplies = this._powerState.Supplies.Count
			};
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x00045F84 File Offset: 0x00044184
		public unsafe NetworkPowerStatistics GetNetworkStatistics(PowerState.Network network)
		{
			float consumptionW = network.Loads.Sum((PowerState.NodeId s) => this._powerState.Loads[s]->DesiredPower);
			consumptionW += network.BatteryLoads.Sum((PowerState.NodeId s) => this._powerState.Batteries[s]->CurrentReceiving);
			float maxSupplyW = network.Supplies.Sum((PowerState.NodeId s) => this._powerState.Supplies[s]->MaxSupply);
			float supplyBatteriesW = 0f;
			float storageCurrentJ = 0f;
			float storageMaxJ = 0f;
			foreach (PowerState.NodeId discharger in network.BatterySupplies)
			{
				PowerState.Battery nb = *this._powerState.Batteries[discharger];
				supplyBatteriesW += nb.CurrentSupply;
				storageCurrentJ += nb.CurrentStorage;
				storageMaxJ += nb.Capacity;
				maxSupplyW += nb.MaxSupply;
			}
			float outStorageCurrentJ = 0f;
			float outStorageMaxJ = 0f;
			foreach (PowerState.NodeId charger in network.BatteryLoads)
			{
				PowerState.Battery nb2 = *this._powerState.Batteries[charger];
				outStorageCurrentJ += nb2.CurrentStorage;
				outStorageMaxJ += nb2.Capacity;
			}
			return new NetworkPowerStatistics
			{
				SupplyCurrent = network.LastCombinedMaxSupply,
				SupplyBatteries = supplyBatteriesW,
				SupplyTheoretical = maxSupplyW,
				Consumption = consumptionW,
				InStorageCurrent = storageCurrentJ,
				InStorageMax = storageMaxJ,
				OutStorageCurrent = outStorageCurrentJ,
				OutStorageMax = outStorageMaxJ
			};
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x00046138 File Offset: 0x00044338
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.ReconnectNetworks();
			base.RaiseLocalEvent<NetworkBatteryPreSync>(default(NetworkBatteryPreSync));
			this._solver.Tick(frameTime, this._powerState, this._parMan.ParallelProcessCount);
			base.RaiseLocalEvent<NetworkBatteryPostSync>(default(NetworkBatteryPostSync));
			this.UpdateApcPowerReceiver();
			this.UpdatePowerConsumer();
			this.UpdateNetworkBattery();
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x000461A0 File Offset: 0x000443A0
		private void ReconnectNetworks()
		{
			foreach (ApcNet apcNet in this._apcNetReconnectQueue)
			{
				if (!apcNet.Removed)
				{
					this.DoReconnectApcNet(apcNet);
				}
			}
			this._apcNetReconnectQueue.Clear();
			foreach (PowerNet powerNet in this._powerNetReconnectQueue)
			{
				if (!powerNet.Removed)
				{
					this.DoReconnectPowerNet(powerNet);
				}
			}
			this._powerNetReconnectQueue.Clear();
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x0004625C File Offset: 0x0004445C
		private void UpdateApcPowerReceiver()
		{
			EntityQuery<AppearanceComponent> appearanceQuery = base.GetEntityQuery<AppearanceComponent>();
			ApcPowerReceiverComponent apcReceiver;
			while (base.EntityQueryEnumerator<ApcPowerReceiverComponent>().MoveNext(ref apcReceiver))
			{
				bool powered = apcReceiver.Powered;
				bool flag = powered;
				bool? poweredLastUpdate = apcReceiver.PoweredLastUpdate;
				if (!(flag == poweredLastUpdate.GetValueOrDefault() & poweredLastUpdate != null))
				{
					apcReceiver.PoweredLastUpdate = new bool?(powered);
					PowerChangedEvent ev = new PowerChangedEvent(apcReceiver.Powered, apcReceiver.NetworkLoad.ReceivingPower);
					base.RaiseLocalEvent<PowerChangedEvent>(apcReceiver.Owner, ref ev, false);
					AppearanceComponent appearance;
					if (appearanceQuery.TryGetComponent(apcReceiver.Owner, ref appearance))
					{
						this._appearance.SetData(appearance.Owner, PowerDeviceVisuals.Powered, powered, appearance);
					}
				}
			}
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x00046314 File Offset: 0x00044514
		private void UpdatePowerConsumer()
		{
			PowerConsumerComponent consumer;
			while (base.EntityQueryEnumerator<PowerConsumerComponent>().MoveNext(ref consumer))
			{
				float newRecv = consumer.NetworkLoad.ReceivingPower;
				ref float lastRecv = ref consumer.LastReceived;
				if (!MathHelper.CloseToPercent(lastRecv, newRecv, 1E-05))
				{
					lastRecv = newRecv;
					PowerConsumerReceivedChanged msg = new PowerConsumerReceivedChanged(newRecv, consumer.DrawRate);
					base.RaiseLocalEvent<PowerConsumerReceivedChanged>(consumer.Owner, ref msg, false);
				}
			}
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x0004637C File Offset: 0x0004457C
		private void UpdateNetworkBattery()
		{
			PowerNetworkBatteryComponent powerNetBattery;
			while (base.EntityQueryEnumerator<PowerNetworkBatteryComponent>().MoveNext(ref powerNetBattery))
			{
				float lastSupply = powerNetBattery.LastSupply;
				float currentSupply = powerNetBattery.CurrentSupply;
				if (lastSupply == 0f && currentSupply != 0f)
				{
					PowerNetBatterySupplyEvent ev = new PowerNetBatterySupplyEvent(true);
					base.RaiseLocalEvent<PowerNetBatterySupplyEvent>(powerNetBattery.Owner, ref ev, false);
				}
				else if (lastSupply > 0f && currentSupply == 0f)
				{
					PowerNetBatterySupplyEvent ev2 = new PowerNetBatterySupplyEvent(false);
					base.RaiseLocalEvent<PowerNetBatterySupplyEvent>(powerNetBattery.Owner, ref ev2, false);
				}
				powerNetBattery.LastSupply = currentSupply;
			}
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x00046402 File Offset: 0x00044602
		private unsafe void AllocLoad(PowerState.Load load)
		{
			*this._powerState.Loads.Allocate(out load.Id) = load;
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x0004641C File Offset: 0x0004461C
		private unsafe void AllocSupply(PowerState.Supply supply)
		{
			*this._powerState.Supplies.Allocate(out supply.Id) = supply;
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x00046436 File Offset: 0x00044636
		private unsafe void AllocBattery(PowerState.Battery battery)
		{
			*this._powerState.Batteries.Allocate(out battery.Id) = battery;
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x00046450 File Offset: 0x00044650
		private unsafe void AllocNetwork(PowerState.Network network)
		{
			*this._powerState.Networks.Allocate(out network.Id) = network;
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x0004646C File Offset: 0x0004466C
		private void DoReconnectApcNet(ApcNet net)
		{
			PowerState.Network netNode = net.NetworkNode;
			netNode.Loads.Clear();
			netNode.BatterySupplies.Clear();
			netNode.BatteryLoads.Clear();
			netNode.Supplies.Clear();
			foreach (ApcPowerProviderComponent apcPowerProviderComponent in net.Providers)
			{
				foreach (ApcPowerReceiverComponent receiver in apcPowerProviderComponent.LinkedReceivers)
				{
					netNode.Loads.Add(receiver.NetworkLoad.Id);
					receiver.NetworkLoad.LinkedNetwork = netNode.Id;
				}
			}
			foreach (PowerConsumerComponent consumer in net.Consumers)
			{
				netNode.Loads.Add(consumer.NetworkLoad.Id);
				consumer.NetworkLoad.LinkedNetwork = netNode.Id;
			}
			EntityQuery<PowerNetworkBatteryComponent> batteryQuery = base.GetEntityQuery<PowerNetworkBatteryComponent>();
			foreach (ApcComponent apc in net.Apcs)
			{
				PowerNetworkBatteryComponent netBattery = batteryQuery.GetComponent(apc.Owner);
				netNode.BatterySupplies.Add(netBattery.NetworkBattery.Id);
				netBattery.NetworkBattery.LinkedNetworkDischarging = netNode.Id;
			}
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x00046634 File Offset: 0x00044834
		private void DoReconnectPowerNet(PowerNet net)
		{
			PowerState.Network netNode = net.NetworkNode;
			netNode.Loads.Clear();
			netNode.Supplies.Clear();
			netNode.BatteryLoads.Clear();
			netNode.BatterySupplies.Clear();
			foreach (PowerConsumerComponent consumer in net.Consumers)
			{
				netNode.Loads.Add(consumer.NetworkLoad.Id);
				consumer.NetworkLoad.LinkedNetwork = netNode.Id;
			}
			foreach (PowerSupplierComponent supplier in net.Suppliers)
			{
				netNode.Supplies.Add(supplier.NetworkSupply.Id);
				supplier.NetworkSupply.LinkedNetwork = netNode.Id;
			}
			EntityQuery<PowerNetworkBatteryComponent> batteryQuery = base.GetEntityQuery<PowerNetworkBatteryComponent>();
			foreach (BatteryChargerComponent charger in net.Chargers)
			{
				PowerNetworkBatteryComponent battery = batteryQuery.GetComponent(charger.Owner);
				netNode.BatteryLoads.Add(battery.NetworkBattery.Id);
				battery.NetworkBattery.LinkedNetworkCharging = netNode.Id;
			}
			foreach (BatteryDischargerComponent discharger in net.Dischargers)
			{
				PowerNetworkBatteryComponent battery2 = batteryQuery.GetComponent(discharger.Owner);
				netNode.BatterySupplies.Add(battery2.NetworkBattery.Id);
				battery2.NetworkBattery.LinkedNetworkDischarging = netNode.Id;
			}
		}

		// Token: 0x040007F8 RID: 2040
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x040007F9 RID: 2041
		[Dependency]
		private readonly IParallelManager _parMan;

		// Token: 0x040007FA RID: 2042
		private readonly PowerState _powerState = new PowerState();

		// Token: 0x040007FB RID: 2043
		private readonly HashSet<PowerNet> _powerNetReconnectQueue = new HashSet<PowerNet>();

		// Token: 0x040007FC RID: 2044
		private readonly HashSet<ApcNet> _apcNetReconnectQueue = new HashSet<ApcNet>();

		// Token: 0x040007FD RID: 2045
		private readonly BatteryRampPegSolver _solver = new BatteryRampPegSolver();

		// Token: 0x02000945 RID: 2373
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04001F9A RID: 8090
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<ApcPowerReceiverComponent, EntityPausedEvent> <0>__ApcPowerReceiverPaused;

			// Token: 0x04001F9B RID: 8091
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<ApcPowerReceiverComponent, EntityUnpausedEvent> <1>__ApcPowerReceiverUnpaused;

			// Token: 0x04001F9C RID: 8092
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<PowerNetworkBatteryComponent, EntityPausedEvent> <2>__BatteryPaused;

			// Token: 0x04001F9D RID: 8093
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<PowerNetworkBatteryComponent, EntityUnpausedEvent> <3>__BatteryUnpaused;

			// Token: 0x04001F9E RID: 8094
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<PowerConsumerComponent, EntityPausedEvent> <4>__PowerConsumerPaused;

			// Token: 0x04001F9F RID: 8095
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<PowerConsumerComponent, EntityUnpausedEvent> <5>__PowerConsumerUnpaused;

			// Token: 0x04001FA0 RID: 8096
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<PowerSupplierComponent, EntityPausedEvent> <6>__PowerSupplierPaused;

			// Token: 0x04001FA1 RID: 8097
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<PowerSupplierComponent, EntityUnpausedEvent> <7>__PowerSupplierUnpaused;
		}
	}
}
