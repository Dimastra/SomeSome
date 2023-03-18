using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Disease
{
	// Token: 0x02000503 RID: 1283
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class DiseaseCure
	{
		// Token: 0x06000F81 RID: 3969
		public abstract bool Cure(DiseaseEffectArgs args);

		// Token: 0x06000F82 RID: 3970
		public abstract string CureText();

		// Token: 0x04000ECD RID: 3789
		[DataField("stages", false, 1, false, false, null)]
		public readonly int[] Stages = new int[1];
	}
}
