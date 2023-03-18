using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
	// Token: 0x020006AD RID: 1709
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GasVentPumpData : IAtmosDeviceData
	{
		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x060014C0 RID: 5312 RVA: 0x00044DC1 File Offset: 0x00042FC1
		// (set) Token: 0x060014C1 RID: 5313 RVA: 0x00044DC9 File Offset: 0x00042FC9
		public bool Enabled { get; set; }

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x060014C2 RID: 5314 RVA: 0x00044DD2 File Offset: 0x00042FD2
		// (set) Token: 0x060014C3 RID: 5315 RVA: 0x00044DDA File Offset: 0x00042FDA
		public bool Dirty { get; set; }

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x060014C4 RID: 5316 RVA: 0x00044DE3 File Offset: 0x00042FE3
		// (set) Token: 0x060014C5 RID: 5317 RVA: 0x00044DEB File Offset: 0x00042FEB
		public bool IgnoreAlarms { get; set; }

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x060014C6 RID: 5318 RVA: 0x00044DF4 File Offset: 0x00042FF4
		// (set) Token: 0x060014C7 RID: 5319 RVA: 0x00044DFC File Offset: 0x00042FFC
		public VentPumpDirection PumpDirection { get; set; } = VentPumpDirection.Releasing;

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x060014C8 RID: 5320 RVA: 0x00044E05 File Offset: 0x00043005
		// (set) Token: 0x060014C9 RID: 5321 RVA: 0x00044E0D File Offset: 0x0004300D
		public VentPressureBound PressureChecks { get; set; } = VentPressureBound.ExternalBound;

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x060014CA RID: 5322 RVA: 0x00044E16 File Offset: 0x00043016
		// (set) Token: 0x060014CB RID: 5323 RVA: 0x00044E1E File Offset: 0x0004301E
		public float ExternalPressureBound { get; set; } = 101.325f;

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x060014CC RID: 5324 RVA: 0x00044E27 File Offset: 0x00043027
		// (set) Token: 0x060014CD RID: 5325 RVA: 0x00044E2F File Offset: 0x0004302F
		public float InternalPressureBound { get; set; }

		// Token: 0x040014DE RID: 5342
		public static GasVentPumpData FilterModePreset = new GasVentPumpData
		{
			Enabled = true,
			PumpDirection = VentPumpDirection.Releasing,
			PressureChecks = VentPressureBound.ExternalBound,
			ExternalPressureBound = 101.325f,
			InternalPressureBound = 0f
		};

		// Token: 0x040014DF RID: 5343
		public static GasVentPumpData FillModePreset = new GasVentPumpData
		{
			Enabled = true,
			Dirty = true,
			PumpDirection = VentPumpDirection.Releasing,
			PressureChecks = VentPressureBound.ExternalBound,
			ExternalPressureBound = 5066.25f,
			InternalPressureBound = 0f
		};

		// Token: 0x040014E0 RID: 5344
		public static GasVentPumpData PanicModePreset = new GasVentPumpData
		{
			Enabled = false,
			Dirty = true,
			PumpDirection = VentPumpDirection.Releasing,
			PressureChecks = VentPressureBound.ExternalBound,
			ExternalPressureBound = 101.325f,
			InternalPressureBound = 0f
		};

		// Token: 0x040014E1 RID: 5345
		public static GasVentPumpData ReplaceModePreset = new GasVentPumpData
		{
			Enabled = false,
			IgnoreAlarms = true,
			Dirty = true,
			PumpDirection = VentPumpDirection.Releasing,
			PressureChecks = VentPressureBound.ExternalBound,
			ExternalPressureBound = 101.325f,
			InternalPressureBound = 0f
		};
	}
}
