using System;
using System.IO;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Holiday.ShouldCelebrate
{
	// Token: 0x02000465 RID: 1125
	[DataDefinition]
	public sealed class Computus : DefaultHolidayShouldCelebrate
	{
		// Token: 0x060016B1 RID: 5809 RVA: 0x00077790 File Offset: 0x00075990
		[return: TupleElementNames(new string[]
		{
			"day",
			"month"
		})]
		public ValueTuple<int, int> DoComputus(DateTime date)
		{
			int i;
			int num = i = date.Year;
			int j;
			int k;
			if (i >= 1900 && i <= 2099)
			{
				j = 24;
				k = 5;
			}
			else
			{
				int l = i;
				if (l >= 2100 && l <= 2199)
				{
					j = 24;
					k = 6;
				}
				else
				{
					int m = i;
					if (m < 2200 || m > 2299)
					{
						throw new InvalidDataException("Easter machine broke.");
					}
					j = 25;
					k = 0;
				}
			}
			int a = num % 19;
			int b = num % 4;
			int c = num % 7;
			int d = (19 * a + j) % 30;
			int e = (2 * b + 4 * c + 6 * d + k) % 7;
			ValueTuple<int, int> easterDate = new ValueTuple<int, int>(0, 0);
			if (d + e < 10)
			{
				easterDate.Item2 = 3;
				easterDate.Item1 = d + e + 22;
			}
			else if (d + e > 9)
			{
				easterDate.Item2 = 4;
				easterDate.Item1 = d + e - 9;
			}
			if (easterDate.Item2 == 4 && easterDate.Item1 == 26)
			{
				easterDate.Item1 = 19;
			}
			if (easterDate.Item2 == 4 && easterDate.Item1 == 25 && d == 28 && e == 6 && a > 10)
			{
				easterDate.Item1 = 18;
			}
			return easterDate;
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x000778D0 File Offset: 0x00075AD0
		[NullableContext(1)]
		public override bool ShouldCelebrate(DateTime date, HolidayPrototype holiday)
		{
			if (holiday.BeginMonth == Month.Invalid)
			{
				ValueTuple<int, int> valueTuple = this.DoComputus(date);
				int day = valueTuple.Item1;
				int month = valueTuple.Item2;
				holiday.BeginDay = (byte)day;
				holiday.BeginMonth = (Month)month;
				holiday.EndDay = holiday.BeginDay + this._daysExtra;
				holiday.EndMonth = holiday.BeginMonth;
				if (holiday.EndDay >= 32 && holiday.EndMonth == Month.March)
				{
					holiday.EndDay -= 31;
					Month month2 = holiday.EndMonth;
					holiday.EndMonth = month2 + 1;
				}
				if (holiday.EndDay >= 31 && holiday.EndMonth == Month.April)
				{
					holiday.EndDay -= 30;
					Month month2 = holiday.EndMonth;
					holiday.EndMonth = month2 + 1;
				}
				holiday.BeginDay -= this._daysEarly;
				if (holiday.BeginDay <= 0 && holiday.BeginMonth == Month.April)
				{
					holiday.BeginDay += 31;
					Month month2 = holiday.BeginMonth;
					holiday.BeginMonth = (Month)(month2 - Month.January);
				}
			}
			return base.ShouldCelebrate(date, holiday);
		}

		// Token: 0x04000E37 RID: 3639
		[DataField("daysEarly", false, 1, false, false, null)]
		private byte _daysEarly = 1;

		// Token: 0x04000E38 RID: 3640
		[DataField("daysExtra", false, 1, false, false, null)]
		private byte _daysExtra = 1;
	}
}
