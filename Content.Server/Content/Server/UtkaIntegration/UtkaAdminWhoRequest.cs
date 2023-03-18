using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E9 RID: 233
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaAdminWhoRequest : UtkaBaseMessage
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00015772 File Offset: 0x00013972
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "adminwho";
			}
		}
	}
}
