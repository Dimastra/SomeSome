using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Explosion.Components
{
	// Token: 0x0200051B RID: 1307
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class OnUseTimerTriggerComponent : Component
	{
		// Token: 0x04001176 RID: 4470
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 1f;

		// Token: 0x04001177 RID: 4471
		[DataField("delayOptions", false, 1, false, false, null)]
		public List<float> DelayOptions;

		// Token: 0x04001178 RID: 4472
		[DataField("beepSound", false, 1, false, false, null)]
		public SoundSpecifier BeepSound;

		// Token: 0x04001179 RID: 4473
		[DataField("initialBeepDelay", false, 1, false, false, null)]
		public float? InitialBeepDelay;

		// Token: 0x0400117A RID: 4474
		[DataField("beepInterval", false, 1, false, false, null)]
		public float BeepInterval = 1f;

		// Token: 0x0400117B RID: 4475
		[DataField("beepParams", false, 1, false, false, null)]
		public AudioParams BeepParams = AudioParams.Default.WithVolume(-2f);

		// Token: 0x0400117C RID: 4476
		[DataField("startOnStick", false, 1, false, false, null)]
		public bool StartOnStick;

		// Token: 0x0400117D RID: 4477
		[DataField("canToggleStartOnStick", false, 1, false, false, null)]
		public bool AllowToggleStartOnStick;
	}
}
