using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio.Midi;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments
{
	// Token: 0x020003E5 RID: 997
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class InstrumentMidiEventEvent : EntityEventArgs
	{
		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000BB6 RID: 2998 RVA: 0x000264F4 File Offset: 0x000246F4
		public EntityUid Uid { get; }

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000BB7 RID: 2999 RVA: 0x000264FC File Offset: 0x000246FC
		public RobustMidiEvent[] MidiEvent { get; }

		// Token: 0x06000BB8 RID: 3000 RVA: 0x00026504 File Offset: 0x00024704
		public InstrumentMidiEventEvent(EntityUid uid, RobustMidiEvent[] midiEvent)
		{
			this.Uid = uid;
			this.MidiEvent = midiEvent;
		}
	}
}
