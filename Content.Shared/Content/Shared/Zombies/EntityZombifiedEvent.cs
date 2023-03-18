using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Zombies
{
	// Token: 0x02000012 RID: 18
	public readonly struct EntityZombifiedEvent
	{
		// Token: 0x0600001A RID: 26 RVA: 0x0000236B File Offset: 0x0000056B
		public EntityZombifiedEvent(EntityUid target)
		{
			this.Target = target;
		}

		// Token: 0x0400001B RID: 27
		public readonly EntityUid Target;
	}
}
