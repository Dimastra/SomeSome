using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Timing;

namespace Content.Server.CPUJob.JobQueues.Queues
{
	// Token: 0x020005E0 RID: 1504
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class JobQueue
	{
		// Token: 0x0600202B RID: 8235 RVA: 0x000A7D6F File Offset: 0x000A5F6F
		public JobQueue() : this(new Stopwatch())
		{
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000A7D7C File Offset: 0x000A5F7C
		public JobQueue(IStopwatch stopwatch)
		{
			this._stopwatch = stopwatch;
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x0600202D RID: 8237 RVA: 0x000A7DB0 File Offset: 0x000A5FB0
		public virtual double MaxTime { get; } = 0.002;

		// Token: 0x0600202E RID: 8238 RVA: 0x000A7DB8 File Offset: 0x000A5FB8
		public void EnqueueJob(IJob job)
		{
			this._pendingQueue.Enqueue(job);
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000A7DC8 File Offset: 0x000A5FC8
		public void Process()
		{
			foreach (IJob waitingJob in this._waitingJobs)
			{
				if (waitingJob.Status != JobStatus.Waiting)
				{
					this._pendingQueue.Enqueue(waitingJob);
				}
			}
			this._waitingJobs.RemoveAll((IJob p) => p.Status != JobStatus.Waiting);
			this._stopwatch.Restart();
			IJob job;
			while (this._stopwatch.Elapsed.TotalSeconds < this.MaxTime && this._pendingQueue.TryDequeue(out job))
			{
				job.Run();
				JobStatus status = job.Status;
				if (status != JobStatus.Waiting)
				{
					if (status != JobStatus.Finished)
					{
						this._pendingQueue.Enqueue(job);
					}
				}
				else
				{
					this._waitingJobs.Add(job);
				}
			}
		}

		// Token: 0x040013F6 RID: 5110
		private readonly IStopwatch _stopwatch;

		// Token: 0x040013F8 RID: 5112
		private readonly Queue<IJob> _pendingQueue = new Queue<IJob>();

		// Token: 0x040013F9 RID: 5113
		private readonly List<IJob> _waitingJobs = new List<IJob>();
	}
}
