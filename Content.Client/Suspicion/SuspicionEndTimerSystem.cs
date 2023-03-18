using System;
using System.Runtime.CompilerServices;
using Content.Shared.Suspicion;
using Robust.Shared.GameObjects;

namespace Content.Client.Suspicion
{
	// Token: 0x020000FD RID: 253
	public sealed class SuspicionEndTimerSystem : EntitySystem
	{
		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600071F RID: 1823 RVA: 0x00025981 File Offset: 0x00023B81
		// (set) Token: 0x06000720 RID: 1824 RVA: 0x00025989 File Offset: 0x00023B89
		public TimeSpan? EndTime { get; private set; }

		// Token: 0x06000721 RID: 1825 RVA: 0x00025992 File Offset: 0x00023B92
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<SuspicionMessages.SetSuspicionEndTimerMessage>(new EntityEventHandler<SuspicionMessages.SetSuspicionEndTimerMessage>(this.RxTimerMessage), null, null);
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x000259AE File Offset: 0x00023BAE
		[NullableContext(1)]
		private void RxTimerMessage(SuspicionMessages.SetSuspicionEndTimerMessage ev)
		{
			this.EndTime = ev.EndTime;
		}
	}
}
