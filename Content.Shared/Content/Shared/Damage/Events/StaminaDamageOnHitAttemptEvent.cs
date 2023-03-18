using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage.Events
{
	// Token: 0x0200053F RID: 1343
	[NullableContext(2)]
	[Nullable(0)]
	[ByRefEvent]
	public struct StaminaDamageOnHitAttemptEvent
	{
		// Token: 0x06001067 RID: 4199 RVA: 0x00035CC4 File Offset: 0x00033EC4
		public StaminaDamageOnHitAttemptEvent(bool cancelled, SoundSpecifier hitSoundOverride)
		{
			this.Cancelled = cancelled;
			this.HitSoundOverride = hitSoundOverride;
		}

		// Token: 0x04000F67 RID: 3943
		public bool Cancelled;

		// Token: 0x04000F68 RID: 3944
		public SoundSpecifier HitSoundOverride;
	}
}
