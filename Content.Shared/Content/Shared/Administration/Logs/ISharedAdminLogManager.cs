using System;
using Content.Shared.Database;

namespace Content.Shared.Administration.Logs
{
	// Token: 0x0200074E RID: 1870
	public interface ISharedAdminLogManager
	{
		// Token: 0x060016C3 RID: 5827
		void Add(LogType type, LogImpact impact, ref LogStringHandler handler);

		// Token: 0x060016C4 RID: 5828
		void Add(LogType type, ref LogStringHandler handler);
	}
}
