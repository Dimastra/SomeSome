using System;
using System.Text.Json.Serialization;

namespace Content.Server.White.Stalin
{
	// Token: 0x0200008C RID: 140
	public sealed class DiscordUserData
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000BE5C File Offset: 0x0000A05C
		// (set) Token: 0x06000218 RID: 536 RVA: 0x0000BE64 File Offset: 0x0000A064
		[JsonPropertyName("registered")]
		public bool Registered { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000219 RID: 537 RVA: 0x0000BE6D File Offset: 0x0000A06D
		// (set) Token: 0x0600021A RID: 538 RVA: 0x0000BE75 File Offset: 0x0000A075
		[JsonPropertyName("created_at")]
		public double UnixTimestamp { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000BE7E File Offset: 0x0000A07E
		public DateTime DiscordAge
		{
			get
			{
				return DiscordUserData.UnixTimeStampToDateTime(this.UnixTimestamp);
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000BE8C File Offset: 0x0000A08C
		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dateTime;
		}
	}
}
