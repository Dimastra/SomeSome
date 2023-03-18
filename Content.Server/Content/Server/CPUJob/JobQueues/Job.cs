using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Timing;

namespace Content.Server.CPUJob.JobQueues
{
	// Token: 0x020005DE RID: 1502
	[NullableContext(2)]
	[Nullable(0)]
	public abstract class Job<T> : IJob
	{
		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06002018 RID: 8216 RVA: 0x000A7AD6 File Offset: 0x000A5CD6
		// (set) Token: 0x06002019 RID: 8217 RVA: 0x000A7ADE File Offset: 0x000A5CDE
		public JobStatus Status { get; private set; }

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x0600201A RID: 8218 RVA: 0x000A7AE7 File Offset: 0x000A5CE7
		[Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<T> AsTask { [return: Nullable(new byte[]
		{
			1,
			2
		})] get; }

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x0600201B RID: 8219 RVA: 0x000A7AEF File Offset: 0x000A5CEF
		// (set) Token: 0x0600201C RID: 8220 RVA: 0x000A7AF7 File Offset: 0x000A5CF7
		public T Result { get; private set; }

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x0600201D RID: 8221 RVA: 0x000A7B00 File Offset: 0x000A5D00
		// (set) Token: 0x0600201E RID: 8222 RVA: 0x000A7B08 File Offset: 0x000A5D08
		public Exception Exception { get; private set; }

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x0600201F RID: 8223 RVA: 0x000A7B11 File Offset: 0x000A5D11
		protected CancellationToken Cancellation { get; }

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06002020 RID: 8224 RVA: 0x000A7B19 File Offset: 0x000A5D19
		// (set) Token: 0x06002021 RID: 8225 RVA: 0x000A7B21 File Offset: 0x000A5D21
		public double DebugTime { get; private set; }

		// Token: 0x06002022 RID: 8226 RVA: 0x000A7B2A File Offset: 0x000A5D2A
		protected Job(double maxTime, CancellationToken cancellation = default(CancellationToken)) : this(maxTime, new Stopwatch(), cancellation)
		{
		}

		// Token: 0x06002023 RID: 8227 RVA: 0x000A7B39 File Offset: 0x000A5D39
		[NullableContext(1)]
		protected Job(double maxTime, IStopwatch stopwatch, CancellationToken cancellation = default(CancellationToken))
		{
			this._maxTime = maxTime;
			this.StopWatch = stopwatch;
			this.Cancellation = cancellation;
			this._taskTcs = new TaskCompletionSource<T>();
			this.AsTask = this._taskTcs.Task;
		}

		// Token: 0x06002024 RID: 8228 RVA: 0x000A7B74 File Offset: 0x000A5D74
		[NullableContext(1)]
		protected Task SuspendNow()
		{
			this._resume = new TaskCompletionSource<object>();
			this.Status = JobStatus.Paused;
			this.DebugTime += this.StopWatch.Elapsed.TotalSeconds;
			return this._resume.Task;
		}

		// Token: 0x06002025 RID: 8229 RVA: 0x000A7BC0 File Offset: 0x000A5DC0
		protected ValueTask SuspendIfOutOfTime()
		{
			if (this.StopWatch.Elapsed.TotalSeconds <= this._maxTime || this._maxTime == 0.0)
			{
				return default(ValueTask);
			}
			return new ValueTask(this.SuspendNow());
		}

		// Token: 0x06002026 RID: 8230 RVA: 0x000A7C10 File Offset: 0x000A5E10
		[NullableContext(1)]
		protected Task<TTask> WaitAsyncTask<[Nullable(2)] TTask>(Task<TTask> task)
		{
			Job<T>.<WaitAsyncTask>d__31<TTask> <WaitAsyncTask>d__;
			<WaitAsyncTask>d__.<>t__builder = AsyncTaskMethodBuilder<TTask>.Create();
			<WaitAsyncTask>d__.<>4__this = this;
			<WaitAsyncTask>d__.task = task;
			<WaitAsyncTask>d__.<>1__state = -1;
			<WaitAsyncTask>d__.<>t__builder.Start<Job<T>.<WaitAsyncTask>d__31<TTask>>(ref <WaitAsyncTask>d__);
			return <WaitAsyncTask>d__.<>t__builder.Task;
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x000A7C5C File Offset: 0x000A5E5C
		[NullableContext(1)]
		protected Task WaitAsyncTask(Task task)
		{
			Job<T>.<WaitAsyncTask>d__32 <WaitAsyncTask>d__;
			<WaitAsyncTask>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitAsyncTask>d__.<>4__this = this;
			<WaitAsyncTask>d__.task = task;
			<WaitAsyncTask>d__.<>1__state = -1;
			<WaitAsyncTask>d__.<>t__builder.Start<Job<T>.<WaitAsyncTask>d__32>(ref <WaitAsyncTask>d__);
			return <WaitAsyncTask>d__.<>t__builder.Task;
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x000A7CA8 File Offset: 0x000A5EA8
		public void Run()
		{
			this.StopWatch.Restart();
			if (this._workInProgress == null)
			{
				this._workInProgress = this.ProcessWrap();
			}
			if (this.Status == JobStatus.Finished)
			{
				return;
			}
			TaskCompletionSource<object> resume = this._resume;
			this._resume = null;
			this.Status = JobStatus.Running;
			if (this.Cancellation.IsCancellationRequested)
			{
				if (resume != null)
				{
					resume.TrySetCanceled();
				}
			}
			else if (resume != null)
			{
				resume.SetResult(null);
			}
			if (this.Status != JobStatus.Finished)
			{
				JobStatus status = this.Status;
			}
		}

		// Token: 0x06002029 RID: 8233
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		protected abstract Task<T> Process();

		// Token: 0x0600202A RID: 8234 RVA: 0x000A7D2C File Offset: 0x000A5F2C
		[NullableContext(1)]
		private Task ProcessWrap()
		{
			Job<T>.<ProcessWrap>d__35 <ProcessWrap>d__;
			<ProcessWrap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessWrap>d__.<>4__this = this;
			<ProcessWrap>d__.<>1__state = -1;
			<ProcessWrap>d__.<>t__builder.Start<Job<T>.<ProcessWrap>d__35>(ref <ProcessWrap>d__);
			return <ProcessWrap>d__.<>t__builder.Task;
		}

		// Token: 0x040013EB RID: 5099
		private readonly double _maxTime;

		// Token: 0x040013EC RID: 5100
		[Nullable(1)]
		protected readonly IStopwatch StopWatch;

		// Token: 0x040013ED RID: 5101
		[Nullable(new byte[]
		{
			1,
			2
		})]
		private readonly TaskCompletionSource<T> _taskTcs;

		// Token: 0x040013EE RID: 5102
		private TaskCompletionSource<object> _resume;

		// Token: 0x040013EF RID: 5103
		private Task _workInProgress;
	}
}
