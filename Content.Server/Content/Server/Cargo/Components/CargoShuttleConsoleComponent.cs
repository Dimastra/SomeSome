using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Cargo.Components
{
	// Token: 0x020006E8 RID: 1768
	[RegisterComponent]
	public sealed class CargoShuttleConsoleComponent : Component
	{
		// Token: 0x040016BA RID: 5818
		[Nullable(1)]
		[ViewVariables]
		[DataField("soundDeny", false, 1, false, false, null)]
		public SoundSpecifier DenySound = new SoundPathSpecifier("/Audio/Effects/Cargo/buzz_two.ogg", null);
	}
}
