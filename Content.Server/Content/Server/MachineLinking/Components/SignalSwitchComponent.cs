using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x02000402 RID: 1026
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SignalSwitchComponent : Component
	{
		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x0006D0C5 File Offset: 0x0006B2C5
		// (set) Token: 0x060014D3 RID: 5331 RVA: 0x0006D0CD File Offset: 0x0006B2CD
		[DataField("clickSound", false, 1, false, false, null)]
		public SoundSpecifier ClickSound { get; set; } = new SoundPathSpecifier("/Audio/Machines/lightswitch.ogg", null);

		// Token: 0x04000CE4 RID: 3300
		[DataField("onPort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string OnPort = "On";

		// Token: 0x04000CE5 RID: 3301
		[DataField("offPort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string OffPort = "Off";

		// Token: 0x04000CE6 RID: 3302
		[DataField("state", false, 1, false, false, null)]
		public bool State;
	}
}
