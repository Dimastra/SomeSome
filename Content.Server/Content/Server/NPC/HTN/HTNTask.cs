using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;

namespace Content.Server.NPC.HTN
{
	// Token: 0x0200034A RID: 842
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class HTNTask : IPrototype
	{
		// Token: 0x1700027D RID: 637
		// (get) Token: 0x060011A1 RID: 4513 RVA: 0x0005D339 File Offset: 0x0005B539
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
