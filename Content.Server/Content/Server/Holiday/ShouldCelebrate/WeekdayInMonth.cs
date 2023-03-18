using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Holiday.ShouldCelebrate
{
	// Token: 0x02000469 RID: 1129
	public sealed class WeekdayInMonth : DefaultHolidayShouldCelebrate
	{
		// Token: 0x060016BA RID: 5818 RVA: 0x00077B6C File Offset: 0x00075D6C
		[NullableContext(1)]
		public override bool ShouldCelebrate(DateTime date, HolidayPrototype holiday)
		{
			if (date.Month != (int)holiday.BeginMonth)
			{
				return false;
			}
			this._occurrence = Math.Max(1U, Math.Min(this._occurrence, 4U));
			GregorianCalendar calendar = new GregorianCalendar();
			DateTime d = new DateTime(date.Year, date.Month, 1, calendar);
			for (int i = 1; i <= 7; i++)
			{
				if (d.DayOfWeek == this._weekday)
				{
					d = d.AddDays(7U * (this._occurrence - 1U));
					return date.Day == d.Day;
				}
				d = d.AddDays(1.0);
			}
			return false;
		}

		// Token: 0x04000E3A RID: 3642
		[DataField("weekday", false, 1, false, false, null)]
		private DayOfWeek _weekday = DayOfWeek.Monday;

		// Token: 0x04000E3B RID: 3643
		[DataField("occurrence", false, 1, false, false, null)]
		private uint _occurrence = 1U;
	}
}
