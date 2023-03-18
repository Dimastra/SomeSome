using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Tag
{
	// Token: 0x020000EB RID: 235
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("Tag", 1)]
	public sealed class TagPrototype : IPrototype
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000C2B5 File Offset: 0x0000A4B5
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
