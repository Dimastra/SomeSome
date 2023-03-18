using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Content.Shared.Roles;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Server.Station.Components
{
	// Token: 0x020001A3 RID: 419
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StationJobsSystem)
	})]
	public sealed class StationJobsComponent : Component
	{
		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000856 RID: 2134 RVA: 0x0002A9DC File Offset: 0x00028BDC
		[ViewVariables]
		public float? PercentJobsRemaining
		{
			get
			{
				if (this.MidRoundTotalJobs <= 0)
				{
					return null;
				}
				return new float?((float)this.TotalJobs / (float)this.MidRoundTotalJobs);
			}
		}

		// Token: 0x0400051A RID: 1306
		[DataField("roundStartTotalJobs", false, 1, false, false, null)]
		public int RoundStartTotalJobs;

		// Token: 0x0400051B RID: 1307
		[DataField("midRoundTotalJobs", false, 1, false, false, null)]
		public int MidRoundTotalJobs;

		// Token: 0x0400051C RID: 1308
		[DataField("totalJobs", false, 1, false, false, null)]
		public int TotalJobs;

		// Token: 0x0400051D RID: 1309
		[DataField("extendedAccess", false, 1, false, false, null)]
		public bool ExtendedAccess;

		// Token: 0x0400051E RID: 1310
		[DataField("jobList", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<uint?, JobPrototype>))]
		public Dictionary<string, uint?> JobList = new Dictionary<string, uint?>();

		// Token: 0x0400051F RID: 1311
		[DataField("roundStartJobList", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<uint?, JobPrototype>))]
		public Dictionary<string, uint?> RoundStartJobList = new Dictionary<string, uint?>();

		// Token: 0x04000520 RID: 1312
		[DataField("overflowJobs", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<JobPrototype>))]
		public HashSet<string> OverflowJobs = new HashSet<string>();
	}
}
