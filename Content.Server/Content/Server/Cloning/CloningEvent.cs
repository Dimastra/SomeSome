using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Cloning
{
	// Token: 0x02000641 RID: 1601
	[ByRefEvent]
	public struct CloningEvent
	{
		// Token: 0x06002210 RID: 8720 RVA: 0x000B2012 File Offset: 0x000B0212
		public CloningEvent(EntityUid source, EntityUid target)
		{
			this.NameHandled = false;
			this.Source = source;
			this.Target = target;
		}

		// Token: 0x040014EB RID: 5355
		public bool NameHandled;

		// Token: 0x040014EC RID: 5356
		public readonly EntityUid Source;

		// Token: 0x040014ED RID: 5357
		public readonly EntityUid Target;
	}
}
