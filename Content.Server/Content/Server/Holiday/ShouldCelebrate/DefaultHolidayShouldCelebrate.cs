using System;
using System.Runtime.CompilerServices;
using Content.Server.Holiday.Interfaces;
using Robust.Shared.Analyzers;

namespace Content.Server.Holiday.ShouldCelebrate
{
	// Token: 0x02000467 RID: 1127
	[Virtual]
	public class DefaultHolidayShouldCelebrate : IHolidayShouldCelebrate
	{
		// Token: 0x060016B6 RID: 5814 RVA: 0x00077A1C File Offset: 0x00075C1C
		[NullableContext(1)]
		public virtual bool ShouldCelebrate(DateTime date, HolidayPrototype holiday)
		{
			if (holiday.EndDay == 0)
			{
				holiday.EndDay = holiday.BeginDay;
			}
			if (holiday.EndMonth == Month.Invalid)
			{
				holiday.EndMonth = holiday.BeginMonth;
			}
			if (holiday.EndMonth > holiday.BeginMonth)
			{
				if (date.Month == (int)holiday.EndMonth && date.Day <= (int)holiday.EndDay)
				{
					return true;
				}
				if (date.Month == (int)holiday.BeginMonth && date.Day >= (int)holiday.BeginDay)
				{
					return true;
				}
				if (date.Month > (int)holiday.BeginMonth && date.Month < (int)holiday.EndMonth)
				{
					return true;
				}
			}
			else if (holiday.EndMonth == holiday.BeginMonth)
			{
				if (date.Month == (int)holiday.BeginMonth && date.Day >= (int)holiday.BeginDay && date.Day <= (int)holiday.EndDay)
				{
					return true;
				}
			}
			else
			{
				if (date.Month >= (int)holiday.BeginMonth && date.Day >= (int)holiday.BeginDay)
				{
					return true;
				}
				if (date.Month <= (int)holiday.EndMonth && date.Day <= (int)holiday.EndDay)
				{
					return true;
				}
			}
			return false;
		}
	}
}
