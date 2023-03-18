using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.NameIdentifier
{
	// Token: 0x020002D6 RID: 726
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("nameIdentifierGroup", 1)]
	public sealed class NameIdentifierGroupPrototype : IPrototype
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060007ED RID: 2029 RVA: 0x0001A569 File Offset: 0x00018769
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000829 RID: 2089
		[DataField("fullName", false, 1, false, false, null)]
		public bool FullName;

		// Token: 0x0400082A RID: 2090
		[Nullable(2)]
		[DataField("prefix", false, 1, false, false, null)]
		public string Prefix;

		// Token: 0x0400082B RID: 2091
		[DataField("maxValue", false, 1, false, false, null)]
		public int MaxValue = 999;

		// Token: 0x0400082C RID: 2092
		[DataField("minValue", false, 1, false, false, null)]
		public int MinValue;
	}
}
