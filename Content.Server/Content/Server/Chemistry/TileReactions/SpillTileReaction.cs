using System;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.Components;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Slippery;
using Content.Shared.StepTrigger.Components;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.TileReactions
{
	// Token: 0x02000653 RID: 1619
	[DataDefinition]
	public sealed class SpillTileReaction : ITileReaction
	{
		// Token: 0x06002244 RID: 8772 RVA: 0x000B35D0 File Offset: 0x000B17D0
		[NullableContext(1)]
		public FixedPoint2 TileReact(TileRef tile, ReagentPrototype reagent, FixedPoint2 reactVolume)
		{
			if (reactVolume < 5)
			{
				return FixedPoint2.Zero;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			PuddleComponent puddle = entityManager.EntitySysManager.GetEntitySystem<SpillableSystem>().SpillAt(tile, new Solution(reagent.ID, reactVolume), "PuddleSmear", this._overflow, false, true, true);
			if (puddle != null)
			{
				SlipperyComponent slippery = entityManager.EnsureComponent<SlipperyComponent>(puddle.Owner);
				slippery.LaunchForwardsMultiplier = this._launchForwardsMultiplier;
				slippery.ParalyzeTime = this._paralyzeTime;
				entityManager.Dirty(slippery, null);
				StepTriggerComponent step = entityManager.EnsureComponent<StepTriggerComponent>(puddle.Owner);
				entityManager.EntitySysManager.GetEntitySystem<StepTriggerSystem>().SetRequiredTriggerSpeed(puddle.Owner, this._requiredSlipSpeed, step);
				return reactVolume;
			}
			return FixedPoint2.Zero;
		}

		// Token: 0x0400152C RID: 5420
		[DataField("launchForwardsMultiplier", false, 1, false, false, null)]
		private float _launchForwardsMultiplier = 1f;

		// Token: 0x0400152D RID: 5421
		[DataField("requiredSlipSpeed", false, 1, false, false, null)]
		private float _requiredSlipSpeed = 6f;

		// Token: 0x0400152E RID: 5422
		[DataField("paralyzeTime", false, 1, false, false, null)]
		private float _paralyzeTime = 1f;

		// Token: 0x0400152F RID: 5423
		[DataField("overflow", false, 1, false, false, null)]
		private bool _overflow;
	}
}
