using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
	// Token: 0x020006B0 RID: 1712
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GasVentScrubberData : IAtmosDeviceData
	{
		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x060014D0 RID: 5328 RVA: 0x00044F59 File Offset: 0x00043159
		// (set) Token: 0x060014D1 RID: 5329 RVA: 0x00044F61 File Offset: 0x00043161
		public bool Enabled { get; set; }

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x00044F6A File Offset: 0x0004316A
		// (set) Token: 0x060014D3 RID: 5331 RVA: 0x00044F72 File Offset: 0x00043172
		public bool Dirty { get; set; }

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x060014D4 RID: 5332 RVA: 0x00044F7B File Offset: 0x0004317B
		// (set) Token: 0x060014D5 RID: 5333 RVA: 0x00044F83 File Offset: 0x00043183
		public bool IgnoreAlarms { get; set; }

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x060014D6 RID: 5334 RVA: 0x00044F8C File Offset: 0x0004318C
		// (set) Token: 0x060014D7 RID: 5335 RVA: 0x00044F94 File Offset: 0x00043194
		public HashSet<Gas> FilterGases { get; set; } = new HashSet<Gas>(GasVentScrubberData.DefaultFilterGases);

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x060014D8 RID: 5336 RVA: 0x00044F9D File Offset: 0x0004319D
		// (set) Token: 0x060014D9 RID: 5337 RVA: 0x00044FA5 File Offset: 0x000431A5
		public ScrubberPumpDirection PumpDirection { get; set; } = ScrubberPumpDirection.Scrubbing;

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x060014DA RID: 5338 RVA: 0x00044FAE File Offset: 0x000431AE
		// (set) Token: 0x060014DB RID: 5339 RVA: 0x00044FB6 File Offset: 0x000431B6
		public float VolumeRate { get; set; } = 200f;

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x060014DC RID: 5340 RVA: 0x00044FBF File Offset: 0x000431BF
		// (set) Token: 0x060014DD RID: 5341 RVA: 0x00044FC7 File Offset: 0x000431C7
		public bool WideNet { get; set; }

		// Token: 0x040014F1 RID: 5361
		public static HashSet<Gas> DefaultFilterGases = new HashSet<Gas>
		{
			Gas.CarbonDioxide,
			Gas.Plasma,
			Gas.Tritium,
			Gas.WaterVapor,
			Gas.Miasma,
			Gas.NitrousOxide,
			Gas.Frezon
		};

		// Token: 0x040014F2 RID: 5362
		public static GasVentScrubberData FilterModePreset = new GasVentScrubberData
		{
			Enabled = true,
			FilterGases = new HashSet<Gas>(GasVentScrubberData.DefaultFilterGases),
			PumpDirection = ScrubberPumpDirection.Scrubbing,
			VolumeRate = 200f,
			WideNet = false
		};

		// Token: 0x040014F3 RID: 5363
		public static GasVentScrubberData WideFilterModePreset = new GasVentScrubberData
		{
			Enabled = true,
			FilterGases = new HashSet<Gas>(GasVentScrubberData.DefaultFilterGases),
			PumpDirection = ScrubberPumpDirection.Scrubbing,
			VolumeRate = 200f,
			WideNet = true
		};

		// Token: 0x040014F4 RID: 5364
		public static GasVentScrubberData FillModePreset = new GasVentScrubberData
		{
			Enabled = false,
			Dirty = true,
			FilterGases = new HashSet<Gas>(GasVentScrubberData.DefaultFilterGases),
			PumpDirection = ScrubberPumpDirection.Scrubbing,
			VolumeRate = 200f,
			WideNet = false
		};

		// Token: 0x040014F5 RID: 5365
		public static GasVentScrubberData PanicModePreset = new GasVentScrubberData
		{
			Enabled = true,
			Dirty = true,
			FilterGases = new HashSet<Gas>(GasVentScrubberData.DefaultFilterGases),
			PumpDirection = ScrubberPumpDirection.Siphoning,
			VolumeRate = 200f,
			WideNet = false
		};

		// Token: 0x040014F6 RID: 5366
		public static GasVentScrubberData ReplaceModePreset = new GasVentScrubberData
		{
			Enabled = true,
			IgnoreAlarms = true,
			Dirty = true,
			FilterGases = new HashSet<Gas>(GasVentScrubberData.DefaultFilterGases),
			PumpDirection = ScrubberPumpDirection.Siphoning,
			VolumeRate = 200f,
			WideNet = false
		};
	}
}
