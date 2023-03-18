using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000EC RID: 236
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaStatusResponse : UtkaBaseMessage
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x000157B0 File Offset: 0x000139B0
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "status";
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x000157B7 File Offset: 0x000139B7
		// (set) Token: 0x0600044A RID: 1098 RVA: 0x000157BF File Offset: 0x000139BF
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[JsonPropertyName("players")]
		public List<string> Players { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x000157C8 File Offset: 0x000139C8
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x000157D0 File Offset: 0x000139D0
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[JsonPropertyName("admins")]
		public List<string> Admins { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600044D RID: 1101 RVA: 0x000157D9 File Offset: 0x000139D9
		// (set) Token: 0x0600044E RID: 1102 RVA: 0x000157E1 File Offset: 0x000139E1
		[JsonPropertyName("map")]
		public string Map { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600044F RID: 1103 RVA: 0x000157EA File Offset: 0x000139EA
		// (set) Token: 0x06000450 RID: 1104 RVA: 0x000157F2 File Offset: 0x000139F2
		[JsonPropertyName("round_duration")]
		public double RoundDuration { get; set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x000157FB File Offset: 0x000139FB
		// (set) Token: 0x06000452 RID: 1106 RVA: 0x00015803 File Offset: 0x00013A03
		[JsonPropertyName("shuttle_status")]
		public string ShuttleStatus { get; set; }
	}
}
