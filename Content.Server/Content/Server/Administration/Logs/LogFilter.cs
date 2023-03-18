using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;

namespace Content.Server.Administration.Logs
{
	// Token: 0x0200081C RID: 2076
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class LogFilter
	{
		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x06002D96 RID: 11670 RVA: 0x000EF9F0 File Offset: 0x000EDBF0
		// (set) Token: 0x06002D97 RID: 11671 RVA: 0x000EF9F8 File Offset: 0x000EDBF8
		public CancellationToken CancellationToken { get; set; }

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x06002D98 RID: 11672 RVA: 0x000EFA01 File Offset: 0x000EDC01
		// (set) Token: 0x06002D99 RID: 11673 RVA: 0x000EFA09 File Offset: 0x000EDC09
		public int? Round { get; set; }

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06002D9A RID: 11674 RVA: 0x000EFA12 File Offset: 0x000EDC12
		// (set) Token: 0x06002D9B RID: 11675 RVA: 0x000EFA1A File Offset: 0x000EDC1A
		public string Search { get; set; }

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06002D9C RID: 11676 RVA: 0x000EFA23 File Offset: 0x000EDC23
		// (set) Token: 0x06002D9D RID: 11677 RVA: 0x000EFA2B File Offset: 0x000EDC2B
		public HashSet<LogType> Types { get; set; }

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06002D9E RID: 11678 RVA: 0x000EFA34 File Offset: 0x000EDC34
		// (set) Token: 0x06002D9F RID: 11679 RVA: 0x000EFA3C File Offset: 0x000EDC3C
		public HashSet<LogImpact> Impacts { get; set; }

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06002DA0 RID: 11680 RVA: 0x000EFA45 File Offset: 0x000EDC45
		// (set) Token: 0x06002DA1 RID: 11681 RVA: 0x000EFA4D File Offset: 0x000EDC4D
		public DateTime? Before { get; set; }

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x06002DA2 RID: 11682 RVA: 0x000EFA56 File Offset: 0x000EDC56
		// (set) Token: 0x06002DA3 RID: 11683 RVA: 0x000EFA5E File Offset: 0x000EDC5E
		public DateTime? After { get; set; }

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x06002DA4 RID: 11684 RVA: 0x000EFA67 File Offset: 0x000EDC67
		// (set) Token: 0x06002DA5 RID: 11685 RVA: 0x000EFA6F File Offset: 0x000EDC6F
		public Guid[] AnyPlayers { get; set; }

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x06002DA6 RID: 11686 RVA: 0x000EFA78 File Offset: 0x000EDC78
		// (set) Token: 0x06002DA7 RID: 11687 RVA: 0x000EFA80 File Offset: 0x000EDC80
		public Guid[] AllPlayers { get; set; }

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x06002DA8 RID: 11688 RVA: 0x000EFA89 File Offset: 0x000EDC89
		// (set) Token: 0x06002DA9 RID: 11689 RVA: 0x000EFA91 File Offset: 0x000EDC91
		public int? LastLogId { get; set; }

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06002DAA RID: 11690 RVA: 0x000EFA9A File Offset: 0x000EDC9A
		// (set) Token: 0x06002DAB RID: 11691 RVA: 0x000EFAA2 File Offset: 0x000EDCA2
		public int LogsSent { get; set; }

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x06002DAC RID: 11692 RVA: 0x000EFAAB File Offset: 0x000EDCAB
		// (set) Token: 0x06002DAD RID: 11693 RVA: 0x000EFAB3 File Offset: 0x000EDCB3
		public int? Limit { get; set; }

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x06002DAE RID: 11694 RVA: 0x000EFABC File Offset: 0x000EDCBC
		// (set) Token: 0x06002DAF RID: 11695 RVA: 0x000EFAC4 File Offset: 0x000EDCC4
		public DateOrder DateOrder { get; set; } = DateOrder.Descending;
	}
}
