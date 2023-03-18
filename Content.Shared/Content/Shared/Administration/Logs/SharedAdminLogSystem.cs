using System;
using Content.Shared.Database;
using Robust.Shared.GameObjects;

namespace Content.Shared.Administration.Logs
{
	// Token: 0x02000752 RID: 1874
	public abstract class SharedAdminLogSystem : EntitySystem
	{
		// Token: 0x060016EC RID: 5868 RVA: 0x0004A646 File Offset: 0x00048846
		public virtual void Add(LogType type, LogImpact impact, ref LogStringHandler handler)
		{
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x0004A648 File Offset: 0x00048848
		public virtual void Add(LogType type, ref LogStringHandler handler)
		{
		}
	}
}
