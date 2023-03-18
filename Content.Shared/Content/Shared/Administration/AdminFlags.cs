using System;

namespace Content.Shared.Administration
{
	// Token: 0x02000730 RID: 1840
	[Flags]
	public enum AdminFlags : uint
	{
		// Token: 0x04001693 RID: 5779
		None = 0U,
		// Token: 0x04001694 RID: 5780
		Admin = 1U,
		// Token: 0x04001695 RID: 5781
		Ban = 2U,
		// Token: 0x04001696 RID: 5782
		Debug = 4U,
		// Token: 0x04001697 RID: 5783
		Fun = 8U,
		// Token: 0x04001698 RID: 5784
		Permissions = 16U,
		// Token: 0x04001699 RID: 5785
		Server = 32U,
		// Token: 0x0400169A RID: 5786
		Spawn = 64U,
		// Token: 0x0400169B RID: 5787
		VarEdit = 128U,
		// Token: 0x0400169C RID: 5788
		Mapping = 256U,
		// Token: 0x0400169D RID: 5789
		Logs = 512U,
		// Token: 0x0400169E RID: 5790
		Round = 1024U,
		// Token: 0x0400169F RID: 5791
		Query = 2048U,
		// Token: 0x040016A0 RID: 5792
		Adminhelp = 4096U,
		// Token: 0x040016A1 RID: 5793
		ViewNotes = 8192U,
		// Token: 0x040016A2 RID: 5794
		EditNotes = 16384U,
		// Token: 0x040016A3 RID: 5795
		MeatyOre = 32768U,
		// Token: 0x040016A4 RID: 5796
		Host = 2147483648U
	}
}
