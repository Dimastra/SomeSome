using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Bible.Components;
using Content.Server.Chat.Managers;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Prayer
{
	// Token: 0x02000270 RID: 624
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PrayerSystem : EntitySystem
	{
		// Token: 0x06000C7A RID: 3194 RVA: 0x000411F2 File Offset: 0x0003F3F2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PrayableComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<PrayableComponent, GetVerbsEvent<ActivationVerb>>(this.AddPrayVerb), null, null);
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x00041210 File Offset: 0x0003F410
		private void AddPrayVerb(EntityUid uid, PrayableComponent comp, GetVerbsEvent<ActivationVerb> args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			if (!args.CanAccess)
			{
				return;
			}
			Action<string> <>9__1;
			ActivationVerb prayerVerb = new ActivationVerb
			{
				Text = Loc.GetString(comp.Verb),
				Icon = comp.VerbImage,
				Act = delegate()
				{
					BibleUserComponent bibleUser;
					if (comp.BibleUserOnly && !this.EntityManager.TryGetComponent<BibleUserComponent>(args.User, ref bibleUser))
					{
						this._popupSystem.PopupEntity(Loc.GetString("prayer-popup-notify-locked"), uid, actor.PlayerSession, PopupType.Large);
						return;
					}
					QuickDialogSystem quickDialog = this._quickDialog;
					IPlayerSession playerSession = actor.PlayerSession;
					string @string = Loc.GetString(comp.Verb);
					string prompt = "Message";
					Action<string> okAction;
					if ((okAction = <>9__1) == null)
					{
						okAction = (<>9__1 = delegate(string message)
						{
							this.Pray(actor.PlayerSession, comp, message);
						});
					}
					quickDialog.OpenDialog<string>(playerSession, @string, prompt, okAction, null);
				},
				Impact = LogImpact.Low
			};
			prayerVerb.Impact = LogImpact.Low;
			args.Verbs.Add(prayerVerb);
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x000412CC File Offset: 0x0003F4CC
		public void SendSubtleMessage(IPlayerSession target, IPlayerSession source, string messageString, string popupMessage)
		{
			if (target.AttachedEntity == null)
			{
				return;
			}
			string message = (popupMessage == "") ? "" : (popupMessage + ((messageString == "") ? "" : (" \"" + messageString + "\"")));
			this._popupSystem.PopupEntity(popupMessage, target.AttachedEntity.Value, target, PopupType.Large);
			this._chatManager.ChatMessageToOne(ChatChannel.Local, messageString, message, EntityUid.Invalid, false, target.ConnectedClient, null, false, null, 0f);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AdminMessage;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(32, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target.AttachedEntity.Value), "player", "ToPrettyString(target.AttachedEntity.Value)");
			logStringHandler.AppendLiteral(" received subtle message from ");
			logStringHandler.AppendFormatted(source.Name);
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x000413E0 File Offset: 0x0003F5E0
		public void Pray(IPlayerSession sender, PrayableComponent comp, string message)
		{
			if (sender.AttachedEntity == null)
			{
				return;
			}
			this._popupSystem.PopupEntity(Loc.GetString(comp.SentMessage), sender.AttachedEntity.Value, sender, PopupType.Medium);
			IChatManager chatManager = this._chatManager;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 3);
			defaultInterpolatedStringHandler.AppendFormatted(Loc.GetString(comp.NotifiactionPrefix));
			defaultInterpolatedStringHandler.AppendLiteral(" <");
			defaultInterpolatedStringHandler.AppendFormatted(sender.Name);
			defaultInterpolatedStringHandler.AppendLiteral(">: ");
			defaultInterpolatedStringHandler.AppendFormatted(message);
			chatManager.SendAdminAnnouncement(defaultInterpolatedStringHandler.ToStringAndClear());
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AdminMessage;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(17, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(sender.AttachedEntity.Value), "player", "ToPrettyString(sender.AttachedEntity.Value)");
			logStringHandler.AppendLiteral(" sent prayer (");
			logStringHandler.AppendFormatted(Loc.GetString(comp.NotifiactionPrefix));
			logStringHandler.AppendLiteral("): ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x040007B1 RID: 1969
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040007B2 RID: 1970
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040007B3 RID: 1971
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x040007B4 RID: 1972
		[Dependency]
		private readonly QuickDialogSystem _quickDialog;
	}
}
