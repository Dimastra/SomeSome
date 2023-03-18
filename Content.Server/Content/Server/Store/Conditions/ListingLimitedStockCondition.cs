using System;
using Content.Shared.Store;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Store.Conditions
{
	// Token: 0x02000157 RID: 343
	public sealed class ListingLimitedStockCondition : ListingCondition
	{
		// Token: 0x06000692 RID: 1682 RVA: 0x0001FEFC File Offset: 0x0001E0FC
		public override bool Condition(ListingConditionArgs args)
		{
			return args.Listing.PurchaseAmount < this.Stock;
		}

		// Token: 0x040003CD RID: 973
		[DataField("stock", false, 1, true, false, null)]
		public int Stock;
	}
}
