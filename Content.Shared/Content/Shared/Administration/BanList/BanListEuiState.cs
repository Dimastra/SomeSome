using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.BanList
{
	// Token: 0x02000757 RID: 1879
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class BanListEuiState : EuiStateBase
	{
		// Token: 0x060016F3 RID: 5875 RVA: 0x0004A67D File Offset: 0x0004887D
		public BanListEuiState(string banListPlayerName, List<SharedServerBan> bans)
		{
			this.BanListPlayerName = banListPlayerName;
			this.Bans = bans;
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x060016F4 RID: 5876 RVA: 0x0004A693 File Offset: 0x00048893
		public string BanListPlayerName { get; }

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x060016F5 RID: 5877 RVA: 0x0004A69B File Offset: 0x0004889B
		public List<SharedServerBan> Bans { get; }
	}
}
