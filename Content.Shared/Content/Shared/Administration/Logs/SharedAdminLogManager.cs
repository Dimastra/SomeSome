using System;
using Content.Shared.Database;
using Robust.Shared.Analyzers;

namespace Content.Shared.Administration.Logs
{
	// Token: 0x02000751 RID: 1873
	[Virtual]
	public class SharedAdminLogManager : ISharedAdminLogManager
	{
		// Token: 0x060016E9 RID: 5865 RVA: 0x0004A63A File Offset: 0x0004883A
		public virtual void Add(LogType type, LogImpact impact, ref LogStringHandler handler)
		{
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x0004A63C File Offset: 0x0004883C
		public virtual void Add(LogType type, ref LogStringHandler handler)
		{
		}
	}
}
