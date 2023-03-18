using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Disease;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Disease.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200065A RID: 1626
	public sealed class ChemCauseRandomDisease : ReagentEffect
	{
		// Token: 0x06002252 RID: 8786 RVA: 0x000B3A58 File Offset: 0x000B1C58
		public override void Effect(ReagentEffectArgs args)
		{
			DiseasedComponent diseased;
			if (args.EntityManager.TryGetComponent<DiseasedComponent>(args.SolutionEntity, ref diseased))
			{
				return;
			}
			if (args.Scale != 1f)
			{
				return;
			}
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			EntitySystem.Get<DiseaseSystem>().TryAddDisease(args.SolutionEntity, RandomExtensions.Pick<string>(random, this.Diseases), null);
		}

		// Token: 0x0400153B RID: 5435
		[Nullable(1)]
		[DataField("diseases", false, 1, true, false, null)]
		[ViewVariables]
		public List<string> Diseases;
	}
}
