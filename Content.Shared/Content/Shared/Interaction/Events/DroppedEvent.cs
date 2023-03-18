using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D5 RID: 981
	public sealed class DroppedEvent : HandledEntityEventArgs
	{
		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000B7D RID: 2941 RVA: 0x000261EE File Offset: 0x000243EE
		public EntityUid User { get; }

		// Token: 0x06000B7E RID: 2942 RVA: 0x000261F6 File Offset: 0x000243F6
		public DroppedEvent(EntityUid user)
		{
			this.User = user;
		}
	}
}
