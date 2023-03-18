using System;
using System.Collections.Generic;
using Content.Server.Mind.Components;
using Content.Server.Roles;
using Content.Server.Traitor;
using Content.Shared.Store;

namespace Content.Server.Store.Conditions
{
	// Token: 0x02000152 RID: 338
	public sealed class BuyerBlockForAntagCondition : ListingCondition
	{
		// Token: 0x06000688 RID: 1672 RVA: 0x0001FC44 File Offset: 0x0001DE44
		public override bool Condition(ListingConditionArgs args)
		{
			MindComponent mind;
			if (!args.EntityManager.TryGetComponent<MindComponent>(args.Buyer, ref mind) || mind.Mind == null)
			{
				return false;
			}
			using (IEnumerator<Role> enumerator = mind.Mind.AllRoles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is TraitorRole)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
