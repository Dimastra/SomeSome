using System;
using System.Runtime.CompilerServices;
using Content.Server.Holiday.Interfaces;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Holiday.ShouldCelebrate
{
	// Token: 0x02000466 RID: 1126
	[DataDefinition]
	public sealed class DayOfYear : IHolidayShouldCelebrate
	{
		// Token: 0x060016B4 RID: 5812 RVA: 0x000779F9 File Offset: 0x00075BF9
		[NullableContext(1)]
		public bool ShouldCelebrate(DateTime date, HolidayPrototype holiday)
		{
			return (long)date.DayOfYear == (long)((ulong)this._dayOfYear);
		}

		// Token: 0x04000E39 RID: 3641
		[DataField("dayOfYear", false, 1, false, false, null)]
		private uint _dayOfYear = 1U;
	}
}
