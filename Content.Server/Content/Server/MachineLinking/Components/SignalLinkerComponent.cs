using System;
using System.Runtime.CompilerServices;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x02000400 RID: 1024
	[RegisterComponent]
	public sealed class SignalLinkerComponent : Component
	{
		// Token: 0x04000CE0 RID: 3296
		[ViewVariables]
		public EntityUid? SavedTransmitter;

		// Token: 0x04000CE1 RID: 3297
		[ViewVariables]
		public EntityUid? SavedReceiver;

		// Token: 0x04000CE2 RID: 3298
		[Nullable(2)]
		[DataField("requiredQuality", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		[ViewVariables]
		public string RequiredQuality;
	}
}
