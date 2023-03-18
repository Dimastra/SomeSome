using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000EA RID: 234
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaAdminWhoResponse : UtkaBaseMessage
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00015781 File Offset: 0x00013981
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "adminwho";
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x00015788 File Offset: 0x00013988
		// (set) Token: 0x06000444 RID: 1092 RVA: 0x00015790 File Offset: 0x00013990
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[JsonPropertyName("admins")]
		public List<string> Admins { [return: Nullable(new byte[]
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
