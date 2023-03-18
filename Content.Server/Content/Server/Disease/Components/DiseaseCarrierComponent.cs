using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Components
{
	// Token: 0x02000573 RID: 1395
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DiseaseCarrierComponent : Component
	{
		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06001D65 RID: 7525 RVA: 0x0009CB54 File Offset: 0x0009AD54
		[ViewVariables]
		public List<DiseasePrototype> AllDiseases
		{
			get
			{
				return this.PastDiseases.Concat(this.Diseases).ToList<DiseasePrototype>();
			}
		}

		// Token: 0x040012D6 RID: 4822
		[ViewVariables]
		public List<DiseasePrototype> Diseases = new List<DiseasePrototype>();

		// Token: 0x040012D7 RID: 4823
		[DataField("diseaseResist", false, 1, false, false, null)]
		[ViewVariables]
		public float DiseaseResist;

		// Token: 0x040012D8 RID: 4824
		[ViewVariables]
		public List<DiseasePrototype> PastDiseases = new List<DiseasePrototype>();

		// Token: 0x040012D9 RID: 4825
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("carrierDiseases", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<DiseasePrototype>))]
		public HashSet<string> CarrierDiseases;

		// Token: 0x040012DA RID: 4826
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("naturalImmunities", false, 1, false, false, null)]
		public List<string> NaturalImmunities;
	}
}
