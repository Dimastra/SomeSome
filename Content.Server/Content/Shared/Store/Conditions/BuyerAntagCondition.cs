using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind.Components;
using Content.Server.Roles;
using Content.Server.Traitor;
using Content.Shared.Roles;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Store.Conditions
{
	// Token: 0x02000014 RID: 20
	public sealed class BuyerAntagCondition : ListingCondition
	{
		// Token: 0x06000039 RID: 57 RVA: 0x00002C14 File Offset: 0x00000E14
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
					TraitorRole blacklistantag = role as TraitorRole;
					if (blacklistantag != null && this.Blacklist.Contains(blacklistantag.Prototype.ID))
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
					TraitorRole antag = role2 as TraitorRole;
					if (antag != null && this.Whitelist.Contains(antag.Prototype.ID))
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

		// Token: 0x04000020 RID: 32
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("whitelist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<AntagPrototype>))]
		public HashSet<string> Whitelist;

		// Token: 0x04000021 RID: 33
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("blacklist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<AntagPrototype>))]
		public HashSet<string> Blacklist;
	}
}
