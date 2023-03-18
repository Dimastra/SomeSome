using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Dispenser;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006A9 RID: 1705
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ReagentDispenserSystem)
	})]
	public sealed class ReagentDispenserComponent : Component
	{
		// Token: 0x040015ED RID: 5613
		[DataField("pack", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentDispenserInventoryPrototype>))]
		[ViewVariables]
		public string PackPrototypeId;

		// Token: 0x040015EE RID: 5614
		[DataField("emagPack", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentDispenserInventoryPrototype>))]
		[ViewVariables]
		public string EmagPackPrototypeId;

		// Token: 0x040015EF RID: 5615
		[Nullable(1)]
		[DataField("clickSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier ClickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null);

		// Token: 0x040015F0 RID: 5616
		[ViewVariables]
		public ReagentDispenserDispenseAmount DispenseAmount = ReagentDispenserDispenseAmount.U10;
	}
}
