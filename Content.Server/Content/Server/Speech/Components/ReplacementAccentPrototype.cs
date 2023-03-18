using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Speech.Components
{
	// Token: 0x020001C8 RID: 456
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("accent", 1)]
	public sealed class ReplacementAccentPrototype : IPrototype
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060008D2 RID: 2258 RVA: 0x0002D10B File Offset: 0x0002B30B
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000555 RID: 1365
		[DataField("words", false, 1, false, false, null)]
		public string[] Words;
	}
}
