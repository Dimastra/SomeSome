using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments
{
	// Token: 0x020003E3 RID: 995
	[NetSerializable]
	[Serializable]
	public sealed class InstrumentStopMidiEvent : EntityEventArgs
	{
		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000BB2 RID: 2994 RVA: 0x000264C6 File Offset: 0x000246C6
		public EntityUid Uid { get; }

		// Token: 0x06000BB3 RID: 2995 RVA: 0x000264CE File Offset: 0x000246CE
		public InstrumentStopMidiEvent(EntityUid uid)
		{
			this.Uid = uid;
		}
	}
}
