using System;
using System.Runtime.CompilerServices;
using Content.Server.Access.Components;
using Content.Server.Popups;
using Content.Server.UserInterface;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Access.Systems
{
	// Token: 0x0200087A RID: 2170
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AgentIDCardSystem : SharedAgentIdCardSystem
	{
		// Token: 0x06002F63 RID: 12131 RVA: 0x000F4F3C File Offset: 0x000F313C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AgentIDCardComponent, AfterInteractEvent>(new ComponentEventHandler<AgentIDCardComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<AgentIDCardComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<AgentIDCardComponent, AfterActivatableUIOpenEvent>(this.AfterUIOpen), null, null);
			base.SubscribeLocalEvent<AgentIDCardComponent, AgentIDCardNameChangedMessage>(new ComponentEventHandler<AgentIDCardComponent, AgentIDCardNameChangedMessage>(this.OnNameChanged), null, null);
			base.SubscribeLocalEvent<AgentIDCardComponent, AgentIDCardJobChangedMessage>(new ComponentEventHandler<AgentIDCardComponent, AgentIDCardJobChangedMessage>(this.OnJobChanged), null, null);
		}

		// Token: 0x06002F64 RID: 12132 RVA: 0x000F4FA0 File Offset: 0x000F31A0
		private void OnAfterInteract(EntityUid uid, AgentIDCardComponent component, AfterInteractEvent args)
		{
			AccessComponent targetAccess;
			if (!base.TryComp<AccessComponent>(args.Target, ref targetAccess) || !base.HasComp<IdCardComponent>(args.Target) || args.Target == null)
			{
				return;
			}
			AccessComponent access;
			if (!base.TryComp<AccessComponent>(uid, ref access) || !base.HasComp<IdCardComponent>(uid))
			{
				return;
			}
			int beforeLength = access.Tags.Count;
			access.Tags.UnionWith(targetAccess.Tags);
			int addedLength = access.Tags.Count - beforeLength;
			if (addedLength == 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("agent-id-no-new", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("card", args.Target)
				}), args.Target.Value, args.User, PopupType.Small);
				return;
			}
			if (addedLength == 1)
			{
				this._popupSystem.PopupEntity(Loc.GetString("agent-id-new-1", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("card", args.Target)
				}), args.Target.Value, args.User, PopupType.Small);
				return;
			}
			this._popupSystem.PopupEntity(Loc.GetString("agent-id-new", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("number", addedLength),
				new ValueTuple<string, object>("card", args.Target)
			}), args.Target.Value, args.User, PopupType.Small);
		}

		// Token: 0x06002F65 RID: 12133 RVA: 0x000F5124 File Offset: 0x000F3324
		private void AfterUIOpen(EntityUid uid, AgentIDCardComponent component, AfterActivatableUIOpenEvent args)
		{
			BoundUserInterface ui;
			if (!this._uiSystem.TryGetUi(component.Owner, AgentIDCardUiKey.Key, ref ui, null))
			{
				return;
			}
			IdCardComponent idCard;
			if (!base.TryComp<IdCardComponent>(uid, ref idCard))
			{
				return;
			}
			AgentIDCardBoundUserInterfaceState state = new AgentIDCardBoundUserInterfaceState(idCard.FullName ?? "", idCard.JobTitle ?? "");
			ui.SetState(state, args.Session, true);
		}

		// Token: 0x06002F66 RID: 12134 RVA: 0x000F518C File Offset: 0x000F338C
		private void OnJobChanged(EntityUid uid, AgentIDCardComponent comp, AgentIDCardJobChangedMessage args)
		{
			IdCardComponent idCard;
			if (!base.TryComp<IdCardComponent>(uid, ref idCard))
			{
				return;
			}
			this._cardSystem.TryChangeJobTitle(uid, args.Job, idCard, null);
		}

		// Token: 0x06002F67 RID: 12135 RVA: 0x000F51C4 File Offset: 0x000F33C4
		private void OnNameChanged(EntityUid uid, AgentIDCardComponent comp, AgentIDCardNameChangedMessage args)
		{
			IdCardComponent idCard;
			if (!base.TryComp<IdCardComponent>(uid, ref idCard))
			{
				return;
			}
			this._cardSystem.TryChangeFullName(uid, args.Name, idCard, null);
		}

		// Token: 0x04001C7F RID: 7295
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001C80 RID: 7296
		[Dependency]
		private readonly IdCardSystem _cardSystem;

		// Token: 0x04001C81 RID: 7297
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
