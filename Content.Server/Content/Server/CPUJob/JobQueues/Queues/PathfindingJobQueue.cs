using System;

namespace Content.Server.CPUJob.JobQueues.Queues
{
	// Token: 0x020005E1 RID: 1505
	public sealed class PathfindingJobQueue : JobQueue
	{
		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06002030 RID: 8240 RVA: 0x000A7EC0 File Offset: 0x000A60C0
		public override double MaxTime
		{
			get
			{
				return 0.003;
			}
		}
	}
}
