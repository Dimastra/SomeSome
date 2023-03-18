using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.VendingMachines;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Server.VendingMachines.Restock
{
	// Token: 0x020000D5 RID: 213
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class VendingMachineRestockComponent : Component
	{
		// Token: 0x0400025F RID: 607
		[ViewVariables]
		[DataField("restockDelay", false, 1, false, false, null)]
		public TimeSpan RestockDelay = TimeSpan.FromSeconds(8.0);

		// Token: 0x04000260 RID: 608
		[ViewVariables]
		[DataField("canRestock", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<VendingMachineInventoryPrototype>))]
		public HashSet<string> CanRestock = new HashSet<string>();

		// Token: 0x04000261 RID: 609
		[ViewVariables]
		[DataField("soundRestockStart", false, 1, false, false, null)]
		public SoundSpecifier SoundRestockStart = new SoundPathSpecifier("/Audio/Machines/vending_restock_start.ogg", null);

		// Token: 0x04000262 RID: 610
		[ViewVariables]
		[DataField("soundRestockDone", false, 1, false, false, null)]
		public SoundSpecifier SoundRestockDone = new SoundPathSpecifier("/Audio/Machines/vending_restock_done.ogg", null);
	}
}
