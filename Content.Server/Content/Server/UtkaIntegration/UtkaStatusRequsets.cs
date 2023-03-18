using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000EB RID: 235
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaStatusRequsets : UtkaBaseMessage
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x000157A1 File Offset: 0x000139A1
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "status";
			}
		}
	}
}
