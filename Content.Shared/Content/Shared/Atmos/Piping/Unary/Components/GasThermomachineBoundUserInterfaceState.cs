using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
	// Token: 0x020006AC RID: 1708
	[NetSerializable]
	[Serializable]
	public sealed class GasThermomachineBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x060014BA RID: 5306 RVA: 0x00044D6C File Offset: 0x00042F6C
		public float MinTemperature { get; }

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x060014BB RID: 5307 RVA: 0x00044D74 File Offset: 0x00042F74
		public float MaxTemperature { get; }

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x060014BC RID: 5308 RVA: 0x00044D7C File Offset: 0x00042F7C
		public float Temperature { get; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x060014BD RID: 5309 RVA: 0x00044D84 File Offset: 0x00042F84
		public bool Enabled { get; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x060014BE RID: 5310 RVA: 0x00044D8C File Offset: 0x00042F8C
		public ThermoMachineMode Mode { get; }

		// Token: 0x060014BF RID: 5311 RVA: 0x00044D94 File Offset: 0x00042F94
		public GasThermomachineBoundUserInterfaceState(float minTemperature, float maxTemperature, float temperature, bool enabled, ThermoMachineMode mode)
		{
			this.MinTemperature = minTemperature;
			this.MaxTemperature = maxTemperature;
			this.Temperature = temperature;
			this.Enabled = enabled;
			this.Mode = mode;
		}
	}
}
