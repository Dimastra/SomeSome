using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002E7 RID: 743
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class FootstepModifierComponent : Component
	{
		// Token: 0x0400086C RID: 2156
		[Nullable(1)]
		[DataField("footstepSoundCollection", false, 1, true, false, null)]
		public SoundSpecifier Sound;
	}
}
