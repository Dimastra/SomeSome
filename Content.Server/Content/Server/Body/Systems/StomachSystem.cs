using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Body.Organ;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Server.Body.Systems
{
	// Token: 0x0200070D RID: 1805
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StomachSystem : EntitySystem
	{
		// Token: 0x0600260C RID: 9740 RVA: 0x000C8FB0 File Offset: 0x000C71B0
		public override void Initialize()
		{
			base.SubscribeLocalEvent<StomachComponent, ComponentInit>(new ComponentEventHandler<StomachComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<StomachComponent, ApplyMetabolicMultiplierEvent>(new ComponentEventHandler<StomachComponent, ApplyMetabolicMultiplierEvent>(this.OnApplyMetabolicMultiplier), null, null);
		}

		// Token: 0x0600260D RID: 9741 RVA: 0x000C8FDC File Offset: 0x000C71DC
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<StomachComponent, OrganComponent, SolutionContainerManagerComponent> valueTuple in this.EntityManager.EntityQuery<StomachComponent, OrganComponent, SolutionContainerManagerComponent>(false))
			{
				StomachComponent stomach = valueTuple.Item1;
				OrganComponent organ = valueTuple.Item2;
				SolutionContainerManagerComponent sol = valueTuple.Item3;
				stomach.AccumulatedFrameTime += frameTime;
				if (stomach.AccumulatedFrameTime >= stomach.UpdateInterval)
				{
					stomach.AccumulatedFrameTime -= stomach.UpdateInterval;
					Solution stomachSolution;
					if (this._solutionContainerSystem.TryGetSolution(stomach.Owner, "stomach", out stomachSolution, sol))
					{
						EntityUid? body2 = organ.Body;
						if (body2 != null)
						{
							EntityUid body = body2.GetValueOrDefault();
							Solution bodySolution;
							if (this._solutionContainerSystem.TryGetSolution(body, stomach.BodySolutionName, out bodySolution, null))
							{
								Solution transferSolution = new Solution();
								RemQueue<StomachComponent.ReagentDelta> queue = default(RemQueue<StomachComponent.ReagentDelta>);
								foreach (StomachComponent.ReagentDelta delta in stomach.ReagentDeltas)
								{
									delta.Increment(stomach.UpdateInterval);
									if (delta.Lifetime > stomach.DigestionDelay)
									{
										FixedPoint2 quant;
										if (stomachSolution.TryGetReagent(delta.ReagentId, out quant))
										{
											if (quant > delta.Quantity)
											{
												quant = delta.Quantity;
											}
											this._solutionContainerSystem.TryRemoveReagent(stomach.Owner, stomachSolution, delta.ReagentId, quant);
											transferSolution.AddReagent(delta.ReagentId, quant, true);
										}
										queue.Add(delta);
									}
								}
								foreach (StomachComponent.ReagentDelta item in queue)
								{
									stomach.ReagentDeltas.Remove(item);
								}
								this._solutionContainerSystem.TryAddSolution(body, bodySolution, transferSolution);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600260E RID: 9742 RVA: 0x000C9218 File Offset: 0x000C7418
		private void OnApplyMetabolicMultiplier(EntityUid uid, StomachComponent component, ApplyMetabolicMultiplierEvent args)
		{
			if (args.Apply)
			{
				component.UpdateInterval *= args.Multiplier;
				return;
			}
			component.UpdateInterval /= args.Multiplier;
			if (component.AccumulatedFrameTime >= component.UpdateInterval)
			{
				component.AccumulatedFrameTime = component.UpdateInterval;
			}
		}

		// Token: 0x0600260F RID: 9743 RVA: 0x000C9270 File Offset: 0x000C7470
		private void OnComponentInit(EntityUid uid, StomachComponent component, ComponentInit args)
		{
			bool flag;
			this._solutionContainerSystem.EnsureSolution(uid, "stomach", component.InitialMaxVolume, out flag, null);
		}

		// Token: 0x06002610 RID: 9744 RVA: 0x000C9298 File Offset: 0x000C7498
		public bool CanTransferSolution(EntityUid uid, Solution solution, [Nullable(2)] SolutionContainerManagerComponent solutions = null)
		{
			Solution stomachSolution;
			return base.Resolve<SolutionContainerManagerComponent>(uid, ref solutions, false) && this._solutionContainerSystem.TryGetSolution(uid, "stomach", out stomachSolution, solutions) && stomachSolution.CanAddSolution(solution);
		}

		// Token: 0x06002611 RID: 9745 RVA: 0x000C92D8 File Offset: 0x000C74D8
		[NullableContext(2)]
		public bool TryTransferSolution(EntityUid uid, [Nullable(1)] Solution solution, StomachComponent stomach = null, SolutionContainerManagerComponent solutions = null)
		{
			if (!base.Resolve<StomachComponent, SolutionContainerManagerComponent>(uid, ref stomach, ref solutions, false))
			{
				return false;
			}
			Solution stomachSolution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, "stomach", out stomachSolution, solutions) || !this.CanTransferSolution(uid, solution, solutions))
			{
				return false;
			}
			this._solutionContainerSystem.TryAddSolution(uid, stomachSolution, solution);
			foreach (Solution.ReagentQuantity reagent in solution.Contents)
			{
				stomach.ReagentDeltas.Add(new StomachComponent.ReagentDelta(reagent.ReagentId, reagent.Quantity));
			}
			return true;
		}

		// Token: 0x04001782 RID: 6018
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04001783 RID: 6019
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04001784 RID: 6020
		public const string DefaultSolutionName = "stomach";
	}
}
