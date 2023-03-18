using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Server.White.Stalin
{
	// Token: 0x0200008E RID: 142
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiscordUsersData
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000221 RID: 545 RVA: 0x0000BEEA File Offset: 0x0000A0EA
		// (set) Token: 0x06000222 RID: 546 RVA: 0x0000BEF2 File Offset: 0x0000A0F2
		public Dictionary<string, DiscordUserData> Users { get; set; } = new Dictionary<string, DiscordUserData>();
	}
}
