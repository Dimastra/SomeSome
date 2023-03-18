using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Content.Shared.Storage;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityList
{
	// Token: 0x020004B8 RID: 1208
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("entityLootTable", 1)]
	public sealed class EntityLootTablePrototype : IPrototype
	{
		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06000EA0 RID: 3744 RVA: 0x0002F26A File Offset: 0x0002D46A
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x06000EA1 RID: 3745 RVA: 0x0002F272 File Offset: 0x0002D472
		[NullableContext(2)]
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public List<string> GetSpawns(IRobustRandom random = null)
		{
			return EntitySpawnCollection.GetSpawns(this.Entries, random);
		}

		// Token: 0x04000DC3 RID: 3523
		[DataField("entries", false, 1, false, false, null)]
		public ImmutableList<EntitySpawnEntry> Entries = ImmutableList<EntitySpawnEntry>.Empty;
	}
}
