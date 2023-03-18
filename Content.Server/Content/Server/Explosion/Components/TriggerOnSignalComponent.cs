using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Explosion.Components
{
	// Token: 0x02000520 RID: 1312
	[RegisterComponent]
	public sealed class TriggerOnSignalComponent : Component
	{
		// Token: 0x0400118C RID: 4492
		[Nullable(1)]
		[DataField("port", false, 1, false, false, typeof(PrototypeIdSerializer<ReceiverPortPrototype>))]
		public string Port = "Trigger";
	}
}
