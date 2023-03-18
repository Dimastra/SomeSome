using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Mining
{
	// Token: 0x02000307 RID: 775
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("ore", 1)]
	public sealed class OrePrototype : IPrototype
	{
		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060008EE RID: 2286 RVA: 0x0001E311 File Offset: 0x0001C511
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x040008D7 RID: 2263
		[Nullable(2)]
		[DataField("oreEntity", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string OreEntity;

		// Token: 0x040008D8 RID: 2264
		[DataField("minOreYield", false, 1, false, false, null)]
		public int MinOreYield = 1;

		// Token: 0x040008D9 RID: 2265
		[DataField("maxOreYield", false, 1, false, false, null)]
		public int MaxOreYield = 1;
	}
}
