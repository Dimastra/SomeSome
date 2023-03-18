using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E6 RID: 230
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaPmRequest : UtkaBaseMessage
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x00015712 File Offset: 0x00013912
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "pm";
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000435 RID: 1077 RVA: 0x00015719 File Offset: 0x00013919
		// (set) Token: 0x06000436 RID: 1078 RVA: 0x00015721 File Offset: 0x00013921
		[JsonPropertyName("name")]
		public string Name { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000437 RID: 1079 RVA: 0x0001572A File Offset: 0x0001392A
		// (set) Token: 0x06000438 RID: 1080 RVA: 0x00015732 File Offset: 0x00013932
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
