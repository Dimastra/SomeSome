using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000655 RID: 1621
	public sealed class AddToSolutionReaction : ReagentEffect
	{
		// Token: 0x06002248 RID: 8776 RVA: 0x000B36E8 File Offset: 0x000B18E8
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Reagent == null)
			{
				return;
			}
			Solution solutionContainer;
			if (!EntitySystem.Get<SolutionContainerSystem>().TryGetSolution(args.SolutionEntity, this._solution, out solutionContainer, null))
			{
				return;
			}
			FixedPoint2 accepted;
			if (EntitySystem.Get<SolutionContainerSystem>().TryAddReagent(args.SolutionEntity, solutionContainer, args.Reagent.ID, args.Quantity, out accepted, null))
			{
				Solution source = args.Source;
				if (source == null)
				{
					return;
				}
				source.RemoveReagent(args.Reagent.ID, accepted);
			}
		}

		// Token: 0x04001530 RID: 5424
		[Nullable(1)]
		[DataField("solution", false, 1, false, false, null)]
		private string _solution = "reagents";
	}
}
