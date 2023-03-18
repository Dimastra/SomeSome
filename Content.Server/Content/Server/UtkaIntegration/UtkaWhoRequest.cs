using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E7 RID: 231
	[NullableContext(2)]
	[Nullable(0)]
	public class UtkaWhoRequest : UtkaBaseMessage
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00015743 File Offset: 0x00013943
		[JsonPropertyName("command")]
		public override string Command
		{
			get
			{
				return "who";
			}
		}
	}
}
