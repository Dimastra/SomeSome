using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;

namespace Content.Server.Administration
{
	// Token: 0x020007FE RID: 2046
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminRank
	{
		// Token: 0x06002C38 RID: 11320 RVA: 0x000E70EB File Offset: 0x000E52EB
		public AdminRank(string name, AdminFlags flags)
		{
			this.Name = name;
			this.Flags = flags;
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06002C39 RID: 11321 RVA: 0x000E7101 File Offset: 0x000E5301
		public string Name { get; }

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06002C3A RID: 11322 RVA: 0x000E7109 File Offset: 0x000E5309
		public AdminFlags Flags { get; }
	}
}
