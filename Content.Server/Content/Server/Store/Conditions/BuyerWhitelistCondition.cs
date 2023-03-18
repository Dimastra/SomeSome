using System;
using System.Runtime.CompilerServices;
using Content.Shared.Store;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Store.Conditions
{
	// Token: 0x02000156 RID: 342
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class BuyerWhitelistCondition : ListingCondition
	{
		// Token: 0x06000690 RID: 1680 RVA: 0x0001FEA0 File Offset: 0x0001E0A0
		public override bool Condition(ListingConditionArgs args)
		{
			IEntityManager ent = args.EntityManager;
			return (this.Whitelist == null || this.Whitelist.IsValid(args.Buyer, ent)) && (this.Blacklist == null || !this.Blacklist.IsValid(args.Buyer, ent));
		}

		// Token: 0x040003CB RID: 971
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x040003CC RID: 972
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist;
	}
}
