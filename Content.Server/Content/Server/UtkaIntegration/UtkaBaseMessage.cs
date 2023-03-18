using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E2 RID: 226
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaBaseMessage
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x00015666 File Offset: 0x00013866
		// (set) Token: 0x06000420 RID: 1056 RVA: 0x0001566E File Offset: 0x0001386E
		[JsonPropertyName("command")]
		public virtual string Command { get; set; }
	}
}
