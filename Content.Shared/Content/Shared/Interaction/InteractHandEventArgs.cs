using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C6 RID: 966
	public sealed class InteractHandEventArgs : EventArgs, ITargetedInteractEventArgs
	{
		// Token: 0x06000B1A RID: 2842 RVA: 0x00024662 File Offset: 0x00022862
		public InteractHandEventArgs(EntityUid user, EntityUid target)
		{
			this.User = user;
			this.Target = target;
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000B1B RID: 2843 RVA: 0x00024678 File Offset: 0x00022878
		public EntityUid User { get; }

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000B1C RID: 2844 RVA: 0x00024680 File Offset: 0x00022880
		public EntityUid Target { get; }
	}
}
