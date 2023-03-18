using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Physics.Pull
{
	// Token: 0x0200027E RID: 638
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class PullMessage : EntityEventArgs
	{
		// Token: 0x06000743 RID: 1859 RVA: 0x00018B2D File Offset: 0x00016D2D
		protected PullMessage(PhysicsComponent puller, PhysicsComponent pulled)
		{
			this.Puller = puller;
			this.Pulled = pulled;
		}

		// Token: 0x0400074D RID: 1869
		public readonly PhysicsComponent Puller;

		// Token: 0x0400074E RID: 1870
		public readonly PhysicsComponent Pulled;
	}
}
