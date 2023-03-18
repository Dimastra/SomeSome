using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C7 RID: 967
	public sealed class InteractHandEvent : HandledEntityEventArgs, ITargetedInteractEventArgs
	{
		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000B1D RID: 2845 RVA: 0x00024688 File Offset: 0x00022888
		public EntityUid User { get; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000B1E RID: 2846 RVA: 0x00024690 File Offset: 0x00022890
		public EntityUid Target { get; }

		// Token: 0x06000B1F RID: 2847 RVA: 0x00024698 File Offset: 0x00022898
		public InteractHandEvent(EntityUid user, EntityUid target)
		{
			this.User = user;
			this.Target = target;
		}
	}
}
