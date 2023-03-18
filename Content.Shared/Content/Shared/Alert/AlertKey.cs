using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert
{
	// Token: 0x0200071B RID: 1819
	[NetSerializable]
	[Serializable]
	public struct AlertKey
	{
		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x060015F7 RID: 5623 RVA: 0x00047D57 File Offset: 0x00045F57
		// (set) Token: 0x060015F8 RID: 5624 RVA: 0x00047D5F File Offset: 0x00045F5F
		public AlertType? AlertType { readonly get; private set; }

		// Token: 0x060015F9 RID: 5625 RVA: 0x00047D68 File Offset: 0x00045F68
		public AlertKey(AlertType? alertType, AlertCategory? alertCategory)
		{
			this.AlertType = new AlertType?(Content.Shared.Alert.AlertType.Error);
			this.AlertCategory = alertCategory;
			this.AlertType = alertType;
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x00047D84 File Offset: 0x00045F84
		public bool Equals(AlertKey other)
		{
			if (this.AlertCategory != null)
			{
				AlertCategory? alertCategory = other.AlertCategory;
				AlertCategory? alertCategory2 = this.AlertCategory;
				return alertCategory.GetValueOrDefault() == alertCategory2.GetValueOrDefault() & alertCategory != null == (alertCategory2 != null);
			}
			AlertType? alertType = this.AlertType;
			AlertType? alertType2 = other.AlertType;
			if (alertType.GetValueOrDefault() == alertType2.GetValueOrDefault() & alertType != null == (alertType2 != null))
			{
				AlertCategory? alertCategory2 = this.AlertCategory;
				AlertCategory? alertCategory = other.AlertCategory;
				return alertCategory2.GetValueOrDefault() == alertCategory.GetValueOrDefault() & alertCategory2 != null == (alertCategory != null);
			}
			return false;
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x00047E34 File Offset: 0x00046034
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			if (obj is AlertKey)
			{
				AlertKey other = (AlertKey)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x00047E5C File Offset: 0x0004605C
		public override int GetHashCode()
		{
			if (this.AlertCategory != null)
			{
				return this.AlertCategory.GetHashCode();
			}
			return this.AlertType.GetHashCode();
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x00047EA0 File Offset: 0x000460A0
		public static AlertKey ForCategory(AlertCategory category)
		{
			return new AlertKey(null, new AlertCategory?(category));
		}

		// Token: 0x04001643 RID: 5699
		public readonly AlertCategory? AlertCategory;
	}
}
