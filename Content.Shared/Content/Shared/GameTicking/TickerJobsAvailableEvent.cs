using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x0200046B RID: 1131
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class TickerJobsAvailableEvent : EntityEventArgs
	{
		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000DB5 RID: 3509 RVA: 0x0002CAE8 File Offset: 0x0002ACE8
		public Dictionary<EntityUid, Dictionary<string, uint?>> JobsAvailableByStation { get; }

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000DB6 RID: 3510 RVA: 0x0002CAF0 File Offset: 0x0002ACF0
		public Dictionary<EntityUid, string> StationNames { get; }

		// Token: 0x06000DB7 RID: 3511 RVA: 0x0002CAF8 File Offset: 0x0002ACF8
		public TickerJobsAvailableEvent(Dictionary<EntityUid, string> stationNames, Dictionary<EntityUid, Dictionary<string, uint?>> jobsAvailableByStation)
		{
			this.StationNames = stationNames;
			this.JobsAvailableByStation = jobsAvailableByStation;
		}
	}
}
