using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store
{
	// Token: 0x02000123 RID: 291
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("storePreset", 1)]
	[DataDefinition]
	public sealed class StorePresetPrototype : IPrototype
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000360 RID: 864 RVA: 0x0000E6E9 File Offset: 0x0000C8E9
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0000E6F1 File Offset: 0x0000C8F1
		[DataField("storeName", false, 1, true, false, null)]
		public string StoreName { get; } = string.Empty;

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000362 RID: 866 RVA: 0x0000E6F9 File Offset: 0x0000C8F9
		[DataField("categories", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<StoreCategoryPrototype>))]
		public HashSet<string> Categories { get; } = new HashSet<string>();

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000363 RID: 867 RVA: 0x0000E701 File Offset: 0x0000C901
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("initialBalance", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, CurrencyPrototype>))]
		public Dictionary<string, FixedPoint2> InitialBalance { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000364 RID: 868 RVA: 0x0000E709 File Offset: 0x0000C909
		[DataField("currencyWhitelist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<CurrencyPrototype>))]
		public HashSet<string> CurrencyWhitelist { get; } = new HashSet<string>();
	}
}
