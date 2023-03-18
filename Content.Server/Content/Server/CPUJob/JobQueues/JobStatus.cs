using System;

namespace Content.Server.CPUJob.JobQueues
{
	// Token: 0x020005DF RID: 1503
	public enum JobStatus
	{
		// Token: 0x040013F1 RID: 5105
		Pending,
		// Token: 0x040013F2 RID: 5106
		Running,
		// Token: 0x040013F3 RID: 5107
		Paused,
		// Token: 0x040013F4 RID: 5108
		Waiting,
		// Token: 0x040013F5 RID: 5109
		Finished
	}
}
