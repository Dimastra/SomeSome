using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.PDA.Ringer
{
	// Token: 0x02000292 RID: 658
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RingerUpdateState : BoundUserInterfaceState
	{
		// Token: 0x06000764 RID: 1892 RVA: 0x000191C5 File Offset: 0x000173C5
		public RingerUpdateState(bool isPlay, Note[] ringtone)
		{
			this.IsPlaying = isPlay;
			this.Ringtone = ringtone;
		}

		// Token: 0x0400077F RID: 1919
		public bool IsPlaying;

		// Token: 0x04000780 RID: 1920
		public Note[] Ringtone;
	}
}
