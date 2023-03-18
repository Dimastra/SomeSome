using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Administration.BanList;
using Content.Shared.Eui;
using Robust.Shared.IoC;

namespace Content.Server.Administration.BanList
{
	// Token: 0x02000871 RID: 2161
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BanListEui : BaseEui
	{
		// Token: 0x06002F3C RID: 12092 RVA: 0x000F4791 File Offset: 0x000F2991
		public BanListEui()
		{
			IoCManager.InjectDependencies<BanListEui>(this);
		}

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06002F3D RID: 12093 RVA: 0x000F47B6 File Offset: 0x000F29B6
		// (set) Token: 0x06002F3E RID: 12094 RVA: 0x000F47BE File Offset: 0x000F29BE
		private Guid BanListPlayer { get; set; }

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06002F3F RID: 12095 RVA: 0x000F47C7 File Offset: 0x000F29C7
		// (set) Token: 0x06002F40 RID: 12096 RVA: 0x000F47CF File Offset: 0x000F29CF
		private string BanListPlayerName { get; set; } = string.Empty;

		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x06002F41 RID: 12097 RVA: 0x000F47D8 File Offset: 0x000F29D8
		// (set) Token: 0x06002F42 RID: 12098 RVA: 0x000F47E0 File Offset: 0x000F29E0
		private List<SharedServerBan> Bans { get; set; } = new List<SharedServerBan>();

		// Token: 0x06002F43 RID: 12099 RVA: 0x000F47E9 File Offset: 0x000F29E9
		public override void Opened()
		{
			base.Opened();
			this._admins.OnPermsChanged += this.OnPermsChanged;
		}

		// Token: 0x06002F44 RID: 12100 RVA: 0x000F4808 File Offset: 0x000F2A08
		public override EuiStateBase GetNewState()
		{
			return new BanListEuiState(this.BanListPlayerName, this.Bans);
		}

		// Token: 0x06002F45 RID: 12101 RVA: 0x000F481B File Offset: 0x000F2A1B
		private void OnPermsChanged(AdminPermsChangedEventArgs args)
		{
			if (args.Player == base.Player && !this._admins.HasAdminFlag(base.Player, AdminFlags.Ban))
			{
				base.Close();
				return;
			}
			base.StateDirty();
		}

		// Token: 0x06002F46 RID: 12102 RVA: 0x000F484C File Offset: 0x000F2A4C
		private Task LoadFromDb()
		{
			BanListEui.<LoadFromDb>d__19 <LoadFromDb>d__;
			<LoadFromDb>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadFromDb>d__.<>4__this = this;
			<LoadFromDb>d__.<>1__state = -1;
			<LoadFromDb>d__.<>t__builder.Start<BanListEui.<LoadFromDb>d__19>(ref <LoadFromDb>d__);
			return <LoadFromDb>d__.<>t__builder.Task;
		}

		// Token: 0x06002F47 RID: 12103 RVA: 0x000F4890 File Offset: 0x000F2A90
		public Task ChangeBanListPlayer(Guid banListPlayer)
		{
			BanListEui.<ChangeBanListPlayer>d__20 <ChangeBanListPlayer>d__;
			<ChangeBanListPlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ChangeBanListPlayer>d__.<>4__this = this;
			<ChangeBanListPlayer>d__.banListPlayer = banListPlayer;
			<ChangeBanListPlayer>d__.<>1__state = -1;
			<ChangeBanListPlayer>d__.<>t__builder.Start<BanListEui.<ChangeBanListPlayer>d__20>(ref <ChangeBanListPlayer>d__);
			return <ChangeBanListPlayer>d__.<>t__builder.Task;
		}

		// Token: 0x04001C67 RID: 7271
		[Dependency]
		private readonly IAdminManager _admins;

		// Token: 0x04001C68 RID: 7272
		[Dependency]
		private readonly IPlayerLocator _playerLocator;

		// Token: 0x04001C69 RID: 7273
		[Dependency]
		private readonly IServerDbManager _db;
	}
}
