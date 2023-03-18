using System;
using System.Runtime.CompilerServices;
using Content.Server.Holiday.Interfaces;

namespace Content.Server.Holiday.ShouldCelebrate
{
	// Token: 0x02000468 RID: 1128
	public sealed class FridayThirteenth : IHolidayShouldCelebrate
	{
		// Token: 0x060016B8 RID: 5816 RVA: 0x00077B48 File Offset: 0x00075D48
		[NullableContext(1)]
		public bool ShouldCelebrate(DateTime date, HolidayPrototype holiday)
		{
			return date.Day == 13 && date.DayOfWeek == DayOfWeek.Friday;
		}
	}
}
