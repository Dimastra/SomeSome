using System;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005B4 RID: 1460
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayerRecord
	{
		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06001E54 RID: 7764 RVA: 0x000A10B8 File Offset: 0x0009F2B8
		public NetUserId UserId { get; }

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06001E55 RID: 7765 RVA: 0x000A10C0 File Offset: 0x0009F2C0
		[Nullable(0)]
		public ImmutableArray<byte>? HWId { [NullableContext(0)] get; }

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06001E56 RID: 7766 RVA: 0x000A10C8 File Offset: 0x0009F2C8
		public DateTimeOffset FirstSeenTime { get; }

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06001E57 RID: 7767 RVA: 0x000A10D0 File Offset: 0x0009F2D0
		public string LastSeenUserName { get; }

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06001E58 RID: 7768 RVA: 0x000A10D8 File Offset: 0x0009F2D8
		public DateTimeOffset LastSeenTime { get; }

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06001E59 RID: 7769 RVA: 0x000A10E0 File Offset: 0x0009F2E0
		public IPAddress LastSeenAddress { get; }

		// Token: 0x06001E5A RID: 7770 RVA: 0x000A10E8 File Offset: 0x0009F2E8
		public PlayerRecord(NetUserId userId, DateTimeOffset firstSeenTime, string lastSeenUserName, DateTimeOffset lastSeenTime, IPAddress lastSeenAddress, [Nullable(0)] ImmutableArray<byte>? hwId)
		{
			this.UserId = userId;
			this.FirstSeenTime = firstSeenTime;
			this.LastSeenUserName = lastSeenUserName;
			this.LastSeenTime = lastSeenTime;
			this.LastSeenAddress = lastSeenAddress;
			this.HWId = hwId;
		}
	}
}
