using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio
{
	// Token: 0x02000685 RID: 1669
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	[NetSerializable]
	[Serializable]
	public class GlobalSoundEvent : EntityEventArgs
	{
		// Token: 0x0600147E RID: 5246 RVA: 0x000443BC File Offset: 0x000425BC
		public GlobalSoundEvent(string filename, AudioParams? audioParams = null)
		{
			this.Filename = filename;
			this.AudioParams = audioParams;
		}

		// Token: 0x0400140C RID: 5132
		public string Filename;

		// Token: 0x0400140D RID: 5133
		public AudioParams? AudioParams;
	}
}
