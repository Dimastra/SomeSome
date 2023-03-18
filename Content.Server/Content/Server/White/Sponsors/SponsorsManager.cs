using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Shared.CCVar;
using Content.Shared.White.Sponsors;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Server.White.Sponsors
{
	// Token: 0x02000091 RID: 145
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SponsorsManager
	{
		// Token: 0x06000238 RID: 568 RVA: 0x0000C49C File Offset: 0x0000A69C
		public void Initialize()
		{
			this._sawmill = Logger.GetSawmill("sponsors");
			this._cfg.OnValueChanged<string>(CCVars.SponsorsApiUrl, delegate(string s)
			{
				this._apiUrl = s;
			}, true);
			this._netMgr.RegisterNetMessage<MsgSponsorInfo>(null, 3);
			this._netMgr.Connecting += this.OnConnecting;
			this._netMgr.Connected += this.OnConnected;
			this._netMgr.Disconnect += this.OnDisconnect;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000C528 File Offset: 0x0000A728
		[NullableContext(2)]
		public bool TryGetInfo(NetUserId userId, [NotNullWhen(true)] out SponsorInfo sponsor)
		{
			return this._cachedSponsors.TryGetValue(userId, out sponsor);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000C538 File Offset: 0x0000A738
		private Task OnConnecting(NetConnectingArgs e)
		{
			SponsorsManager.<OnConnecting>d__8 <OnConnecting>d__;
			<OnConnecting>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<OnConnecting>d__.<>4__this = this;
			<OnConnecting>d__.e = e;
			<OnConnecting>d__.<>1__state = -1;
			<OnConnecting>d__.<>t__builder.Start<SponsorsManager.<OnConnecting>d__8>(ref <OnConnecting>d__);
			return <OnConnecting>d__.<>t__builder.Task;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000C584 File Offset: 0x0000A784
		private void OnConnected([Nullable(2)] object sender, NetChannelArgs e)
		{
			SponsorInfo sponsor;
			SponsorInfo info = this._cachedSponsors.TryGetValue(e.Channel.UserId, out sponsor) ? sponsor : null;
			MsgSponsorInfo msg = new MsgSponsorInfo
			{
				Info = info
			};
			this._netMgr.ServerSendMessage(msg, e.Channel);
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000C5CF File Offset: 0x0000A7CF
		private void OnDisconnect([Nullable(2)] object sender, NetDisconnectedArgs e)
		{
			this._cachedSponsors.Remove(e.Channel.UserId);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000C5E8 File Offset: 0x0000A7E8
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		private Task<SponsorInfo> LoadSponsorInfo(NetUserId userId)
		{
			SponsorsManager.<LoadSponsorInfo>d__11 <LoadSponsorInfo>d__;
			<LoadSponsorInfo>d__.<>t__builder = AsyncTaskMethodBuilder<SponsorInfo>.Create();
			<LoadSponsorInfo>d__.<>4__this = this;
			<LoadSponsorInfo>d__.userId = userId;
			<LoadSponsorInfo>d__.<>1__state = -1;
			<LoadSponsorInfo>d__.<>t__builder.Start<SponsorsManager.<LoadSponsorInfo>d__11>(ref <LoadSponsorInfo>d__);
			return <LoadSponsorInfo>d__.<>t__builder.Task;
		}

		// Token: 0x04000193 RID: 403
		[Dependency]
		private readonly IServerNetManager _netMgr;

		// Token: 0x04000194 RID: 404
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000195 RID: 405
		private readonly HttpClient _httpClient = new HttpClient();

		// Token: 0x04000196 RID: 406
		private ISawmill _sawmill;

		// Token: 0x04000197 RID: 407
		private string _apiUrl = string.Empty;

		// Token: 0x04000198 RID: 408
		private readonly Dictionary<NetUserId, SponsorInfo> _cachedSponsors = new Dictionary<NetUserId, SponsorInfo>();
	}
}
