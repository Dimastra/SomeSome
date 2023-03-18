using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Logs
{
	// Token: 0x0200074C RID: 1868
	public static class AdminLogsEuiMsg
	{
		// Token: 0x020008A3 RID: 2211
		[NetSerializable]
		[Serializable]
		public sealed class Close : EuiMessageBase
		{
		}

		// Token: 0x020008A4 RID: 2212
		[NullableContext(2)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class SetLogFilter : EuiMessageBase
		{
			// Token: 0x06001A1C RID: 6684 RVA: 0x00051D6A File Offset: 0x0004FF6A
			public SetLogFilter(string search = null, bool invertTypes = false, HashSet<LogType> types = null)
			{
				this.Search = search;
				this.InvertTypes = invertTypes;
				this.Types = types;
			}

			// Token: 0x17000552 RID: 1362
			// (get) Token: 0x06001A1D RID: 6685 RVA: 0x00051D87 File Offset: 0x0004FF87
			// (set) Token: 0x06001A1E RID: 6686 RVA: 0x00051D8F File Offset: 0x0004FF8F
			public string Search { get; set; }

			// Token: 0x17000553 RID: 1363
			// (get) Token: 0x06001A1F RID: 6687 RVA: 0x00051D98 File Offset: 0x0004FF98
			// (set) Token: 0x06001A20 RID: 6688 RVA: 0x00051DA0 File Offset: 0x0004FFA0
			public bool InvertTypes { get; set; }

			// Token: 0x17000554 RID: 1364
			// (get) Token: 0x06001A21 RID: 6689 RVA: 0x00051DA9 File Offset: 0x0004FFA9
			// (set) Token: 0x06001A22 RID: 6690 RVA: 0x00051DB1 File Offset: 0x0004FFB1
			public HashSet<LogType> Types { get; set; }
		}

		// Token: 0x020008A5 RID: 2213
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class NewLogs : EuiMessageBase
		{
			// Token: 0x06001A23 RID: 6691 RVA: 0x00051DBA File Offset: 0x0004FFBA
			public NewLogs(List<SharedAdminLog> logs, bool replace, bool hasNext)
			{
				this.Logs = logs;
				this.Replace = replace;
				this.HasNext = hasNext;
			}

			// Token: 0x17000555 RID: 1365
			// (get) Token: 0x06001A24 RID: 6692 RVA: 0x00051DD7 File Offset: 0x0004FFD7
			// (set) Token: 0x06001A25 RID: 6693 RVA: 0x00051DDF File Offset: 0x0004FFDF
			public List<SharedAdminLog> Logs { get; set; }

			// Token: 0x17000556 RID: 1366
			// (get) Token: 0x06001A26 RID: 6694 RVA: 0x00051DE8 File Offset: 0x0004FFE8
			// (set) Token: 0x06001A27 RID: 6695 RVA: 0x00051DF0 File Offset: 0x0004FFF0
			public bool Replace { get; set; }

			// Token: 0x17000557 RID: 1367
			// (get) Token: 0x06001A28 RID: 6696 RVA: 0x00051DF9 File Offset: 0x0004FFF9
			// (set) Token: 0x06001A29 RID: 6697 RVA: 0x00051E01 File Offset: 0x00050001
			public bool HasNext { get; set; }
		}

		// Token: 0x020008A6 RID: 2214
		[NullableContext(2)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class LogsRequest : EuiMessageBase
		{
			// Token: 0x06001A2A RID: 6698 RVA: 0x00051E0C File Offset: 0x0005000C
			public LogsRequest(int? roundId, string search, HashSet<LogType> types, HashSet<LogImpact> impacts, DateTime? before, DateTime? after, Guid[] anyPlayers, Guid[] allPlayers, int? lastLogId, DateOrder dateOrder)
			{
				this.RoundId = roundId;
				this.Search = search;
				this.Types = types;
				this.Impacts = impacts;
				this.Before = before;
				this.After = after;
				this.AnyPlayers = ((anyPlayers != null && anyPlayers.Length > 0) ? anyPlayers : null);
				this.AllPlayers = ((allPlayers != null && allPlayers.Length > 0) ? allPlayers : null);
				this.LastLogId = lastLogId;
				this.DateOrder = dateOrder;
			}

			// Token: 0x17000558 RID: 1368
			// (get) Token: 0x06001A2B RID: 6699 RVA: 0x00051E88 File Offset: 0x00050088
			// (set) Token: 0x06001A2C RID: 6700 RVA: 0x00051E90 File Offset: 0x00050090
			public int? RoundId { get; set; }

			// Token: 0x17000559 RID: 1369
			// (get) Token: 0x06001A2D RID: 6701 RVA: 0x00051E99 File Offset: 0x00050099
			// (set) Token: 0x06001A2E RID: 6702 RVA: 0x00051EA1 File Offset: 0x000500A1
			public string Search { get; set; }

			// Token: 0x1700055A RID: 1370
			// (get) Token: 0x06001A2F RID: 6703 RVA: 0x00051EAA File Offset: 0x000500AA
			// (set) Token: 0x06001A30 RID: 6704 RVA: 0x00051EB2 File Offset: 0x000500B2
			public HashSet<LogType> Types { get; set; }

			// Token: 0x1700055B RID: 1371
			// (get) Token: 0x06001A31 RID: 6705 RVA: 0x00051EBB File Offset: 0x000500BB
			// (set) Token: 0x06001A32 RID: 6706 RVA: 0x00051EC3 File Offset: 0x000500C3
			public HashSet<LogImpact> Impacts { get; set; }

			// Token: 0x1700055C RID: 1372
			// (get) Token: 0x06001A33 RID: 6707 RVA: 0x00051ECC File Offset: 0x000500CC
			// (set) Token: 0x06001A34 RID: 6708 RVA: 0x00051ED4 File Offset: 0x000500D4
			public DateTime? Before { get; set; }

			// Token: 0x1700055D RID: 1373
			// (get) Token: 0x06001A35 RID: 6709 RVA: 0x00051EDD File Offset: 0x000500DD
			// (set) Token: 0x06001A36 RID: 6710 RVA: 0x00051EE5 File Offset: 0x000500E5
			public DateTime? After { get; set; }

			// Token: 0x1700055E RID: 1374
			// (get) Token: 0x06001A37 RID: 6711 RVA: 0x00051EEE File Offset: 0x000500EE
			// (set) Token: 0x06001A38 RID: 6712 RVA: 0x00051EF6 File Offset: 0x000500F6
			public Guid[] AnyPlayers { get; set; }

			// Token: 0x1700055F RID: 1375
			// (get) Token: 0x06001A39 RID: 6713 RVA: 0x00051EFF File Offset: 0x000500FF
			// (set) Token: 0x06001A3A RID: 6714 RVA: 0x00051F07 File Offset: 0x00050107
			public Guid[] AllPlayers { get; set; }

			// Token: 0x17000560 RID: 1376
			// (get) Token: 0x06001A3B RID: 6715 RVA: 0x00051F10 File Offset: 0x00050110
			// (set) Token: 0x06001A3C RID: 6716 RVA: 0x00051F18 File Offset: 0x00050118
			public int? LastLogId { get; set; }

			// Token: 0x17000561 RID: 1377
			// (get) Token: 0x06001A3D RID: 6717 RVA: 0x00051F21 File Offset: 0x00050121
			// (set) Token: 0x06001A3E RID: 6718 RVA: 0x00051F29 File Offset: 0x00050129
			public DateOrder DateOrder { get; set; }
		}

		// Token: 0x020008A7 RID: 2215
		[NetSerializable]
		[Serializable]
		public sealed class NextLogsRequest : EuiMessageBase
		{
		}
	}
}
