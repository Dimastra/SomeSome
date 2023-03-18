using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005F3 RID: 1523
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("reactiveGroup", 1)]
	public sealed class ReactiveGroupPrototype : IPrototype
	{
		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x0600127D RID: 4733 RVA: 0x0003C2CA File Offset: 0x0003A4CA
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
