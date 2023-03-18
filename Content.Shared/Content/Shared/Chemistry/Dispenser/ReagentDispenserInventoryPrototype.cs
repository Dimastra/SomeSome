using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Dispenser
{
	// Token: 0x020005F6 RID: 1526
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Prototype("reagentDispenserInventory", 1)]
	[Serializable]
	public sealed class ReagentDispenserInventoryPrototype : IPrototype
	{
		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x0600128C RID: 4748 RVA: 0x0003CB6A File Offset: 0x0003AD6A
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x0600128D RID: 4749 RVA: 0x0003CB72 File Offset: 0x0003AD72
		public List<string> Inventory
		{
			get
			{
				return this._inventory;
			}
		}

		// Token: 0x0400114E RID: 4430
		[DataField("inventory", false, 1, false, false, typeof(PrototypeIdListSerializer<ReagentPrototype>))]
		private List<string> _inventory = new List<string>();
	}
}
