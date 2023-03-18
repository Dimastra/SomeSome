using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004EC RID: 1260
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EvaporationSystem : EntitySystem
	{
		// Token: 0x060019EC RID: 6636 RVA: 0x00087F90 File Offset: 0x00086190
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (EvaporationComponent evaporationComponent in this.EntityManager.EntityQuery<EvaporationComponent>(false))
			{
				EntityUid uid = evaporationComponent.Owner;
				evaporationComponent.Accumulator += frameTime;
				Solution solution;
				if (!this._solutionContainerSystem.TryGetSolution(uid, evaporationComponent.SolutionName, out solution, null))
				{
					this.EntityManager.QueueDeleteEntity(uid);
				}
				else if (evaporationComponent.Accumulator >= evaporationComponent.EvaporateTime)
				{
					evaporationComponent.Accumulator -= evaporationComponent.EvaporateTime;
					if (evaporationComponent.EvaporationToggle)
					{
						this._solutionContainerSystem.SplitSolution(uid, solution, FixedPoint2.Min(FixedPoint2.New(1), solution.Volume));
					}
					evaporationComponent.EvaporationToggle = (solution.Volume > evaporationComponent.LowerLimit && solution.Volume < evaporationComponent.UpperLimit);
				}
			}
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x00088098 File Offset: 0x00086298
		public void CopyConstruct(EntityUid destUid, EvaporationComponent srcEvaporation)
		{
			EvaporationComponent evaporationComponent = this.EntityManager.EnsureComponent<EvaporationComponent>(destUid);
			evaporationComponent.EvaporateTime = srcEvaporation.EvaporateTime;
			evaporationComponent.EvaporationToggle = srcEvaporation.EvaporationToggle;
			evaporationComponent.SolutionName = srcEvaporation.SolutionName;
			evaporationComponent.LowerLimit = srcEvaporation.LowerLimit;
			evaporationComponent.UpperLimit = srcEvaporation.UpperLimit;
		}

		// Token: 0x0400104D RID: 4173
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;
	}
}
