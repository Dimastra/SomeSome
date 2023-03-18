using System;
using System.Runtime.CompilerServices;
using Content.Server.Labels.Components;
using Content.Server.Popups;
using Content.Server.UserInterface;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Labels;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Labels
{
	// Token: 0x02000425 RID: 1061
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandLabelerSystem : EntitySystem
	{
		// Token: 0x06001577 RID: 5495 RVA: 0x00070A28 File Offset: 0x0006EC28
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HandLabelerComponent, AfterInteractEvent>(new ComponentEventHandler<HandLabelerComponent, AfterInteractEvent>(this.AfterInteractOn), null, null);
			base.SubscribeLocalEvent<HandLabelerComponent, ActivateInWorldEvent>(new ComponentEventHandler<HandLabelerComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<HandLabelerComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<HandLabelerComponent, GetVerbsEvent<UtilityVerb>>(this.OnUtilityVerb), null, null);
			base.SubscribeLocalEvent<HandLabelerComponent, HandLabelerLabelChangedMessage>(new ComponentEventHandler<HandLabelerComponent, HandLabelerLabelChangedMessage>(this.OnHandLabelerLabelChanged), null, null);
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x00070A8C File Offset: 0x0006EC8C
		private void OnUtilityVerb(EntityUid uid, HandLabelerComponent handLabeler, GetVerbsEvent<UtilityVerb> args)
		{
			EntityUid target = args.Target;
			if (!target.Valid || !handLabeler.Whitelist.IsValid(target, null) || !args.CanAccess)
			{
				return;
			}
			string labelerText = (handLabeler.AssignedLabel == string.Empty) ? Loc.GetString("hand-labeler-remove-label-text") : Loc.GetString("hand-labeler-add-label-text");
			UtilityVerb verb = new UtilityVerb
			{
				Act = delegate()
				{
					string result;
					this.AddLabelTo(uid, handLabeler, target, out result);
					if (result != null)
					{
						this._popupSystem.PopupEntity(result, args.User, args.User, PopupType.Small);
					}
				},
				Text = labelerText
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001579 RID: 5497 RVA: 0x00070B60 File Offset: 0x0006ED60
		private void AfterInteractOn(EntityUid uid, HandLabelerComponent handLabeler, AfterInteractEvent args)
		{
			EntityUid? target2 = args.Target;
			if (target2 != null)
			{
				EntityUid target = target2.GetValueOrDefault();
				if (target.Valid && handLabeler.Whitelist.IsValid(target, null) && args.CanReach)
				{
					string result;
					this.AddLabelTo(uid, handLabeler, target, out result);
					if (result == null)
					{
						return;
					}
					this._popupSystem.PopupEntity(result, args.User, args.User, PopupType.Small);
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(15, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
					logStringHandler.AppendLiteral(" labeled ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
					logStringHandler.AppendLiteral(" with ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "labeler", "ToPrettyString(uid)");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
			}
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x00070C50 File Offset: 0x0006EE50
		[NullableContext(2)]
		private void AddLabelTo(EntityUid uid, HandLabelerComponent handLabeler, EntityUid target, out string result)
		{
			if (!base.Resolve<HandLabelerComponent>(uid, ref handLabeler, true))
			{
				result = null;
				return;
			}
			if (handLabeler.AssignedLabel == string.Empty)
			{
				this._labelSystem.Label(target, null, null, null);
				result = Loc.GetString("hand-labeler-successfully-removed");
				return;
			}
			this._labelSystem.Label(target, handLabeler.AssignedLabel, null, null);
			result = Loc.GetString("hand-labeler-successfully-applied");
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x00070CC0 File Offset: 0x0006EEC0
		private void OnActivate(EntityUid uid, HandLabelerComponent handLabeler, ActivateInWorldEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			BoundUserInterface uiorNull = handLabeler.Owner.GetUIOrNull(HandLabelerUiKey.Key);
			if (uiorNull != null)
			{
				uiorNull.Open(actor.PlayerSession);
			}
			args.Handled = true;
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x00070D10 File Offset: 0x0006EF10
		private void OnHandLabelerLabelChanged(EntityUid uid, HandLabelerComponent handLabeler, HandLabelerLabelChangedMessage args)
		{
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid player = attachedEntity.GetValueOrDefault();
				if (player.Valid)
				{
					handLabeler.AssignedLabel = args.Label.Trim().Substring(0, Math.Min(handLabeler.MaxLabelChars, args.Label.Length));
					this.DirtyUI(uid, handLabeler);
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(23, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "user", "ToPrettyString(player)");
					logStringHandler.AppendLiteral(" set ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "labeler", "ToPrettyString(uid)");
					logStringHandler.AppendLiteral(" to apply label \"");
					logStringHandler.AppendFormatted(handLabeler.AssignedLabel);
					logStringHandler.AppendLiteral("\"");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
			}
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x00070DF4 File Offset: 0x0006EFF4
		[NullableContext(2)]
		private void DirtyUI(EntityUid uid, HandLabelerComponent handLabeler = null)
		{
			if (!base.Resolve<HandLabelerComponent>(uid, ref handLabeler, true))
			{
				return;
			}
			this._userInterfaceSystem.TrySetUiState(uid, HandLabelerUiKey.Key, new HandLabelerBoundUserInterfaceState(handLabeler.AssignedLabel), null, null, true);
		}

		// Token: 0x04000D5B RID: 3419
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x04000D5C RID: 3420
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000D5D RID: 3421
		[Dependency]
		private readonly LabelSystem _labelSystem;

		// Token: 0x04000D5E RID: 3422
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;
	}
}
