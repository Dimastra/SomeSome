using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments
{
	// Token: 0x020003E4 RID: 996
	[NetSerializable]
	[Serializable]
	public sealed class InstrumentStartMidiEvent : EntityEventArgs
	{
		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000BB4 RID: 2996 RVA: 0x000264DD File Offset: 0x000246DD
		public EntityUid Uid { get; }

		// Token: 0x06000BB5 RID: 2997 RVA: 0x000264E5 File Offset: 0x000246E5
		public InstrumentStartMidiEvent(EntityUid uid)
		{
			this.Uid = uid;
		}
	}
}
