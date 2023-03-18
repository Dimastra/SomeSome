using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Robust.Shared.GameObjects;

namespace Content.Server.Roles
{
	// Token: 0x0200022E RID: 558
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class RoleEvent : EntityEventArgs
	{
		// Token: 0x06000B22 RID: 2850 RVA: 0x0003A2D9 File Offset: 0x000384D9
		public RoleEvent(Mind mind, Role role)
		{
			this.Mind = mind;
			this.Role = role;
		}

		// Token: 0x040006CD RID: 1741
		public readonly Mind Mind;

		// Token: 0x040006CE RID: 1742
		public readonly Role Role;
	}
}
