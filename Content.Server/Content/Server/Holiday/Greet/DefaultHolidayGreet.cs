using System;
using System.Runtime.CompilerServices;
using Content.Server.Holiday.Interfaces;
using Robust.Shared.Localization;

namespace Content.Server.Holiday.Greet
{
	// Token: 0x0200046E RID: 1134
	public sealed class DefaultHolidayGreet : IHolidayGreet
	{
		// Token: 0x060016C1 RID: 5825 RVA: 0x00077C4C File Offset: 0x00075E4C
		[NullableContext(1)]
		public string Greet(HolidayPrototype holiday)
		{
			string holidayName = Loc.GetString(holiday.Name);
			return Loc.GetString("holiday-greet", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("holidayName", holidayName)
			});
		}
	}
}
