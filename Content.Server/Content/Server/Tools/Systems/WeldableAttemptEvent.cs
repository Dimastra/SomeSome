using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Tools.Systems
{
	// Token: 0x02000114 RID: 276
	public sealed class WeldableAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x0600050B RID: 1291 RVA: 0x0001890F File Offset: 0x00016B0F
		public WeldableAttemptEvent(EntityUid user, EntityUid tool)
		{
			this.User = user;
			this.Tool = tool;
		}

		// Token: 0x040002EC RID: 748
		public readonly EntityUid User;

		// Token: 0x040002ED RID: 749
		public readonly EntityUid Tool;
	}
}
