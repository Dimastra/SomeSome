using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires
{
	// Token: 0x02000027 RID: 39
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class WiresBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002515 File Offset: 0x00000715
		public string BoardName { get; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000251D File Offset: 0x0000071D
		[Nullable(2)]
		public string SerialNumber { [NullableContext(2)] get; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002525 File Offset: 0x00000725
		public ClientWire[] WiresList { get; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000030 RID: 48 RVA: 0x0000252D File Offset: 0x0000072D
		public StatusEntry[] Statuses { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002535 File Offset: 0x00000735
		public int WireSeed { get; }

		// Token: 0x06000032 RID: 50 RVA: 0x0000253D File Offset: 0x0000073D
		public WiresBoundUserInterfaceState(ClientWire[] wiresList, StatusEntry[] statuses, string boardName, [Nullable(2)] string serialNumber, int wireSeed)
		{
			this.BoardName = boardName;
			this.SerialNumber = serialNumber;
			this.WireSeed = wireSeed;
			this.WiresList = wiresList;
			this.Statuses = statuses;
		}
	}
}
