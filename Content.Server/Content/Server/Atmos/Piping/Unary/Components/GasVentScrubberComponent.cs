using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Unary.Components
{
	// Token: 0x02000755 RID: 1877
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GasVentScrubberSystem)
	})]
	public sealed class GasVentScrubberComponent : Component
	{
		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06002792 RID: 10130 RVA: 0x000CFE4B File Offset: 0x000CE04B
		// (set) Token: 0x06002793 RID: 10131 RVA: 0x000CFE53 File Offset: 0x000CE053
		[ViewVariables]
		public bool Enabled { get; set; } = true;

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x06002794 RID: 10132 RVA: 0x000CFE5C File Offset: 0x000CE05C
		// (set) Token: 0x06002795 RID: 10133 RVA: 0x000CFE64 File Offset: 0x000CE064
		[ViewVariables]
		public bool IsDirty { get; set; }

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x06002796 RID: 10134 RVA: 0x000CFE6D File Offset: 0x000CE06D
		// (set) Token: 0x06002797 RID: 10135 RVA: 0x000CFE75 File Offset: 0x000CE075
		[ViewVariables]
		public bool Welded { get; set; }

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x06002798 RID: 10136 RVA: 0x000CFE7E File Offset: 0x000CE07E
		// (set) Token: 0x06002799 RID: 10137 RVA: 0x000CFE86 File Offset: 0x000CE086
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName { get; set; } = "pipe";

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x0600279A RID: 10138 RVA: 0x000CFE8F File Offset: 0x000CE08F
		// (set) Token: 0x0600279B RID: 10139 RVA: 0x000CFE97 File Offset: 0x000CE097
		[ViewVariables]
		public ScrubberPumpDirection PumpDirection { get; set; } = ScrubberPumpDirection.Scrubbing;

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x0600279C RID: 10140 RVA: 0x000CFEA0 File Offset: 0x000CE0A0
		// (set) Token: 0x0600279D RID: 10141 RVA: 0x000CFEA8 File Offset: 0x000CE0A8
		[ViewVariables]
		public float TransferRate
		{
			get
			{
				return this._transferRate;
			}
			set
			{
				this._transferRate = Math.Clamp(value, 0f, this.MaxTransferRate);
			}
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x0600279E RID: 10142 RVA: 0x000CFEC1 File Offset: 0x000CE0C1
		// (set) Token: 0x0600279F RID: 10143 RVA: 0x000CFEC9 File Offset: 0x000CE0C9
		[ViewVariables]
		public bool WideNet { get; set; }

		// Token: 0x060027A0 RID: 10144 RVA: 0x000CFED4 File Offset: 0x000CE0D4
		public GasVentScrubberData ToAirAlarmData()
		{
			return new GasVentScrubberData
			{
				Enabled = this.Enabled,
				Dirty = this.IsDirty,
				FilterGases = this.FilterGases,
				PumpDirection = this.PumpDirection,
				VolumeRate = this.TransferRate,
				WideNet = this.WideNet
			};
		}

		// Token: 0x060027A1 RID: 10145 RVA: 0x000CFF30 File Offset: 0x000CE130
		public void FromAirAlarmData(GasVentScrubberData data)
		{
			this.Enabled = data.Enabled;
			this.IsDirty = data.Dirty;
			this.PumpDirection = data.PumpDirection;
			this.TransferRate = data.VolumeRate;
			this.WideNet = data.WideNet;
			if (!data.FilterGases.SequenceEqual(this.FilterGases))
			{
				this.FilterGases.Clear();
				foreach (Gas gas in data.FilterGases)
				{
					this.FilterGases.Add(gas);
				}
			}
		}

		// Token: 0x040018AF RID: 6319
		[ViewVariables]
		public readonly HashSet<Gas> FilterGases = new HashSet<Gas>(GasVentScrubberData.DefaultFilterGases);

		// Token: 0x040018B1 RID: 6321
		private float _transferRate = 200f;

		// Token: 0x040018B2 RID: 6322
		[ViewVariables]
		[DataField("maxTransferRate", false, 1, false, false, null)]
		public float MaxTransferRate = 200f;

		// Token: 0x040018B3 RID: 6323
		[DataField("maxPressure", false, 1, false, false, null)]
		public float MaxPressure = 4500f;
	}
}
