using System;

namespace Content.Shared.Chat
{
	// Token: 0x020005FE RID: 1534
	[Flags]
	public enum ChatChannel : ushort
	{
		// Token: 0x04001168 RID: 4456
		None = 0,
		// Token: 0x04001169 RID: 4457
		Local = 1,
		// Token: 0x0400116A RID: 4458
		Whisper = 2,
		// Token: 0x0400116B RID: 4459
		Server = 4,
		// Token: 0x0400116C RID: 4460
		Damage = 8,
		// Token: 0x0400116D RID: 4461
		Radio = 16,
		// Token: 0x0400116E RID: 4462
		LOOC = 32,
		// Token: 0x0400116F RID: 4463
		OOC = 64,
		// Token: 0x04001170 RID: 4464
		Visual = 128,
		// Token: 0x04001171 RID: 4465
		Emotes = 256,
		// Token: 0x04001172 RID: 4466
		Dead = 512,
		// Token: 0x04001173 RID: 4467
		Admin = 1024,
		// Token: 0x04001174 RID: 4468
		AdminChat = 2048,
		// Token: 0x04001175 RID: 4469
		Unspecified = 4096,
		// Token: 0x04001176 RID: 4470
		IC = 923
	}
}
