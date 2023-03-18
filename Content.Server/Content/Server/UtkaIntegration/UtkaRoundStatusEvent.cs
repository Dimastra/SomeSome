using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000EF RID: 239
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class UtkaRoundStatusEvent : UtkaBaseMessage
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600045D RID: 1117 RVA: 0x0001585E File Offset: 0x00013A5E
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "roundstatus";
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x00015865 File Offset: 0x00013A65
		// (set) Token: 0x0600045F RID: 1119 RVA: 0x0001586D File Offset: 0x00013A6D
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
