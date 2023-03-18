using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Speech.Components
{
	// Token: 0x020001C9 RID: 457
	[RegisterComponent]
	public sealed class ReplacementAccentComponent : Component
	{
		// Token: 0x04000556 RID: 1366
		[Nullable(1)]
		[DataField("accent", false, 1, true, false, typeof(PrototypeIdSerializer<ReplacementAccentPrototype>))]
		public string Accent;
	}
}
