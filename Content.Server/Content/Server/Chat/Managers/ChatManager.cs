using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.MoMMI;
using Content.Server.Preferences.Managers;
using Content.Server.UtkaIntegration;
using Content.Server.White.Sponsors;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Preferences;
using Content.Shared.White.Sponsors;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Replays;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Chat.Managers
{
	// Token: 0x020006C8 RID: 1736
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ChatManager : IChatManager
	{
		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06002427 RID: 9255 RVA: 0x000BCA1E File Offset: 0x000BAC1E
		public int MaxMessageLength
		{
			get
			{
				return this._configurationManager.GetCVar<int>(CCVars.ChatMaxMessageLength);
			}
		}

		// Token: 0x06002428 RID: 9256 RVA: 0x000BCA30 File Offset: 0x000BAC30
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<MsgChatMessage>(null, 3);
			this._configurationManager.OnValueChanged<bool>(CCVars.OocEnabled, new Action<bool>(this.OnOocEnabledChanged), true);
			this._configurationManager.OnValueChanged<bool>(CCVars.AdminOocEnabled, new Action<bool>(this.OnAdminOocEnabledChanged), true);
			this._configurationManager.OnValueChanged<int>(CCVars.CooldownAllMessage, delegate(int value)
			{
				this._cooldownAllMessage = value;
			}, true);
		}

		// Token: 0x06002429 RID: 9257 RVA: 0x000BCAA1 File Offset: 0x000BACA1
		public void ClearCache()
		{
			this.LastUserMessage.Clear();
			this.LastTimeUserMessage.Clear();
		}

		// Token: 0x0600242A RID: 9258 RVA: 0x000BCABC File Offset: 0x000BACBC
		private void OnOocEnabledChanged(bool val)
		{
			if (this._oocEnabled == val)
			{
				return;
			}
			this._oocEnabled = val;
			this.DispatchServerAnnouncement(Loc.GetString(val ? "chat-manager-ooc-chat-enabled-message" : "chat-manager-ooc-chat-disabled-message"), null);
		}

		// Token: 0x0600242B RID: 9259 RVA: 0x000BCB00 File Offset: 0x000BAD00
		private void OnAdminOocEnabledChanged(bool val)
		{
			if (this._adminOocEnabled == val)
			{
				return;
			}
			this._adminOocEnabled = val;
			this.DispatchServerAnnouncement(Loc.GetString(val ? "chat-manager-admin-ooc-chat-enabled-message" : "chat-manager-admin-ooc-chat-disabled-message"), null);
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x000BCB44 File Offset: 0x000BAD44
		public void DispatchServerAnnouncement(string message, Color? colorOverride = null)
		{
			string wrappedMessage = Loc.GetString("chat-manager-server-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			this.ChatMessageToAll(ChatChannel.Server, message, wrappedMessage, EntityUid.Invalid, false, true, colorOverride, null, 0f);
			Logger.InfoS("SERVER", message);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Chat;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(21, 1);
			logStringHandler.AppendLiteral("Server announcement: ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x0600242D RID: 9261 RVA: 0x000BCBCC File Offset: 0x000BADCC
		public void DispatchServerMessage(IPlayerSession player, string message, bool suppressLog = false)
		{
			string wrappedMessage = Loc.GetString("chat-manager-server-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			this.ChatMessageToOne(ChatChannel.Server, message, wrappedMessage, default(EntityUid), false, player.ConnectedClient, null, false, null, 0f);
			if (!suppressLog)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Chat;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(20, 2);
				logStringHandler.AppendLiteral("Server message to ");
				logStringHandler.AppendFormatted<IPlayerSession>(player, "Player", "player");
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(message);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
		}

		// Token: 0x0600242E RID: 9262 RVA: 0x000BCC7C File Offset: 0x000BAE7C
		public void SendAdminAnnouncement(string message)
		{
			IEnumerable<INetChannel> clients = from p in this._adminManager.AdminsWithFlag
			select p.ConnectedClient;
			string wrappedMessage = Loc.GetString("chat-manager-send-admin-announcement-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("adminChannelName", Loc.GetString("chat-manager-admin-channel-name")),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			this.ChatMessageToMany(ChatChannel.Admin, message, wrappedMessage, default(EntityUid), false, true, clients, null, null, 0f);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Chat;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(20, 1);
			logStringHandler.AppendLiteral("Admin announcement: ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x0600242F RID: 9263 RVA: 0x000BCD54 File Offset: 0x000BAF54
		public void SendHookOOC(string sender, string message)
		{
			if (this._configurationManager.GetCVar<bool>(CCVars.DisableHookedOOC))
			{
				return;
			}
			string wrappedMessage = Loc.GetString("chat-manager-send-hook-ooc-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("senderName", sender),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			this.ChatMessageToAll(ChatChannel.OOC, message, wrappedMessage, EntityUid.Invalid, false, true, null, null, 0f);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Chat;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(16, 2);
			logStringHandler.AppendLiteral("Hook OOC from ");
			logStringHandler.AppendFormatted(sender);
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06002430 RID: 9264 RVA: 0x000BCE10 File Offset: 0x000BB010
		public void SendHookAdminChat(string sender, string message)
		{
			IEnumerable<IPlayerSession> admins = this._adminManager.ActiveAdmins;
			string wrappedMessage = Loc.GetString("chat-manager-send-admin-chat-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("adminChannelName", Loc.GetString("chat-manager-admin-discord-channel-name")),
				new ValueTuple<string, object>("playerName", sender),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			this.ChatMessageToMany(ChatChannel.Admin, message, wrappedMessage, EntityUid.Invalid, false, true, from p in admins
			select p.ConnectedClient, null, null, 0f);
			UtkaChatEventMessage asayEventMessage = new UtkaChatEventMessage
			{
				Command = "asay",
				Ckey = sender,
				Message = message
			};
			this._utkaSocketWrapper.SendMessageToAll(asayEventMessage);
		}

		// Token: 0x06002431 RID: 9265 RVA: 0x000BCEF4 File Offset: 0x000BB0F4
		public void TrySendOOCMessage(IPlayerSession player, string message, OOCChatType type)
		{
			if (message.Length > this.MaxMessageLength)
			{
				this.DispatchServerMessage(player, Loc.GetString("chat-manager-max-message-length-exceeded-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("limit", this.MaxMessageLength)
				}), false);
				return;
			}
			if (type == OOCChatType.OOC)
			{
				this.SendOOC(player, message);
				return;
			}
			if (type != OOCChatType.Admin)
			{
				return;
			}
			this.SendAdminChat(player, message);
		}

		// Token: 0x06002432 RID: 9266 RVA: 0x000BCF60 File Offset: 0x000BB160
		private void SendOOC(IPlayerSession player, string message)
		{
			if (this.CheckSpamUserMessage(EntityUid.Invalid, message, player, false))
			{
				return;
			}
			if (this._adminManager.IsAdmin(player, false))
			{
				if (!this._adminOocEnabled)
				{
					return;
				}
			}
			else if (!this._oocEnabled)
			{
				return;
			}
			Color? colorOverride = null;
			string wrappedMessage = Loc.GetString("chat-manager-send-ooc-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("playerName", player.Name),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			if (this._adminManager.HasAdminFlag(player, AdminFlags.Admin))
			{
				PlayerPreferences prefs = this._preferencesManager.GetPreferences(player.UserId);
				colorOverride = new Color?(prefs.AdminOOCColor);
			}
			string patron = player.ConnectedClient.UserData.PatronTier;
			string patronColor;
			if (patron != null && ChatManager.PatronOocColors.TryGetValue(patron, out patronColor))
			{
				wrappedMessage = Loc.GetString("chat-manager-send-ooc-patron-wrap-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("patronColor", patronColor),
					new ValueTuple<string, object>("playerName", player.Name),
					new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
				});
			}
			SponsorInfo sponsorData;
			if (this._sponsorsManager.TryGetInfo(player.UserId, out sponsorData) && sponsorData.OOCColor != null)
			{
				wrappedMessage = Loc.GetString("chat-manager-send-ooc-patron-wrap-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("patronColor", sponsorData.OOCColor),
					new ValueTuple<string, object>("playerName", player.Name),
					new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
				});
			}
			this.ChatMessageToAll(ChatChannel.OOC, message, wrappedMessage, EntityUid.Invalid, false, true, colorOverride, null, 0f);
			this._mommiLink.SendOOCMessage(player.Name, message);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Chat;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(11, 2);
			logStringHandler.AppendLiteral("OOC from ");
			logStringHandler.AppendFormatted<IPlayerSession>(player, "Player", "player");
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
			UtkaChatEventMessage toUtkaMessage = new UtkaChatEventMessage
			{
				Command = "ooc",
				Ckey = player.Name,
				Message = message
			};
			this._utkaSocketWrapper.SendMessageToAll(toUtkaMessage);
		}

		// Token: 0x06002433 RID: 9267 RVA: 0x000BD1A8 File Offset: 0x000BB3A8
		private void SendAdminChat(IPlayerSession player, string message)
		{
			LogStringHandler logStringHandler;
			if (!this._adminManager.HasAdminFlag(player, AdminFlags.Admin))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Chat;
				LogImpact impact = LogImpact.Extreme;
				logStringHandler = new LogStringHandler(50, 1);
				logStringHandler.AppendFormatted<IPlayerSession>(player, "Player", "player");
				logStringHandler.AppendLiteral(" attempted to send admin message but was not admin");
				adminLogger.Add(type, impact, ref logStringHandler);
				return;
			}
			IEnumerable<INetChannel> clients = from p in this._adminManager.ActiveAdmins
			where this._adminManager.HasAdminFlag(p, AdminFlags.Admin)
			select p.ConnectedClient;
			string wrappedMessage = Loc.GetString("chat-manager-send-admin-chat-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("adminChannelName", Loc.GetString("chat-manager-admin-channel-name")),
				new ValueTuple<string, object>("playerName", player.Name),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			foreach (INetChannel client in clients)
			{
				bool isSource = client != player.ConnectedClient;
				ChatChannel channel = ChatChannel.AdminChat;
				string wrappedMessage2 = wrappedMessage;
				EntityUid source = default(EntityUid);
				bool hideChat = false;
				INetChannel client2 = client;
				string audioPath = isSource ? this._netConfigManager.GetClientCVar<string>(client, CCVars.AdminChatSoundPath) : null;
				float audioVolume = isSource ? this._netConfigManager.GetClientCVar<float>(client, CCVars.AdminChatSoundVolume) : 0f;
				this.ChatMessageToOne(channel, message, wrappedMessage2, source, hideChat, client2, null, false, audioPath, audioVolume);
			}
			ISharedAdminLogManager adminLogger2 = this._adminLogger;
			LogType type2 = LogType.Chat;
			logStringHandler = new LogStringHandler(18, 2);
			logStringHandler.AppendLiteral("Admin chat from ");
			logStringHandler.AppendFormatted<IPlayerSession>(player, "Player", "player");
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(message);
			adminLogger2.Add(type2, ref logStringHandler);
			UtkaChatEventMessage asayEventMessage = new UtkaChatEventMessage
			{
				Command = "asay",
				Ckey = player.Name,
				Message = message
			};
			this._utkaSocketWrapper.SendMessageToAll(asayEventMessage);
		}

		// Token: 0x06002434 RID: 9268 RVA: 0x000BD3C4 File Offset: 0x000BB5C4
		public void CheckMessageCoolDown(IPlayerSession player, Dictionary<NetUserId, TimeSpan> lastSendMessageStorage, int coolDown, out int remainingTime)
		{
			if (this._adminManager.HasAdminFlag(player, AdminFlags.Admin))
			{
				remainingTime = -1;
				return;
			}
			if (lastSendMessageStorage.ContainsKey(player.UserId))
			{
				TimeSpan delta = this._gameTiming.CurTime.Subtract(lastSendMessageStorage[player.UserId]);
				TimeSpan coolDownСonverted = TimeSpan.FromSeconds((double)coolDown);
				if (!(delta >= coolDownСonverted))
				{
					remainingTime = (int)Math.Ceiling(coolDownСonverted.Subtract(delta).TotalSeconds);
					return;
				}
				lastSendMessageStorage[player.UserId] = this._gameTiming.CurTime;
			}
			else
			{
				lastSendMessageStorage.Add(player.UserId, this._gameTiming.CurTime);
			}
			remainingTime = -1;
		}

		// Token: 0x06002435 RID: 9269 RVA: 0x000BD474 File Offset: 0x000BB674
		public bool CheckSpamUserMessage(EntityUid source, string message, [Nullable(2)] IPlayerSession player, bool hideChat)
		{
			if (player == null)
			{
				return true;
			}
			if (this._adminManager.HasAdminFlag(player, AdminFlags.Admin))
			{
				return false;
			}
			if (this.LastUserMessage.ContainsKey(player.UserId) && this.LastUserMessage[player.UserId] == message)
			{
				string mes = Loc.GetString("chat-manager-warn-spam-message");
				this.ChatMessageToOne(ChatChannel.LOOC, mes, mes, source, hideChat, player.ConnectedClient, new Color?(Color.White), false, null, 0f);
				return true;
			}
			this.LastUserMessage[player.UserId] = message;
			int remainingTime;
			this.CheckMessageCoolDown(player, this.LastTimeUserMessage, this._cooldownAllMessage, out remainingTime);
			if (remainingTime != -1)
			{
				string mes2 = Loc.GetString("chat-manager-cooldown-warn-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("remainingTime", remainingTime)
				});
				this.ChatMessageToOne(ChatChannel.LOOC, mes2, mes2, source, hideChat, player.ConnectedClient, new Color?(Color.White), false, null, 0f);
				return true;
			}
			return false;
		}

		// Token: 0x06002436 RID: 9270 RVA: 0x000BD570 File Offset: 0x000BB770
		public void ChatMessageToOne(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, INetChannel client, Color? colorOverride = null, bool recordReplay = false, [Nullable(2)] string audioPath = null, float audioVolume = 0f)
		{
			ChatMessage msg = new ChatMessage(channel, message, wrappedMessage, source, hideChat, colorOverride, audioPath, audioVolume);
			this._netManager.ServerSendMessage(new MsgChatMessage
			{
				Message = msg
			}, client);
			if (recordReplay)
			{
				this._replay.QueueReplayMessage(msg);
			}
		}

		// Token: 0x06002437 RID: 9271 RVA: 0x000BD5BC File Offset: 0x000BB7BC
		public void ChatMessageToMany(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, bool recordReplay, IEnumerable<INetChannel> clients, Color? colorOverride = null, [Nullable(2)] string audioPath = null, float audioVolume = 0f)
		{
			this.ChatMessageToMany(channel, message, wrappedMessage, source, hideChat, recordReplay, clients.ToList<INetChannel>(), colorOverride, audioPath, audioVolume);
		}

		// Token: 0x06002438 RID: 9272 RVA: 0x000BD5E8 File Offset: 0x000BB7E8
		public void ChatMessageToMany(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, bool recordReplay, List<INetChannel> clients, Color? colorOverride = null, [Nullable(2)] string audioPath = null, float audioVolume = 0f)
		{
			ChatMessage msg = new ChatMessage(channel, message, wrappedMessage, source, hideChat, colorOverride, audioPath, audioVolume);
			this._netManager.ServerSendToMany(new MsgChatMessage
			{
				Message = msg
			}, clients);
			if (recordReplay)
			{
				this._replay.QueueReplayMessage(msg);
			}
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x000BD634 File Offset: 0x000BB834
		public void ChatMessageToManyFiltered(Filter filter, ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, bool recordReplay, Color? colorOverride = null, [Nullable(2)] string audioPath = null, float audioVolume = 0f)
		{
			if (!recordReplay && !filter.Recipients.Any<ICommonSession>())
			{
				return;
			}
			List<INetChannel> clients = new List<INetChannel>();
			foreach (ICommonSession recipient in filter.Recipients)
			{
				clients.Add(recipient.ConnectedClient);
			}
			this.ChatMessageToMany(channel, message, wrappedMessage, source, hideChat, recordReplay, clients, colorOverride, audioPath, audioVolume);
		}

		// Token: 0x0600243A RID: 9274 RVA: 0x000BD6B8 File Offset: 0x000BB8B8
		public void ChatMessageToAll(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, bool recordReplay, Color? colorOverride = null, [Nullable(2)] string audioPath = null, float audioVolume = 0f)
		{
			ChatMessage msg = new ChatMessage(channel, message, wrappedMessage, source, hideChat, colorOverride, audioPath, audioVolume);
			this._netManager.ServerSendToAll(new MsgChatMessage
			{
				Message = msg
			});
			if (recordReplay)
			{
				this._replay.QueueReplayMessage(msg);
			}
		}

		// Token: 0x0600243B RID: 9275 RVA: 0x000BD700 File Offset: 0x000BB900
		public bool MessageCharacterLimit([Nullable(2)] IPlayerSession player, string message)
		{
			bool isOverLength = false;
			if (player == null)
			{
				return false;
			}
			if (message.Length > this.MaxMessageLength)
			{
				string feedback = Loc.GetString("chat-manager-max-message-length-exceeded-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("limit", this.MaxMessageLength)
				});
				this.DispatchServerMessage(player, feedback, false);
				isOverLength = true;
			}
			return isOverLength;
		}

		// Token: 0x04001667 RID: 5735
		private static readonly Dictionary<string, string> PatronOocColors = new Dictionary<string, string>
		{
			{
				"nuclear_operative",
				"#aa00ff"
			},
			{
				"syndicate_agent",
				"#aa00ff"
			},
			{
				"revolutionary",
				"#aa00ff"
			}
		};

		// Token: 0x04001668 RID: 5736
		[Dependency]
		private readonly IReplayRecordingManager _replay;

		// Token: 0x04001669 RID: 5737
		[Dependency]
		private readonly IServerNetManager _netManager;

		// Token: 0x0400166A RID: 5738
		[Dependency]
		private readonly IMoMMILink _mommiLink;

		// Token: 0x0400166B RID: 5739
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x0400166C RID: 5740
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400166D RID: 5741
		[Dependency]
		private readonly IServerPreferencesManager _preferencesManager;

		// Token: 0x0400166E RID: 5742
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x0400166F RID: 5743
		[Dependency]
		private readonly SponsorsManager _sponsorsManager;

		// Token: 0x04001670 RID: 5744
		[Dependency]
		private readonly UtkaTCPWrapper _utkaSocketWrapper;

		// Token: 0x04001671 RID: 5745
		[Dependency]
		private readonly INetConfigurationManager _netConfigManager;

		// Token: 0x04001672 RID: 5746
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001673 RID: 5747
		public Dictionary<NetUserId, string> LastUserMessage = new Dictionary<NetUserId, string>();

		// Token: 0x04001674 RID: 5748
		public Dictionary<NetUserId, TimeSpan> LastTimeUserMessage = new Dictionary<NetUserId, TimeSpan>();

		// Token: 0x04001675 RID: 5749
		private int _cooldownAllMessage;

		// Token: 0x04001676 RID: 5750
		private bool _oocEnabled = true;

		// Token: 0x04001677 RID: 5751
		private bool _adminOocEnabled = true;
	}
}
