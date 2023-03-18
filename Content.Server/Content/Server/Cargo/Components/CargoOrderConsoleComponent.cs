using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Cargo.Components
{
	// Token: 0x020006E4 RID: 1764
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class CargoOrderConsoleComponent : Component
	{
		// Token: 0x040016B7 RID: 5815
		[DataField("soundError", false, 1, false, false, null)]
		public SoundSpecifier ErrorSound = new SoundPathSpecifier("/Audio/Effects/Cargo/buzz_sigh.ogg", null);

		// Token: 0x040016B8 RID: 5816
		[DataField("soundConfirm", false, 1, false, false, null)]
		public SoundSpecifier ConfirmSound = new SoundPathSpecifier("/Audio/Effects/Cargo/ping.ogg", null);
	}
}
