using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Chemistry.Reagent
{
	// Token: 0x020005E3 RID: 1507
	public static class ReagentEffectExt
	{
		// Token: 0x06001212 RID: 4626 RVA: 0x0003B4A0 File Offset: 0x000396A0
		[NullableContext(1)]
		public static bool ShouldApply(this ReagentEffect effect, ReagentEffectArgs args, [Nullable(2)] IRobustRandom random = null)
		{
			if (random == null)
			{
				random = IoCManager.Resolve<IRobustRandom>();
			}
			if (effect.Probability < 1f && !RandomExtensions.Prob(random, effect.Probability))
			{
				return false;
			}
			if (effect.Conditions != null)
			{
				ReagentEffectCondition[] conditions = effect.Conditions;
				for (int i = 0; i < conditions.Length; i++)
				{
					if (!conditions[i].Condition(args))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
