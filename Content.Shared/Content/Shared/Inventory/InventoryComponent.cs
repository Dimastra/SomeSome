using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Inventory
{
	// Token: 0x020003AB RID: 939
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class InventoryComponent : Component
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000AC1 RID: 2753 RVA: 0x0002312D File Offset: 0x0002132D
		[DataField("templateId", false, 1, false, false, typeof(PrototypeIdSerializer<InventoryTemplatePrototype>))]
		public string TemplateId { get; } = "human";
	}
}
