using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Stacks
{
	// Token: 0x0200016E RID: 366
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("stack", 1)]
	public sealed class StackPrototype : IPrototype
	{
		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000477 RID: 1143 RVA: 0x00011D48 File Offset: 0x0000FF48
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x00011D50 File Offset: 0x0000FF50
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000479 RID: 1145 RVA: 0x00011D58 File Offset: 0x0000FF58
		[Nullable(2)]
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon { [NullableContext(2)] get; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x00011D60 File Offset: 0x0000FF60
		[DataField("spawn", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Spawn { get; } = string.Empty;

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600047B RID: 1147 RVA: 0x00011D68 File Offset: 0x0000FF68
		[DataField("maxCount", false, 1, false, false, null)]
		public int? MaxCount { get; }
	}
}
