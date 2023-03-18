using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Components
{
	// Token: 0x0200058E RID: 1422
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public struct GenericPartInfo
	{
		// Token: 0x04001015 RID: 4117
		[DataField("Amount", false, 1, false, false, null)]
		public int Amount;

		// Token: 0x04001016 RID: 4118
		[DataField("ExamineName", false, 1, false, false, null)]
		public string ExamineName;

		// Token: 0x04001017 RID: 4119
		[DataField("DefaultPrototype", false, 1, false, false, null)]
		public string DefaultPrototype;
	}
}
