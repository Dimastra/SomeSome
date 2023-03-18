using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Content.Server.Holiday.Interfaces;

namespace Content.Server.Holiday.ShouldCelebrate
{
	// Token: 0x02000464 RID: 1124
	public sealed class ChineseNewYear : IHolidayShouldCelebrate
	{
		// Token: 0x060016AF RID: 5807 RVA: 0x00077740 File Offset: 0x00075940
		[NullableContext(1)]
		public bool ShouldCelebrate(DateTime date, HolidayPrototype holiday)
		{
			DateTime chineseNewYear = new ChineseLunisolarCalendar().ToDateTime(date.Year, 1, 1, 0, 0, 0, 0);
			return date.Day == chineseNewYear.Day && date.Month == chineseNewYear.Month;
		}
	}
}
