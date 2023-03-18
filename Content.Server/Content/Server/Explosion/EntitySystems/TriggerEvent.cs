using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x02000511 RID: 1297
	public sealed class TriggerEvent : HandledEntityEventArgs
	{
		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06001B04 RID: 6916 RVA: 0x00090CA3 File Offset: 0x0008EEA3
		public EntityUid Triggered { get; }

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06001B05 RID: 6917 RVA: 0x00090CAB File Offset: 0x0008EEAB
		public EntityUid? User { get; }

		// Token: 0x06001B06 RID: 6918 RVA: 0x00090CB3 File Offset: 0x0008EEB3
		public TriggerEvent(EntityUid triggered, EntityUid? user = null)
		{
			this.Triggered = triggered;
			this.User = user;
		}
	}
}
