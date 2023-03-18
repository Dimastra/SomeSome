using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E3 RID: 227
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaHandshakeMessage : UtkaBaseMessage
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x0001567F File Offset: 0x0001387F
		[Nullable(1)]
		[JsonPropertyName("command")]
		public override string Command
		{
			[NullableContext(1)]
			get
			{
				return "handshake";
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x00015686 File Offset: 0x00013886
		// (set) Token: 0x06000424 RID: 1060 RVA: 0x0001568E File Offset: 0x0001388E
		[JsonPropertyName("key")]
		public string Key { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000425 RID: 1061 RVA: 0x00015697 File Offset: 0x00013897
		// (set) Token: 0x06000426 RID: 1062 RVA: 0x0001569F File Offset: 0x0001389F
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
