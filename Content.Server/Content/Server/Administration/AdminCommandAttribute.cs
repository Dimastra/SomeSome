using System;
using Content.Shared.Administration;

namespace Content.Server.Administration
{
	// Token: 0x020007FC RID: 2044
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class AdminCommandAttribute : Attribute
	{
		// Token: 0x06002C32 RID: 11314 RVA: 0x000E7092 File Offset: 0x000E5292
		public AdminCommandAttribute(AdminFlags flags)
		{
			this.Flags = flags;
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x06002C33 RID: 11315 RVA: 0x000E70A1 File Offset: 0x000E52A1
		public AdminFlags Flags { get; }
	}
}
