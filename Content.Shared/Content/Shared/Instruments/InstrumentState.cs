using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments
{
	// Token: 0x020003E6 RID: 998
	[NetSerializable]
	[Serializable]
	public sealed class InstrumentState : ComponentState
	{
		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x0002651A File Offset: 0x0002471A
		public bool Playing { get; }

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000BBA RID: 3002 RVA: 0x00026522 File Offset: 0x00024722
		public byte InstrumentProgram { get; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000BBB RID: 3003 RVA: 0x0002652A File Offset: 0x0002472A
		public byte InstrumentBank { get; }

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000BBC RID: 3004 RVA: 0x00026532 File Offset: 0x00024732
		public bool AllowPercussion { get; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000BBD RID: 3005 RVA: 0x0002653A File Offset: 0x0002473A
		public bool AllowProgramChange { get; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000BBE RID: 3006 RVA: 0x00026542 File Offset: 0x00024742
		public bool RespectMidiLimits { get; }

		// Token: 0x06000BBF RID: 3007 RVA: 0x0002654A File Offset: 0x0002474A
		public InstrumentState(bool playing, byte instrumentProgram, byte instrumentBank, bool allowPercussion, bool allowProgramChange, bool respectMidiLimits)
		{
			this.Playing = playing;
			this.InstrumentProgram = instrumentProgram;
			this.InstrumentBank = instrumentBank;
			this.AllowPercussion = allowPercussion;
			this.AllowProgramChange = allowProgramChange;
			this.RespectMidiLimits = respectMidiLimits;
		}
	}
}
