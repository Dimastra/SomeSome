using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.MachineLinking
{
	// Token: 0x02000349 RID: 841
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("transmitterPort", 1)]
	public sealed class TransmitterPortPrototype : MachinePortPrototype, IPrototype
	{
		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x060009E1 RID: 2529 RVA: 0x0002067C File Offset: 0x0001E87C
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x0400099A RID: 2458
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("defaultLinks", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<ReceiverPortPrototype>))]
		public HashSet<string> DefaultLinks;
	}
}
