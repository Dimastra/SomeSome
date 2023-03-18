using System;
using System.Runtime.CompilerServices;

namespace Content.Server.Holiday.Interfaces
{
	// Token: 0x0200046A RID: 1130
	[NullableContext(1)]
	public interface IHolidayCelebrate
	{
		// Token: 0x060016BC RID: 5820
		void Celebrate(HolidayPrototype holiday);
	}
}
