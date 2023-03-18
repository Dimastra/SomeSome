using System;

namespace Content.Server.CPUJob.JobQueues
{
	// Token: 0x020005DD RID: 1501
	public interface IJob
	{
		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06002016 RID: 8214
		JobStatus Status { get; }

		// Token: 0x06002017 RID: 8215
		void Run();
	}
}
