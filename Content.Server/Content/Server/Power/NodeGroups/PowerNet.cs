using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Power.Pow3r;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.NodeGroups
{
	// Token: 0x02000286 RID: 646
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	[NodeGroup(new NodeGroupID[]
	{
		NodeGroupID.HVPower,
		NodeGroupID.MVPower
	})]
	public sealed class PowerNet : BaseNetConnectorNodeGroup<IPowerNet>, IPowerNet, IBasePowerNet
	{
		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000CE6 RID: 3302 RVA: 0x000436AC File Offset: 0x000418AC
		[ViewVariables]
		public PowerState.Network NetworkNode { get; } = new PowerState.Network();

		// Token: 0x06000CE7 RID: 3303 RVA: 0x000436B4 File Offset: 0x000418B4
		public override void Initialize(Node sourceNode, IEntityManager entMan)
		{
			base.Initialize(sourceNode, entMan);
			this._entMan = entMan;
			this._powerNetSystem = entMan.EntitySysManager.GetEntitySystem<PowerNetSystem>();
			this._powerNetSystem.InitPowerNet(this);
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x000436E2 File Offset: 0x000418E2
		public override void AfterRemake([Nullable(new byte[]
		{
			1,
			1,
			2,
			1
		})] IEnumerable<IGrouping<INodeGroup, Node>> newGroups)
		{
			base.AfterRemake(newGroups);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.DestroyPowerNet(this);
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x000436FC File Offset: 0x000418FC
		protected override void SetNetConnectorNet(IBaseNetConnectorComponent<IPowerNet> netConnectorComponent)
		{
			netConnectorComponent.Net = this;
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x00043705 File Offset: 0x00041905
		public void AddSupplier(PowerSupplierComponent supplier)
		{
			supplier.NetworkSupply.LinkedNetwork = default(PowerState.NodeId);
			this.Suppliers.Add(supplier);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectPowerNet(this);
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x00043735 File Offset: 0x00041935
		public void RemoveSupplier(PowerSupplierComponent supplier)
		{
			supplier.NetworkSupply.LinkedNetwork = default(PowerState.NodeId);
			this.Suppliers.Remove(supplier);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectPowerNet(this);
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x00043766 File Offset: 0x00041966
		public void AddConsumer(PowerConsumerComponent consumer)
		{
			consumer.NetworkLoad.LinkedNetwork = default(PowerState.NodeId);
			this.Consumers.Add(consumer);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectPowerNet(this);
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x00043796 File Offset: 0x00041996
		public void RemoveConsumer(PowerConsumerComponent consumer)
		{
			consumer.NetworkLoad.LinkedNetwork = default(PowerState.NodeId);
			this.Consumers.Remove(consumer);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectPowerNet(this);
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x000437C8 File Offset: 0x000419C8
		public void AddDischarger(BatteryDischargerComponent discharger)
		{
			if (this._entMan == null)
			{
				return;
			}
			this._entMan.GetComponent<PowerNetworkBatteryComponent>(discharger.Owner).NetworkBattery.LinkedNetworkDischarging = default(PowerState.NodeId);
			this.Dischargers.Add(discharger);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectPowerNet(this);
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x0004381C File Offset: 0x00041A1C
		public void RemoveDischarger(BatteryDischargerComponent discharger)
		{
			if (this._entMan == null)
			{
				return;
			}
			PowerNetworkBatteryComponent battery;
			if (this._entMan.TryGetComponent<PowerNetworkBatteryComponent>(discharger.Owner, ref battery))
			{
				battery.NetworkBattery.LinkedNetworkDischarging = default(PowerState.NodeId);
			}
			this.Dischargers.Remove(discharger);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectPowerNet(this);
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x00043878 File Offset: 0x00041A78
		public void AddCharger(BatteryChargerComponent charger)
		{
			if (this._entMan == null)
			{
				return;
			}
			this._entMan.GetComponent<PowerNetworkBatteryComponent>(charger.Owner).NetworkBattery.LinkedNetworkCharging = default(PowerState.NodeId);
			this.Chargers.Add(charger);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectPowerNet(this);
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x000438CC File Offset: 0x00041ACC
		public void RemoveCharger(BatteryChargerComponent charger)
		{
			if (this._entMan == null)
			{
				return;
			}
			PowerNetworkBatteryComponent battery;
			if (this._entMan.TryGetComponent<PowerNetworkBatteryComponent>(charger.Owner, ref battery))
			{
				battery.NetworkBattery.LinkedNetworkCharging = default(PowerState.NodeId);
			}
			this.Chargers.Remove(charger);
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectPowerNet(this);
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x00043928 File Offset: 0x00041B28
		[NullableContext(2)]
		public override string GetDebugData()
		{
			if (this._powerNetSystem == null)
			{
				return null;
			}
			NetworkPowerStatistics ps = this._powerNetSystem.GetNetworkStatistics(this.NetworkNode);
			float storageRatio = ps.InStorageCurrent / Math.Max(ps.InStorageMax, 1f);
			float outStorageRatio = ps.OutStorageCurrent / Math.Max(ps.OutStorageMax, 1f);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(119, 10);
			defaultInterpolatedStringHandler.AppendLiteral("Current Supply: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ps.SupplyCurrent, "G3");
			defaultInterpolatedStringHandler.AppendLiteral("\nFrom Batteries: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ps.SupplyBatteries, "G3");
			defaultInterpolatedStringHandler.AppendLiteral("\nTheoretical Supply: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ps.SupplyTheoretical, "G3");
			defaultInterpolatedStringHandler.AppendLiteral("\nIdeal Consumption: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ps.Consumption, "G3");
			defaultInterpolatedStringHandler.AppendLiteral("\nInput Storage: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ps.InStorageCurrent, "G3");
			defaultInterpolatedStringHandler.AppendLiteral(" / ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ps.InStorageMax, "G3");
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted<float>(storageRatio, "P1");
			defaultInterpolatedStringHandler.AppendLiteral(")\nOutput Storage: ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ps.OutStorageCurrent, "G3");
			defaultInterpolatedStringHandler.AppendLiteral(" / ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ps.OutStorageMax, "G3");
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted<float>(outStorageRatio, "P1");
			defaultInterpolatedStringHandler.AppendLiteral(")");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x040007D3 RID: 2003
		[Nullable(2)]
		private PowerNetSystem _powerNetSystem;

		// Token: 0x040007D4 RID: 2004
		[Nullable(2)]
		private IEntityManager _entMan;

		// Token: 0x040007D5 RID: 2005
		[ViewVariables]
		public readonly List<PowerSupplierComponent> Suppliers = new List<PowerSupplierComponent>();

		// Token: 0x040007D6 RID: 2006
		[ViewVariables]
		public readonly List<PowerConsumerComponent> Consumers = new List<PowerConsumerComponent>();

		// Token: 0x040007D7 RID: 2007
		[ViewVariables]
		public readonly List<BatteryChargerComponent> Chargers = new List<BatteryChargerComponent>();

		// Token: 0x040007D8 RID: 2008
		[ViewVariables]
		public readonly List<BatteryDischargerComponent> Dischargers = new List<BatteryDischargerComponent>();
	}
}
