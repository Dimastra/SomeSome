using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Content.Server.Chat.Managers;
using Content.Shared.CCVar;
using Content.Shared.White.SaltedYayca;
using Robust.Server.Player;
using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Random;

namespace Content.Server.White.Stalin
{
	// Token: 0x0200008F RID: 143
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StalinManager
	{
		// Token: 0x06000224 RID: 548 RVA: 0x0000BF10 File Offset: 0x0000A110
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<DiscordAuthRequest>(new ProcessMessage<DiscordAuthRequest>(this.OnDiscordAuthRequest), 3);
			this._netManager.RegisterNetMessage<DiscordAuthResponse>(null, 3);
			this._chatManager = IoCManager.Resolve<IChatManager>();
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
			this._configurationManager.OnValueChanged<string>(CCVars.StalinApiUrl, delegate(string newValue)
			{
				this._stalinApiUrl = newValue;
			}, true);
			this._configurationManager.OnValueChanged<string>(CCVars.StalinAuthUrl, delegate(string newValue)
			{
				this._stalinAuthUrl = newValue;
			}, true);
			this._configurationManager.OnValueChanged<float>(CCVars.StalinDiscordMinimumAge, delegate(float newValue)
			{
				this._minimalDiscordAccountAge = newValue;
			}, true);
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000BFBC File Offset: 0x0000A1BC
		public Task RefreshUsersData()
		{
			StalinManager.<RefreshUsersData>d__13 <RefreshUsersData>d__;
			<RefreshUsersData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RefreshUsersData>d__.<>4__this = this;
			<RefreshUsersData>d__.<>1__state = -1;
			<RefreshUsersData>d__.<>t__builder.Start<StalinManager.<RefreshUsersData>d__13>(ref <RefreshUsersData>d__);
			return <RefreshUsersData>d__.<>t__builder.Task;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000C000 File Offset: 0x0000A200
		[return: TupleElementNames(new string[]
		{
			"allow",
			"errorMessage"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public Task<ValueTuple<bool, string>> AllowEnter(IPlayerSession session, bool requestIfNull = true)
		{
			StalinManager.<AllowEnter>d__14 <AllowEnter>d__;
			<AllowEnter>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
			<AllowEnter>d__.<>4__this = this;
			<AllowEnter>d__.session = session;
			<AllowEnter>d__.requestIfNull = requestIfNull;
			<AllowEnter>d__.<>1__state = -1;
			<AllowEnter>d__.<>t__builder.Start<StalinManager.<AllowEnter>d__14>(ref <AllowEnter>d__);
			return <AllowEnter>d__.<>t__builder.Task;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000C054 File Offset: 0x0000A254
		[return: TupleElementNames(new string[]
		{
			"passed",
			"errorMessage"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1
		})]
		private ValueTuple<bool, string> VerifyDiscordAge(double discordAge)
		{
			if (discordAge < (double)this._minimalDiscordAccountAge)
			{
				long needed = (long)((double)this._minimalDiscordAccountAge - discordAge);
				return new ValueTuple<bool, string>(false, Loc.GetString("stalin-discord-age-check-fail", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("needed", needed)
				}));
			}
			return new ValueTuple<bool, string>(true, string.Empty);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000C0B0 File Offset: 0x0000A2B0
		private double GetDiscordAccountAge(DiscordUserData data)
		{
			return (DateTime.Now - data.DiscordAge).TotalSeconds;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000C0D8 File Offset: 0x0000A2D8
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			StalinManager.<OnPlayerStatusChanged>d__17 <OnPlayerStatusChanged>d__;
			<OnPlayerStatusChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnPlayerStatusChanged>d__.<>4__this = this;
			<OnPlayerStatusChanged>d__.e = e;
			<OnPlayerStatusChanged>d__.<>1__state = -1;
			<OnPlayerStatusChanged>d__.<>t__builder.Start<StalinManager.<OnPlayerStatusChanged>d__17>(ref <OnPlayerStatusChanged>d__);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000C118 File Offset: 0x0000A318
		private Task<DiscordUserData> RequestDiscordUserDataAsync(IPlayerSession session)
		{
			StalinManager.<RequestDiscordUserDataAsync>d__18 <RequestDiscordUserDataAsync>d__;
			<RequestDiscordUserDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DiscordUserData>.Create();
			<RequestDiscordUserDataAsync>d__.<>4__this = this;
			<RequestDiscordUserDataAsync>d__.session = session;
			<RequestDiscordUserDataAsync>d__.<>1__state = -1;
			<RequestDiscordUserDataAsync>d__.<>t__builder.Start<StalinManager.<RequestDiscordUserDataAsync>d__18>(ref <RequestDiscordUserDataAsync>d__);
			return <RequestDiscordUserDataAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000C164 File Offset: 0x0000A364
		private Task<DiscordUsersData> RequestDiscordUsersDataAsync(List<IPlayerSession> sessions)
		{
			StalinManager.<RequestDiscordUsersDataAsync>d__19 <RequestDiscordUsersDataAsync>d__;
			<RequestDiscordUsersDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DiscordUsersData>.Create();
			<RequestDiscordUsersDataAsync>d__.<>4__this = this;
			<RequestDiscordUsersDataAsync>d__.sessions = sessions;
			<RequestDiscordUsersDataAsync>d__.<>1__state = -1;
			<RequestDiscordUsersDataAsync>d__.<>t__builder.Start<StalinManager.<RequestDiscordUsersDataAsync>d__19>(ref <RequestDiscordUsersDataAsync>d__);
			return <RequestDiscordUsersDataAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000C1B0 File Offset: 0x0000A3B0
		private void OnDiscordAuthRequest(DiscordAuthRequest message)
		{
			IPlayerSession playerSession = this._playerManager.GetSessionByChannel(message.MsgChannel);
			string saltedYayca = this.GenerateDiscordAuthUri(playerSession.Name, playerSession.UserId.ToString());
			DiscordAuthResponse response = new DiscordAuthResponse
			{
				Uri = saltedYayca
			};
			this._netManager.ServerSendMessage(response, message.MsgChannel);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000C210 File Offset: 0x0000A410
		private string GenerateDiscordAuthUri(string ckey, string uid)
		{
			string result;
			using (SHA1Managed sha = new SHA1Managed())
			{
				byte[] saltBytes = Encoding.UTF8.GetBytes(this._configurationManager.GetCVar<string>(CCVars.StalinSalt));
				IEnumerable<byte> bytes = Encoding.UTF8.GetBytes(ckey);
				byte[] uidBytes = Encoding.UTF8.GetBytes(uid);
				byte[] saltedBytes = bytes.Concat(uidBytes).Concat(saltBytes).ToArray<byte>();
				string hash = StalinManager.ToHexStr(sha.ComputeHash(saltedBytes));
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
				defaultInterpolatedStringHandler.AppendFormatted(ckey);
				defaultInterpolatedStringHandler.AppendLiteral("@");
				defaultInterpolatedStringHandler.AppendFormatted(uid);
				defaultInterpolatedStringHandler.AppendLiteral("@");
				defaultInterpolatedStringHandler.AppendFormatted(hash);
				string request = WebUtility.UrlEncode(defaultInterpolatedStringHandler.ToStringAndClear());
				result = this._stalinAuthUrl + request;
			}
			return result;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000C2EC File Offset: 0x0000A4EC
		private static string ToHexStr(byte[] hash)
		{
			StringBuilder hex = new StringBuilder(hash.Length * 2);
			foreach (byte b in hash)
			{
				hex.AppendFormat("{0:x2}", b);
			}
			return hex.ToString();
		}

		// Token: 0x04000186 RID: 390
		[Dependency]
		private readonly INetManager _netManager;

		// Token: 0x04000187 RID: 391
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000188 RID: 392
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000189 RID: 393
		[Dependency]
		private readonly ITaskManager _taskManager;

		// Token: 0x0400018A RID: 394
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400018B RID: 395
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x0400018C RID: 396
		private IChatManager _chatManager;

		// Token: 0x0400018D RID: 397
		private readonly Dictionary<string, DiscordUserData> _registeredStalinCache = new Dictionary<string, DiscordUserData>();

		// Token: 0x0400018E RID: 398
		private readonly Dictionary<string, DateTime> _nextStalinAllowedCheck = new Dictionary<string, DateTime>();

		// Token: 0x0400018F RID: 399
		private string _stalinApiUrl = string.Empty;

		// Token: 0x04000190 RID: 400
		private string _stalinAuthUrl = string.Empty;

		// Token: 0x04000191 RID: 401
		private float _minimalDiscordAccountAge;
	}
}
