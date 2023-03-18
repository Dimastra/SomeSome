using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Administration.BanList;
using Content.Shared.Eui;

namespace Content.Client.Administration.UI.BanList
{
	// Token: 0x020004D6 RID: 1238
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BanListEui : BaseEui
	{
		// Token: 0x06001F8F RID: 8079 RVA: 0x000B8685 File Offset: 0x000B6885
		public BanListEui()
		{
			this.BanWindow = new BanListWindow();
			this.BanControl = this.BanWindow.BanList;
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x06001F90 RID: 8080 RVA: 0x000B86A9 File Offset: 0x000B68A9
		private BanListWindow BanWindow { get; }

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06001F91 RID: 8081 RVA: 0x000B86B1 File Offset: 0x000B68B1
		private BanListControl BanControl { get; }

		// Token: 0x06001F92 RID: 8082 RVA: 0x000B86BC File Offset: 0x000B68BC
		public override void HandleState(EuiStateBase state)
		{
			BanListEuiState banListEuiState = state as BanListEuiState;
			if (banListEuiState == null)
			{
				return;
			}
			this.BanWindow.SetTitlePlayer(banListEuiState.BanListPlayerName);
			banListEuiState.Bans.Sort((SharedServerBan a, SharedServerBan b) => a.BanTime.CompareTo(b.BanTime));
			this.BanControl.SetBans(banListEuiState.Bans);
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x000B8720 File Offset: 0x000B6920
		public override void Opened()
		{
			this.BanWindow.OpenCentered();
		}
	}
}
