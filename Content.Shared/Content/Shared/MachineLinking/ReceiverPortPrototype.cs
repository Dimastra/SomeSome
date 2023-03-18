using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;

namespace Content.Shared.MachineLinking
{
	// Token: 0x02000348 RID: 840
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("receiverPort", 1)]
	public sealed class ReceiverPortPrototype : MachinePortPrototype, IPrototype
	{
		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x060009DF RID: 2527 RVA: 0x0002066C File Offset: 0x0001E86C
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
