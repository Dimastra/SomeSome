using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Server.Radio.Components;
using Content.Server.VoiceMask;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Replays;
using Robust.Shared.Utility;

namespace Content.Server.Radio.EntitySystems
{
	// Token: 0x0200025C RID: 604
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadioSystem : EntitySystem
	{
		// Token: 0x06000BFC RID: 3068 RVA: 0x0003F3C2 File Offset: 0x0003D5C2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<IntrinsicRadioReceiverComponent, RadioReceiveEvent>(new ComponentEventHandler<IntrinsicRadioReceiverComponent, RadioReceiveEvent>(this.OnIntrinsicReceive), null, null);
			base.SubscribeLocalEvent<IntrinsicRadioTransmitterComponent, EntitySpokeEvent>(new ComponentEventHandler<IntrinsicRadioTransmitterComponent, EntitySpokeEvent>(this.OnIntrinsicSpeak), null, null);
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x0003F3F4 File Offset: 0x0003D5F4
		private void OnIntrinsicSpeak(EntityUid uid, IntrinsicRadioTransmitterComponent component, EntitySpokeEvent args)
		{
			if (args.Channel != null && component.Channels.Contains(args.Channel.ID))
			{
				this.SendRadioMessage(uid, args.Message, args.Channel, null);
				args.Channel = null;
			}
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0003F444 File Offset: 0x0003D644
		private void OnIntrinsicReceive(EntityUid uid, IntrinsicRadioReceiverComponent component, RadioReceiveEvent args)
		{
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(uid, ref actor))
			{
				this._netMan.ServerSendMessage(args.ChatMsg, actor.PlayerSession.ConnectedClient);
			}
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x0003F478 File Offset: 0x0003D678
		public void SendRadioMessage(EntityUid source, string message, RadioChannelPrototype channel, EntityUid? radioSource = null)
		{
			if (!this._messages.Add(message))
			{
				return;
			}
			VoiceMaskComponent mask;
			string name = (base.TryComp<VoiceMaskComponent>(source, ref mask) && mask.Enabled) ? mask.VoiceName : base.MetaData(source).EntityName;
			name = FormattedMessage.EscapeText(name);
			ChatMessage chat = new ChatMessage(ChatChannel.Radio, message, Loc.GetString("chat-radio-message-wrap", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", channel.Color),
				new ValueTuple<string, object>("channel", "\\[" + channel.LocalizedName + "\\]"),
				new ValueTuple<string, object>("name", name),
				new ValueTuple<string, object>("message", message)
			}), EntityUid.Invalid, false, null, null, 0f);
			MsgChatMessage chatMsg = new MsgChatMessage
			{
				Message = chat
			};
			RadioReceiveEvent ev = new RadioReceiveEvent(message, source, channel, chatMsg, radioSource);
			RadioReceiveAttemptEvent attemptEv = new RadioReceiveAttemptEvent(message, source, channel, radioSource);
			bool sentAtLeastOnce = false;
			foreach (ActiveRadioComponent activeRadioComponent in base.EntityQuery<ActiveRadioComponent>(false))
			{
				EntityUid ent = activeRadioComponent.Owner;
				if (activeRadioComponent.Channels.Contains(channel.ID))
				{
					base.RaiseLocalEvent<RadioReceiveAttemptEvent>(ent, attemptEv, false);
					if (attemptEv.Cancelled)
					{
						attemptEv.Uncancel();
					}
					else
					{
						sentAtLeastOnce = true;
						base.RaiseLocalEvent<RadioReceiveEvent>(ent, ev, false);
					}
				}
			}
			if (!sentAtLeastOnce)
			{
				this._popupSystem.PopupEntity(Loc.GetString("failed-to-send-message"), source, source, PopupType.MediumCaution);
			}
			if (name != base.Name(source, null))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Chat;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(29, 4);
				logStringHandler.AppendLiteral("Radio message from ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
				logStringHandler.AppendLiteral(" as ");
				logStringHandler.AppendFormatted(name);
				logStringHandler.AppendLiteral(" on ");
				logStringHandler.AppendFormatted(channel.LocalizedName);
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(message);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Chat;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(25, 3);
				logStringHandler.AppendLiteral("Radio message from ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "user", "ToPrettyString(source)");
				logStringHandler.AppendLiteral(" on ");
				logStringHandler.AppendFormatted(channel.LocalizedName);
				logStringHandler.AppendLiteral(": ");
				logStringHandler.AppendFormatted(message);
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			this._replay.QueueReplayMessage(chat);
			this._messages.Remove(message);
		}

		// Token: 0x0400077F RID: 1919
		[Dependency]
		private readonly INetManager _netMan;

		// Token: 0x04000780 RID: 1920
		[Dependency]
		private readonly IReplayRecordingManager _replay;

		// Token: 0x04000781 RID: 1921
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000782 RID: 1922
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000783 RID: 1923
		private readonly HashSet<string> _messages = new HashSet<string>();
	}
}
