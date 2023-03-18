using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Maps.NameGenerators;
using Content.Shared.Roles;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Utility;

namespace Content.Server.Station
{
	// Token: 0x02000197 RID: 407
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class StationConfig
	{
		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000800 RID: 2048 RVA: 0x00028001 File Offset: 0x00026201
		[DataField("mapNameTemplate", false, 1, true, false, null)]
		public string StationNameTemplate { get; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000801 RID: 2049 RVA: 0x00028009 File Offset: 0x00026209
		[Nullable(2)]
		[DataField("nameGenerator", false, 1, false, false, null)]
		public StationNameGenerator NameGenerator { [NullableContext(2)] get; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000802 RID: 2050 RVA: 0x00028011 File Offset: 0x00026211
		public IReadOnlyList<string> OverflowJobs
		{
			get
			{
				return this._overflowJobs;
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000803 RID: 2051 RVA: 0x00028019 File Offset: 0x00026219
		public IReadOnlyDictionary<string, List<int?>> AvailableJobs
		{
			get
			{
				return this._availableJobs;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000804 RID: 2052 RVA: 0x00028021 File Offset: 0x00026221
		// (set) Token: 0x06000805 RID: 2053 RVA: 0x00028029 File Offset: 0x00026229
		[DataField("extendedAccessThreshold", false, 1, false, false, null)]
		public int ExtendedAccessThreshold { get; set; } = 15;

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000806 RID: 2054 RVA: 0x00028032 File Offset: 0x00026232
		// (set) Token: 0x06000807 RID: 2055 RVA: 0x0002803A File Offset: 0x0002623A
		[DataField("emergencyShuttlePath", false, 1, false, false, typeof(ResourcePathSerializer))]
		public ResourcePath EmergencyShuttlePath { get; set; } = new ResourcePath("/Maps/Shuttles/emergency.yml", "/");

		// Token: 0x040004E5 RID: 1253
		[DataField("overflowJobs", false, 1, true, false, typeof(PrototypeIdListSerializer<JobPrototype>))]
		private readonly List<string> _overflowJobs;

		// Token: 0x040004E6 RID: 1254
		[DataField("availableJobs", false, 1, true, false, typeof(PrototypeIdDictionarySerializer<List<int?>, JobPrototype>))]
		private readonly Dictionary<string, List<int?>> _availableJobs;
	}
}
