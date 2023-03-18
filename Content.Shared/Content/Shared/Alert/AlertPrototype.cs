using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Alert
{
	// Token: 0x0200071D RID: 1821
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("alert", 1)]
	public sealed class AlertPrototype : IPrototype, ISerializationHooks
	{
		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06001604 RID: 5636 RVA: 0x0004815C File Offset: 0x0004635C
		[ViewVariables]
		string IPrototype.ID
		{
			get
			{
				return this.AlertType.ToString();
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06001605 RID: 5637 RVA: 0x0004817D File Offset: 0x0004637D
		// (set) Token: 0x06001606 RID: 5638 RVA: 0x00048185 File Offset: 0x00046385
		[IdDataField(1, null)]
		public AlertType AlertType { get; private set; }

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06001607 RID: 5639 RVA: 0x0004818E File Offset: 0x0004638E
		// (set) Token: 0x06001608 RID: 5640 RVA: 0x00048196 File Offset: 0x00046396
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = "";

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06001609 RID: 5641 RVA: 0x0004819F File Offset: 0x0004639F
		// (set) Token: 0x0600160A RID: 5642 RVA: 0x000481A7 File Offset: 0x000463A7
		[DataField("description", false, 1, false, false, null)]
		public string Description { get; private set; } = "";

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x0600160B RID: 5643 RVA: 0x000481B0 File Offset: 0x000463B0
		// (set) Token: 0x0600160C RID: 5644 RVA: 0x000481B8 File Offset: 0x000463B8
		[DataField("category", false, 1, false, false, null)]
		public AlertCategory? Category { get; private set; }

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x0600160D RID: 5645 RVA: 0x000481C1 File Offset: 0x000463C1
		// (set) Token: 0x0600160E RID: 5646 RVA: 0x000481C9 File Offset: 0x000463C9
		public AlertKey AlertKey { get; private set; }

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x0600160F RID: 5647 RVA: 0x000481D2 File Offset: 0x000463D2
		public short MinSeverity
		{
			get
			{
				if (this.MaxSeverity != -1)
				{
					return this._minSeverity;
				}
				return -1;
			}
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06001610 RID: 5648 RVA: 0x000481E5 File Offset: 0x000463E5
		public bool SupportsSeverity
		{
			get
			{
				return this.MaxSeverity != -1;
			}
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06001611 RID: 5649 RVA: 0x000481F3 File Offset: 0x000463F3
		// (set) Token: 0x06001612 RID: 5650 RVA: 0x000481FB File Offset: 0x000463FB
		[Nullable(2)]
		[DataField("onClick", false, 1, false, true, null)]
		public IAlertClick OnClick { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x06001613 RID: 5651 RVA: 0x00048204 File Offset: 0x00046404
		void ISerializationHooks.AfterDeserialization()
		{
			if (this.AlertType == AlertType.Error)
			{
				Logger.ErrorS("alert", "missing or invalid alertType for alert with name {0}", new object[]
				{
					this.Name
				});
			}
			this.AlertKey = new AlertKey(new AlertType?(this.AlertType), this.Category);
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x00048254 File Offset: 0x00046454
		public SpriteSpecifier GetIcon(short? severity = null)
		{
			if (!this.SupportsSeverity && severity != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
				defaultInterpolatedStringHandler.AppendLiteral("This alert (");
				defaultInterpolatedStringHandler.AppendFormatted<AlertKey>(this.AlertKey);
				defaultInterpolatedStringHandler.AppendLiteral(") does not support severity");
				throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			int minIcons = (int)(this.SupportsSeverity ? (this.MaxSeverity - this.MinSeverity) : 1);
			if (this.Icons.Count < minIcons)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Insufficient number of icons given for alert ");
				defaultInterpolatedStringHandler.AppendFormatted<AlertType>(this.AlertType);
				throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (!this.SupportsSeverity)
			{
				return this.Icons[0];
			}
			if (severity == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No severity specified but this alert (");
				defaultInterpolatedStringHandler.AppendFormatted<AlertKey>(this.AlertKey);
				defaultInterpolatedStringHandler.AppendLiteral(") has severity.");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "severity");
			}
			short? num = severity;
			int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			int num3 = (int)this.MinSeverity;
			if (num2.GetValueOrDefault() < num3 & num2 != null)
			{
				string paramName = "severity";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Severity below minimum severity in ");
				defaultInterpolatedStringHandler.AppendFormatted<AlertKey>(this.AlertKey);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				throw new ArgumentOutOfRangeException(paramName, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			num = severity;
			num2 = ((num != null) ? new int?((int)num.GetValueOrDefault()) : null);
			num3 = (int)this.MaxSeverity;
			if (num2.GetValueOrDefault() > num3 & num2 != null)
			{
				string paramName2 = "severity";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Severity above maximum severity in ");
				defaultInterpolatedStringHandler.AppendFormatted<AlertKey>(this.AlertKey);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				throw new ArgumentOutOfRangeException(paramName2, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return this.Icons[(int)(severity.Value - this._minSeverity)];
		}

		// Token: 0x04001648 RID: 5704
		[DataField("icons", false, 1, true, false, null)]
		public readonly List<SpriteSpecifier> Icons = new List<SpriteSpecifier>();

		// Token: 0x0400164D RID: 5709
		[DataField("minSeverity", false, 1, false, false, null)]
		private short _minSeverity = 1;

		// Token: 0x0400164E RID: 5710
		[DataField("maxSeverity", false, 1, false, false, null)]
		public short MaxSeverity = -1;
	}
}
