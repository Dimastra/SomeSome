using System;
using System.Runtime.CompilerServices;
using Content.Shared.Store;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Store.Conditions
{
	// Token: 0x02000158 RID: 344
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class StoreWhitelistCondition : ListingCondition
	{
		// Token: 0x06000694 RID: 1684 RVA: 0x0001FF1C File Offset: 0x0001E11C
		public override bool Condition(ListingConditionArgs args)
		{
			if (args.StoreEntity == null)
			{
				return false;
			}
			IEntityManager ent = args.EntityManager;
			return (this.Whitelist == null || this.Whitelist.IsValid(args.StoreEntity.Value, ent)) && (this.Blacklist == null || !this.Blacklist.IsValid(args.StoreEntity.Value, ent));
		}

		// Token: 0x040003CE RID: 974
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x040003CF RID: 975
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist;
	}
}
