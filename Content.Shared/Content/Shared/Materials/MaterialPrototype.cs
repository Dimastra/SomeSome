using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Materials
{
	// Token: 0x02000330 RID: 816
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("material", 1)]
	public sealed class MaterialPrototype : IPrototype, IInheritingPrototype
	{
		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x0001F6E5 File Offset: 0x0001D8E5
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ViewVariables]
		[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<MaterialPrototype>), 1)]
		public string[] Parents { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x0001F6ED File Offset: 0x0001D8ED
		[ViewVariables]
		[AbstractDataField(1)]
		public bool Abstract { get; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x0600095E RID: 2398 RVA: 0x0001F6F5 File Offset: 0x0001D8F5
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x0001F6FD File Offset: 0x0001D8FD
		[DataField("stackEntity", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string StackEntity { get; } = "";

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x0001F705 File Offset: 0x0001D905
		[DataField("color", false, 1, false, false, null)]
		public Color Color { get; } = Color.Gray;

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000961 RID: 2401 RVA: 0x0001F70D File Offset: 0x0001D90D
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon { get; } = SpriteSpecifier.Invalid;

		// Token: 0x0400094C RID: 2380
		[DataField("name", false, 1, false, false, null)]
		public string Name = "";

		// Token: 0x0400094F RID: 2383
		[DataField("price", false, 1, true, false, null)]
		public double Price;
	}
}
