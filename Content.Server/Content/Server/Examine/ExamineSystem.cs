using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Verbs;
using Content.Shared.Examine;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Players;
using Robust.Shared.Utility;

namespace Content.Server.Examine
{
	// Token: 0x02000524 RID: 1316
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExamineSystem : ExamineSystemShared
	{
		// Token: 0x06001B55 RID: 6997 RVA: 0x00092767 File Offset: 0x00090967
		static ExamineSystem()
		{
			ExamineSystem._entityNotFoundMessage.AddText(Loc.GetString("examine-system-entity-does-not-exist"));
			ExamineSystem._entityOutOfRangeMessage = new FormattedMessage();
			ExamineSystem._entityOutOfRangeMessage.AddText(Loc.GetString("examine-system-cant-see-entity"));
		}

		// Token: 0x06001B56 RID: 6998 RVA: 0x000927A5 File Offset: 0x000909A5
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<ExamineSystemMessages.RequestExamineInfoMessage>(new EntitySessionEventHandler<ExamineSystemMessages.RequestExamineInfoMessage>(this.ExamineInfoRequest), null, null);
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x000927C4 File Offset: 0x000909C4
		public override void SendExamineTooltip(EntityUid player, EntityUid target, FormattedMessage message, bool getVerbs, bool centerAtCursor)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(player, ref actor))
			{
				return;
			}
			IPlayerSession session = actor.PlayerSession;
			SortedSet<Verb> verbs = null;
			if (getVerbs)
			{
				verbs = this._verbSystem.GetLocalVerbs(target, player, typeof(ExamineVerb), false);
			}
			ExamineSystemMessages.ExamineInfoResponseMessage ev = new ExamineSystemMessages.ExamineInfoResponseMessage(target, 0, message, (verbs != null) ? verbs.ToList<Verb>() : null, centerAtCursor, true, true);
			base.RaiseNetworkEvent(ev, session.ConnectedClient);
		}

		// Token: 0x06001B58 RID: 7000 RVA: 0x0009282C File Offset: 0x00090A2C
		private void ExamineInfoRequest(ExamineSystemMessages.RequestExamineInfoMessage request, EntitySessionEventArgs eventArgs)
		{
			IPlayerSession player = (IPlayerSession)eventArgs.SenderSession;
			ICommonSession senderSession = eventArgs.SenderSession;
			INetChannel channel = player.ConnectedClient;
			EntityUid? attachedEntity = senderSession.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid playerEnt = attachedEntity.GetValueOrDefault();
				if (playerEnt.Valid && this.EntityManager.EntityExists(request.EntityUid))
				{
					if (!base.CanExamine(playerEnt, request.EntityUid))
					{
						base.RaiseNetworkEvent(new ExamineSystemMessages.ExamineInfoResponseMessage(request.EntityUid, request.Id, ExamineSystem._entityOutOfRangeMessage, null, true, true, false), channel);
						return;
					}
					SortedSet<Verb> verbs = null;
					if (request.GetVerbs)
					{
						verbs = this._verbSystem.GetLocalVerbs(request.EntityUid, playerEnt, typeof(ExamineVerb), false);
					}
					FormattedMessage text = base.GetExamineText(request.EntityUid, player.AttachedEntity);
					base.RaiseNetworkEvent(new ExamineSystemMessages.ExamineInfoResponseMessage(request.EntityUid, request.Id, text, (verbs != null) ? verbs.ToList<Verb>() : null, true, true, true), channel);
					return;
				}
			}
			base.RaiseNetworkEvent(new ExamineSystemMessages.ExamineInfoResponseMessage(request.EntityUid, request.Id, ExamineSystem._entityNotFoundMessage, null, true, true, true), channel);
		}

		// Token: 0x04001194 RID: 4500
		[Dependency]
		private readonly VerbSystem _verbSystem;

		// Token: 0x04001195 RID: 4501
		private static readonly FormattedMessage _entityNotFoundMessage = new FormattedMessage();

		// Token: 0x04001196 RID: 4502
		private static readonly FormattedMessage _entityOutOfRangeMessage;
	}
}
