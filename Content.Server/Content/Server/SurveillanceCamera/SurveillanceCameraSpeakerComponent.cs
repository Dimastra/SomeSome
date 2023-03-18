using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x02000140 RID: 320
	[RegisterComponent]
	public sealed class SurveillanceCameraSpeakerComponent : Component
	{
		// Token: 0x04000383 RID: 899
		[DataField("speechEnabled", false, 1, false, false, null)]
		public bool SpeechEnabled = true;

		// Token: 0x04000384 RID: 900
		[ViewVariables]
		public float SpeechSoundCooldown = 0.5f;

		// Token: 0x04000385 RID: 901
		public TimeSpan LastSoundPlayed = TimeSpan.Zero;
	}
}
