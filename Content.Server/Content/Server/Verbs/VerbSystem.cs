using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.Popups;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Server.Verbs
{
	// Token: 0x020000CF RID: 207
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VerbSystem : SharedVerbSystem
	{
		// Token: 0x06000399 RID: 921 RVA: 0x00012E70 File Offset: 0x00011070
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<RequestServerVerbsEvent>(new EntitySessionEventHandler<RequestServerVerbsEvent>(this.HandleVerbRequest), null, null);
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00012E8C File Offset: 0x0001108C
		private void HandleVerbRequest(RequestServerVerbsEvent args, EntitySessionEventArgs eventArgs)
		{
			IPlayerSession player = (IPlayerSession)eventArgs.SenderSession;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (!this.EntityManager.EntityExists(args.EntityUid))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 3);
				defaultInterpolatedStringHandler.AppendFormatted("HandleVerbRequest");
				defaultInterpolatedStringHandler.AppendLiteral(" called on a non-existent entity with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(args.EntityUid);
				defaultInterpolatedStringHandler.AppendLiteral(" by player ");
				defaultInterpolatedStringHandler.AppendFormatted<IPlayerSession>(player);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid attached = attachedEntity.GetValueOrDefault();
				bool flag;
				if (args.AdminRequest)
				{
					IPlayerSession playerSession = eventArgs.SenderSession as IPlayerSession;
					if (playerSession != null)
					{
						flag = this._adminMgr.HasAdminFlag(playerSession, AdminFlags.Admin);
						goto IL_102;
					}
				}
				flag = false;
				IL_102:
				bool force = flag;
				List<Type> verbTypes = new List<Type>();
				using (List<string>.Enumerator enumerator = args.VerbTypes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string key = enumerator.Current;
						Type type = Verb.VerbTypes.FirstOrDefault((Type x) => x.Name == key);
						if (type != null)
						{
							verbTypes.Add(type);
						}
						else
						{
							Logger.Error("Unknown verb type received: " + key);
						}
					}
				}
				VerbsResponseEvent response = new VerbsResponseEvent(args.EntityUid, base.GetLocalVerbs(args.EntityUid, attached, verbTypes, force));
				base.RaiseNetworkEvent(response, player.ConnectedClient);
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
			defaultInterpolatedStringHandler.AppendFormatted("HandleVerbRequest");
			defaultInterpolatedStringHandler.AppendLiteral(" called by player ");
			defaultInterpolatedStringHandler.AppendFormatted<IPlayerSession>(player);
			defaultInterpolatedStringHandler.AppendLiteral(" with no attached entity.");
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00013060 File Offset: 0x00011260
		public override void ExecuteVerb(Verb verb, EntityUid user, EntityUid target, bool forced = false)
		{
			if (verb.Disabled)
			{
				if (!string.IsNullOrWhiteSpace(verb.Message))
				{
					this._popupSystem.PopupEntity(verb.Message, user, user, PopupType.Small);
				}
				return;
			}
			this.LogVerb(verb, user, target, forced);
			base.ExecuteVerb(verb, user, target, forced);
		}

		// Token: 0x0600039C RID: 924 RVA: 0x000130B0 File Offset: 0x000112B0
		public void LogVerb(Verb verb, EntityUid user, EntityUid target, bool forced)
		{
			EntityUid? holding = null;
			SharedHandsComponent hands;
			if (base.TryComp<SharedHandsComponent>(user, ref hands))
			{
				EntityUid? activeHandEntity = hands.ActiveHandEntity;
				if (activeHandEntity != null)
				{
					EntityUid heldEntity = activeHandEntity.GetValueOrDefault();
					holding = new EntityUid?(heldEntity);
				}
			}
			HandVirtualItemComponent pull;
			if (holding != null && base.TryComp<HandVirtualItemComponent>(holding, ref pull))
			{
				holding = new EntityUid?(pull.BlockingEntity);
			}
			VerbCategory category = verb.Category;
			string verbText = (((category != null) ? category.Text : null) + " " + verb.Text).Trim();
			string executionText = forced ? "was forced to execute" : "executed";
			LogStringHandler logStringHandler;
			if (holding == null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Verb;
				LogImpact impact = verb.Impact;
				logStringHandler = new LogStringHandler(24, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted(executionText);
				logStringHandler.AppendLiteral(" the [");
				logStringHandler.AppendFormatted(verbText, 0, "verb");
				logStringHandler.AppendLiteral("] verb targeting ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
				adminLogger.Add(type, impact, ref logStringHandler);
				return;
			}
			ISharedAdminLogManager adminLogger2 = this._adminLogger;
			LogType type2 = LogType.Verb;
			LogImpact impact2 = verb.Impact;
			logStringHandler = new LogStringHandler(39, 5);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
			logStringHandler.AppendLiteral(" ");
			logStringHandler.AppendFormatted(executionText);
			logStringHandler.AppendLiteral(" the [");
			logStringHandler.AppendFormatted(verbText, 0, "verb");
			logStringHandler.AppendLiteral("] verb targeting ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
			logStringHandler.AppendLiteral(" while holding ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(holding.Value), "held", "ToPrettyString(holding.Value)");
			adminLogger2.Add(type2, impact2, ref logStringHandler);
		}

		// Token: 0x04000248 RID: 584
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000249 RID: 585
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400024A RID: 586
		[Dependency]
		private readonly IAdminManager _adminMgr;
	}
}
