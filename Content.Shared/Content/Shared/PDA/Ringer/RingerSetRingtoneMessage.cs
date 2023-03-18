using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.PDA.Ringer
{
	// Token: 0x02000291 RID: 657
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RingerSetRingtoneMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x000191AE File Offset: 0x000173AE
		public Note[] Ringtone { get; }

		// Token: 0x06000763 RID: 1891 RVA: 0x000191B6 File Offset: 0x000173B6
		public RingerSetRingtoneMessage(Note[] ringTone)
		{
			this.Ringtone = ringTone;
		}
	}
}
