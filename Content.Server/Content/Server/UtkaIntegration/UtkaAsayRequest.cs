using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E5 RID: 229
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaAsayRequest : UtkaBaseMessage
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600042E RID: 1070 RVA: 0x000156E1 File Offset: 0x000138E1
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "asay";
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x000156E8 File Offset: 0x000138E8
		// (set) Token: 0x06000430 RID: 1072 RVA: 0x000156F0 File Offset: 0x000138F0
		[JsonPropertyName("a_ckey")]
		public string ACkey { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x000156F9 File Offset: 0x000138F9
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x00015701 File Offset: 0x00013901
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
