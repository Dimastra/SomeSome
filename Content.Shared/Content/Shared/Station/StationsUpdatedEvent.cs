using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Station
{
	// Token: 0x02000162 RID: 354
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StationsUpdatedEvent : EntityEventArgs
	{
		// Token: 0x06000444 RID: 1092 RVA: 0x00011283 File Offset: 0x0000F483
		public StationsUpdatedEvent(HashSet<EntityUid> stations)
		{
			this.Stations = stations;
		}

		// Token: 0x04000415 RID: 1045
		public readonly HashSet<EntityUid> Stations;
	}
}
