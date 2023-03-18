using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000EE RID: 238
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class UtkaChatEventMessage : UtkaBaseMessage
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x00015834 File Offset: 0x00013A34
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x0001583C File Offset: 0x00013A3C
		[JsonPropertyName("ckey")]
		public string Ckey { get; set; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x00015845 File Offset: 0x00013A45
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x0001584D File Offset: 0x00013A4D
		[JsonPropertyName("message")]
		public string Message { get; set; }
	}
}
