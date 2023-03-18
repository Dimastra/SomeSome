using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Magic.Events
{
	// Token: 0x020003E9 RID: 1001
	public sealed class KnockSpellEvent : InstantActionEvent
	{
		// Token: 0x04000CB5 RID: 3253
		[DataField("range", false, 1, false, false, null)]
		public float Range = 4f;

		// Token: 0x04000CB6 RID: 3254
		[Nullable(1)]
		[DataField("knockSound", false, 1, false, false, null)]
		public SoundSpecifier KnockSound = new SoundPathSpecifier("/Audio/Magic/knock.ogg", null);

		// Token: 0x04000CB7 RID: 3255
		[DataField("knockVolume", false, 1, false, false, null)]
		public float KnockVolume = 5f;
	}
}
