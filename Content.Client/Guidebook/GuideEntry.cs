using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook
{
	// Token: 0x020002EC RID: 748
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class GuideEntry
	{
		// Token: 0x04000977 RID: 2423
		[DataField("text", false, 1, true, false, null)]
		public ResourcePath Text;

		// Token: 0x04000978 RID: 2424
		[IdDataField(1, null)]
		public string Id;

		// Token: 0x04000979 RID: 2425
		[DataField("name", false, 1, true, false, null)]
		public string Name;

		// Token: 0x0400097A RID: 2426
		[DataField("children", false, 1, false, false, typeof(PrototypeIdListSerializer<GuideEntryPrototype>))]
		public List<string> Children = new List<string>();

		// Token: 0x0400097B RID: 2427
		[DataField("priority", false, 1, false, false, null)]
		public int Priority;
	}
}
