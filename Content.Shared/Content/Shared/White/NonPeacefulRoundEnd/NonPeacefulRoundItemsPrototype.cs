using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.White.NonPeacefulRoundEnd
{
	// Token: 0x02000035 RID: 53
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("nonPeacefulRoundEndItems", 1)]
	public sealed class NonPeacefulRoundItemsPrototype : IPrototype
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00002D72 File Offset: 0x00000F72
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00002D7A File Offset: 0x00000F7A
		[DataField("items", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> Items { get; }
	}
}
