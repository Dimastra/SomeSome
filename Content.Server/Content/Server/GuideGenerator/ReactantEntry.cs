using System;
using System.Text.Json.Serialization;

namespace Content.Server.GuideGenerator
{
	// Token: 0x0200047F RID: 1151
	public sealed class ReactantEntry
	{
		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060016FF RID: 5887 RVA: 0x00079215 File Offset: 0x00077415
		[JsonPropertyName("amount")]
		public float Amount { get; }

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06001700 RID: 5888 RVA: 0x0007921D File Offset: 0x0007741D
		[JsonPropertyName("catalyst")]
		public bool Catalyst { get; }

		// Token: 0x06001701 RID: 5889 RVA: 0x00079225 File Offset: 0x00077425
		public ReactantEntry(float amnt, bool cata)
		{
			this.Amount = amnt;
			this.Catalyst = cata;
		}
	}
}
