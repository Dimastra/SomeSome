using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.Ghost.Components;
using Content.Server.Players;
using Content.Server.Popups;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.UtkaIntegration;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Systems;
using Content.Shared.Radio;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Replays;
using Robust.Shared.Utility;

namespace Content.Server.Chat.Systems
{
	// Token: 0x020006BE RID: 1726
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChatSystem : SharedChatSystem
	{
		// Token: 0x060023E4 RID: 9188 RVA: 0x000BAD54 File Offset: 0x000B8F54
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeEmotes();
			this._configurationManager.OnValueChanged<bool>(CCVars.LoocEnabled, new Action<bool>(this.OnLoocEnabledChanged), true);
			this._configurationManager.OnValueChanged<bool>(CCVars.DeadLoocEnabled, new Action<bool>(this.OnDeadLoocEnabledChanged), true);
			this._configurationManager.OnValueChanged<int>(CCVars.CooldownLOOCMessage, delegate(int value)
			{
				this._cooldownLOOCMessage = value;
			}, true);
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnGameChange), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnClear), null, null);
		}

		// Token: 0x060023E5 RID: 9189 RVA: 0x000BADEC File Offset: 0x000B8FEC
		private void OnClear(RoundRestartCleanupEvent ev)
		{
			this._chatManager.ClearCache();
			this.LOOCCooldownRecordUser.Clear();
		}

		// Token: 0x060023E6 RID: 9190 RVA: 0x000BAE04 File Offset: 0x000B9004
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownEmotes();
			this._configurationManager.UnsubValueChanged<bool>(CCVars.LoocEnabled, new Action<bool>(this.OnLoocEnabledChanged));
			this._chatManager.ClearCache();
			this.LOOCCooldownRecordUser.Clear();
		}

		// Token: 0x060023E7 RID: 9191 RVA: 0x000BAE44 File Offset: 0x000B9044
		private void OnLoocEnabledChanged(bool val)
		{
			if (this._loocEnabled == val)
			{
				return;
			}
			this._loocEnabled = val;
			this._chatManager.DispatchServerAnnouncement(Loc.GetString(val ? "chat-manager-looc-chat-enabled-message" : "chat-manager-looc-chat-disabled-message"), null);
		}

		// Token: 0x060023E8 RID: 9192 RVA: 0x000BAE8C File Offset: 0x000B908C
		private void OnDeadLoocEnabledChanged(bool val)
		{
			if (this._deadLoocEnabled == val)
			{
				return;
			}
			this._deadLoocEnabled = val;
			this._chatManager.DispatchServerAnnouncement(Loc.GetString(val ? "chat-manager-dead-looc-chat-enabled-message" : "chat-manager-dead-looc-chat-disabled-message"), null);
		}

		// Token: 0x060023E9 RID: 9193 RVA: 0x000BAED4 File Offset: 0x000B90D4
		private void OnGameChange(GameRunLevelChangedEvent ev)
		{
			if (this._configurationManager.GetCVar<bool>(CCVars.OocEnableDuringRound))
			{
				return;
			}
			if (ev.New == GameRunLevel.InRound)
			{
				this._configurationManager.SetCVar<bool>(CCVars.OocEnabled, false, false);
				return;
			}
			if (ev.New == GameRunLevel.PostRound)
			{
				this._configurationManager.SetCVar<bool>(CCVars.OocEnabled, true, false);
			}
		}

		// Token: 0x060023EA RID: 9194 RVA: 0x000BAF2C File Offset: 0x000B912C
		[NullableContext(2)]
		public void TrySendInGameICMessage(EntityUid source, [Nullable(1)] string message, InGameICChatType desiredType, bool hideChat, bool hideGlobalGhostChat = false, IConsoleShell shell = null, IPlayerSession player = null, string nameOverride = null, bool checkRadioPrefix = true, bool force = false)
		{
			if (base.HasComp<GhostComponent>(source))
			{
				if (desiredType == InGameICChatType.Emote)
				{
					return;
				}
				this.TrySendInGameOOCMessage(source, message, InGameOOCChatType.Dead, hideChat, shell, player);
				return;
			}
			else
			{
				if (this._chatManager.CheckSpamUserMessage(source, message, player, hideChat))
				{
					return;
				}
				EntityUid? entityUid = (player != null) ? player.AttachedEntity : null;
				if (entityUid != null)
				{
					EntityUid entity = entityUid.GetValueOrDefault();
					if (entity.Valid && source != entity)
					{
						return;
					}
				}
				if (!force && !this.CanSendInGame(message, shell, player))
				{
					return;
				}
				if (desiredType == InGameICChatType.Speak && message.StartsWith('.'))
				{
					checkRadioPrefix = false;
					string text = message;
					message = text.Substring(1, text.Length - 1);
				}
				hideGlobalGhostChat = (hideGlobalGhostChat || hideChat);
				bool shouldCapitalize = desiredType != InGameICChatType.Emote;
				bool shouldPunctuate = this._configurationManager.GetCVar<bool>(CCVars.ChatPunctuation);
				bool sanitizeSlang = this._configurationManager.GetCVar<bool>(CCVars.ChatSlangFilter);
				string emoteStr;
				message = this.SanitizeInGameICMessage(source, message, out emoteStr, shouldCapitalize, shouldPunctuate, sanitizeSlang);
				if (emoteStr != message && emoteStr != null)
				{
					this.SendEntityEmote(source, emoteStr, hideChat, hideGlobalGhostChat, nameOverride, force, true);
				}
				if (string.IsNullOrEmpty(message))
				{
					return;
				}
				string modMessage;
				RadioChannelPrototype channel;
				if (checkRadioPrefix && base.TryProccessRadioMessage(source, message, out modMessage, out channel, false))
				{
					this.SendEntityWhisper(source, modMessage, hideChat, hideGlobalGhostChat, channel, nameOverride);
					return;
				}
				switch (desiredType)
				{
				case InGameICChatType.Speak:
					this.SendEntitySpeak(source, message, hideChat, hideGlobalGhostChat, nameOverride);
					return;
				case InGameICChatType.Emote:
					this.SendEntityEmote(source, message, hideChat, hideGlobalGhostChat, nameOverride, force, true);
					return;
				case InGameICChatType.Whisper:
					this.SendEntityWhisper(source, message, hideChat, hideGlobalGhostChat, null, nameOverride);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x060023EB RID: 9195 RVA: 0x000BB0B4 File Offset: 0x000B92B4
		[NullableContext(2)]
		public void TrySendInGameOOCMessage(EntityUid source, [Nullable(1)] string message, InGameOOCChatType type, bool hideChat, IConsoleShell shell = null, IPlayerSession player = null)
		{
			if (!this.CanSendInGame(message, shell, player))
			{
				return;
			}
			EntityUid? entityUid = (player != null) ? player.AttachedEntity : null;
			if (entityUid != null)
			{
				EntityUid entity = entityUid.GetValueOrDefault();
				if (entity.Valid && !(source != entity))
				{
					message = this.SanitizeInGameOOCMessage(message);
					InGameOOCChatType sendType = type;
					if (!this._adminManager.IsAdmin(player, false) && !this._deadLoocEnabled && (base.HasComp<GhostComponent>(source) || this._mobStateSystem.IsDead(source, null)))
					{
						sendType = InGameOOCChatType.Dead;
					}
					if (sendType != InGameOOCChatType.Looc)
					{
						if (sendType == InGameOOCChatType.Dead)
						{
							this.SendDeadChat(source, player, message, hideChat);
							return;
						}
					}
					else
					{
						this.SendLOOC(source, player, message, hideChat);
					}
					return;
				}
			}
		}

		// Token: 0x060023EC RID: 9196 RVA: 0x000BB168 File Offset: 0x000B9368
		public void DispatchGlobalAnnouncement(string message, string sender = "Central Command", bool playSound = true, [Nullable(2)] SoundSpecifier announcementSound = null, Color? colorOverride = null)
		{
			string wrappedMessage = Loc.GetString("chat-manager-sender-announcement-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("sender", sender),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			this._chatManager.ChatMessageToAll(ChatChannel.Radio, message, wrappedMessage, default(EntityUid), false, true, colorOverride, null, 0f);
			if (playSound)
			{
				SoundSystem.Play(((announcementSound != null) ? announcementSound.GetSound(null, null) : null) ?? "/Audio/Announcements/announce.ogg", Filter.Broadcast(), new AudioParams?(AudioParams.Default.WithVolume(-2f)));
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Chat;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(35, 2);
			logStringHandler.AppendLiteral("Global station announcement from ");
			logStringHandler.AppendFormatted(sender);
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x060023ED RID: 9197 RVA: 0x000BB250 File Offset: 0x000B9450
		public void DispatchStationAnnouncement(EntityUid source, string message, string sender = "Central Command", bool playDefaultSound = true, [Nullable(2)] SoundSpecifier announcementSound = null, Color? colorOverride = null)
		{
			string wrappedMessage = Loc.GetString("chat-manager-sender-announcement-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("sender", sender),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			EntityUid? station = this._stationSystem.GetOwningStation(source, null);
			if (station == null)
			{
				return;
			}
			StationDataComponent stationDataComp;
			if (!this.EntityManager.TryGetComponent<StationDataComponent>(station, ref stationDataComp))
			{
				return;
			}
			Filter filter = this._stationSystem.GetInStation(stationDataComp, 32f);
			this._chatManager.ChatMessageToManyFiltered(filter, ChatChannel.Radio, message, wrappedMessage, source, false, true, colorOverride, null, 0f);
			if (playDefaultSound)
			{
				SoundSystem.Play(((announcementSound != null) ? announcementSound.GetSound(null, null) : null) ?? "/Audio/Announcements/announce.ogg", filter, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Chat;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(32, 3);
			logStringHandler.AppendLiteral("Station Announcement on ");
			logStringHandler.AppendFormatted<EntityUid?>(station, "station");
			logStringHandler.AppendLiteral(" from ");
			logStringHandler.AppendFormatted(sender);
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x060023EE RID: 9198 RVA: 0x000BB380 File Offset: 0x000B9580
		private void SendEntitySpeak(EntityUid source, string originalMessage, bool hideChat, bool hideGlobalGhostChat, [Nullable(2)] string nameOverride)
		{
			if (!this._actionBlocker.CanSpeak(source))
			{
				return;
			}
			string message = this.TransformSpeech(source, originalMessage);
			if (message.Length == 0)
			{
				return;
			}
			message = this.AfterSpeechTransformed(source, message);
			string name;
			if (nameOverride != null)
			{
				name = nameOverride;
			}
			else
			{
				TransformSpeakerNameEvent nameEv = new TransformSpeakerNameEvent(source, base.Name(source, null));
				base.RaiseLocalEvent<TransformSpeakerNameEvent>(source, nameEv, false);
				name = nameEv.Name;
			}
			SetSpeakerColorEvent colorEv = new SetSpeakerColorEvent(source, name);
			base.RaiseLocalEvent<SetSpeakerColorEvent>(source, colorEv, false);
			name = colorEv.Name;
			string wrappedMessage = Loc.GetString("chat-manager-entity-say-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entityName", name),
				new ValueTuple<string, object>("message", message)
			});
			this.SendInVoiceRange(ChatChannel.Local, message, wrappedMessage, source, hideChat, hideGlobalGhostChat);
			EntitySpokeEvent ev = new EntitySpokeEvent(source, message, originalMessage, null, null);
			base.RaiseLocalEvent<EntitySpokeEvent>(source, ev, true);
			if (!base.HasComp<ActorComponent>(source))
			{
				return;
			}
			if (originalMessage == message)
			{
				LogStringHandler logStringHandler;
				if (name != base.Name(source, null))
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Chat;
					LogImpact impact = LogImpact.Low;
					logStringHandler = new LogStringHandler(16, 3);
					logStringHandler.AppendLiteral("Say from ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
					logStringHandler.AppendLiteral(" as ");
					logStringHandler.AppendFormatted(name);
					logStringHandler.AppendLiteral(": ");
					logStringHandler.AppendFormatted(originalMessage);
					logStringHandler.AppendLiteral(".");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Chat;
				LogImpact impact2 = LogImpact.Low;
				logStringHandler = new LogStringHandler(12, 2);
				logStringHandler.AppendLiteral("Say from ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(originalMessage);
				logStringHandler.AppendLiteral(".");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
				return;
			}
			else
			{
				LogStringHandler logStringHandler;
				if (name != base.Name(source, null))
				{
					ISharedAdminLogManager adminLogger3 = this._adminLogger;
					LogType type3 = LogType.Chat;
					LogImpact impact3 = LogImpact.Low;
					logStringHandler = new LogStringHandler(41, 4);
					logStringHandler.AppendLiteral("Say from ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
					logStringHandler.AppendLiteral(" as ");
					logStringHandler.AppendFormatted(name);
					logStringHandler.AppendLiteral(", original: ");
					logStringHandler.AppendFormatted(originalMessage);
					logStringHandler.AppendLiteral(", transformed: ");
					logStringHandler.AppendFormatted(message);
					logStringHandler.AppendLiteral(".");
					adminLogger3.Add(type3, impact3, ref logStringHandler);
					return;
				}
				ISharedAdminLogManager adminLogger4 = this._adminLogger;
				LogType type4 = LogType.Chat;
				LogImpact impact4 = LogImpact.Low;
				logStringHandler = new LogStringHandler(37, 3);
				logStringHandler.AppendLiteral("Say from ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
				logStringHandler.AppendLiteral(", original: ");
				logStringHandler.AppendFormatted(originalMessage);
				logStringHandler.AppendLiteral(", transformed: ");
				logStringHandler.AppendFormatted(message);
				logStringHandler.AppendLiteral(".");
				adminLogger4.Add(type4, impact4, ref logStringHandler);
				return;
			}
		}

		// Token: 0x060023EF RID: 9199 RVA: 0x000BB658 File Offset: 0x000B9858
		[NullableContext(2)]
		private void SendEntityWhisper(EntityUid source, [Nullable(1)] string originalMessage, bool hideChat, bool hideGlobalGhostChat, RadioChannelPrototype channel, string nameOverride)
		{
			if (!this._actionBlocker.CanSpeak(source))
			{
				return;
			}
			string message = this.TransformSpeech(source, originalMessage);
			if (message.Length == 0)
			{
				return;
			}
			message = this.AfterSpeechTransformed(source, message);
			string obfuscatedMessage = this.ObfuscateMessageReadability(message, 0.2f);
			string name;
			if (nameOverride != null)
			{
				name = nameOverride;
			}
			else
			{
				TransformSpeakerNameEvent nameEv = new TransformSpeakerNameEvent(source, base.Name(source, null));
				base.RaiseLocalEvent<TransformSpeakerNameEvent>(source, nameEv, false);
				name = nameEv.Name;
			}
			string wrappedMessage = Loc.GetString("chat-manager-entity-whisper-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entityName", name),
				new ValueTuple<string, object>("message", message)
			});
			string wrappedobfuscatedMessage = Loc.GetString("chat-manager-entity-whisper-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entityName", name),
				new ValueTuple<string, object>("message", obfuscatedMessage)
			});
			foreach (KeyValuePair<ICommonSession, ChatSystem.ICChatRecipientData> keyValuePair in this.GetRecipients(source, 10f))
			{
				ICommonSession commonSession;
				ChatSystem.ICChatRecipientData icchatRecipientData;
				keyValuePair.Deconstruct(out commonSession, out icchatRecipientData);
				ICommonSession session = commonSession;
				ChatSystem.ICChatRecipientData data = icchatRecipientData;
				EntityUid? attachedEntity = session.AttachedEntity;
				if (attachedEntity != null && attachedEntity.GetValueOrDefault().Valid && (!hideGlobalGhostChat || !data.Observer || data.Range >= 0f))
				{
					if (data.Range <= 2f)
					{
						this._chatManager.ChatMessageToOne(ChatChannel.Whisper, message, wrappedMessage, source, data.HideChatOverride ?? hideChat, session.ConnectedClient, null, false, null, 0f);
					}
					else
					{
						this._chatManager.ChatMessageToOne(ChatChannel.Whisper, obfuscatedMessage, wrappedobfuscatedMessage, source, data.HideChatOverride ?? hideChat, session.ConnectedClient, null, false, null, 0f);
					}
				}
			}
			this._replay.QueueReplayMessage(new ChatMessage(ChatChannel.Whisper, message, wrappedMessage, source, hideChat, null, null, 0f));
			EntitySpokeEvent ev = new EntitySpokeEvent(source, message, originalMessage, channel, obfuscatedMessage);
			base.RaiseLocalEvent<EntitySpokeEvent>(source, ev, true);
			if (originalMessage == message)
			{
				LogStringHandler logStringHandler;
				if (name != base.Name(source, null))
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Chat;
					LogImpact impact = LogImpact.Low;
					logStringHandler = new LogStringHandler(20, 3);
					logStringHandler.AppendLiteral("Whisper from ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
					logStringHandler.AppendLiteral(" as ");
					logStringHandler.AppendFormatted(name);
					logStringHandler.AppendLiteral(": ");
					logStringHandler.AppendFormatted(originalMessage);
					logStringHandler.AppendLiteral(".");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Chat;
				LogImpact impact2 = LogImpact.Low;
				logStringHandler = new LogStringHandler(16, 2);
				logStringHandler.AppendLiteral("Whisper from ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(originalMessage);
				logStringHandler.AppendLiteral(".");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
				return;
			}
			else
			{
				LogStringHandler logStringHandler;
				if (name != base.Name(source, null))
				{
					ISharedAdminLogManager adminLogger3 = this._adminLogger;
					LogType type3 = LogType.Chat;
					LogImpact impact3 = LogImpact.Low;
					logStringHandler = new LogStringHandler(45, 4);
					logStringHandler.AppendLiteral("Whisper from ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
					logStringHandler.AppendLiteral(" as ");
					logStringHandler.AppendFormatted(name);
					logStringHandler.AppendLiteral(", original: ");
					logStringHandler.AppendFormatted(originalMessage);
					logStringHandler.AppendLiteral(", transformed: ");
					logStringHandler.AppendFormatted(message);
					logStringHandler.AppendLiteral(".");
					adminLogger3.Add(type3, impact3, ref logStringHandler);
					return;
				}
				ISharedAdminLogManager adminLogger4 = this._adminLogger;
				LogType type4 = LogType.Chat;
				LogImpact impact4 = LogImpact.Low;
				logStringHandler = new LogStringHandler(41, 3);
				logStringHandler.AppendLiteral("Whisper from ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
				logStringHandler.AppendLiteral(", original: ");
				logStringHandler.AppendFormatted(originalMessage);
				logStringHandler.AppendLiteral(", transformed: ");
				logStringHandler.AppendFormatted(message);
				logStringHandler.AppendLiteral(".");
				adminLogger4.Add(type4, impact4, ref logStringHandler);
				return;
			}
		}

		// Token: 0x060023F0 RID: 9200 RVA: 0x000BBAB4 File Offset: 0x000B9CB4
		private void SendEntityEmote(EntityUid source, string action, bool hideChat, bool hideGlobalGhostChat, [Nullable(2)] string nameOverride, bool force = false, bool checkEmote = true)
		{
			if (!force && !this._actionBlocker.CanEmote(source))
			{
				return;
			}
			string name = FormattedMessage.EscapeText(nameOverride ?? Identity.Name(source, this.EntityManager, null));
			string wrappedMessage = Loc.GetString("chat-manager-entity-me-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entityName", name),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(action))
			});
			if (checkEmote)
			{
				this.TryEmoteChatInput(source, action);
			}
			this.SendInVoiceRange(ChatChannel.Emotes, action, wrappedMessage, source, hideChat, hideGlobalGhostChat);
			if (name != base.Name(source, null))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Chat;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(17, 3);
				logStringHandler.AppendLiteral("Emote from ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
				logStringHandler.AppendLiteral(" as ");
				logStringHandler.AppendFormatted(name);
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(action);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Chat;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(13, 2);
				logStringHandler.AppendLiteral("Emote from ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(action);
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			string ckey = string.Empty;
			ActorComponent actorComponent;
			if (base.TryComp<ActorComponent>(source, ref actorComponent))
			{
				ckey = actorComponent.PlayerSession.Name;
			}
			if (string.IsNullOrEmpty(ckey))
			{
				return;
			}
			UtkaChatMeEvent utkaEmoteEvent = new UtkaChatMeEvent
			{
				Ckey = ckey,
				Message = action,
				CharacterName = base.MetaData(source).EntityName
			};
			this._utkaSockets.SendMessageToAll(utkaEmoteEvent);
		}

		// Token: 0x060023F1 RID: 9201 RVA: 0x000BBC74 File Offset: 0x000B9E74
		private void SendLOOC(EntityUid source, IPlayerSession player, string message, bool hideChat)
		{
			int remainingTime;
			this._chatManager.CheckMessageCoolDown(player, this.LOOCCooldownRecordUser, this._cooldownLOOCMessage, out remainingTime);
			if (remainingTime != -1)
			{
				string mes = Loc.GetString("chat-manager-cooldown-warn-message_channel", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("inChat", "в LOOC"),
					new ValueTuple<string, object>("remainingTime", remainingTime)
				});
				this._chatManager.ChatMessageToOne(ChatChannel.LOOC, mes, mes, source, hideChat, player.ConnectedClient, new Color?(Color.White), false, null, 0f);
				return;
			}
			string name = FormattedMessage.EscapeText(Identity.Name(source, this.EntityManager, null));
			if (this._adminManager.IsAdmin(player, false))
			{
				if (!this._adminLoocEnabled)
				{
					return;
				}
			}
			else if (!this._loocEnabled)
			{
				return;
			}
			string wrappedMessage = Loc.GetString("chat-manager-entity-looc-wrap-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entityName", name),
				new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
			});
			this.SendInVoiceRange(ChatChannel.LOOC, message, wrappedMessage, source, hideChat, false);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Chat;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(12, 2);
			logStringHandler.AppendLiteral("LOOC from ");
			logStringHandler.AppendFormatted<IPlayerSession>(player, "Player", "player");
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x060023F2 RID: 9202 RVA: 0x000BBDD8 File Offset: 0x000B9FD8
		private void SendDeadChat(EntityUid source, IPlayerSession player, string message, bool hideChat)
		{
			if (this._chatManager.CheckSpamUserMessage(source, message, player, hideChat))
			{
				return;
			}
			IEnumerable<INetChannel> clients = this.GetDeadChatClients();
			string playerName = base.Name(source, null);
			string wrappedMessage;
			if (this._adminManager.HasAdminFlag(player, AdminFlags.Admin))
			{
				wrappedMessage = Loc.GetString("chat-manager-send-admin-dead-chat-wrap-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("adminChannelName", Loc.GetString("chat-manager-admin-channel-name")),
					new ValueTuple<string, object>("userName", player.ConnectedClient.UserName),
					new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
				});
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Chat;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(23, 2);
				logStringHandler.AppendLiteral("Admin dead chat from ");
				logStringHandler.AppendFormatted<IPlayerSession>(player, "Player", "player");
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(message);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				wrappedMessage = Loc.GetString("chat-manager-send-dead-chat-wrap-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("deadChannelName", Loc.GetString("chat-manager-dead-channel-name")),
					new ValueTuple<string, object>("playerName", playerName),
					new ValueTuple<string, object>("message", FormattedMessage.EscapeText(message))
				});
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Chat;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(17, 2);
				logStringHandler.AppendLiteral("Dead chat from ");
				logStringHandler.AppendFormatted<IPlayerSession>(player, "Player", "player");
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(message);
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			this._chatManager.ChatMessageToMany(ChatChannel.Dead, message, wrappedMessage, source, hideChat, false, clients.ToList<INetChannel>(), null, null, 0f);
		}

		// Token: 0x060023F3 RID: 9203 RVA: 0x000BBF98 File Offset: 0x000BA198
		private void SendInVoiceRange(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, bool hideGlobalGhostChat)
		{
			foreach (KeyValuePair<ICommonSession, ChatSystem.ICChatRecipientData> keyValuePair in this.GetRecipients(source, 10f))
			{
				ICommonSession commonSession;
				ChatSystem.ICChatRecipientData icchatRecipientData;
				keyValuePair.Deconstruct(out commonSession, out icchatRecipientData);
				ICommonSession session = commonSession;
				ChatSystem.ICChatRecipientData data = icchatRecipientData;
				bool entHideChat = data.HideChatOverride ?? (hideChat || (hideGlobalGhostChat && data.Observer && data.Range < 0f));
				this._chatManager.ChatMessageToOne(channel, message, wrappedMessage, source, entHideChat, session.ConnectedClient, null, false, null, 0f);
			}
			this._replay.QueueReplayMessage(new ChatMessage(channel, message, wrappedMessage, source, hideChat, null, null, 0f));
		}

		// Token: 0x060023F4 RID: 9204 RVA: 0x000BC094 File Offset: 0x000BA294
		[NullableContext(2)]
		private bool CanSendInGame([Nullable(1)] string message, IConsoleShell shell = null, IPlayerSession player = null)
		{
			if (player == null)
			{
				return true;
			}
			PlayerData playerData = player.ContentData();
			if (((playerData != null) ? playerData.Mind : null) == null)
			{
				if (shell != null)
				{
					shell.WriteError("You don't have a mind!");
				}
				return false;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity == null || !attachedEntity.GetValueOrDefault().Valid)
			{
				if (shell != null)
				{
					shell.WriteError("You don't have an entity!");
				}
				return false;
			}
			return !this._chatManager.MessageCharacterLimit(player, message);
		}

		// Token: 0x060023F5 RID: 9205 RVA: 0x000BC10C File Offset: 0x000BA30C
		private string SanitizeInGameICMessage(EntityUid source, string message, [Nullable(2)] out string emoteStr, bool capitalize = true, bool punctuate = false, bool sanitizeSlang = true)
		{
			string newMessage = message.Trim();
			if (sanitizeSlang)
			{
				newMessage = this._sanitizer.SanitizeOutSlang(newMessage);
			}
			if (capitalize)
			{
				newMessage = base.SanitizeMessageCapital(newMessage);
			}
			if (punctuate)
			{
				newMessage = this.SanitizeMessagePeriod(newMessage);
			}
			this._sanitizer.TrySanitizeOutSmilies(newMessage, source, out newMessage, out emoteStr);
			return newMessage;
		}

		// Token: 0x060023F6 RID: 9206 RVA: 0x000BC15B File Offset: 0x000BA35B
		private string SanitizeInGameOOCMessage(string message)
		{
			return FormattedMessage.EscapeText(message.Trim());
		}

		// Token: 0x060023F7 RID: 9207 RVA: 0x000BC168 File Offset: 0x000BA368
		public string TransformSpeech(EntityUid sender, string message)
		{
			TransformSpeechEvent ev = new TransformSpeechEvent(sender, message);
			base.RaiseLocalEvent<TransformSpeechEvent>(ev);
			return ev.Message;
		}

		// Token: 0x060023F8 RID: 9208 RVA: 0x000BC18C File Offset: 0x000BA38C
		public string AfterSpeechTransformed(EntityUid sender, string message)
		{
			SpeechTransformedEvent ev = new SpeechTransformedEvent(sender, message);
			base.RaiseLocalEvent<SpeechTransformedEvent>(ev);
			return ev.Message;
		}

		// Token: 0x060023F9 RID: 9209 RVA: 0x000BC1B0 File Offset: 0x000BA3B0
		private IEnumerable<INetChannel> GetDeadChatClients()
		{
			return from p in Filter.Empty().AddWhereAttachedEntity(new Predicate<EntityUid>(base.HasComp<GhostComponent>)).Recipients.Union(this._adminManager.AdminsWithFlag)
			select p.ConnectedClient;
		}

		// Token: 0x060023FA RID: 9210 RVA: 0x000BC20C File Offset: 0x000BA40C
		private string SanitizeMessagePeriod(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return message;
			}
			string text = message;
			if (char.IsLetter(text[text.Length - 1]))
			{
				message += ".";
			}
			return message;
		}

		// Token: 0x060023FB RID: 9211 RVA: 0x000BC23C File Offset: 0x000BA43C
		private Dictionary<ICommonSession, ChatSystem.ICChatRecipientData> GetRecipients(EntityUid source, float voiceRange)
		{
			Dictionary<ICommonSession, ChatSystem.ICChatRecipientData> recipients = new Dictionary<ICommonSession, ChatSystem.ICChatRecipientData>();
			EntityQuery<GhostComponent> ghosts = base.GetEntityQuery<GhostComponent>();
			EntityQuery<TransformComponent> xforms = base.GetEntityQuery<TransformComponent>();
			TransformComponent component = xforms.GetComponent(source);
			MapId sourceMapId = component.MapID;
			EntityCoordinates sourceCoords = component.Coordinates;
			foreach (ICommonSession player in this._playerManager.Sessions)
			{
				EntityUid? attachedEntity = player.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid playerEntity = attachedEntity.GetValueOrDefault();
					if (playerEntity.Valid)
					{
						TransformComponent transformEntity = xforms.GetComponent(playerEntity);
						if (!(transformEntity.MapID != sourceMapId))
						{
							bool observer = ghosts.HasComponent(playerEntity);
							float distance;
							if (sourceCoords.TryDistance(this.EntityManager, transformEntity.Coordinates, ref distance) && distance < voiceRange)
							{
								recipients.Add(player, new ChatSystem.ICChatRecipientData(distance, observer, null));
							}
							else if (observer)
							{
								recipients.Add(player, new ChatSystem.ICChatRecipientData(-1f, true, null));
							}
						}
					}
				}
			}
			base.RaiseLocalEvent<ExpandICChatRecipientstEvent>(new ExpandICChatRecipientstEvent(source, voiceRange, recipients));
			return recipients;
		}

		// Token: 0x060023FC RID: 9212 RVA: 0x000BC378 File Offset: 0x000BA578
		private string ObfuscateMessageReadability(string message, float chance)
		{
			StringBuilder modifiedMessage = new StringBuilder(message);
			for (int i = 0; i < message.Length; i++)
			{
				if (!char.IsWhiteSpace(modifiedMessage[i]) && RandomExtensions.Prob(this._random, 1f - chance))
				{
					modifiedMessage[i] = '~';
				}
			}
			return modifiedMessage.ToString();
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x000BC3CE File Offset: 0x000BA5CE
		private void InitializeEmotes()
		{
			this._prototypeManager.PrototypesReloaded += this.OnPrototypeReloadEmotes;
			this.CacheEmotes();
		}

		// Token: 0x060023FE RID: 9214 RVA: 0x000BC3ED File Offset: 0x000BA5ED
		private void ShutdownEmotes()
		{
			this._prototypeManager.PrototypesReloaded -= this.OnPrototypeReloadEmotes;
		}

		// Token: 0x060023FF RID: 9215 RVA: 0x000BC406 File Offset: 0x000BA606
		private void OnPrototypeReloadEmotes(PrototypesReloadedEventArgs obj)
		{
			this.CacheEmotes();
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x000BC410 File Offset: 0x000BA610
		private void CacheEmotes()
		{
			this._wordEmoteDict.Clear();
			foreach (EmotePrototype emote in this._prototypeManager.EnumeratePrototypes<EmotePrototype>())
			{
				if (emote.ChatTriggers != null)
				{
					foreach (string text in emote.ChatTriggers)
					{
						string lowerWord = text.ToLower();
						if (this._wordEmoteDict.ContainsKey(lowerWord))
						{
							string existingId = this._wordEmoteDict[lowerWord].ID;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 3);
							defaultInterpolatedStringHandler.AppendLiteral("Duplicate of emote word ");
							defaultInterpolatedStringHandler.AppendFormatted(lowerWord);
							defaultInterpolatedStringHandler.AppendLiteral(" in emotes ");
							defaultInterpolatedStringHandler.AppendFormatted(emote.ID);
							defaultInterpolatedStringHandler.AppendLiteral(" and ");
							defaultInterpolatedStringHandler.AppendFormatted(existingId);
							Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							this._wordEmoteDict.Add(lowerWord, emote);
						}
					}
				}
			}
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x000BC548 File Offset: 0x000BA748
		public void TryEmoteWithChat(EntityUid source, string emoteId, bool hideChat = false, bool hideGlobalGhostChat = false, [Nullable(2)] string nameOverride = null)
		{
			EmotePrototype proto;
			if (!this._prototypeManager.TryIndex<EmotePrototype>(emoteId, ref proto))
			{
				return;
			}
			this.TryEmoteWithChat(source, proto, hideChat, hideGlobalGhostChat, nameOverride);
		}

		// Token: 0x06002402 RID: 9218 RVA: 0x000BC574 File Offset: 0x000BA774
		public void TryEmoteWithChat(EntityUid source, EmotePrototype emote, bool hideChat = false, bool hideGlobalGhostChat = false, [Nullable(2)] string nameOverride = null)
		{
			if (emote.ChatMessages.Count != 0)
			{
				string action = RandomExtensions.Pick<string>(this._random, emote.ChatMessages);
				this.SendEntityEmote(source, action, hideChat, hideGlobalGhostChat, nameOverride, false, true);
			}
			this.TryEmoteWithoutChat(source, emote);
		}

		// Token: 0x06002403 RID: 9219 RVA: 0x000BC5B8 File Offset: 0x000BA7B8
		public void TryEmoteWithoutChat(EntityUid uid, string emoteId)
		{
			EmotePrototype proto;
			if (!this._prototypeManager.TryIndex<EmotePrototype>(emoteId, ref proto))
			{
				return;
			}
			this.TryEmoteWithoutChat(uid, proto);
		}

		// Token: 0x06002404 RID: 9220 RVA: 0x000BC5DE File Offset: 0x000BA7DE
		public void TryEmoteWithoutChat(EntityUid uid, EmotePrototype proto)
		{
			if (!this._actionBlocker.CanEmote(uid))
			{
				return;
			}
			this.InvokeEmoteEvent(uid, proto);
		}

		// Token: 0x06002405 RID: 9221 RVA: 0x000BC5F7 File Offset: 0x000BA7F7
		public bool TryPlayEmoteSound(EntityUid uid, [Nullable(2)] EmoteSoundsPrototype proto, EmotePrototype emote)
		{
			return this.TryPlayEmoteSound(uid, proto, emote.ID);
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x000BC608 File Offset: 0x000BA808
		public bool TryPlayEmoteSound(EntityUid uid, [Nullable(2)] EmoteSoundsPrototype proto, string emoteId)
		{
			if (proto == null)
			{
				return false;
			}
			SoundSpecifier sound;
			if (!proto.Sounds.TryGetValue(emoteId, out sound))
			{
				sound = proto.FallbackSound;
				if (sound == null)
				{
					return false;
				}
			}
			AudioParams param = proto.GeneralParams ?? sound.Params;
			this._audio.PlayPvs(sound, uid, new AudioParams?(param));
			return true;
		}

		// Token: 0x06002407 RID: 9223 RVA: 0x000BC66C File Offset: 0x000BA86C
		private void TryEmoteChatInput(EntityUid uid, string textInput)
		{
			string actionLower = textInput.ToLower();
			EmotePrototype emote;
			if (!this._wordEmoteDict.TryGetValue(actionLower, out emote))
			{
				return;
			}
			this.InvokeEmoteEvent(uid, emote);
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x000BC69C File Offset: 0x000BA89C
		private void InvokeEmoteEvent(EntityUid uid, EmotePrototype proto)
		{
			EmoteEvent ev = new EmoteEvent(proto);
			base.RaiseLocalEvent<EmoteEvent>(uid, ref ev, false);
		}

		// Token: 0x04001635 RID: 5685
		[Dependency]
		private readonly IReplayRecordingManager _replay;

		// Token: 0x04001636 RID: 5686
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04001637 RID: 5687
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04001638 RID: 5688
		[Dependency]
		private readonly IChatSanitizationManager _sanitizer;

		// Token: 0x04001639 RID: 5689
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x0400163A RID: 5690
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400163B RID: 5691
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400163C RID: 5692
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400163D RID: 5693
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400163E RID: 5694
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x0400163F RID: 5695
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x04001640 RID: 5696
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04001641 RID: 5697
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04001642 RID: 5698
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04001643 RID: 5699
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04001644 RID: 5700
		[Dependency]
		private readonly UtkaTCPWrapper _utkaSockets;

		// Token: 0x04001645 RID: 5701
		public Dictionary<NetUserId, TimeSpan> LOOCCooldownRecordUser = new Dictionary<NetUserId, TimeSpan>();

		// Token: 0x04001646 RID: 5702
		public const int VoiceRange = 10;

		// Token: 0x04001647 RID: 5703
		public const int WhisperRange = 2;

		// Token: 0x04001648 RID: 5704
		public const string DefaultAnnouncementSound = "/Audio/Announcements/announce.ogg";

		// Token: 0x04001649 RID: 5705
		private int _cooldownLOOCMessage;

		// Token: 0x0400164A RID: 5706
		private bool _loocEnabled = true;

		// Token: 0x0400164B RID: 5707
		private bool _deadLoocEnabled;

		// Token: 0x0400164C RID: 5708
		private readonly bool _adminLoocEnabled = true;

		// Token: 0x0400164D RID: 5709
		private readonly Dictionary<string, EmotePrototype> _wordEmoteDict = new Dictionary<string, EmotePrototype>();

		// Token: 0x02000AFB RID: 2811
		[NullableContext(0)]
		public readonly struct ICChatRecipientData : IEquatable<ChatSystem.ICChatRecipientData>
		{
			// Token: 0x0600368F RID: 13967 RVA: 0x00121945 File Offset: 0x0011FB45
			public ICChatRecipientData(float Range, bool Observer, bool? HideChatOverride = null)
			{
				this.Range = Range;
				this.Observer = Observer;
				this.HideChatOverride = HideChatOverride;
			}

			// Token: 0x17000863 RID: 2147
			// (get) Token: 0x06003690 RID: 13968 RVA: 0x0012195C File Offset: 0x0011FB5C
			// (set) Token: 0x06003691 RID: 13969 RVA: 0x00121964 File Offset: 0x0011FB64
			public float Range { get; set; }

			// Token: 0x17000864 RID: 2148
			// (get) Token: 0x06003692 RID: 13970 RVA: 0x0012196D File Offset: 0x0011FB6D
			// (set) Token: 0x06003693 RID: 13971 RVA: 0x00121975 File Offset: 0x0011FB75
			public bool Observer { get; set; }

			// Token: 0x17000865 RID: 2149
			// (get) Token: 0x06003694 RID: 13972 RVA: 0x0012197E File Offset: 0x0011FB7E
			// (set) Token: 0x06003695 RID: 13973 RVA: 0x00121986 File Offset: 0x0011FB86
			public bool? HideChatOverride { get; set; }

			// Token: 0x06003696 RID: 13974 RVA: 0x00121990 File Offset: 0x0011FB90
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("ICChatRecipientData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003697 RID: 13975 RVA: 0x001219DC File Offset: 0x0011FBDC
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Range = ");
				builder.Append(this.Range.ToString());
				builder.Append(", Observer = ");
				builder.Append(this.Observer.ToString());
				builder.Append(", HideChatOverride = ");
				builder.Append(this.HideChatOverride.ToString());
				return true;
			}

			// Token: 0x06003698 RID: 13976 RVA: 0x00121A5F File Offset: 0x0011FC5F
			[CompilerGenerated]
			public static bool operator !=(ChatSystem.ICChatRecipientData left, ChatSystem.ICChatRecipientData right)
			{
				return !(left == right);
			}

			// Token: 0x06003699 RID: 13977 RVA: 0x00121A6B File Offset: 0x0011FC6B
			[CompilerGenerated]
			public static bool operator ==(ChatSystem.ICChatRecipientData left, ChatSystem.ICChatRecipientData right)
			{
				return left.Equals(right);
			}

			// Token: 0x0600369A RID: 13978 RVA: 0x00121A75 File Offset: 0x0011FC75
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<float>.Default.GetHashCode(this.<Range>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Observer>k__BackingField)) * -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(this.<HideChatOverride>k__BackingField);
			}

			// Token: 0x0600369B RID: 13979 RVA: 0x00121AB5 File Offset: 0x0011FCB5
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return obj is ChatSystem.ICChatRecipientData && this.Equals((ChatSystem.ICChatRecipientData)obj);
			}

			// Token: 0x0600369C RID: 13980 RVA: 0x00121AD0 File Offset: 0x0011FCD0
			[CompilerGenerated]
			public bool Equals(ChatSystem.ICChatRecipientData other)
			{
				return EqualityComparer<float>.Default.Equals(this.<Range>k__BackingField, other.<Range>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Observer>k__BackingField, other.<Observer>k__BackingField) && EqualityComparer<bool?>.Default.Equals(this.<HideChatOverride>k__BackingField, other.<HideChatOverride>k__BackingField);
			}

			// Token: 0x0600369D RID: 13981 RVA: 0x00121B25 File Offset: 0x0011FD25
			[CompilerGenerated]
			public void Deconstruct(out float Range, out bool Observer, out bool? HideChatOverride)
			{
				Range = this.Range;
				Observer = this.Observer;
				HideChatOverride = this.HideChatOverride;
			}
		}
	}
}
