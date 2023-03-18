using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Content.Server.Power.Pow3r
{
	// Token: 0x02000278 RID: 632
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BatteryRampPegSolver : IPowerSolver
	{
		// Token: 0x06000CB0 RID: 3248 RVA: 0x00042458 File Offset: 0x00040658
		public void Tick(float frameTime, PowerState state, int parallel)
		{
			this.ClearLoadsAndSupplies(state);
			PowerState state2 = state;
			if (state2.GroupedNets == null)
			{
				state2.GroupedNets = this.GroupByNetworkDepth(state);
			}
			ParallelOptions opts = new ParallelOptions
			{
				MaxDegreeOfParallelism = parallel
			};
			Action<PowerState.Network> <>9__0;
			foreach (IEnumerable<PowerState.Network> source in state.GroupedNets)
			{
				ParallelOptions parallelOptions = opts;
				Action<PowerState.Network> body;
				if ((body = <>9__0) == null)
				{
					body = (<>9__0 = delegate(PowerState.Network net)
					{
						this.UpdateNetwork(net, state, frameTime);
					});
				}
				Parallel.ForEach<PowerState.Network>(source, parallelOptions, body);
			}
			this.ClearBatteries(state);
			PowerSolverShared.UpdateRampPositions(frameTime, state);
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x00042544 File Offset: 0x00040744
		private void ClearLoadsAndSupplies(PowerState state)
		{
			foreach (PowerState.Load load in state.Loads.Values)
			{
				if (!load.Paused)
				{
					load.ReceivingPower = 0f;
				}
			}
			foreach (PowerState.Supply supply in state.Supplies.Values)
			{
				if (!supply.Paused)
				{
					supply.CurrentSupply = 0f;
					supply.SupplyRampTarget = 0f;
				}
			}
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x00042614 File Offset: 0x00040814
		private unsafe void UpdateNetwork(PowerState.Network network, PowerState state, float frameTime)
		{
			float demand = 0f;
			foreach (PowerState.NodeId loadId in network.Loads)
			{
				PowerState.Load load = *state.Loads[loadId];
				if (load.Enabled && !load.Paused)
				{
					demand += load.DesiredPower;
				}
			}
			foreach (PowerState.NodeId batteryId in network.BatteryLoads)
			{
				PowerState.Battery battery = *state.Batteries[batteryId];
				if (battery.Enabled && battery.CanCharge && !battery.Paused)
				{
					float batterySpace = (battery.Capacity - battery.CurrentStorage) * (1f / battery.Efficiency);
					batterySpace = Math.Max(0f, batterySpace);
					float scaledSpace = batterySpace / frameTime;
					float chargeRate = battery.MaxChargeRate + battery.LoadingNetworkDemand / battery.Efficiency;
					battery.DesiredPower = Math.Min(chargeRate, scaledSpace);
					demand += battery.DesiredPower;
				}
			}
			float totalSupply = 0f;
			float totalMaxSupply = 0f;
			foreach (PowerState.NodeId supplyId in network.Supplies)
			{
				PowerState.Supply supply = *state.Supplies[supplyId];
				if (supply.Enabled && !supply.Paused)
				{
					float effectiveSupply = Math.Min(supply.SupplyRampPosition + supply.SupplyRampTolerance, supply.MaxSupply);
					supply.AvailableSupply = effectiveSupply;
					totalSupply += effectiveSupply;
					totalMaxSupply += supply.MaxSupply;
				}
			}
			float unmet = Math.Max(0f, demand - totalSupply);
			float totalBatterySupply = 0f;
			float totalMaxBatterySupply = 0f;
			if (unmet > 0f)
			{
				foreach (PowerState.NodeId batteryId2 in network.BatterySupplies)
				{
					PowerState.Battery battery2 = *state.Batteries[batteryId2];
					if (battery2.Enabled && battery2.CanDischarge && !battery2.Paused)
					{
						float scaledSpace2 = battery2.CurrentStorage / frameTime;
						float supplyAndPassthrough = Math.Min(battery2.MaxSupply, battery2.SupplyRampPosition + battery2.SupplyRampTolerance) + battery2.CurrentReceiving * battery2.Efficiency;
						battery2.AvailableSupply = Math.Min(scaledSpace2, supplyAndPassthrough);
						battery2.LoadingNetworkDemand = unmet;
						battery2.MaxEffectiveSupply = Math.Min(battery2.CurrentStorage / frameTime, battery2.MaxSupply + battery2.CurrentReceiving * battery2.Efficiency);
						totalBatterySupply += battery2.AvailableSupply;
						totalMaxBatterySupply += battery2.MaxEffectiveSupply;
					}
				}
			}
			network.LastCombinedSupply = totalSupply + totalBatterySupply;
			network.LastCombinedMaxSupply = totalMaxSupply + totalMaxBatterySupply;
			float met = Math.Min(demand, network.LastCombinedSupply);
			if (met == 0f)
			{
				return;
			}
			float supplyRatio = met / demand;
			foreach (PowerState.NodeId loadId2 in network.Loads)
			{
				PowerState.Load load2 = *state.Loads[loadId2];
				if (load2.Enabled && load2.DesiredPower != 0f && !load2.Paused)
				{
					load2.ReceivingPower = load2.DesiredPower * supplyRatio;
				}
			}
			foreach (PowerState.NodeId batteryId3 in network.BatteryLoads)
			{
				PowerState.Battery battery3 = *state.Batteries[batteryId3];
				if (battery3.Enabled && battery3.DesiredPower != 0f && !battery3.Paused)
				{
					battery3.LoadingMarked = true;
					battery3.CurrentReceiving = battery3.DesiredPower * supplyRatio;
					battery3.CurrentStorage += frameTime * battery3.CurrentReceiving * battery3.Efficiency;
					battery3.CurrentStorage = MathF.Min(battery3.CurrentStorage, battery3.Capacity);
				}
			}
			float metSupply = Math.Min(demand, totalSupply);
			if (metSupply > 0f)
			{
				float relativeSupplyOutput = metSupply / totalSupply;
				float targetRelativeSupplyOutput = Math.Min(demand, totalMaxSupply) / totalMaxSupply;
				foreach (PowerState.NodeId supplyId2 in network.Supplies)
				{
					PowerState.Supply supply2 = *state.Supplies[supplyId2];
					if (supply2.Enabled && !supply2.Paused)
					{
						supply2.CurrentSupply = supply2.AvailableSupply * relativeSupplyOutput;
						supply2.SupplyRampTarget = supply2.MaxSupply * targetRelativeSupplyOutput;
					}
				}
			}
			if (unmet <= 0f || totalBatterySupply <= 0f)
			{
				return;
			}
			float relativeBatteryOutput = Math.Min(unmet, totalBatterySupply) / totalBatterySupply;
			float relativeTargetBatteryOutput = Math.Min(unmet, totalMaxBatterySupply) / totalMaxBatterySupply;
			foreach (PowerState.NodeId batteryId4 in network.BatterySupplies)
			{
				PowerState.Battery battery4 = *state.Batteries[batteryId4];
				if (battery4.Enabled && !battery4.Paused)
				{
					battery4.SupplyingMarked = true;
					battery4.CurrentSupply = battery4.AvailableSupply * relativeBatteryOutput;
					battery4.CurrentStorage -= frameTime * battery4.CurrentSupply;
					battery4.CurrentStorage = MathF.Max(0f, battery4.CurrentStorage);
					battery4.SupplyRampTarget = battery4.MaxEffectiveSupply * relativeTargetBatteryOutput - battery4.CurrentReceiving * battery4.Efficiency;
				}
			}
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x00042C50 File Offset: 0x00040E50
		private void ClearBatteries(PowerState state)
		{
			foreach (PowerState.Battery battery in state.Batteries.Values)
			{
				if (!battery.Paused)
				{
					if (!battery.SupplyingMarked)
					{
						battery.CurrentSupply = 0f;
						battery.SupplyRampTarget = 0f;
						battery.LoadingNetworkDemand = 0f;
					}
					if (!battery.LoadingMarked)
					{
						battery.CurrentReceiving = 0f;
					}
					battery.SupplyingMarked = false;
					battery.LoadingMarked = false;
				}
			}
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x00042CF8 File Offset: 0x00040EF8
		private List<List<PowerState.Network>> GroupByNetworkDepth(PowerState state)
		{
			List<List<PowerState.Network>> groupedNetworks = new List<List<PowerState.Network>>();
			foreach (PowerState.Network network2 in state.Networks.Values)
			{
				network2.Height = -1;
			}
			foreach (PowerState.Network network in state.Networks.Values)
			{
				if (network.Height == -1)
				{
					BatteryRampPegSolver.RecursivelyEstimateNetworkDepth(state, network, groupedNetworks);
				}
			}
			return groupedNetworks;
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x00042DAC File Offset: 0x00040FAC
		private unsafe static void RecursivelyEstimateNetworkDepth(PowerState state, PowerState.Network network, List<List<PowerState.Network>> groupedNetworks)
		{
			network.Height = -2;
			int height = -1;
			foreach (PowerState.NodeId batteryId in network.BatteryLoads)
			{
				PowerState.Battery battery = *state.Batteries[batteryId];
				if (!(battery.LinkedNetworkDischarging == default(PowerState.NodeId)) && !(battery.LinkedNetworkDischarging == network.Id))
				{
					PowerState.Network subNet = *state.Networks[battery.LinkedNetworkDischarging];
					if (subNet.Height == -1)
					{
						BatteryRampPegSolver.RecursivelyEstimateNetworkDepth(state, subNet, groupedNetworks);
					}
					else if (subNet.Height == -2)
					{
						continue;
					}
					height = Math.Max(subNet.Height, height);
				}
			}
			network.Height = 1 + height;
			if (network.Height >= groupedNetworks.Count)
			{
				groupedNetworks.Add(new List<PowerState.Network>
				{
					network
				});
				return;
			}
			groupedNetworks[network.Height].Add(network);
		}

		// Token: 0x0200092F RID: 2351
		[Nullable(new byte[]
		{
			0,
			1
		})]
		private sealed class HeightComparer : Comparer<PowerState.Network>
		{
			// Token: 0x17000800 RID: 2048
			// (get) Token: 0x06003173 RID: 12659 RVA: 0x000FED45 File Offset: 0x000FCF45
			public static BatteryRampPegSolver.HeightComparer Instance { get; } = new BatteryRampPegSolver.HeightComparer();

			// Token: 0x06003174 RID: 12660 RVA: 0x000FED4C File Offset: 0x000FCF4C
			[NullableContext(2)]
			public override int Compare(PowerState.Network x, PowerState.Network y)
			{
				if (x.Height == y.Height)
				{
					return 0;
				}
				if (x.Height > y.Height)
				{
					return 1;
				}
				return -1;
			}
		}
	}
}
