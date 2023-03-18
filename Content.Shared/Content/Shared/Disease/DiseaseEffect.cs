using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Disease
{
	// Token: 0x02000504 RID: 1284
	[ImplicitDataDefinitionForInheritors]
	public abstract class DiseaseEffect
	{
		// Token: 0x06000F84 RID: 3972
		public abstract void Effect(DiseaseEffectArgs args);

		// Token: 0x04000ECE RID: 3790
		[DataField("probability", false, 1, false, false, null)]
		public float Probability = 1f;

		// Token: 0x04000ECF RID: 3791
		[Nullable(1)]
		[DataField("stages", false, 1, false, false, null)]
		public readonly int[] Stages = new int[1];
	}
}
