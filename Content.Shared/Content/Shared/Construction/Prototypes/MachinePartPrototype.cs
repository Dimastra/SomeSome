using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Construction.Prototypes
{
	// Token: 0x0200057C RID: 1404
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("machinePart", 1)]
	public sealed class MachinePartPrototype : IPrototype
	{
		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06001140 RID: 4416 RVA: 0x00038C29 File Offset: 0x00036E29
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06001141 RID: 4417 RVA: 0x00038C31 File Offset: 0x00036E31
		[Nullable(2)]
		[DataField("stockPartPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string StockPartPrototype { [NullableContext(2)] get; }
	}
}
