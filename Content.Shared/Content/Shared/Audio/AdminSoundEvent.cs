using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio
{
	// Token: 0x02000686 RID: 1670
	[NetSerializable]
	[Serializable]
	public sealed class AdminSoundEvent : GlobalSoundEvent
	{
		// Token: 0x0600147F RID: 5247 RVA: 0x000443D2 File Offset: 0x000425D2
		[NullableContext(1)]
		public AdminSoundEvent(string filename, AudioParams? audioParams = null) : base(filename, audioParams)
		{
		}
	}
}
