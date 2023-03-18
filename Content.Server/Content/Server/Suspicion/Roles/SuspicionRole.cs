using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Roles;

namespace Content.Server.Suspicion.Roles
{
	// Token: 0x02000139 RID: 313
	public abstract class SuspicionRole : Role
	{
		// Token: 0x060005BC RID: 1468 RVA: 0x0001C18D File Offset: 0x0001A38D
		[NullableContext(1)]
		protected SuspicionRole(Mind mind) : base(mind)
		{
		}
	}
}
