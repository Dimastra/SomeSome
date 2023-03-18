using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Physics.Pull
{
	// Token: 0x0200027F RID: 639
	public sealed class PullStartedMessage : PullMessage
	{
		// Token: 0x06000744 RID: 1860 RVA: 0x00018B43 File Offset: 0x00016D43
		[NullableContext(1)]
		public PullStartedMessage(PhysicsComponent puller, PhysicsComponent pulled) : base(puller, pulled)
		{
		}
	}
}
