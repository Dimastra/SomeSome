using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;

namespace Content.Server.Roles
{
	// Token: 0x0200022F RID: 559
	public sealed class RoleRemovedEvent : RoleEvent
	{
		// Token: 0x06000B23 RID: 2851 RVA: 0x0003A2EF File Offset: 0x000384EF
		[NullableContext(1)]
		public RoleRemovedEvent(Mind mind, Role role) : base(mind, role)
		{
		}
	}
}
