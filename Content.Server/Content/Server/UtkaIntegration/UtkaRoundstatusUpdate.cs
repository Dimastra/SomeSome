using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000ED RID: 237
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaRoundstatusUpdate : UtkaBaseMessage
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x00015814 File Offset: 0x00013A14
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "roundstatus";
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000455 RID: 1109 RVA: 0x0001581B File Offset: 0x00013A1B
		// (set) Token: 0x06000456 RID: 1110 RVA: 0x00015823 File Offset: 0x00013A23
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
