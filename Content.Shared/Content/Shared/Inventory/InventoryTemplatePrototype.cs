using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Inventory
{
	// Token: 0x020003AF RID: 943
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("inventoryTemplate", 1)]
	public sealed class InventoryTemplatePrototype : IPrototype
	{
		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x000242FD File Offset: 0x000224FD
		[IdDataField(1, null)]
		public string ID { get; } = string.Empty;

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x00024305 File Offset: 0x00022505
		[DataField("slots", false, 1, false, false, null)]
		public SlotDefinition[] Slots { get; } = Array.Empty<SlotDefinition>();
	}
}
