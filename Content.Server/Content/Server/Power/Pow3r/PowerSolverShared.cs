using System;
using System.Runtime.CompilerServices;

namespace Content.Server.Power.Pow3r
{
	// Token: 0x0200027B RID: 635
	public static class PowerSolverShared
	{
		// Token: 0x06000CBA RID: 3258 RVA: 0x00042ED4 File Offset: 0x000410D4
		[NullableContext(1)]
		public static void UpdateRampPositions(float frameTime, PowerState state)
		{
			foreach (PowerState.Supply supply in state.Supplies.Values)
			{
				if (!supply.Paused)
				{
					if (!supply.Enabled)
					{
						supply.SupplyRampPosition = 0f;
					}
					else
					{
						float rampDev = supply.SupplyRampTarget - supply.SupplyRampPosition;
						if (Math.Abs(rampDev) > 0.001f)
						{
							float newPos;
							if (rampDev > 0f)
							{
								newPos = Math.Min(supply.SupplyRampTarget, supply.SupplyRampPosition + supply.SupplyRampRate * frameTime);
							}
							else
							{
								newPos = Math.Max(supply.SupplyRampTarget, supply.SupplyRampPosition - supply.SupplyRampRate * frameTime);
							}
							supply.SupplyRampPosition = Math.Clamp(newPos, 0f, supply.MaxSupply);
						}
						else
						{
							supply.SupplyRampPosition = supply.SupplyRampTarget;
						}
					}
				}
			}
			foreach (PowerState.Battery battery in state.Batteries.Values)
			{
				if (!battery.Paused)
				{
					if (!battery.Enabled)
					{
						battery.SupplyRampPosition = 0f;
					}
					else
					{
						float rampDev2 = battery.SupplyRampTarget - battery.SupplyRampPosition;
						if (Math.Abs(rampDev2) > 0.001f)
						{
							float newPos2;
							if (rampDev2 > 0f)
							{
								newPos2 = Math.Min(battery.SupplyRampTarget, battery.SupplyRampPosition + battery.SupplyRampRate * frameTime);
							}
							else
							{
								newPos2 = Math.Max(battery.SupplyRampTarget, battery.SupplyRampPosition - battery.SupplyRampRate * frameTime);
							}
							battery.SupplyRampPosition = Math.Clamp(newPos2, 0f, battery.MaxSupply);
						}
						else
						{
							battery.SupplyRampPosition = battery.SupplyRampTarget;
						}
					}
				}
			}
		}
	}
}
