using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Doors.Systems;
using Content.Server.Power.EntitySystems;
using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Remotes
{
	// Token: 0x02000247 RID: 583
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DoorRemoteSystem : EntitySystem
	{
		// Token: 0x06000BAA RID: 2986 RVA: 0x0003D413 File Offset: 0x0003B613
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DoorRemoteComponent, UseInHandEvent>(new ComponentEventHandler<DoorRemoteComponent, UseInHandEvent>(this.OnInHandActivation), null, null);
			base.SubscribeLocalEvent<DoorRemoteComponent, BeforeRangedInteractEvent>(new ComponentEventHandler<DoorRemoteComponent, BeforeRangedInteractEvent>(this.OnBeforeInteract), null, null);
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x0003D440 File Offset: 0x0003B640
		public void OnInHandActivation(EntityUid user, DoorRemoteComponent component, UseInHandEvent args)
		{
			string switchMessageId;
			switch (component.Mode)
			{
			case DoorRemoteComponent.OperatingMode.OpenClose:
				component.Mode = DoorRemoteComponent.OperatingMode.ToggleBolts;
				switchMessageId = "door-remote-switch-state-toggle-bolts";
				break;
			case DoorRemoteComponent.OperatingMode.ToggleBolts:
				component.Mode = DoorRemoteComponent.OperatingMode.ToggleEmergencyAccess;
				switchMessageId = "door-remote-switch-state-toggle-emergency-access";
				break;
			case DoorRemoteComponent.OperatingMode.ToggleEmergencyAccess:
				component.Mode = DoorRemoteComponent.OperatingMode.OpenClose;
				switchMessageId = "door-remote-switch-state-open-close";
				break;
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
				defaultInterpolatedStringHandler.AppendFormatted("DoorRemoteComponent");
				defaultInterpolatedStringHandler.AppendLiteral(" had invalid mode ");
				defaultInterpolatedStringHandler.AppendFormatted<DoorRemoteComponent.OperatingMode>(component.Mode);
				throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			}
			this.ShowPopupToUser(switchMessageId, args.User);
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x0003D4E0 File Offset: 0x0003B6E0
		private void OnBeforeInteract(EntityUid uid, DoorRemoteComponent component, BeforeRangedInteractEvent args)
		{
			DoorComponent doorComp;
			AirlockComponent airlockComp;
			if (args.Handled || args.Target == null || !base.TryComp<DoorComponent>(args.Target, ref doorComp) || !base.TryComp<AirlockComponent>(args.Target, ref airlockComp) || !this._interactionSystem.InRangeUnobstructed(args.User, args.Target.Value, 100f, CollisionGroup.Opaque, null, false))
			{
				return;
			}
			args.Handled = true;
			if (!this.IsPowered(args.Target.Value, this.EntityManager, null))
			{
				this.ShowPopupToUser("door-remote-no-power", args.User);
				return;
			}
			AccessReaderComponent accessComponent;
			if (base.TryComp<AccessReaderComponent>(args.Target, ref accessComponent) && !this._doorSystem.HasAccess(args.Target.Value, new EntityUid?(args.Used), accessComponent))
			{
				this._doorSystem.Deny(args.Target.Value, doorComp, new EntityUid?(args.User), false);
				this.ShowPopupToUser("door-remote-denied", args.User);
				return;
			}
			switch (component.Mode)
			{
			case DoorRemoteComponent.OperatingMode.OpenClose:
				if (this._doorSystem.TryToggleDoor(args.Target.Value, doorComp, new EntityUid?(args.Used), false))
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Medium;
					LogStringHandler logStringHandler = new LogStringHandler(12, 4);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
					logStringHandler.AppendLiteral(" used ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Used), "ToPrettyString(args.Used)");
					logStringHandler.AppendLiteral(" on ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target.Value), "ToPrettyString(args.Target.Value)");
					logStringHandler.AppendLiteral(": ");
					logStringHandler.AppendFormatted<DoorState>(doorComp.State, "doorComp.State");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
				break;
			case DoorRemoteComponent.OperatingMode.ToggleBolts:
				if (!airlockComp.BoltWireCut)
				{
					this._airlock.SetBoltsWithAudio(uid, airlockComp, !airlockComp.BoltsDown);
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.Action;
					LogImpact impact2 = LogImpact.Medium;
					LogStringHandler logStringHandler = new LogStringHandler(21, 4);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
					logStringHandler.AppendLiteral(" used ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Used), "ToPrettyString(args.Used)");
					logStringHandler.AppendLiteral(" on ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target.Value), "ToPrettyString(args.Target.Value)");
					logStringHandler.AppendLiteral(" to ");
					logStringHandler.AppendFormatted(airlockComp.BoltsDown ? "" : "un");
					logStringHandler.AppendLiteral("bolt it");
					adminLogger2.Add(type2, impact2, ref logStringHandler);
					return;
				}
				break;
			case DoorRemoteComponent.OperatingMode.ToggleEmergencyAccess:
			{
				this._airlock.ToggleEmergencyAccess(uid, airlockComp);
				ISharedAdminLogManager adminLogger3 = this._adminLogger;
				LogType type3 = LogType.Action;
				LogImpact impact3 = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(35, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
				logStringHandler.AppendLiteral(" used ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Used), "ToPrettyString(args.Used)");
				logStringHandler.AppendLiteral(" on ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target.Value), "ToPrettyString(args.Target.Value)");
				logStringHandler.AppendLiteral(" to set emergency access ");
				logStringHandler.AppendFormatted(airlockComp.EmergencyAccess ? "on" : "off");
				adminLogger3.Add(type3, impact3, ref logStringHandler);
				return;
			}
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
				defaultInterpolatedStringHandler.AppendFormatted("DoorRemoteComponent");
				defaultInterpolatedStringHandler.AppendLiteral(" had invalid mode ");
				defaultInterpolatedStringHandler.AppendFormatted<DoorRemoteComponent.OperatingMode>(component.Mode);
				throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			}
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x0003D8B9 File Offset: 0x0003BAB9
		private void ShowPopupToUser(string messageId, EntityUid user)
		{
			this._popupSystem.PopupEntity(Loc.GetString(messageId), user, user, PopupType.Small);
		}

		// Token: 0x04000726 RID: 1830
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000727 RID: 1831
		[Dependency]
		private readonly AirlockSystem _airlock;

		// Token: 0x04000728 RID: 1832
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000729 RID: 1833
		[Dependency]
		private readonly DoorSystem _doorSystem;

		// Token: 0x0400072A RID: 1834
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;
	}
}
