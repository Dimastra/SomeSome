using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio
{
	// Token: 0x02000689 RID: 1673
	[NetSerializable]
	[Serializable]
	public sealed class StationEventMusicEvent : GlobalSoundEvent
	{
		// Token: 0x06001481 RID: 5249 RVA: 0x000443E6 File Offset: 0x000425E6
		[NullableContext(1)]
		public StationEventMusicEvent(string filename, StationEventMusicType type, AudioParams? audioParams = null) : base(filename, audioParams)
		{
			this.Type = type;
		}

		// Token: 0x04001410 RID: 5136
		public StationEventMusicType Type;
	}
}
