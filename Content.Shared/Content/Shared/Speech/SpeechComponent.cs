using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Speech
{
	// Token: 0x0200017B RID: 379
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class SpeechComponent : Component
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x00011EAC File Offset: 0x000100AC
		// (set) Token: 0x0600048C RID: 1164 RVA: 0x00011EB4 File Offset: 0x000100B4
		[ViewVariables]
		[DataField("soundCooldownTime", false, 1, false, false, null)]
		public float SoundCooldownTime { get; set; } = 0.5f;

		// Token: 0x04000445 RID: 1093
		[DataField("enabled", false, 1, false, false, null)]
		[Access]
		public bool Enabled = true;

		// Token: 0x04000446 RID: 1094
		[Nullable(2)]
		[ViewVariables]
		[DataField("speechSounds", false, 1, false, false, typeof(PrototypeIdSerializer<SpeechSoundsPrototype>))]
		public string SpeechSounds;

		// Token: 0x04000447 RID: 1095
		[DataField("audioParams", false, 1, false, false, null)]
		public AudioParams AudioParams = AudioParams.Default.WithVolume(6f).WithRolloffFactor(4.5f);

		// Token: 0x04000449 RID: 1097
		public TimeSpan LastTimeSoundPlayed = TimeSpan.Zero;
	}
}
