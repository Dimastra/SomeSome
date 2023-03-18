using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Holiday.Interfaces
{
	// Token: 0x0200046C RID: 1132
	[NullableContext(1)]
	[ImplicitDataDefinitionForInheritors]
	public interface IHolidayShouldCelebrate
	{
		// Token: 0x060016BE RID: 5822
		bool ShouldCelebrate(DateTime date, HolidayPrototype holiday);
	}
}
