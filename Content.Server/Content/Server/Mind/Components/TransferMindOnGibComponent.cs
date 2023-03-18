using System;
using System.Runtime.CompilerServices;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Mind.Components
{
	// Token: 0x020003A7 RID: 935
	[RegisterComponent]
	public sealed class TransferMindOnGibComponent : Component
	{
		// Token: 0x04000BB3 RID: 2995
		[Nullable(1)]
		[DataField("targetTag", false, 1, false, false, typeof(PrototypeIdSerializer<TagPrototype>))]
		public string TargetTag = "MindTransferTarget";
	}
}
