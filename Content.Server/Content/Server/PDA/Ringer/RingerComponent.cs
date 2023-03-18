using System;
using System.Runtime.CompilerServices;
using Content.Shared.PDA;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.PDA.Ringer
{
	// Token: 0x020002DF RID: 735
	[RegisterComponent]
	public sealed class RingerComponent : Component
	{
		// Token: 0x040008D5 RID: 2261
		[Nullable(1)]
		[DataField("ringtone", false, 1, false, false, null)]
		public Note[] Ringtone = new Note[4];

		// Token: 0x040008D6 RID: 2262
		[DataField("timeElapsed", false, 1, false, false, null)]
		public float TimeElapsed;

		// Token: 0x040008D7 RID: 2263
		[DataField("noteCount", false, 1, false, false, null)]
		public int NoteCount;

		// Token: 0x040008D8 RID: 2264
		[ViewVariables]
		[DataField("range", false, 1, false, false, null)]
		public float Range = 3f;

		// Token: 0x040008D9 RID: 2265
		[ViewVariables]
		[DataField("volume", false, 1, false, false, null)]
		public float Volume = -4f;
	}
}
