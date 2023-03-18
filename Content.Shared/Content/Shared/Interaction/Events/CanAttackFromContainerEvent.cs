using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D2 RID: 978
	public sealed class CanAttackFromContainerEvent : EntityEventArgs
	{
		// Token: 0x06000B79 RID: 2937 RVA: 0x000261B2 File Offset: 0x000243B2
		public CanAttackFromContainerEvent(EntityUid uid, EntityUid? target = null)
		{
			this.Uid = uid;
			this.Target = target;
		}

		// Token: 0x04000B33 RID: 2867
		public EntityUid Uid;

		// Token: 0x04000B34 RID: 2868
		public EntityUid? Target;

		// Token: 0x04000B35 RID: 2869
		public bool CanAttack;
	}
}
