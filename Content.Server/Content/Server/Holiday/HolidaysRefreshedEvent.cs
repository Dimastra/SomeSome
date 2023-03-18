using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Holiday
{
	// Token: 0x02000462 RID: 1122
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HolidaysRefreshedEvent : EntityEventArgs
	{
		// Token: 0x060016AE RID: 5806 RVA: 0x00077731 File Offset: 0x00075931
		public HolidaysRefreshedEvent(IEnumerable<HolidayPrototype> holidays)
		{
			this.Holidays = holidays;
		}

		// Token: 0x04000E28 RID: 3624
		public readonly IEnumerable<HolidayPrototype> Holidays;
	}
}
