using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;

namespace Content.Server.Roles
{
	// Token: 0x0200022D RID: 557
	public sealed class RoleAddedEvent : RoleEvent
	{
		// Token: 0x06000B21 RID: 2849 RVA: 0x0003A2CF File Offset: 0x000384CF
		[NullableContext(1)]
		public RoleAddedEvent(Mind mind, Role role) : base(mind, role)
		{
		}
	}
}
