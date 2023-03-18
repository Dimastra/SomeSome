using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Database;

namespace Content.Server.Administration.Logs
{
	// Token: 0x0200081D RID: 2077
	[NullableContext(1)]
	[Nullable(0)]
	public readonly struct QueuedLog
	{
		// Token: 0x06002DB1 RID: 11697 RVA: 0x000EFADC File Offset: 0x000EDCDC
		public QueuedLog(AdminLog log, [Nullable(new byte[]
		{
			1,
			2
		})] Dictionary<int, string> entities)
		{
			this.Log = log;
			this.Entities = entities;
		}

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x06002DB2 RID: 11698 RVA: 0x000EFAEC File Offset: 0x000EDCEC
		public AdminLog Log { get; }

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x06002DB3 RID: 11699 RVA: 0x000EFAF4 File Offset: 0x000EDCF4
		[Nullable(new byte[]
		{
			1,
			2
		})]
		public Dictionary<int, string> Entities { [return: Nullable(new byte[]
		{
			1,
			2
		})] get; }

		// Token: 0x06002DB4 RID: 11700 RVA: 0x000EFAFC File Offset: 0x000EDCFC
		public void Deconstruct(out AdminLog log, [Nullable(new byte[]
		{
			1,
			2
		})] out Dictionary<int, string> entities)
		{
			log = this.Log;
			entities = this.Entities;
		}
	}
}
