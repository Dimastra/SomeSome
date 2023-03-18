using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
	// Token: 0x020006A9 RID: 1705
	[NetSerializable]
	[Serializable]
	public enum ThermoMachineMode : byte
	{
		// Token: 0x040014CF RID: 5327
		Freezer,
		// Token: 0x040014D0 RID: 5328
		Heater
	}
}
