using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Monitor
{
	// Token: 0x020006CA RID: 1738
	[Prototype("alarmThreshold", 1)]
	[NetSerializable]
	[Serializable]
	public sealed class AtmosAlarmThreshold : IPrototype, ISerializationHooks
	{
		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001515 RID: 5397 RVA: 0x0004543C File Offset: 0x0004363C
		[Nullable(1)]
		[IdDataField(1, null)]
		public string ID { [NullableContext(1)] get; }

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001516 RID: 5398 RVA: 0x00045444 File Offset: 0x00043644
		// (set) Token: 0x06001517 RID: 5399 RVA: 0x0004544C File Offset: 0x0004364C
		public AtmosAlarmThreshold.AlarmThresholdSetting UpperBound
		{
			get
			{
				return this._UpperBound;
			}
			private set
			{
				AtmosAlarmThreshold.AlarmThresholdSetting oldWarning = this.UpperWarningBound;
				this._UpperBound = value;
				this.UpperWarningBound = oldWarning;
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06001518 RID: 5400 RVA: 0x0004546E File Offset: 0x0004366E
		// (set) Token: 0x06001519 RID: 5401 RVA: 0x00045478 File Offset: 0x00043678
		public AtmosAlarmThreshold.AlarmThresholdSetting LowerBound
		{
			get
			{
				return this._LowerBound;
			}
			private set
			{
				AtmosAlarmThreshold.AlarmThresholdSetting oldWarning = this.LowerWarningBound;
				this._LowerBound = value;
				this.LowerWarningBound = oldWarning;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x0600151A RID: 5402 RVA: 0x0004549A File Offset: 0x0004369A
		// (set) Token: 0x0600151B RID: 5403 RVA: 0x000454A2 File Offset: 0x000436A2
		[DataField("upperWarnAround", false, 1, false, false, null)]
		public AtmosAlarmThreshold.AlarmThresholdSetting UpperWarningPercentage { get; private set; }

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x0600151C RID: 5404 RVA: 0x000454AB File Offset: 0x000436AB
		// (set) Token: 0x0600151D RID: 5405 RVA: 0x000454B3 File Offset: 0x000436B3
		[DataField("lowerWarnAround", false, 1, false, false, null)]
		public AtmosAlarmThreshold.AlarmThresholdSetting LowerWarningPercentage { get; private set; }

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x0600151E RID: 5406 RVA: 0x000454BC File Offset: 0x000436BC
		// (set) Token: 0x0600151F RID: 5407 RVA: 0x000454C5 File Offset: 0x000436C5
		[ViewVariables]
		public AtmosAlarmThreshold.AlarmThresholdSetting UpperWarningBound
		{
			get
			{
				return this.CalculateWarningBound(AtmosMonitorThresholdBound.Upper);
			}
			set
			{
				this.UpperWarningPercentage = this.CalculateWarningPercentage(AtmosMonitorThresholdBound.Upper, value);
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001520 RID: 5408 RVA: 0x000454D5 File Offset: 0x000436D5
		// (set) Token: 0x06001521 RID: 5409 RVA: 0x000454DE File Offset: 0x000436DE
		[ViewVariables]
		public AtmosAlarmThreshold.AlarmThresholdSetting LowerWarningBound
		{
			get
			{
				return this.CalculateWarningBound(AtmosMonitorThresholdBound.Lower);
			}
			set
			{
				this.LowerWarningPercentage = this.CalculateWarningPercentage(AtmosMonitorThresholdBound.Lower, value);
			}
		}

		// Token: 0x06001522 RID: 5410 RVA: 0x000454EE File Offset: 0x000436EE
		public AtmosAlarmThreshold()
		{
			this.UpperBound = new AtmosAlarmThreshold.AlarmThresholdSetting();
			this.LowerBound = new AtmosAlarmThreshold.AlarmThresholdSetting();
			this.UpperWarningPercentage = new AtmosAlarmThreshold.AlarmThresholdSetting();
			this.LowerWarningPercentage = new AtmosAlarmThreshold.AlarmThresholdSetting();
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x00045524 File Offset: 0x00043724
		[NullableContext(1)]
		public AtmosAlarmThreshold(AtmosAlarmThreshold other)
		{
			this.Ignore = other.Ignore;
			this.UpperBound = other.UpperBound;
			this.LowerBound = other.LowerBound;
			this.UpperWarningPercentage = other.UpperWarningPercentage;
			this.LowerWarningPercentage = other.LowerWarningPercentage;
		}

		// Token: 0x06001524 RID: 5412 RVA: 0x00045574 File Offset: 0x00043774
		void ISerializationHooks.AfterDeserialization()
		{
			AtmosAlarmThreshold.AlarmThresholdSetting alarmThresholdSetting = new AtmosAlarmThreshold.AlarmThresholdSetting
			{
				Enabled = (this.UpperBound.Value != 0f),
				Value = this.UpperBound.Value
			};
			this.UpperBound = alarmThresholdSetting;
			alarmThresholdSetting = new AtmosAlarmThreshold.AlarmThresholdSetting
			{
				Enabled = (this.LowerBound.Value != 0f),
				Value = this.LowerBound.Value
			};
			this.LowerBound = alarmThresholdSetting;
			alarmThresholdSetting = new AtmosAlarmThreshold.AlarmThresholdSetting
			{
				Enabled = (this.UpperWarningPercentage.Value != 0f),
				Value = this.UpperWarningPercentage.Value
			};
			this.UpperWarningPercentage = alarmThresholdSetting;
			alarmThresholdSetting = new AtmosAlarmThreshold.AlarmThresholdSetting
			{
				Enabled = (this.LowerWarningPercentage.Value != 0f),
				Value = this.LowerWarningPercentage.Value
			};
			this.LowerWarningPercentage = alarmThresholdSetting;
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x0004568C File Offset: 0x0004388C
		public bool CheckThreshold(float value, out AtmosAlarmType state)
		{
			AtmosMonitorThresholdBound atmosMonitorThresholdBound;
			return this.CheckThreshold(value, out state, out atmosMonitorThresholdBound);
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x000456A4 File Offset: 0x000438A4
		public bool CheckThreshold(float value, out AtmosAlarmType state, out AtmosMonitorThresholdBound whichFailed)
		{
			state = AtmosAlarmType.Normal;
			whichFailed = AtmosMonitorThresholdBound.Upper;
			if (this.Ignore)
			{
				return false;
			}
			if (value >= this.UpperBound)
			{
				state = AtmosAlarmType.Danger;
				whichFailed = AtmosMonitorThresholdBound.Upper;
				return true;
			}
			if (value <= this.LowerBound)
			{
				state = AtmosAlarmType.Danger;
				whichFailed = AtmosMonitorThresholdBound.Lower;
				return true;
			}
			if (value >= this.UpperWarningBound)
			{
				state = AtmosAlarmType.Warning;
				whichFailed = AtmosMonitorThresholdBound.Upper;
				return true;
			}
			if (value <= this.LowerWarningBound)
			{
				state = AtmosAlarmType.Warning;
				whichFailed = AtmosMonitorThresholdBound.Lower;
				return true;
			}
			return false;
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x0004571C File Offset: 0x0004391C
		public AtmosAlarmThreshold.AlarmThresholdSetting CalculateWarningBound(AtmosMonitorThresholdBound bound)
		{
			AtmosAlarmThreshold.AlarmThresholdSetting result;
			if (bound == AtmosMonitorThresholdBound.Upper)
			{
				result = new AtmosAlarmThreshold.AlarmThresholdSetting
				{
					Enabled = this.UpperWarningPercentage.Enabled,
					Value = this.UpperBound.Value * this.UpperWarningPercentage.Value
				};
				return result;
			}
			if (bound != AtmosMonitorThresholdBound.Lower)
			{
				return new AtmosAlarmThreshold.AlarmThresholdSetting();
			}
			result = new AtmosAlarmThreshold.AlarmThresholdSetting
			{
				Enabled = this.LowerWarningPercentage.Enabled,
				Value = this.LowerBound.Value * this.LowerWarningPercentage.Value
			};
			return result;
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x000457C0 File Offset: 0x000439C0
		public AtmosAlarmThreshold.AlarmThresholdSetting CalculateWarningPercentage(AtmosMonitorThresholdBound bound, AtmosAlarmThreshold.AlarmThresholdSetting warningBound)
		{
			AtmosAlarmThreshold.AlarmThresholdSetting result;
			if (bound == AtmosMonitorThresholdBound.Upper)
			{
				result = new AtmosAlarmThreshold.AlarmThresholdSetting
				{
					Enabled = this.UpperWarningPercentage.Enabled,
					Value = ((this.UpperBound.Value == 0f) ? 0f : (warningBound.Value / this.UpperBound.Value))
				};
				return result;
			}
			if (bound != AtmosMonitorThresholdBound.Lower)
			{
				return new AtmosAlarmThreshold.AlarmThresholdSetting();
			}
			result = new AtmosAlarmThreshold.AlarmThresholdSetting
			{
				Enabled = this.LowerWarningPercentage.Enabled,
				Value = ((this.LowerBound.Value == 0f) ? 0f : (warningBound.Value / this.LowerBound.Value))
			};
			return result;
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x0004588C File Offset: 0x00043A8C
		public void SetEnabled(AtmosMonitorLimitType whichLimit, bool isEnabled)
		{
			switch (whichLimit)
			{
			case AtmosMonitorLimitType.LowerDanger:
				this.LowerBound = this.LowerBound.WithEnabled(isEnabled);
				return;
			case AtmosMonitorLimitType.LowerWarning:
				this.LowerWarningPercentage = this.LowerWarningPercentage.WithEnabled(isEnabled);
				return;
			case AtmosMonitorLimitType.UpperWarning:
				this.UpperWarningPercentage = this.UpperWarningPercentage.WithEnabled(isEnabled);
				return;
			case AtmosMonitorLimitType.UpperDanger:
				this.UpperBound = this.UpperBound.WithEnabled(isEnabled);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x00045908 File Offset: 0x00043B08
		public void SetLimit(AtmosMonitorLimitType whichLimit, float limit)
		{
			if (limit <= 0f)
			{
				return;
			}
			switch (whichLimit)
			{
			case AtmosMonitorLimitType.LowerDanger:
				this.LowerBound = this.LowerBound.WithThreshold(limit);
				this.LowerWarningBound = this.LowerWarningBound.WithThreshold(Math.Max(limit, this.LowerWarningBound.Value));
				this.UpperWarningBound = this.UpperWarningBound.WithThreshold(Math.Max(limit, this.UpperWarningBound.Value));
				this.UpperBound = this.UpperBound.WithThreshold(Math.Max(limit, this.UpperBound.Value));
				return;
			case AtmosMonitorLimitType.LowerWarning:
				this.LowerBound = this.LowerBound.WithThreshold(Math.Min(this.LowerBound.Value, limit));
				this.LowerWarningBound = this.LowerWarningBound.WithThreshold(limit);
				this.UpperWarningBound = this.UpperWarningBound.WithThreshold(Math.Max(limit, this.UpperWarningBound.Value));
				this.UpperBound = this.UpperBound.WithThreshold(Math.Max(limit, this.UpperBound.Value));
				return;
			case AtmosMonitorLimitType.UpperWarning:
				this.LowerBound = this.LowerBound.WithThreshold(Math.Min(this.LowerBound.Value, limit));
				this.LowerWarningBound = this.LowerWarningBound.WithThreshold(Math.Min(this.LowerWarningBound.Value, limit));
				this.UpperWarningBound = this.UpperWarningBound.WithThreshold(limit);
				this.UpperBound = this.UpperBound.WithThreshold(Math.Max(limit, this.UpperBound.Value));
				return;
			case AtmosMonitorLimitType.UpperDanger:
				this.LowerBound = this.LowerBound.WithThreshold(Math.Min(this.LowerBound.Value, limit));
				this.LowerWarningBound = this.LowerWarningBound.WithThreshold(Math.Min(this.LowerWarningBound.Value, limit));
				this.UpperWarningBound = this.UpperWarningBound.WithThreshold(Math.Min(this.UpperWarningBound.Value, limit));
				this.UpperBound = this.UpperBound.WithThreshold(limit);
				return;
			default:
				return;
			}
		}

		// Token: 0x0400152C RID: 5420
		[DataField("ignore", false, 1, false, false, null)]
		public bool Ignore;

		// Token: 0x0400152D RID: 5421
		[DataField("upperBound", false, 1, false, false, null)]
		private AtmosAlarmThreshold.AlarmThresholdSetting _UpperBound;

		// Token: 0x0400152E RID: 5422
		[DataField("lowerBound", false, 1, false, false, null)]
		public AtmosAlarmThreshold.AlarmThresholdSetting _LowerBound;

		// Token: 0x02000864 RID: 2148
		[DataDefinition]
		[Serializable]
		public struct AlarmThresholdSetting
		{
			// Token: 0x17000545 RID: 1349
			// (get) Token: 0x060019C7 RID: 6599 RVA: 0x00051697 File Offset: 0x0004F897
			// (set) Token: 0x060019C8 RID: 6600 RVA: 0x0005169F File Offset: 0x0004F89F
			[DataField("enabled", false, 1, false, false, null)]
			public bool Enabled { readonly get; set; }

			// Token: 0x17000546 RID: 1350
			// (get) Token: 0x060019C9 RID: 6601 RVA: 0x000516A8 File Offset: 0x0004F8A8
			// (set) Token: 0x060019CA RID: 6602 RVA: 0x000516B0 File Offset: 0x0004F8B0
			[DataField("threshold", false, 1, false, false, null)]
			public float Value { readonly get; set; }

			// Token: 0x060019CB RID: 6603 RVA: 0x000516B9 File Offset: 0x0004F8B9
			public AlarmThresholdSetting()
			{
				this.Enabled = false;
				this.Value = 0f;
			}

			// Token: 0x060019CC RID: 6604 RVA: 0x000516CD File Offset: 0x0004F8CD
			public static bool operator <=(float a, AtmosAlarmThreshold.AlarmThresholdSetting b)
			{
				return b.Enabled && a <= b.Value;
			}

			// Token: 0x060019CD RID: 6605 RVA: 0x000516E7 File Offset: 0x0004F8E7
			public static bool operator >=(float a, AtmosAlarmThreshold.AlarmThresholdSetting b)
			{
				return b.Enabled && a >= b.Value;
			}

			// Token: 0x060019CE RID: 6606 RVA: 0x00051704 File Offset: 0x0004F904
			public AtmosAlarmThreshold.AlarmThresholdSetting WithThreshold(float threshold)
			{
				return new AtmosAlarmThreshold.AlarmThresholdSetting
				{
					Enabled = this.Enabled,
					Value = threshold
				};
			}

			// Token: 0x060019CF RID: 6607 RVA: 0x00051730 File Offset: 0x0004F930
			public AtmosAlarmThreshold.AlarmThresholdSetting WithEnabled(bool enabled)
			{
				return new AtmosAlarmThreshold.AlarmThresholdSetting
				{
					Enabled = enabled,
					Value = this.Value
				};
			}
		}
	}
}
