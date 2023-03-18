using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Magic.Events
{
	// Token: 0x020003EC RID: 1004
	public sealed class TeleportSpellEvent : WorldTargetActionEvent
	{
		// Token: 0x04000CBB RID: 3259
		[Nullable(1)]
		[DataField("blinkSound", false, 1, false, false, null)]
		public SoundSpecifier BlinkSound = new SoundPathSpecifier("/Audio/Magic/blink.ogg", null);

		// Token: 0x04000CBC RID: 3260
		[DataField("blinkVolume", false, 1, false, false, null)]
		public float BlinkVolume = 5f;
	}
}
