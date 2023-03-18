using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Content.Server.Maps;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;

namespace Content.Server.White.RoundNotifications
{
	// Token: 0x0200009A RID: 154
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RoundNotificationsSystem : EntitySystem
	{
		// Token: 0x06000269 RID: 617 RVA: 0x0000D0EC File Offset: 0x0000B2EC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
			base.SubscribeLocalEvent<RoundStartedEvent>(new EntityEventHandler<RoundStartedEvent>(this.OnRoundStarted), null, null);
			base.SubscribeLocalEvent<RoundEndedEvent>(new EntityEventHandler<RoundEndedEvent>(this.OnRoundEnded), null, null);
			this._config.OnValueChanged<string>(CCVars.DiscordRoundWebhook, delegate(string value)
			{
				this._webhookUrl = value;
			}, true);
			this._config.OnValueChanged<string>(CCVars.DiscordRoundRoleId, delegate(string value)
			{
				this._roleId = value;
			}, true);
			this._config.OnValueChanged<bool>(CCVars.DiscordRoundStartOnly, delegate(bool value)
			{
				this._roundStartOnly = value;
			}, true);
			this._sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("notifications");
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000D1A4 File Offset: 0x0000B3A4
		private void OnRoundRestart(RoundRestartCleanupEvent e)
		{
			if (string.IsNullOrEmpty(this._webhookUrl))
			{
				return;
			}
			RoundNotificationsSystem.WebhookPayload webhookPayload = new RoundNotificationsSystem.WebhookPayload
			{
				Content = Loc.GetString("discord-round-new")
			};
			RoundNotificationsSystem.WebhookPayload payload = webhookPayload;
			if (!string.IsNullOrEmpty(this._roleId))
			{
				webhookPayload = new RoundNotificationsSystem.WebhookPayload
				{
					Content = "<@&" + this._roleId + "> " + Loc.GetString("discord-round-new"),
					AllowedMentions = new Dictionary<string, string[]>
					{
						{
							"roles",
							new string[]
							{
								this._roleId
							}
						}
					}
				};
				payload = webhookPayload;
			}
			this.SendDiscordMessage(payload);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000D244 File Offset: 0x0000B444
		private void OnRoundStarted(RoundStartedEvent e)
		{
			if (string.IsNullOrEmpty(this._webhookUrl))
			{
				return;
			}
			GameMapPrototype selectedMap = this._gameMapManager.GetSelectedMap();
			string mapName = ((selectedMap != null) ? selectedMap.MapName : null) ?? Loc.GetString("discord-round-unknown-map");
			string text = Loc.GetString("discord-round-start", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("id", e.RoundId),
				new ValueTuple<string, object>("map", mapName)
			});
			RoundNotificationsSystem.WebhookPayload payload = new RoundNotificationsSystem.WebhookPayload
			{
				Content = text
			};
			this.SendDiscordMessage(payload);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000D2DC File Offset: 0x0000B4DC
		private void OnRoundEnded(RoundEndedEvent e)
		{
			if (string.IsNullOrEmpty(this._webhookUrl) || this._roundStartOnly)
			{
				return;
			}
			string text = Loc.GetString("discord-round-end", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("id", e.RoundId),
				new ValueTuple<string, object>("hours", e.RoundDuration.Hours),
				new ValueTuple<string, object>("minutes", e.RoundDuration.Minutes),
				new ValueTuple<string, object>("seconds", e.RoundDuration.Seconds)
			});
			RoundNotificationsSystem.WebhookPayload payload = new RoundNotificationsSystem.WebhookPayload
			{
				Content = text
			};
			this.SendDiscordMessage(payload);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000D3B0 File Offset: 0x0000B5B0
		private void SendDiscordMessage(RoundNotificationsSystem.WebhookPayload payload)
		{
			RoundNotificationsSystem.<SendDiscordMessage>d__11 <SendDiscordMessage>d__;
			<SendDiscordMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SendDiscordMessage>d__.<>4__this = this;
			<SendDiscordMessage>d__.payload = payload;
			<SendDiscordMessage>d__.<>1__state = -1;
			<SendDiscordMessage>d__.<>t__builder.Start<RoundNotificationsSystem.<SendDiscordMessage>d__11>(ref <SendDiscordMessage>d__);
		}

		// Token: 0x040001C5 RID: 453
		[Dependency]
		private readonly IConfigurationManager _config;

		// Token: 0x040001C6 RID: 454
		[Dependency]
		private readonly IGameMapManager _gameMapManager;

		// Token: 0x040001C7 RID: 455
		private ISawmill _sawmill;

		// Token: 0x040001C8 RID: 456
		private readonly HttpClient _httpClient = new HttpClient();

		// Token: 0x040001C9 RID: 457
		private string _webhookUrl = string.Empty;

		// Token: 0x040001CA RID: 458
		private string _roleId = string.Empty;

		// Token: 0x040001CB RID: 459
		private bool _roundStartOnly;

		// Token: 0x020008B3 RID: 2227
		[Nullable(0)]
		private struct WebhookPayload
		{
			// Token: 0x170007F2 RID: 2034
			// (get) Token: 0x06003022 RID: 12322 RVA: 0x000F934A File Offset: 0x000F754A
			// (set) Token: 0x06003023 RID: 12323 RVA: 0x000F9352 File Offset: 0x000F7552
			[JsonPropertyName("content")]
			public string Content { readonly get; set; }

			// Token: 0x170007F3 RID: 2035
			// (get) Token: 0x06003024 RID: 12324 RVA: 0x000F935B File Offset: 0x000F755B
			// (set) Token: 0x06003025 RID: 12325 RVA: 0x000F9363 File Offset: 0x000F7563
			[JsonPropertyName("allowed_mentions")]
			public Dictionary<string, string[]> AllowedMentions { readonly get; set; }

			// Token: 0x06003026 RID: 12326 RVA: 0x000F936C File Offset: 0x000F756C
			public WebhookPayload()
			{
				this.Content = "";
				this.AllowedMentions = new Dictionary<string, string[]>
				{
					{
						"parse",
						Array.Empty<string>()
					}
				};
			}
		}
	}
}
