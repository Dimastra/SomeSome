using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Logs
{
	// Token: 0x0200074B RID: 1867
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AdminLogsEuiState : EuiStateBase
	{
		// Token: 0x060016BE RID: 5822 RVA: 0x0004A089 File Offset: 0x00048289
		public AdminLogsEuiState(int roundId, Dictionary<Guid, string> players)
		{
			this.RoundId = roundId;
			this.Players = players;
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x060016BF RID: 5823 RVA: 0x0004A09F File Offset: 0x0004829F
		// (set) Token: 0x060016C0 RID: 5824 RVA: 0x0004A0A7 File Offset: 0x000482A7
		public bool IsLoading { get; set; }

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x060016C1 RID: 5825 RVA: 0x0004A0B0 File Offset: 0x000482B0
		public int RoundId { get; }

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x060016C2 RID: 5826 RVA: 0x0004A0B8 File Offset: 0x000482B8
		public Dictionary<Guid, string> Players { get; }
	}
}
