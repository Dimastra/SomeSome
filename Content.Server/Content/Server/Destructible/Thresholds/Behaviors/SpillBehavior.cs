using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.Components;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005AE RID: 1454
	[DataDefinition]
	public sealed class SpillBehavior : IThresholdBehavior
	{
		// Token: 0x06001E27 RID: 7719 RVA: 0x0009F6DC File Offset: 0x0009D8DC
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			SolutionContainerSystem solutionContainerSystem = EntitySystem.Get<SolutionContainerSystem>();
			SpillableSystem spillableSystem = EntitySystem.Get<SpillableSystem>();
			EntityCoordinates coordinates = system.EntityManager.GetComponent<TransformComponent>(owner).Coordinates;
			SpillableComponent spillableComponent;
			Solution compSolution;
			if (system.EntityManager.TryGetComponent<SpillableComponent>(owner, ref spillableComponent) && solutionContainerSystem.TryGetSolution(owner, spillableComponent.SolutionName, out compSolution, null))
			{
				spillableSystem.SpillAt(compSolution, coordinates, "PuddleSmear", false, true, true);
				return;
			}
			Solution behaviorSolution;
			if (this.Solution != null && solutionContainerSystem.TryGetSolution(owner, this.Solution, out behaviorSolution, null))
			{
				spillableSystem.SpillAt(behaviorSolution, coordinates, "PuddleSmear", true, true, true);
			}
		}

		// Token: 0x04001349 RID: 4937
		[Nullable(2)]
		[DataField("solution", false, 1, false, false, null)]
		public string Solution;
	}
}
