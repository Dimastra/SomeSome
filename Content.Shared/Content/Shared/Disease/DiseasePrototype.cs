using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Disease
{
	// Token: 0x02000507 RID: 1287
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("disease", 1)]
	[DataDefinition]
	public sealed class DiseasePrototype : IPrototype, IInheritingPrototype
	{
		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06000F95 RID: 3989 RVA: 0x0003281B File Offset: 0x00030A1B
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06000F96 RID: 3990 RVA: 0x00032823 File Offset: 0x00030A23
		// (set) Token: 0x06000F97 RID: 3991 RVA: 0x0003282B File Offset: 0x00030A2B
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = string.Empty;

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06000F98 RID: 3992 RVA: 0x00032834 File Offset: 0x00030A34
		// (set) Token: 0x06000F99 RID: 3993 RVA: 0x0003283C File Offset: 0x00030A3C
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<DiseasePrototype>), 1)]
		public string[] Parents { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] private set; }

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06000F9A RID: 3994 RVA: 0x00032845 File Offset: 0x00030A45
		// (set) Token: 0x06000F9B RID: 3995 RVA: 0x0003284D File Offset: 0x00030A4D
		[NeverPushInheritance]
		[AbstractDataField(1)]
		public bool Abstract { get; private set; }

		// Token: 0x04000EDA RID: 3802
		[ViewVariables]
		public float TickTime = 1f;

		// Token: 0x04000EDB RID: 3803
		public float Accumulator;

		// Token: 0x04000EDC RID: 3804
		public float TotalAccumulator;

		// Token: 0x04000EDD RID: 3805
		[DataField("stages", false, 1, false, true, null)]
		public readonly List<float> Stages = new List<float>
		{
			0f
		};

		// Token: 0x04000EDE RID: 3806
		[DataField("effects", false, 1, false, true, null)]
		public readonly List<DiseaseEffect> Effects = new List<DiseaseEffect>(0);

		// Token: 0x04000EDF RID: 3807
		[DataField("cures", false, 1, false, true, null)]
		public readonly List<DiseaseCure> Cures = new List<DiseaseCure>(0);

		// Token: 0x04000EE0 RID: 3808
		[DataField("cureResist", false, 1, false, true, null)]
		public float CureResist = 0.05f;

		// Token: 0x04000EE1 RID: 3809
		[DataField("infectious", false, 1, false, true, null)]
		public bool Infectious = true;
	}
}
