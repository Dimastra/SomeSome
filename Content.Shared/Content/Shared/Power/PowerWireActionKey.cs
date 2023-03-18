using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power
{
	// Token: 0x02000255 RID: 597
	[NetSerializable]
	[Serializable]
	public enum PowerWireActionKey : byte
	{
		// Token: 0x040006B4 RID: 1716
		Key,
		// Token: 0x040006B5 RID: 1717
		Status,
		// Token: 0x040006B6 RID: 1718
		Pulsed,
		// Token: 0x040006B7 RID: 1719
		Electrified,
		// Token: 0x040006B8 RID: 1720
		PulseCancel,
		// Token: 0x040006B9 RID: 1721
		ElectrifiedCancel,
		// Token: 0x040006BA RID: 1722
		MainWire,
		// Token: 0x040006BB RID: 1723
		WireCount,
		// Token: 0x040006BC RID: 1724
		CutWires
	}
}
