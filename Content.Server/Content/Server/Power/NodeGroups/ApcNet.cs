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
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.NodeGroups
{
	// Token: 0x02000282 RID: 642
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	[NodeGroup(new NodeGroupID[]
	{
		NodeGroupID.Apc
	})]
	public sealed class ApcNet : BaseNetConnectorNodeGroup<IApcNet>, IApcNet, IBasePowerNet
	{
		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000CCB RID: 3275 RVA: 0x000431FE File Offset: 0x000413FE
		[ViewVariables]
		private int TotalReceivers
		{
			get
			{
				return this.Providers.Sum((ApcPowerProviderComponent provider) => provider.LinkedReceivers.Count);
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000CCC RID: 3276 RVA: 0x0004322A File Offset: 0x0004142A
		[ViewVariables]
		private IEnumerable<ApcPowerReceiverComponent> AllReceivers
		{
			get
			{
				return this.Providers.SelectMany((ApcPowerProviderComponent provider) => provider.LinkedReceivers);
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000CCD RID: 3277 RVA: 0x00043256 File Offset: 0x00041456
		[ViewVariables]
		public PowerState.Network NetworkNode { get; } = new PowerState.Network();

		// Token: 0x06000CCE RID: 3278 RVA: 0x0004325E File Offset: 0x0004145E
		public override void Initialize(Node sourceNode, IEntityManager entMan)
		{
			base.Initialize(sourceNode, entMan);
			this._powerNetSystem = entMan.EntitySysManager.GetEntitySystem<PowerNetSystem>();
			this._powerNetSystem.InitApcNet(this);
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x00043285 File Offset: 0x00041485
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
			powerNetSystem.DestroyApcNet(this);
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x000432A0 File Offset: 0x000414A0
		public void AddApc(ApcComponent apc)
		{
			PowerNetworkBatteryComponent netBattery;
			if (IoCManager.Resolve<IEntityManager>().TryGetComponent<PowerNetworkBatteryComponent>(apc.Owner, ref netBattery))
			{
				netBattery.NetworkBattery.LinkedNetworkDischarging = default(PowerState.NodeId);
			}
			this.QueueNetworkReconnect();
			this.Apcs.Add(apc);
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x000432E4 File Offset: 0x000414E4
		public void RemoveApc(ApcComponent apc)
		{
			PowerNetworkBatteryComponent netBattery;
			if (IoCManager.Resolve<IEntityManager>().TryGetComponent<PowerNetworkBatteryComponent>(apc.Owner, ref netBattery))
			{
				netBattery.NetworkBattery.LinkedNetworkDischarging = default(PowerState.NodeId);
			}
			this.QueueNetworkReconnect();
			this.Apcs.Remove(apc);
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x00043329 File Offset: 0x00041529
		public void AddPowerProvider(ApcPowerProviderComponent provider)
		{
			this.Providers.Add(provider);
			this.QueueNetworkReconnect();
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x0004333D File Offset: 0x0004153D
		public void RemovePowerProvider(ApcPowerProviderComponent provider)
		{
			this.Providers.Remove(provider);
			this.QueueNetworkReconnect();
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x00043352 File Offset: 0x00041552
		public void AddConsumer(PowerConsumerComponent consumer)
		{
			consumer.NetworkLoad.LinkedNetwork = default(PowerState.NodeId);
			this.Consumers.Add(consumer);
			this.QueueNetworkReconnect();
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x00043377 File Offset: 0x00041577
		public void RemoveConsumer(PowerConsumerComponent consumer)
		{
			consumer.NetworkLoad.LinkedNetwork = default(PowerState.NodeId);
			this.Consumers.Remove(consumer);
			this.QueueNetworkReconnect();
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0004339D File Offset: 0x0004159D
		public void QueueNetworkReconnect()
		{
			PowerNetSystem powerNetSystem = this._powerNetSystem;
			if (powerNetSystem == null)
			{
				return;
			}
			powerNetSystem.QueueReconnectApcNet(this);
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x000433B0 File Offset: 0x000415B0
		protected override void SetNetConnectorNet(IBaseNetConnectorComponent<IApcNet> netConnectorComponent)
		{
			netConnectorComponent.Net = this;
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x000433BC File Offset: 0x000415BC
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

		// Token: 0x040007CE RID: 1998
		[Nullable(2)]
		private PowerNetSystem _powerNetSystem;

		// Token: 0x040007CF RID: 1999
		[ViewVariables]
		public readonly List<ApcComponent> Apcs = new List<ApcComponent>();

		// Token: 0x040007D0 RID: 2000
		[ViewVariables]
		public readonly List<ApcPowerProviderComponent> Providers = new List<ApcPowerProviderComponent>();

		// Token: 0x040007D1 RID: 2001
		[ViewVariables]
		public readonly List<PowerConsumerComponent> Consumers = new List<PowerConsumerComponent>();
	}
}
