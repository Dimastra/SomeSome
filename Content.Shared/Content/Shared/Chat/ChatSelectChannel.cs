using System;

namespace Content.Shared.Chat
{
	// Token: 0x02000600 RID: 1536
	[Flags]
	public enum ChatSelectChannel : ushort
	{
		// Token: 0x04001178 RID: 4472
		None = 0,
		// Token: 0x04001179 RID: 4473
		Local = 1,
		// Token: 0x0400117A RID: 4474
		Whisper = 2,
		// Token: 0x0400117B RID: 4475
		Radio = 16,
		// Token: 0x0400117C RID: 4476
		LOOC = 32,
		// Token: 0x0400117D RID: 4477
		OOC = 64,
		// Token: 0x0400117E RID: 4478
		Emotes = 256,
		// Token: 0x0400117F RID: 4479
		Dead = 512,
		// Token: 0x04001180 RID: 4480
		Admin = 2048,
		// Token: 0x04001181 RID: 4481
		Console = 4096
	}
}
