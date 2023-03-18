using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Holiday;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Maps.Conditions
{
	// Token: 0x020003DD RID: 989
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HolidayMapCondition : GameMapCondition
	{
		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06001456 RID: 5206 RVA: 0x000692F9 File Offset: 0x000674F9
		[DataField("holidays", false, 1, false, false, null)]
		public string[] Holidays { get; }

		// Token: 0x06001457 RID: 5207 RVA: 0x00069304 File Offset: 0x00067504
		public override bool Check(GameMapPrototype map)
		{
			HolidaySystem holidaySystem = EntitySystem.Get<HolidaySystem>();
			return this.Holidays.Any((string holiday) => holidaySystem.IsCurrentlyHoliday(holiday)) ^ base.Inverted;
		}
	}
}
