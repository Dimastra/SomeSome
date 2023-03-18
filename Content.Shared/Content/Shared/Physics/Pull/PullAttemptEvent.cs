using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Physics.Pull
{
	// Token: 0x0200027D RID: 637
	public sealed class PullAttemptEvent : PullMessage
	{
		// Token: 0x06000740 RID: 1856 RVA: 0x00018B12 File Offset: 0x00016D12
		[NullableContext(1)]
		public PullAttemptEvent(PhysicsComponent puller, PhysicsComponent pulled) : base(puller, pulled)
		{
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x00018B1C File Offset: 0x00016D1C
		// (set) Token: 0x06000742 RID: 1858 RVA: 0x00018B24 File Offset: 0x00016D24
		public bool Cancelled { get; set; }
	}
}
