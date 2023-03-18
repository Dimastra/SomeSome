using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Store;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Store.Conditions
{
	// Token: 0x02000155 RID: 341
	public sealed class BuyerSpeciesCondition : ListingCondition
	{
		// Token: 0x0600068E RID: 1678 RVA: 0x0001FE34 File Offset: 0x0001E034
		public override bool Condition(ListingConditionArgs args)
		{
			HumanoidAppearanceComponent appearance;
			return !args.EntityManager.TryGetComponent<HumanoidAppearanceComponent>(args.Buyer, ref appearance) || ((this.Blacklist == null || !this.Blacklist.Contains(appearance.Species)) && (this.Whitelist == null || this.Whitelist.Contains(appearance.Species)));
		}

		// Token: 0x040003C9 RID: 969
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("whitelist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<SpeciesPrototype>))]
		public HashSet<string> Whitelist;

		// Token: 0x040003CA RID: 970
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("blacklist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<SpeciesPrototype>))]
		public HashSet<string> Blacklist;
	}
}
