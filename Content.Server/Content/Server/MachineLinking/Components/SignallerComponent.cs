using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x020003FF RID: 1023
	[RegisterComponent]
	public sealed class SignallerComponent : Component
	{
		// Token: 0x04000CDF RID: 3295
		[Nullable(1)]
		[DataField("port", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string Port = "Pressed";
	}
}
