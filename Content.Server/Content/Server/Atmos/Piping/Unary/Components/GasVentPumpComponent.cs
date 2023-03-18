using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.MachineLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Unary.Components
{
	// Token: 0x02000754 RID: 1876
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasVentPumpComponent : Component
	{
		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x0600277B RID: 10107 RVA: 0x000CFC40 File Offset: 0x000CDE40
		// (set) Token: 0x0600277C RID: 10108 RVA: 0x000CFC48 File Offset: 0x000CDE48
		[ViewVariables]
		public bool Enabled { get; set; } = true;

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x0600277D RID: 10109 RVA: 0x000CFC51 File Offset: 0x000CDE51
		// (set) Token: 0x0600277E RID: 10110 RVA: 0x000CFC59 File Offset: 0x000CDE59
		[ViewVariables]
		public bool IsDirty { get; set; }

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x0600277F RID: 10111 RVA: 0x000CFC62 File Offset: 0x000CDE62
		// (set) Token: 0x06002780 RID: 10112 RVA: 0x000CFC6A File Offset: 0x000CDE6A
		[ViewVariables]
		public bool Welded { get; set; }

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06002781 RID: 10113 RVA: 0x000CFC73 File Offset: 0x000CDE73
		// (set) Token: 0x06002782 RID: 10114 RVA: 0x000CFC7B File Offset: 0x000CDE7B
		[ViewVariables]
		[DataField("inlet", false, 1, false, false, null)]
		public string Inlet { get; set; } = "pipe";

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06002783 RID: 10115 RVA: 0x000CFC84 File Offset: 0x000CDE84
		// (set) Token: 0x06002784 RID: 10116 RVA: 0x000CFC8C File Offset: 0x000CDE8C
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string Outlet { get; set; } = "pipe";

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06002785 RID: 10117 RVA: 0x000CFC95 File Offset: 0x000CDE95
		// (set) Token: 0x06002786 RID: 10118 RVA: 0x000CFC9D File Offset: 0x000CDE9D
		[ViewVariables]
		[DataField("pumpDirection", false, 1, false, false, null)]
		public VentPumpDirection PumpDirection { get; set; } = VentPumpDirection.Releasing;

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x06002787 RID: 10119 RVA: 0x000CFCA6 File Offset: 0x000CDEA6
		// (set) Token: 0x06002788 RID: 10120 RVA: 0x000CFCAE File Offset: 0x000CDEAE
		[ViewVariables]
		[DataField("pressureChecks", false, 1, false, false, null)]
		public VentPressureBound PressureChecks { get; set; } = VentPressureBound.ExternalBound;

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x06002789 RID: 10121 RVA: 0x000CFCB7 File Offset: 0x000CDEB7
		// (set) Token: 0x0600278A RID: 10122 RVA: 0x000CFCBF File Offset: 0x000CDEBF
		[ViewVariables]
		[DataField("underPressureLockout", false, 1, false, false, null)]
		public bool UnderPressureLockout { get; set; }

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x0600278B RID: 10123 RVA: 0x000CFCC8 File Offset: 0x000CDEC8
		// (set) Token: 0x0600278C RID: 10124 RVA: 0x000CFCD0 File Offset: 0x000CDED0
		[ViewVariables]
		[DataField("externalPressureBound", false, 1, false, false, null)]
		public float ExternalPressureBound
		{
			get
			{
				return this._externalPressureBound;
			}
			set
			{
				this._externalPressureBound = Math.Clamp(value, 0f, this.MaxPressure);
			}
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x0600278D RID: 10125 RVA: 0x000CFCE9 File Offset: 0x000CDEE9
		// (set) Token: 0x0600278E RID: 10126 RVA: 0x000CFCF1 File Offset: 0x000CDEF1
		[ViewVariables]
		[DataField("internalPressureBound", false, 1, false, false, null)]
		public float InternalPressureBound
		{
			get
			{
				return this._internalPressureBound;
			}
			set
			{
				this._internalPressureBound = Math.Clamp(value, 0f, this.MaxPressure);
			}
		}

		// Token: 0x0600278F RID: 10127 RVA: 0x000CFD0C File Offset: 0x000CDF0C
		public GasVentPumpData ToAirAlarmData()
		{
			return new GasVentPumpData
			{
				Enabled = this.Enabled,
				Dirty = this.IsDirty,
				PumpDirection = this.PumpDirection,
				PressureChecks = this.PressureChecks,
				ExternalPressureBound = this.ExternalPressureBound,
				InternalPressureBound = this.InternalPressureBound
			};
		}

		// Token: 0x06002790 RID: 10128 RVA: 0x000CFD68 File Offset: 0x000CDF68
		public void FromAirAlarmData(GasVentPumpData data)
		{
			this.Enabled = data.Enabled;
			this.IsDirty = data.Dirty;
			this.PumpDirection = data.PumpDirection;
			this.PressureChecks = data.PressureChecks;
			this.ExternalPressureBound = data.ExternalPressureBound;
			this.InternalPressureBound = data.InternalPressureBound;
		}

		// Token: 0x040018A1 RID: 6305
		[ViewVariables]
		[DataField("underPressureLockoutThreshold", false, 1, false, false, null)]
		public float UnderPressureLockoutThreshold = 1f;

		// Token: 0x040018A2 RID: 6306
		private float _externalPressureBound = 101.325f;

		// Token: 0x040018A3 RID: 6307
		private float _internalPressureBound;

		// Token: 0x040018A4 RID: 6308
		[ViewVariables]
		[DataField("maxPressure", false, 1, false, false, null)]
		public float MaxPressure = 4500f;

		// Token: 0x040018A5 RID: 6309
		[ViewVariables]
		[DataField("targetPressureChange", false, 1, false, false, null)]
		public float TargetPressureChange = 101.325f;

		// Token: 0x040018A6 RID: 6310
		[DataField("canLink", false, 1, false, false, null)]
		public readonly bool CanLink;

		// Token: 0x040018A7 RID: 6311
		[DataField("pressurizePort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string PressurizePort = "Pressurize";

		// Token: 0x040018A8 RID: 6312
		[DataField("depressurizePort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string DepressurizePort = "Depressurize";

		// Token: 0x040018A9 RID: 6313
		[ViewVariables]
		[DataField("pressurizePressure", false, 1, false, false, null)]
		public float PressurizePressure = 101.325f;

		// Token: 0x040018AA RID: 6314
		[ViewVariables]
		[DataField("depressurizePressure", false, 1, false, false, null)]
		public float DepressurizePressure;
	}
}
