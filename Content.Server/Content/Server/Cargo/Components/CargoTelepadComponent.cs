using System;
using System.Runtime.CompilerServices;
using Content.Server.Cargo.Systems;
using Content.Shared.Cargo;
using Content.Shared.Cargo.Components;
using Content.Shared.MachineLinking;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Cargo.Components
{
	// Token: 0x020006E9 RID: 1769
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(CargoSystem)
	})]
	public sealed class CargoTelepadComponent : SharedCargoTelepadComponent
	{
		// Token: 0x040016BB RID: 5819
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 45f;

		// Token: 0x040016BC RID: 5820
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x040016BD RID: 5821
		[ViewVariables]
		public CargoTelepadState CurrentState;

		// Token: 0x040016BE RID: 5822
		[DataField("teleportSound", false, 1, false, false, null)]
		public SoundSpecifier TeleportSound = new SoundPathSpecifier("/Audio/Machines/phasein.ogg", null);

		// Token: 0x040016BF RID: 5823
		[DataField("printerOutput", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string PrinterOutput = "PaperCargoInvoice";

		// Token: 0x040016C0 RID: 5824
		[DataField("receiverPort", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string ReceiverPort = "OrderReceiver";
	}
}
