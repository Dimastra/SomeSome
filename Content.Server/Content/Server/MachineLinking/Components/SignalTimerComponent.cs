using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Content.Shared.Radio;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x02000403 RID: 1027
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SignalTimerComponent : Component
	{
		// Token: 0x04000CE8 RID: 3304
		[DataField("delay", false, 1, false, false, null)]
		[ViewVariables]
		public double Delay = 5.0;

		// Token: 0x04000CE9 RID: 3305
		[DataField("user", false, 1, false, false, null)]
		public EntityUid? User;

		// Token: 0x04000CEA RID: 3306
		[DataField("canEditLabel", false, 1, false, false, null)]
		[ViewVariables]
		public bool CanEditLabel = true;

		// Token: 0x04000CEB RID: 3307
		[DataField("timerCanAnnounce", false, 1, false, false, null)]
		[ViewVariables]
		public bool TimerCanAnnounce;

		// Token: 0x04000CEC RID: 3308
		[DataField("label", false, 1, false, false, null)]
		public string Label = "";

		// Token: 0x04000CED RID: 3309
		[DataField("triggerPort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string TriggerPort = "Timer";

		// Token: 0x04000CEE RID: 3310
		[DataField("startPort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public string StartPort = "Start";

		// Token: 0x04000CEF RID: 3311
		[DataField("SecChannel", false, 1, false, false, typeof(PrototypeIdSerializer<RadioChannelPrototype>))]
		public static string SecChannel = "Security";

		// Token: 0x04000CF0 RID: 3312
		[Nullable(2)]
		[DataField("doneSound", false, 1, false, false, null)]
		public SoundSpecifier DoneSound = new SoundPathSpecifier("/Audio/Machines/doneSound.ogg", null);

		// Token: 0x04000CF1 RID: 3313
		[DataField("soundParams", false, 1, false, false, null)]
		public AudioParams SoundParams = AudioParams.Default.WithVolume(-2f);
	}
}
