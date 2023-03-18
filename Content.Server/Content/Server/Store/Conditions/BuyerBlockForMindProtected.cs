using System;
using Content.Server.Mind.Components;
using Content.Server.Roles;
using Content.Shared.Store;
using Robust.Shared.GameObjects;

namespace Content.Server.Store.Conditions
{
	// Token: 0x02000153 RID: 339
	public sealed class BuyerBlockForMindProtected : ListingCondition
	{
		// Token: 0x0600068A RID: 1674 RVA: 0x0001FCC8 File Offset: 0x0001DEC8
		public override bool Condition(ListingConditionArgs args)
		{
			EntityUid buyer = args.Buyer;
			MindComponent mind;
			if (!args.EntityManager.TryGetComponent<MindComponent>(buyer, ref mind) || mind.Mind == null)
			{
				return false;
			}
			Job currentJob = mind.Mind.CurrentJob;
			return currentJob != null && currentJob.CanBeAntag;
		}
	}
}
