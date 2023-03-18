using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.VendingMachines
{
	// Token: 0x020000A0 RID: 160
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Prototype("vendingMachineInventory", 1)]
	[Serializable]
	public sealed class VendingMachineInventoryPrototype : IPrototype
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x00009B2D File Offset: 0x00007D2D
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x00009B35 File Offset: 0x00007D35
		[DataField("startingInventory", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
		public Dictionary<string, uint> StartingInventory { get; } = new Dictionary<string, uint>();

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x00009B3D File Offset: 0x00007D3D
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("emaggedInventory", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
		public Dictionary<string, uint> EmaggedInventory { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x00009B45 File Offset: 0x00007D45
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("contrabandInventory", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
		public Dictionary<string, uint> ContrabandInventory { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }
	}
}
