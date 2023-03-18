using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio
{
	// Token: 0x02000687 RID: 1671
	[NetSerializable]
	[Serializable]
	public sealed class GameGlobalSoundEvent : GlobalSoundEvent
	{
		// Token: 0x06001480 RID: 5248 RVA: 0x000443DC File Offset: 0x000425DC
		[NullableContext(1)]
		public GameGlobalSoundEvent(string filename, AudioParams? audioParams = null) : base(filename, audioParams)
		{
		}
	}
}
