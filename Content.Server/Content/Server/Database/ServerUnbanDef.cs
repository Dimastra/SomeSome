using System;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005BE RID: 1470
	public sealed class ServerUnbanDef
	{
		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06001F6A RID: 8042 RVA: 0x000A5066 File Offset: 0x000A3266
		public int BanId { get; }

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06001F6B RID: 8043 RVA: 0x000A506E File Offset: 0x000A326E
		public NetUserId? UnbanningAdmin { get; }

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06001F6C RID: 8044 RVA: 0x000A5076 File Offset: 0x000A3276
		public DateTimeOffset UnbanTime { get; }

		// Token: 0x06001F6D RID: 8045 RVA: 0x000A507E File Offset: 0x000A327E
		public ServerUnbanDef(int banId, NetUserId? unbanningAdmin, DateTimeOffset unbanTime)
		{
			this.BanId = banId;
			this.UnbanningAdmin = unbanningAdmin;
			this.UnbanTime = unbanTime;
		}
	}
}
