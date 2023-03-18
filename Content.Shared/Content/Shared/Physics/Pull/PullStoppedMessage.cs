using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Physics.Pull
{
	// Token: 0x02000280 RID: 640
	public sealed class PullStoppedMessage : PullMessage
	{
		// Token: 0x06000745 RID: 1861 RVA: 0x00018B4D File Offset: 0x00016D4D
		[NullableContext(1)]
		public PullStoppedMessage(PhysicsComponent puller, PhysicsComponent pulled) : base(puller, pulled)
		{
		}
	}
}
