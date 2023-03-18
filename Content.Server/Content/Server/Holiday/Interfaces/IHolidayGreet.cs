using System;
using System.Runtime.CompilerServices;

namespace Content.Server.Holiday.Interfaces
{
	// Token: 0x0200046B RID: 1131
	[NullableContext(1)]
	public interface IHolidayGreet
	{
		// Token: 0x060016BD RID: 5821
		string Greet(HolidayPrototype holiday);
	}
}
