using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C1 RID: 961
	public sealed class ActivateInWorldEvent : HandledEntityEventArgs, ITargetedInteractEventArgs
	{
		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000B07 RID: 2823 RVA: 0x00024563 File Offset: 0x00022763
		public EntityUid User { get; }

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000B08 RID: 2824 RVA: 0x0002456B File Offset: 0x0002276B
		public EntityUid Target { get; }

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000B09 RID: 2825 RVA: 0x00024573 File Offset: 0x00022773
		// (set) Token: 0x06000B0A RID: 2826 RVA: 0x0002457B File Offset: 0x0002277B
		public bool WasLogged { get; set; }

		// Token: 0x06000B0B RID: 2827 RVA: 0x00024584 File Offset: 0x00022784
		public ActivateInWorldEvent(EntityUid user, EntityUid target)
		{
			this.User = user;
			this.Target = target;
		}
	}
}
