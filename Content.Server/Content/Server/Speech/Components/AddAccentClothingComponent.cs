using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Speech.Components
{
	// Token: 0x020001C2 RID: 450
	[RegisterComponent]
	public sealed class AddAccentClothingComponent : Component
	{
		// Token: 0x04000550 RID: 1360
		[Nullable(1)]
		[DataField("accent", false, 1, true, false, null)]
		public string Accent;

		// Token: 0x04000551 RID: 1361
		[Nullable(2)]
		[DataField("replacement", false, 1, false, false, typeof(PrototypeIdSerializer<ReplacementAccentPrototype>))]
		public string ReplacementPrototype;

		// Token: 0x04000552 RID: 1362
		public bool IsActive;
	}
}
