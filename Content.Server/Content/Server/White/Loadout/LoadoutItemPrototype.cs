using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.White.Loadout
{
	// Token: 0x02000097 RID: 151
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("loadout", 1)]
	public sealed class LoadoutItemPrototype : IPrototype
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000257 RID: 599 RVA: 0x0000CBFA File Offset: 0x0000ADFA
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000CC02 File Offset: 0x0000AE02
		[DataField("entity", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string EntityId { get; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0000CC0A File Offset: 0x0000AE0A
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("whitelistJobs", false, 1, false, false, typeof(PrototypeIdListSerializer<JobPrototype>))]
		public List<string> WhitelistJobs { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000CC12 File Offset: 0x0000AE12
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("blacklistJobs", false, 1, false, false, typeof(PrototypeIdListSerializer<JobPrototype>))]
		public List<string> BlacklistJobs { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600025B RID: 603 RVA: 0x0000CC1A File Offset: 0x0000AE1A
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("speciesRestriction", false, 1, false, false, null)]
		public List<string> SpeciesRestrictions { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x040001B5 RID: 437
		[DataField("sponsorOnly", false, 1, false, false, null)]
		public bool SponsorOnly;
	}
}
