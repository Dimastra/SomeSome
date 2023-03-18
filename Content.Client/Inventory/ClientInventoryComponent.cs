using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Inventory
{
	// Token: 0x020002A3 RID: 675
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(InventoryComponent))]
	[Access(new Type[]
	{
		typeof(ClientInventorySystem)
	})]
	public sealed class ClientInventoryComponent : InventoryComponent
	{
		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x060010F7 RID: 4343 RVA: 0x00065557 File Offset: 0x00063757
		// (set) Token: 0x060010F8 RID: 4344 RVA: 0x0006555F File Offset: 0x0006375F
		[DataField("speciesId", false, 1, false, false, null)]
		public string SpeciesId { get; set; }

		// Token: 0x04000851 RID: 2129
		[Nullable(1)]
		[ViewVariables]
		public readonly Dictionary<string, ClientInventorySystem.SlotData> SlotData = new Dictionary<string, ClientInventorySystem.SlotData>();

		// Token: 0x04000852 RID: 2130
		[Nullable(1)]
		[ViewVariables]
		[Access]
		public readonly Dictionary<string, HashSet<string>> VisualLayerKeys = new Dictionary<string, HashSet<string>>();
	}
}
