using System;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005BD RID: 1469
	public sealed class ServerRoleUnbanDef
	{
		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06001F66 RID: 8038 RVA: 0x000A5031 File Offset: 0x000A3231
		public int BanId { get; }

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06001F67 RID: 8039 RVA: 0x000A5039 File Offset: 0x000A3239
		public NetUserId? UnbanningAdmin { get; }

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06001F68 RID: 8040 RVA: 0x000A5041 File Offset: 0x000A3241
		public DateTimeOffset UnbanTime { get; }

		// Token: 0x06001F69 RID: 8041 RVA: 0x000A5049 File Offset: 0x000A3249
		public ServerRoleUnbanDef(int banId, NetUserId? unbanningAdmin, DateTimeOffset unbanTime)
		{
			this.BanId = banId;
			this.UnbanningAdmin = unbanningAdmin;
			this.UnbanTime = unbanTime;
		}
	}
}
