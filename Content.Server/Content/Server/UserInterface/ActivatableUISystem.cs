using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.Ghost.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.UserInterface
{
	// Token: 0x020000F3 RID: 243
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ActivatableUISystem : EntitySystem
	{
		// Token: 0x06000479 RID: 1145 RVA: 0x000159E8 File Offset: 0x00013BE8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ActivatableUIComponent, ActivateInWorldEvent>(new ComponentEventHandler<ActivatableUIComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<ActivatableUIComponent, UseInHandEvent>(new ComponentEventHandler<ActivatableUIComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<ActivatableUIComponent, HandDeselectedEvent>(new ComponentEventHandler<ActivatableUIComponent, HandDeselectedEvent>(this.OnHandDeselected), null, null);
			base.SubscribeLocalEvent<ActivatableUIComponent, GotUnequippedHandEvent>(delegate(EntityUid uid, ActivatableUIComponent aui, GotUnequippedHandEvent _)
			{
				this.CloseAll(uid, aui);
			}, null, null);
			base.SubscribeLocalEvent<ActivatableUIComponent, EntParentChangedMessage>(new ComponentEventRefHandler<ActivatableUIComponent, EntParentChangedMessage>(this.OnParentChanged), null, null);
			base.SubscribeLocalEvent<ActivatableUIComponent, BoundUIClosedEvent>(new ComponentEventHandler<ActivatableUIComponent, BoundUIClosedEvent>(this.OnUIClose), null, null);
			base.SubscribeLocalEvent<BoundUserInterfaceMessageAttempt>(new EntityEventHandler<BoundUserInterfaceMessageAttempt>(this.OnBoundInterfaceInteractAttempt), null, null);
			base.SubscribeLocalEvent<ActivatableUIComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<ActivatableUIComponent, GetVerbsEvent<ActivationVerb>>(this.AddOpenUiVerb), null, null);
			base.SubscribeLocalEvent<ServerUserInterfaceComponent, OpenUiActionEvent>(new ComponentEventHandler<ServerUserInterfaceComponent, OpenUiActionEvent>(this.OnActionPerform), null, null);
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00015AB0 File Offset: 0x00013CB0
		private void OnBoundInterfaceInteractAttempt(BoundUserInterfaceMessageAttempt ev)
		{
			ActivatableUIComponent comp;
			if (!base.TryComp<ActivatableUIComponent>(ev.Target, ref comp))
			{
				return;
			}
			if (!comp.RequireHands)
			{
				return;
			}
			SharedHandsComponent hands;
			if (!base.TryComp<SharedHandsComponent>(ev.Sender.AttachedEntity, ref hands) || hands.Hands.Count == 0)
			{
				ev.Cancel();
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00015B00 File Offset: 0x00013D00
		private void OnActionPerform(EntityUid uid, ServerUserInterfaceComponent component, OpenUiActionEvent args)
		{
			if (args.Handled || args.Key == null)
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.Performer, ref actor))
			{
				return;
			}
			args.Handled = this._uiSystem.TryToggleUi(uid, args.Key, actor.PlayerSession, null);
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00015B50 File Offset: 0x00013D50
		private void AddOpenUiVerb(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<ActivationVerb> args)
		{
			if (!args.CanAccess)
			{
				return;
			}
			if (component.RequireHands && args.Hands == null)
			{
				return;
			}
			if (component.InHandsOnly)
			{
				EntityUid? @using = args.Using;
				if (@using == null || (@using != null && @using.GetValueOrDefault() != uid))
				{
					return;
				}
			}
			if (!args.CanInteract && (!component.AllowSpectator || !base.HasComp<GhostComponent>(args.User)))
			{
				return;
			}
			ActivationVerb verb = new ActivationVerb();
			verb.Act = delegate()
			{
				this.InteractUI(args.User, component);
			};
			verb.Text = Loc.GetString(component.VerbText);
			args.Verbs.Add(verb);
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00015C52 File Offset: 0x00013E52
		private void OnActivate(EntityUid uid, ActivatableUIComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (component.InHandsOnly)
			{
				return;
			}
			args.Handled = this.InteractUI(args.User, component);
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x00015C79 File Offset: 0x00013E79
		private void OnUseInHand(EntityUid uid, ActivatableUIComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = this.InteractUI(args.User, component);
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x00015C97 File Offset: 0x00013E97
		private void OnParentChanged(EntityUid uid, ActivatableUIComponent aui, ref EntParentChangedMessage args)
		{
			this.CloseAll(uid, aui);
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00015CA1 File Offset: 0x00013EA1
		private void OnUIClose(EntityUid uid, ActivatableUIComponent component, BoundUIClosedEvent args)
		{
			if (args.Session != component.CurrentSingleUser)
			{
				return;
			}
			if (args.UiKey != component.Key)
			{
				return;
			}
			this.SetCurrentSingleUser(uid, null, component);
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00015CCC File Offset: 0x00013ECC
		private bool InteractUI(EntityUid user, ActivatableUIComponent aui)
		{
			if (!this._blockerSystem.CanInteract(user, new EntityUid?(aui.Owner)) && (!aui.AllowSpectator || !base.HasComp<GhostComponent>(user)))
			{
				return false;
			}
			if (aui.RequireHands && !base.HasComp<SharedHandsComponent>(user))
			{
				return false;
			}
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(user, ref actor))
			{
				return false;
			}
			if (aui.AdminOnly && !this._adminManager.IsAdmin(actor.PlayerSession, false))
			{
				return false;
			}
			BoundUserInterface ui = aui.UserInterface;
			if (ui == null)
			{
				return false;
			}
			if (aui.SingleUser && aui.CurrentSingleUser != null && actor.PlayerSession != aui.CurrentSingleUser && ui.SubscribedSessions.Count != 0)
			{
				return false;
			}
			ActivatableUIOpenAttemptEvent oae = new ActivatableUIOpenAttemptEvent(user);
			UserOpenActivatableUIAttemptEvent uae = new UserOpenActivatableUIAttemptEvent(user, aui.Owner);
			base.RaiseLocalEvent<UserOpenActivatableUIAttemptEvent>(user, uae, false);
			base.RaiseLocalEvent<ActivatableUIOpenAttemptEvent>(aui.Owner, oae, false);
			if (oae.Cancelled || uae.Cancelled)
			{
				return false;
			}
			BeforeActivatableUIOpenEvent bae = new BeforeActivatableUIOpenEvent(user);
			base.RaiseLocalEvent<BeforeActivatableUIOpenEvent>(aui.Owner, bae, false);
			this.SetCurrentSingleUser(aui.Owner, actor.PlayerSession, aui);
			ui.Toggle(actor.PlayerSession);
			AfterActivatableUIOpenEvent aae = new AfterActivatableUIOpenEvent(user, actor.PlayerSession);
			base.RaiseLocalEvent<AfterActivatableUIOpenEvent>(aui.Owner, aae, false);
			return true;
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00015E12 File Offset: 0x00014012
		[NullableContext(2)]
		public void SetCurrentSingleUser(EntityUid uid, IPlayerSession v, ActivatableUIComponent aui = null)
		{
			if (!base.Resolve<ActivatableUIComponent>(uid, ref aui, true))
			{
				return;
			}
			if (!aui.SingleUser)
			{
				return;
			}
			aui.CurrentSingleUser = v;
			base.RaiseLocalEvent<ActivatableUIPlayerChangedEvent>(uid, new ActivatableUIPlayerChangedEvent(), false);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00015E3E File Offset: 0x0001403E
		[NullableContext(2)]
		public void CloseAll(EntityUid uid, ActivatableUIComponent aui = null)
		{
			if (!base.Resolve<ActivatableUIComponent>(uid, ref aui, false))
			{
				return;
			}
			BoundUserInterface userInterface = aui.UserInterface;
			if (userInterface == null)
			{
				return;
			}
			userInterface.CloseAll();
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00015E5D File Offset: 0x0001405D
		private void OnHandDeselected(EntityUid uid, [Nullable(2)] ActivatableUIComponent aui, HandDeselectedEvent args)
		{
			if (!base.Resolve<ActivatableUIComponent>(uid, ref aui, false))
			{
				return;
			}
			if (!aui.CloseOnHandDeselect)
			{
				return;
			}
			this.CloseAll(uid, aui);
		}

		// Token: 0x040002A5 RID: 677
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x040002A6 RID: 678
		[Dependency]
		private readonly ActionBlockerSystem _blockerSystem;

		// Token: 0x040002A7 RID: 679
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
