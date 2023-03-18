using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E4 RID: 228
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaOOCRequest : UtkaBaseMessage
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000428 RID: 1064 RVA: 0x000156B0 File Offset: 0x000138B0
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "ooc";
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x000156B7 File Offset: 0x000138B7
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x000156BF File Offset: 0x000138BF
		[JsonPropertyName("ckey")]
		public string CKey { get; set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x000156C8 File Offset: 0x000138C8
		// (set) Token: 0x0600042C RID: 1068 RVA: 0x000156D0 File Offset: 0x000138D0
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
