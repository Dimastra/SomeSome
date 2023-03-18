using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Explosion.Components
{
	// Token: 0x02000513 RID: 1299
	[RegisterComponent]
	public sealed class ActiveTimerTriggerComponent : Component
	{
		// Token: 0x0400115D RID: 4445
		[DataField("timeRemaining", false, 1, false, false, null)]
		public float TimeRemaining;

		// Token: 0x0400115E RID: 4446
		[DataField("user", false, 1, false, false, null)]
		public EntityUid? User;

		// Token: 0x0400115F RID: 4447
		[DataField("beepInterval", false, 1, false, false, null)]
		public float BeepInterval;

		// Token: 0x04001160 RID: 4448
		[DataField("timeUntilBeep", false, 1, false, false, null)]
		public float TimeUntilBeep;

		// Token: 0x04001161 RID: 4449
		[Nullable(2)]
		[DataField("beepSound", false, 1, false, false, null)]
		public SoundSpecifier BeepSound;

		// Token: 0x04001162 RID: 4450
		[DataField("beepParams", false, 1, false, false, null)]
		public AudioParams BeepParams = AudioParams.Default;
	}
}
