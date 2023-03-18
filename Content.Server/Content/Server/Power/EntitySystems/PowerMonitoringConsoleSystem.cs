using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.NodeGroups;
using Content.Shared.Power;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x02000291 RID: 657
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class PowerMonitoringConsoleSystem : EntitySystem
	{
		// Token: 0x06000D43 RID: 3395 RVA: 0x00045804 File Offset: 0x00043A04
		public override void Update(float frameTime)
		{
			this._updateTimer += frameTime;
			if (this._updateTimer >= 1f)
			{
				this._updateTimer -= 1f;
				foreach (PowerMonitoringConsoleComponent component in base.EntityQuery<PowerMonitoringConsoleComponent>(false))
				{
					this.UpdateUIState(component.Owner, component, null);
				}
			}
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x00045888 File Offset: 0x00043A88
		[NullableContext(2)]
		public void UpdateUIState(EntityUid target, PowerMonitoringConsoleComponent pmcComp = null, NodeContainerComponent ncComp = null)
		{
			if (!base.Resolve<PowerMonitoringConsoleComponent>(target, ref pmcComp, true))
			{
				return;
			}
			if (!base.Resolve<NodeContainerComponent>(target, ref ncComp, true))
			{
				return;
			}
			double totalSources = 0.0;
			double totalLoads = 0.0;
			List<PowerMonitoringConsoleEntry> sources = new List<PowerMonitoringConsoleEntry>();
			List<PowerMonitoringConsoleEntry> loads = new List<PowerMonitoringConsoleEntry>();
			PowerNet netQ = ncComp.GetNode<Node>("hv").NodeGroup as PowerNet;
			if (netQ != null)
			{
				foreach (PowerConsumerComponent pcc in netQ.Consumers)
				{
					loads.Add(this.<UpdateUIState>g__LoadOrSource|4_0(pcc, (double)pcc.DrawRate, false));
					totalLoads += (double)pcc.DrawRate;
				}
				foreach (BatteryChargerComponent pcc2 in netQ.Chargers)
				{
					PowerNetworkBatteryComponent batteryComp;
					if (base.TryComp<PowerNetworkBatteryComponent>(pcc2.Owner, ref batteryComp))
					{
						float rate = batteryComp.NetworkBattery.CurrentReceiving;
						loads.Add(this.<UpdateUIState>g__LoadOrSource|4_0(pcc2, (double)rate, true));
						totalLoads += (double)rate;
					}
				}
				foreach (PowerSupplierComponent pcc3 in netQ.Suppliers)
				{
					sources.Add(this.<UpdateUIState>g__LoadOrSource|4_0(pcc3, (double)pcc3.MaxSupply, false));
					totalSources += (double)pcc3.MaxSupply;
				}
				foreach (BatteryDischargerComponent pcc4 in netQ.Dischargers)
				{
					PowerNetworkBatteryComponent batteryComp2;
					if (base.TryComp<PowerNetworkBatteryComponent>(pcc4.Owner, ref batteryComp2))
					{
						float rate2 = batteryComp2.NetworkBattery.CurrentSupply;
						sources.Add(this.<UpdateUIState>g__LoadOrSource|4_0(pcc4, (double)rate2, true));
						totalSources += (double)rate2;
					}
				}
			}
			loads.Sort(new Comparison<PowerMonitoringConsoleEntry>(this.CompareLoadOrSources));
			sources.Sort(new Comparison<PowerMonitoringConsoleEntry>(this.CompareLoadOrSources));
			PowerMonitoringConsoleBoundInterfaceState state = new PowerMonitoringConsoleBoundInterfaceState(totalSources, totalLoads, sources.ToArray(), loads.ToArray());
			BoundUserInterface uiOrNull = this._userInterfaceSystem.GetUiOrNull(target, PowerMonitoringConsoleUiKey.Key, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.SetState(state, null, true);
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x00045AF8 File Offset: 0x00043CF8
		private int CompareLoadOrSources(PowerMonitoringConsoleEntry x, PowerMonitoringConsoleEntry y)
		{
			return -x.Size.CompareTo(y.Size);
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x00045B14 File Offset: 0x00043D14
		[CompilerGenerated]
		private PowerMonitoringConsoleEntry <UpdateUIState>g__LoadOrSource|4_0(Component comp, double rate, bool isBattery)
		{
			MetaDataComponent metaDataComponent = base.MetaData(comp.Owner);
			EntityPrototype entityPrototype = metaDataComponent.EntityPrototype;
			string prototype = ((entityPrototype != null) ? entityPrototype.ID : null) ?? "";
			return new PowerMonitoringConsoleEntry(metaDataComponent.EntityName, prototype, rate, isBattery);
		}

		// Token: 0x040007F5 RID: 2037
		private float _updateTimer;

		// Token: 0x040007F6 RID: 2038
		private const float UpdateTime = 1f;

		// Token: 0x040007F7 RID: 2039
		[Dependency]
		private UserInterfaceSystem _userInterfaceSystem;
	}
}
