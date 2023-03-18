using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E8 RID: 232
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaWhoResponse : UtkaBaseMessage
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00015752 File Offset: 0x00013952
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "who";
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x00015759 File Offset: 0x00013959
		// (set) Token: 0x0600043E RID: 1086 RVA: 0x00015761 File Offset: 0x00013961
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
	}
}
