using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.GameTicking;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Server.Administration.Systems
{
	// Token: 0x0200080F RID: 2063
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BwoinkSystem : SharedBwoinkSystem
	{
		// Token: 0x06002CD3 RID: 11475 RVA: 0x000ECC78 File Offset: 0x000EAE78
		public override void Initialize()
		{
			base.Initialize();
			this._config.OnValueChanged<string>(CCVars.DiscordAHelpWebhook, new Action<string>(this.OnWebhookChanged), true);
			this._config.OnValueChanged<string>(CCVars.DiscordAHelpFooterIcon, new Action<string>(this.OnFooterIconChanged), true);
			this._config.OnValueChanged<string>(CCVars.DiscordAHelpAvatar, new Action<string>(this.OnAvatarChanged), true);
			this._config.OnValueChanged<string>(CVars.GameHostName, new Action<string>(this.OnServerNameChanged), true);
			this._sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("AHELP");
			this._maxAdditionalChars = BwoinkSystem.GenerateAHelpMessage("", "", true, false).Length;
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnGameRunLevelChanged), null, null);
		}

		// Token: 0x06002CD4 RID: 11476 RVA: 0x000ECD44 File Offset: 0x000EAF44
		private void OnGameRunLevelChanged(GameRunLevelChangedEvent args)
		{
			if (args.Old != GameRunLevel.PreRoundLobby)
			{
				GameRunLevel @new = args.New;
				if (@new == GameRunLevel.PreRoundLobby || @new == GameRunLevel.InRound)
				{
					this._oldMessageIds = new Dictionary<NetUserId, string>();
					foreach (KeyValuePair<NetUserId, ValueTuple<string, string, string, string, GameRunLevel>> message in this._relayMessages)
					{
						string id = message.Value.Item1;
						if (id == null)
						{
							return;
						}
						this._oldMessageIds[message.Key] = id;
					}
					this._relayMessages.Clear();
					return;
				}
			}
		}

		// Token: 0x06002CD5 RID: 11477 RVA: 0x000ECDE4 File Offset: 0x000EAFE4
		private void OnServerNameChanged(string obj)
		{
			this._serverName = obj;
		}

		// Token: 0x06002CD6 RID: 11478 RVA: 0x000ECDF0 File Offset: 0x000EAFF0
		public override void Shutdown()
		{
			base.Shutdown();
			this._config.UnsubValueChanged<string>(CCVars.DiscordAHelpWebhook, new Action<string>(this.OnWebhookChanged));
			this._config.UnsubValueChanged<string>(CCVars.DiscordAHelpFooterIcon, new Action<string>(this.OnFooterIconChanged));
			this._config.UnsubValueChanged<string>(CVars.GameHostName, new Action<string>(this.OnServerNameChanged));
		}

		// Token: 0x06002CD7 RID: 11479 RVA: 0x000ECE58 File Offset: 0x000EB058
		private void OnWebhookChanged(string url)
		{
			this._webhookUrl = url;
			if (url == string.Empty)
			{
				return;
			}
			Match match = Regex.Match(url, "^https://discord\\.com/api/webhooks/(\\d+)/((?!.*/).*)$");
			if (!match.Success)
			{
				Logger.Warning("Webhook URL does not appear to be valid. Using anyways...");
				return;
			}
			if (match.Groups.Count <= 2)
			{
				Logger.Error("Could not get webhook ID or token.");
				return;
			}
			string webhookId = match.Groups[1].Value;
			string webhookToken = match.Groups[2].Value;
			this.SetWebhookData(webhookId, webhookToken);
		}

		// Token: 0x06002CD8 RID: 11480 RVA: 0x000ECEE0 File Offset: 0x000EB0E0
		private Task SetWebhookData(string id, string token)
		{
			BwoinkSystem.<SetWebhookData>d__25 <SetWebhookData>d__;
			<SetWebhookData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SetWebhookData>d__.<>4__this = this;
			<SetWebhookData>d__.id = id;
			<SetWebhookData>d__.token = token;
			<SetWebhookData>d__.<>1__state = -1;
			<SetWebhookData>d__.<>t__builder.Start<BwoinkSystem.<SetWebhookData>d__25>(ref <SetWebhookData>d__);
			return <SetWebhookData>d__.<>t__builder.Task;
		}

		// Token: 0x06002CD9 RID: 11481 RVA: 0x000ECF33 File Offset: 0x000EB133
		private void OnFooterIconChanged(string url)
		{
			this._footerIconUrl = url;
		}

		// Token: 0x06002CDA RID: 11482 RVA: 0x000ECF3C File Offset: 0x000EB13C
		private void OnAvatarChanged(string url)
		{
			this._avatarUrl = url;
		}

		// Token: 0x06002CDB RID: 11483 RVA: 0x000ECF48 File Offset: 0x000EB148
		private void ProcessQueue(NetUserId userId, Queue<string> messages)
		{
			BwoinkSystem.<ProcessQueue>d__28 <ProcessQueue>d__;
			<ProcessQueue>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ProcessQueue>d__.<>4__this = this;
			<ProcessQueue>d__.userId = userId;
			<ProcessQueue>d__.messages = messages;
			<ProcessQueue>d__.<>1__state = -1;
			<ProcessQueue>d__.<>t__builder.Start<BwoinkSystem.<ProcessQueue>d__28>(ref <ProcessQueue>d__);
		}

		// Token: 0x06002CDC RID: 11484 RVA: 0x000ECF90 File Offset: 0x000EB190
		private BwoinkSystem.WebhookPayload GeneratePayload(string messages, string username, [Nullable(2)] string characterName = null)
		{
			if (characterName != null)
			{
				username = username + " (" + characterName + ")";
			}
			int color = (this.GetTargetAdmins().Count > 0) ? 4321431 : 16711680;
			string serverName = this._serverName.Substring(0, Math.Min(this._serverName.Length, 1500));
			string text2;
			switch (this._gameTicker.RunLevel)
			{
			case GameRunLevel.PreRoundLobby:
			{
				string text;
				if (this._gameTicker.RoundId != 0)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
					defaultInterpolatedStringHandler.AppendLiteral("pre-round lobby for round ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(this._gameTicker.RoundId + 1);
					text = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				else
				{
					text = "pre-round lobby after server restart";
				}
				text2 = text;
				break;
			}
			case GameRunLevel.InRound:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler.AppendLiteral("round ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this._gameTicker.RoundId);
				text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				break;
			}
			case GameRunLevel.PostRound:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("post-round ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this._gameTicker.RoundId);
				text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				break;
			}
			default:
			{
				string paramName = "RunLevel";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
				defaultInterpolatedStringHandler.AppendFormatted<GameRunLevel>(this._gameTicker.RunLevel);
				defaultInterpolatedStringHandler.AppendLiteral(" was not matched.");
				throw new ArgumentOutOfRangeException(paramName, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			}
			string round = text2;
			return new BwoinkSystem.WebhookPayload
			{
				Username = username,
				AvatarUrl = (string.IsNullOrWhiteSpace(this._avatarUrl) ? null : this._avatarUrl),
				Embeds = new List<BwoinkSystem.Embed>
				{
					new BwoinkSystem.Embed
					{
						Description = messages,
						Color = color,
						Footer = new BwoinkSystem.EmbedFooter?(new BwoinkSystem.EmbedFooter
						{
							Text = serverName + " (" + round + ")",
							IconUrl = (string.IsNullOrWhiteSpace(this._footerIconUrl) ? null : this._footerIconUrl)
						})
					}
				}
			};
		}

		// Token: 0x06002CDD RID: 11485 RVA: 0x000ED1A4 File Offset: 0x000EB3A4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (NetUserId userId in this._messageQueues.Keys.ToArray<NetUserId>())
			{
				if (!this._processingChannels.Contains(userId))
				{
					Queue<string> queue = this._messageQueues[userId];
					this._messageQueues.Remove(userId);
					if (queue.Count != 0)
					{
						this._processingChannels.Add(userId);
						this.ProcessQueue(userId, queue);
					}
				}
			}
		}

		// Token: 0x06002CDE RID: 11486 RVA: 0x000ED224 File Offset: 0x000EB424
		protected override void OnBwoinkTextMessage(SharedBwoinkSystem.BwoinkTextMessage message, EntitySessionEventArgs eventArgs)
		{
			base.OnBwoinkTextMessage(message, eventArgs);
			IPlayerSession senderSession = (IPlayerSession)eventArgs.SenderSession;
			bool personalChannel = senderSession.UserId == message.UserId;
			AdminData senderAdmin = this._adminManager.GetAdminData(senderSession, false);
			bool senderAHelpAdmin = senderAdmin != null && senderAdmin.HasFlag(AdminFlags.Adminhelp);
			if (!personalChannel && !senderAHelpAdmin)
			{
				return;
			}
			string escapedText = FormattedMessage.EscapeText(message.Text);
			AdminData x = senderAdmin;
			string text;
			if (x != null && x.Flags == AdminFlags.Adminhelp)
			{
				text = "[color=purple]" + senderSession.Name + "[/color]: " + escapedText;
			}
			else
			{
				AdminData x2 = x;
				if (x2 != null && x2.HasFlag(AdminFlags.Adminhelp))
				{
					text = "[color=red]" + senderSession.Name + "[/color]: " + escapedText;
				}
				else
				{
					text = senderSession.Name + ": " + escapedText;
				}
			}
			string bwoinkText = text;
			SharedBwoinkSystem.BwoinkTextMessage msg = new SharedBwoinkSystem.BwoinkTextMessage(message.UserId, senderSession.UserId, bwoinkText, null);
			base.LogBwoink(msg);
			IList<INetChannel> admins = this.GetTargetAdmins();
			foreach (INetChannel channel in admins)
			{
				base.RaiseNetworkEvent(msg, channel);
			}
			IPlayerSession session;
			if (this._playerManager.TryGetSessionById(message.UserId, ref session) && !admins.Contains(session.ConnectedClient))
			{
				base.RaiseNetworkEvent(msg, session.ConnectedClient);
			}
			bool sendsWebhook = this._webhookUrl != string.Empty;
			if (sendsWebhook)
			{
				if (!this._messageQueues.ContainsKey(msg.UserId))
				{
					this._messageQueues[msg.UserId] = new Queue<string>();
				}
				string str = message.Text;
				int unameLength = senderSession.Name.Length;
				if (unameLength + str.Length + this._maxAdditionalChars > 4000)
				{
					str = str.Substring(0, 4000 - this._maxAdditionalChars - unameLength);
				}
				this._messageQueues[msg.UserId].Enqueue(BwoinkSystem.GenerateAHelpMessage(senderSession.Name, str, !personalChannel, admins.Count == 0));
			}
			if (admins.Count != 0)
			{
				return;
			}
			string systemText = sendsWebhook ? Loc.GetString("bwoink-system-starmute-message-no-other-users-webhook") : Loc.GetString("bwoink-system-starmute-message-no-other-users");
			SharedBwoinkSystem.BwoinkTextMessage starMuteMsg = new SharedBwoinkSystem.BwoinkTextMessage(message.UserId, SharedBwoinkSystem.SystemUserId, systemText, null);
			base.RaiseNetworkEvent(starMuteMsg, senderSession.ConnectedClient);
		}

		// Token: 0x06002CDF RID: 11487 RVA: 0x000ED4BC File Offset: 0x000EB6BC
		private IList<INetChannel> GetTargetAdmins()
		{
			return (from p in this._adminManager.ActiveAdmins.Where(delegate(IPlayerSession p)
			{
				AdminData adminData = this._adminManager.GetAdminData(p, false);
				return adminData != null && adminData.HasFlag(AdminFlags.Adminhelp);
			})
			select p.ConnectedClient).ToList<INetChannel>();
		}

		// Token: 0x06002CE0 RID: 11488 RVA: 0x000ED510 File Offset: 0x000EB710
		private static string GenerateAHelpMessage(string username, string message, bool admin, bool noReceivers = false)
		{
			StringBuilder stringbuilder = new StringBuilder();
			if (admin)
			{
				stringbuilder.Append(":outbox_tray:");
			}
			else if (noReceivers)
			{
				stringbuilder.Append(":sos:");
			}
			else
			{
				stringbuilder.Append(":inbox_tray:");
			}
			StringBuilder stringBuilder = stringbuilder;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder);
			appendInterpolatedStringHandler.AppendLiteral(" **");
			appendInterpolatedStringHandler.AppendFormatted(username);
			appendInterpolatedStringHandler.AppendLiteral(":** ");
			stringBuilder2.Append(ref appendInterpolatedStringHandler);
			stringbuilder.Append(message);
			return stringbuilder.ToString();
		}

		// Token: 0x04001BCB RID: 7115
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001BCC RID: 7116
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001BCD RID: 7117
		[Dependency]
		private readonly IConfigurationManager _config;

		// Token: 0x04001BCE RID: 7118
		[Dependency]
		private readonly IPlayerLocator _playerLocator;

		// Token: 0x04001BCF RID: 7119
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x04001BD0 RID: 7120
		private ISawmill _sawmill;

		// Token: 0x04001BD1 RID: 7121
		private readonly HttpClient _httpClient = new HttpClient();

		// Token: 0x04001BD2 RID: 7122
		private string _webhookUrl = string.Empty;

		// Token: 0x04001BD3 RID: 7123
		private BwoinkSystem.WebhookData? _webhookData;

		// Token: 0x04001BD4 RID: 7124
		private string _footerIconUrl = string.Empty;

		// Token: 0x04001BD5 RID: 7125
		private string _avatarUrl = string.Empty;

		// Token: 0x04001BD6 RID: 7126
		private string _serverName = string.Empty;

		// Token: 0x04001BD7 RID: 7127
		[TupleElementNames(new string[]
		{
			"id",
			"username",
			"description",
			"characterName",
			"lastRunLevel"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			2,
			1,
			1,
			2
		})]
		private readonly Dictionary<NetUserId, ValueTuple<string, string, string, string, GameRunLevel>> _relayMessages = new Dictionary<NetUserId, ValueTuple<string, string, string, string, GameRunLevel>>();

		// Token: 0x04001BD8 RID: 7128
		private Dictionary<NetUserId, string> _oldMessageIds = new Dictionary<NetUserId, string>();

		// Token: 0x04001BD9 RID: 7129
		private readonly Dictionary<NetUserId, Queue<string>> _messageQueues = new Dictionary<NetUserId, Queue<string>>();

		// Token: 0x04001BDA RID: 7130
		private readonly HashSet<NetUserId> _processingChannels = new HashSet<NetUserId>();

		// Token: 0x04001BDB RID: 7131
		private const ushort DescriptionMax = 4000;

		// Token: 0x04001BDC RID: 7132
		private const ushort MessageLengthCap = 3000;

		// Token: 0x04001BDD RID: 7133
		private const string TooLongText = "... **(too long)**";

		// Token: 0x04001BDE RID: 7134
		private int _maxAdditionalChars;

		// Token: 0x02000B61 RID: 2913
		[Nullable(0)]
		private struct WebhookPayload
		{
			// Token: 0x170008DF RID: 2271
			// (get) Token: 0x060039BA RID: 14778 RVA: 0x0012D3D7 File Offset: 0x0012B5D7
			// (set) Token: 0x060039BB RID: 14779 RVA: 0x0012D3DF File Offset: 0x0012B5DF
			[JsonPropertyName("username")]
			public string Username { readonly get; set; }

			// Token: 0x170008E0 RID: 2272
			// (get) Token: 0x060039BC RID: 14780 RVA: 0x0012D3E8 File Offset: 0x0012B5E8
			// (set) Token: 0x060039BD RID: 14781 RVA: 0x0012D3F0 File Offset: 0x0012B5F0
			[Nullable(2)]
			[JsonPropertyName("avatar_url")]
			public string AvatarUrl { [NullableContext(2)] readonly get; [NullableContext(2)] set; }

			// Token: 0x170008E1 RID: 2273
			// (get) Token: 0x060039BE RID: 14782 RVA: 0x0012D3F9 File Offset: 0x0012B5F9
			// (set) Token: 0x060039BF RID: 14783 RVA: 0x0012D401 File Offset: 0x0012B601
			[Nullable(2)]
			[JsonPropertyName("embeds")]
			public List<BwoinkSystem.Embed> Embeds { [NullableContext(2)] readonly get; [NullableContext(2)] set; }

			// Token: 0x170008E2 RID: 2274
			// (get) Token: 0x060039C0 RID: 14784 RVA: 0x0012D40A File Offset: 0x0012B60A
			// (set) Token: 0x060039C1 RID: 14785 RVA: 0x0012D412 File Offset: 0x0012B612
			[JsonPropertyName("allowed_mentions")]
			public Dictionary<string, string[]> AllowedMentions { readonly get; set; }

			// Token: 0x060039C2 RID: 14786 RVA: 0x0012D41B File Offset: 0x0012B61B
			public WebhookPayload()
			{
				this.Username = "";
				this.AvatarUrl = "";
				this.Embeds = null;
				this.AllowedMentions = new Dictionary<string, string[]>
				{
					{
						"parse",
						Array.Empty<string>()
					}
				};
			}
		}

		// Token: 0x02000B62 RID: 2914
		[Nullable(0)]
		private struct Embed
		{
			// Token: 0x170008E3 RID: 2275
			// (get) Token: 0x060039C3 RID: 14787 RVA: 0x0012D455 File Offset: 0x0012B655
			// (set) Token: 0x060039C4 RID: 14788 RVA: 0x0012D45D File Offset: 0x0012B65D
			[JsonPropertyName("description")]
			public string Description { readonly get; set; }

			// Token: 0x170008E4 RID: 2276
			// (get) Token: 0x060039C5 RID: 14789 RVA: 0x0012D466 File Offset: 0x0012B666
			// (set) Token: 0x060039C6 RID: 14790 RVA: 0x0012D46E File Offset: 0x0012B66E
			[JsonPropertyName("color")]
			public int Color { readonly get; set; }

			// Token: 0x170008E5 RID: 2277
			// (get) Token: 0x060039C7 RID: 14791 RVA: 0x0012D477 File Offset: 0x0012B677
			// (set) Token: 0x060039C8 RID: 14792 RVA: 0x0012D47F File Offset: 0x0012B67F
			[JsonPropertyName("footer")]
			public BwoinkSystem.EmbedFooter? Footer { readonly get; set; }

			// Token: 0x060039C9 RID: 14793 RVA: 0x0012D488 File Offset: 0x0012B688
			public Embed()
			{
				this.Description = "";
				this.Color = 0;
				this.Footer = null;
			}
		}

		// Token: 0x02000B63 RID: 2915
		[Nullable(0)]
		private struct EmbedFooter
		{
			// Token: 0x170008E6 RID: 2278
			// (get) Token: 0x060039CA RID: 14794 RVA: 0x0012D4A8 File Offset: 0x0012B6A8
			// (set) Token: 0x060039CB RID: 14795 RVA: 0x0012D4B0 File Offset: 0x0012B6B0
			[JsonPropertyName("text")]
			public string Text { readonly get; set; }

			// Token: 0x170008E7 RID: 2279
			// (get) Token: 0x060039CC RID: 14796 RVA: 0x0012D4B9 File Offset: 0x0012B6B9
			// (set) Token: 0x060039CD RID: 14797 RVA: 0x0012D4C1 File Offset: 0x0012B6C1
			[Nullable(2)]
			[JsonPropertyName("icon_url")]
			public string IconUrl { [NullableContext(2)] readonly get; [NullableContext(2)] set; }

			// Token: 0x060039CE RID: 14798 RVA: 0x0012D4CA File Offset: 0x0012B6CA
			public EmbedFooter()
			{
				this.IconUrl = null;
				this.Text = "";
			}
		}

		// Token: 0x02000B64 RID: 2916
		[NullableContext(2)]
		[Nullable(0)]
		private struct WebhookData
		{
			// Token: 0x170008E8 RID: 2280
			// (get) Token: 0x060039CF RID: 14799 RVA: 0x0012D4DE File Offset: 0x0012B6DE
			// (set) Token: 0x060039D0 RID: 14800 RVA: 0x0012D4E6 File Offset: 0x0012B6E6
			[JsonPropertyName("guild_id")]
			public string GuildId { readonly get; set; }

			// Token: 0x170008E9 RID: 2281
			// (get) Token: 0x060039D1 RID: 14801 RVA: 0x0012D4EF File Offset: 0x0012B6EF
			// (set) Token: 0x060039D2 RID: 14802 RVA: 0x0012D4F7 File Offset: 0x0012B6F7
			[JsonPropertyName("channel_id")]
			public string ChannelId { readonly get; set; }

			// Token: 0x060039D3 RID: 14803 RVA: 0x0012D500 File Offset: 0x0012B700
			public WebhookData()
			{
				this.GuildId = null;
				this.ChannelId = null;
			}
		}
	}
}
