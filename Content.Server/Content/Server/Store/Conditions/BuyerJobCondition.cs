using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind.Components;
using Content.Server.Roles;
using Content.Shared.Roles;
using Content.Shared.Store;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Store.Conditions
{
	// Token: 0x02000154 RID: 340
	public sealed class BuyerJobCondition : ListingCondition
	{
		// Token: 0x0600068C RID: 1676 RVA: 0x0001FD20 File Offset: 0x0001DF20
		public override bool Condition(ListingConditionArgs args)
		{
			MindComponent mind;
			if (!args.EntityManager.TryGetComponent<MindComponent>(args.Buyer, ref mind) || mind.Mind == null)
			{
				return true;
			}
			if (this.Blacklist != null)
			{
				foreach (Role role in mind.Mind.AllRoles)
				{
					Job job = role as Job;
					if (job != null && this.Blacklist.Contains(job.Prototype.ID))
					{
						return false;
					}
				}
			}
			if (this.Whitelist != null)
			{
				bool found = false;
				foreach (Role role2 in mind.Mind.AllRoles)
				{
					Job job2 = role2 as Job;
					if (job2 != null && this.Whitelist.Contains(job2.Prototype.ID))
					{
						found = true;
					}
				}
				if (!found)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x040003C7 RID: 967
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("whitelist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<JobPrototype>))]
		public HashSet<string> Whitelist;

		// Token: 0x040003C8 RID: 968
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("blacklist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<JobPrototype>))]
		public HashSet<string> Blacklist;
	}
}
