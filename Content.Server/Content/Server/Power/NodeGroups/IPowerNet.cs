using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;

namespace Content.Server.Power.NodeGroups
{
	// Token: 0x02000285 RID: 645
	[NullableContext(1)]
	public interface IPowerNet : IBasePowerNet
	{
		// Token: 0x06000CE0 RID: 3296
		void AddSupplier(PowerSupplierComponent supplier);

		// Token: 0x06000CE1 RID: 3297
		void RemoveSupplier(PowerSupplierComponent supplier);

		// Token: 0x06000CE2 RID: 3298
		void AddDischarger(BatteryDischargerComponent discharger);

		// Token: 0x06000CE3 RID: 3299
		void RemoveDischarger(BatteryDischargerComponent discharger);

		// Token: 0x06000CE4 RID: 3300
		void AddCharger(BatteryChargerComponent charger);

		// Token: 0x06000CE5 RID: 3301
		void RemoveCharger(BatteryChargerComponent charger);
	}
}
