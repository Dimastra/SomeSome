using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x0200030E RID: 782
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FlavorProfileModificationEvent : EntityEventArgs
	{
		// Token: 0x0600101E RID: 4126 RVA: 0x000529B4 File Offset: 0x00050BB4
		public FlavorProfileModificationEvent(EntityUid user, HashSet<string> flavors)
		{
			this.User = user;
			this.Flavors = flavors;
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x0600101F RID: 4127 RVA: 0x000529CA File Offset: 0x00050BCA
		public EntityUid User { get; }

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06001020 RID: 4128 RVA: 0x000529D2 File Offset: 0x00050BD2
		public HashSet<string> Flavors { get; }
	}
}
