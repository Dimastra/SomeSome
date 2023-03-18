using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.White.Stalin
{
	// Token: 0x0200008D RID: 141
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiscordUsersDataRequest
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600021E RID: 542 RVA: 0x0000BEC6 File Offset: 0x0000A0C6
		// (set) Token: 0x0600021F RID: 543 RVA: 0x0000BECE File Offset: 0x0000A0CE
		[JsonPropertyName("uuids")]
		public List<string> Uids { get; set; } = new List<string>();
	}
}
