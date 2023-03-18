using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Dataset;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared.Random.Helpers
{
	// Token: 0x0200021B RID: 539
	[NullableContext(1)]
	[Nullable(0)]
	public static class SharedRandomExtensions
	{
		// Token: 0x06000600 RID: 1536 RVA: 0x000151A4 File Offset: 0x000133A4
		public static string Pick(this IRobustRandom random, DatasetPrototype prototype)
		{
			return RandomExtensions.Pick<string>(random, prototype.Values);
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x000151B4 File Offset: 0x000133B4
		public static string Pick(this WeightedRandomPrototype prototype, [Nullable(2)] IRobustRandom random = null)
		{
			IoCManager.Resolve<IRobustRandom>(ref random);
			Dictionary<string, float> weights = prototype.Weights;
			float sum = weights.Values.Sum();
			float accumulated = 0f;
			float rand = random.NextFloat() * sum;
			foreach (KeyValuePair<string, float> keyValuePair in weights)
			{
				string text;
				float num;
				keyValuePair.Deconstruct(out text, out num);
				string key = text;
				float weight = num;
				accumulated += weight;
				if (accumulated >= rand)
				{
					return key;
				}
			}
			throw new InvalidOperationException("Invalid weighted pick for " + prototype.ID + "!");
		}
	}
}
