using System;
using System.Runtime.CompilerServices;
using Content.Server.Database;
using Content.Shared.Info;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Info
{
	// Token: 0x0200044F RID: 1103
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RulesManager : SharedRulesManager
	{
		// Token: 0x170002FF RID: 767
		// (get) Token: 0x0600163C RID: 5692 RVA: 0x000756BC File Offset: 0x000738BC
		private static DateTime LastValidReadTime
		{
			get
			{
				return DateTime.UtcNow - TimeSpan.FromDays(60.0);
			}
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x000756D8 File Offset: 0x000738D8
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<SharedRulesManager.ShouldShowRulesPopupMessage>(null, 3);
			this._netManager.RegisterNetMessage<SharedRulesManager.ShowRulesPopupMessage>(null, 3);
			this._netManager.RegisterNetMessage<SharedRulesManager.RulesAcceptedMessage>(new ProcessMessage<SharedRulesManager.RulesAcceptedMessage>(this.OnRulesAccepted), 3);
			this._netManager.Connected += this.OnConnected;
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x00075730 File Offset: 0x00073930
		private void OnConnected([Nullable(2)] object sender, NetChannelArgs e)
		{
			RulesManager.<OnConnected>d__6 <OnConnected>d__;
			<OnConnected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnConnected>d__.<>4__this = this;
			<OnConnected>d__.e = e;
			<OnConnected>d__.<>1__state = -1;
			<OnConnected>d__.<>t__builder.Start<RulesManager.<OnConnected>d__6>(ref <OnConnected>d__);
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x00075770 File Offset: 0x00073970
		private void OnRulesAccepted(SharedRulesManager.RulesAcceptedMessage message)
		{
			RulesManager.<OnRulesAccepted>d__7 <OnRulesAccepted>d__;
			<OnRulesAccepted>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnRulesAccepted>d__.<>4__this = this;
			<OnRulesAccepted>d__.message = message;
			<OnRulesAccepted>d__.<>1__state = -1;
			<OnRulesAccepted>d__.<>t__builder.Start<RulesManager.<OnRulesAccepted>d__7>(ref <OnRulesAccepted>d__);
		}

		// Token: 0x04000DEF RID: 3567
		[Dependency]
		private readonly IServerDbManager _dbManager;

		// Token: 0x04000DF0 RID: 3568
		[Dependency]
		private readonly INetManager _netManager;

		// Token: 0x04000DF1 RID: 3569
		[Dependency]
		private readonly IConfigurationManager _cfg;
	}
}
