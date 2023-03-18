using System;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.Components;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005AC RID: 1452
	[DataDefinition]
	public sealed class SolutionExplosionBehavior : IThresholdBehavior
	{
		// Token: 0x06001E1F RID: 7711 RVA: 0x0009F488 File Offset: 0x0009D688
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			Solution explodingSolution;
			ExplosiveComponent explosiveComponent;
			if (system.SolutionContainerSystem.TryGetSolution(owner, this.Solution, out explodingSolution, null) && system.EntityManager.TryGetComponent<ExplosiveComponent>(owner, ref explosiveComponent))
			{
				if (explodingSolution.Volume == 0)
				{
					return;
				}
				float explosionScaleFactor = explodingSolution.FillFraction;
				EntityCoordinates coordinates = system.EntityManager.GetComponent<TransformComponent>(owner).Coordinates;
				system.SpillableSystem.SpillAt(explodingSolution, coordinates, "PuddleSmear", true, true, true);
				float explosiveTotalIntensity = explosiveComponent.TotalIntensity * explosionScaleFactor;
				system.ExplosionSystem.TriggerExplosive(owner, explosiveComponent, false, new float?(explosiveTotalIntensity), null, cause);
			}
		}

		// Token: 0x04001346 RID: 4934
		[Nullable(1)]
		[DataField("solution", false, 1, true, false, null)]
		public string Solution;
	}
}
