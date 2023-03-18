using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Doors.Systems
{
	// Token: 0x02000544 RID: 1348
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirlockSystem : SharedAirlockSystem
	{
		// Token: 0x06001C35 RID: 7221 RVA: 0x00096390 File Offset: 0x00094590
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AirlockComponent, ComponentInit>(new ComponentEventHandler<AirlockComponent, ComponentInit>(this.OnAirlockInit), null, null);
			base.SubscribeLocalEvent<AirlockComponent, PowerChangedEvent>(new ComponentEventRefHandler<AirlockComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<AirlockComponent, DoorStateChangedEvent>(new ComponentEventHandler<AirlockComponent, DoorStateChangedEvent>(this.OnStateChanged), null, null);
			base.SubscribeLocalEvent<AirlockComponent, BeforeDoorOpenedEvent>(new ComponentEventHandler<AirlockComponent, BeforeDoorOpenedEvent>(this.OnBeforeDoorOpened), null, null);
			base.SubscribeLocalEvent<AirlockComponent, BeforeDoorDeniedEvent>(new ComponentEventHandler<AirlockComponent, BeforeDoorDeniedEvent>(this.OnBeforeDoorDenied), null, null);
			base.SubscribeLocalEvent<AirlockComponent, ActivateInWorldEvent>(new ComponentEventHandler<AirlockComponent, ActivateInWorldEvent>(this.OnActivate), new Type[]
			{
				typeof(DoorSystem)
			}, null);
			base.SubscribeLocalEvent<AirlockComponent, DoorGetPryTimeModifierEvent>(new ComponentEventHandler<AirlockComponent, DoorGetPryTimeModifierEvent>(this.OnGetPryMod), null, null);
			base.SubscribeLocalEvent<AirlockComponent, BeforeDoorPryEvent>(new ComponentEventHandler<AirlockComponent, BeforeDoorPryEvent>(this.OnDoorPry), null, null);
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x00096458 File Offset: 0x00094658
		private void OnAirlockInit(EntityUid uid, AirlockComponent component, ComponentInit args)
		{
			ApcPowerReceiverComponent receiverComponent;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref receiverComponent))
			{
				this.Appearance.SetData(uid, DoorVisuals.Powered, receiverComponent.Powered, null);
				this.Appearance.SetData(uid, DoorVisuals.ClosedLights, true, null);
			}
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x000964A8 File Offset: 0x000946A8
		private void OnPowerChanged(EntityUid uid, AirlockComponent component, ref PowerChangedEvent args)
		{
			AppearanceComponent appearanceComponent;
			if (base.TryComp<AppearanceComponent>(uid, ref appearanceComponent))
			{
				this.Appearance.SetData(uid, DoorVisuals.Powered, args.Powered, appearanceComponent);
			}
			DoorComponent door;
			if (!base.TryComp<DoorComponent>(uid, ref door))
			{
				return;
			}
			if (!args.Powered)
			{
				if (door.State == DoorState.Open)
				{
					this.DoorSystem.SetNextStateChange(uid, null, null);
				}
			}
			else
			{
				if (component.BoltWireCut)
				{
					this.SetBoltsWithAudio(uid, component, true);
				}
				this.UpdateAutoClose(uid, null, door);
			}
			this.UpdateBoltLightStatus(uid, component);
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x00096538 File Offset: 0x00094738
		private void OnStateChanged(EntityUid uid, AirlockComponent component, DoorStateChangedEvent args)
		{
			WiresComponent wiresComponent;
			if (base.TryComp<WiresComponent>(uid, ref wiresComponent))
			{
				wiresComponent.IsPanelVisible = (component.OpenPanelVisible || args.State != DoorState.Open);
			}
			this.UpdateBoltLightStatus(uid, component);
			this.UpdateAutoClose(uid, component, null);
			if (args.State == DoorState.Closed)
			{
				component.AutoClose = true;
			}
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x00096590 File Offset: 0x00094790
		[NullableContext(2)]
		public void UpdateAutoClose(EntityUid uid, AirlockComponent airlock = null, DoorComponent door = null)
		{
			if (!base.Resolve<AirlockComponent, DoorComponent>(uid, ref airlock, ref door, true))
			{
				return;
			}
			if (door.State != DoorState.Open)
			{
				return;
			}
			if (!airlock.AutoClose)
			{
				return;
			}
			if (!this.CanChangeState(uid, airlock))
			{
				return;
			}
			BeforeDoorAutoCloseEvent autoev = new BeforeDoorAutoCloseEvent();
			base.RaiseLocalEvent<BeforeDoorAutoCloseEvent>(uid, autoev, false);
			if (autoev.Cancelled)
			{
				return;
			}
			this.DoorSystem.SetNextStateChange(uid, new TimeSpan?(airlock.AutoCloseDelay * (double)airlock.AutoCloseDelayModifier), null);
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x00096606 File Offset: 0x00094806
		private void OnBeforeDoorOpened(EntityUid uid, AirlockComponent component, BeforeDoorOpenedEvent args)
		{
			if (!this.CanChangeState(uid, component))
			{
				args.Cancel();
			}
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x00096618 File Offset: 0x00094818
		protected override void OnBeforeDoorClosed(EntityUid uid, AirlockComponent component, BeforeDoorClosedEvent args)
		{
			base.OnBeforeDoorClosed(uid, component, args);
			if (args.Cancelled)
			{
				return;
			}
			DoorComponent door;
			if (base.TryComp<DoorComponent>(uid, ref door) && !door.Partial && !this.CanChangeState(uid, component))
			{
				args.Cancel();
			}
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x0009665A File Offset: 0x0009485A
		private void OnBeforeDoorDenied(EntityUid uid, AirlockComponent component, BeforeDoorDeniedEvent args)
		{
			if (!this.CanChangeState(uid, component))
			{
				args.Cancel();
			}
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x0009666C File Offset: 0x0009486C
		private void OnActivate(EntityUid uid, AirlockComponent component, ActivateInWorldEvent args)
		{
			WiresComponent wiresComponent;
			ActorComponent actor;
			if (base.TryComp<WiresComponent>(uid, ref wiresComponent) && wiresComponent.IsPanelOpen && this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				this._wiresSystem.OpenUserInterface(uid, actor.PlayerSession);
				args.Handled = true;
				return;
			}
			if (component.KeepOpenIfClicked)
			{
				component.AutoClose = false;
			}
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x000966CA File Offset: 0x000948CA
		private void OnGetPryMod(EntityUid uid, AirlockComponent component, DoorGetPryTimeModifierEvent args)
		{
			if (this._power.IsPowered(uid, null))
			{
				args.PryTimeModifier *= component.PoweredPryModifier;
			}
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x000966F0 File Offset: 0x000948F0
		private void OnDoorPry(EntityUid uid, AirlockComponent component, BeforeDoorPryEvent args)
		{
			if (component.BoltsDown)
			{
				this.Popup.PopupEntity(Loc.GetString("airlock-component-cannot-pry-is-bolted-message"), uid, args.User, PopupType.Small);
				args.Cancel();
			}
			if (this.IsPowered(uid, this.EntityManager, null))
			{
				if (base.HasComp<ToolForcePoweredComponent>(args.Tool))
				{
					return;
				}
				this.Popup.PopupEntity(Loc.GetString("airlock-component-cannot-pry-is-powered-message"), uid, args.User, PopupType.Small);
				args.Cancel();
			}
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x0009676A File Offset: 0x0009496A
		public bool CanChangeState(EntityUid uid, AirlockComponent component)
		{
			return this.IsPowered(uid, this.EntityManager, null) && !component.BoltsDown;
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x00096788 File Offset: 0x00094988
		public void UpdateBoltLightStatus(EntityUid uid, AirlockComponent component)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this.Appearance.SetData(uid, DoorVisuals.BoltLights, this.GetBoltLightsVisible(uid, component), appearance);
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x000967C4 File Offset: 0x000949C4
		public void SetBoltsWithAudio(EntityUid uid, AirlockComponent component, bool newBolts)
		{
			if (newBolts == component.BoltsDown)
			{
				return;
			}
			component.BoltsDown = newBolts;
			this.Audio.PlayPvs(newBolts ? component.BoltDownSound : component.BoltUpSound, uid, null);
			this.UpdateBoltLightStatus(uid, component);
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x00096814 File Offset: 0x00094A14
		public bool GetBoltLightsVisible(EntityUid uid, AirlockComponent component)
		{
			DoorComponent doorComponent;
			return component.BoltLightsEnabled && component.BoltsDown && this.IsPowered(uid, this.EntityManager, null) && base.TryComp<DoorComponent>(uid, ref doorComponent) && doorComponent.State == DoorState.Closed;
		}

		// Token: 0x06001C44 RID: 7236 RVA: 0x00096857 File Offset: 0x00094A57
		public void SetBoltLightsEnabled(EntityUid uid, AirlockComponent component, bool value)
		{
			if (component.BoltLightsEnabled == value)
			{
				return;
			}
			component.BoltLightsEnabled = value;
			this.UpdateBoltLightStatus(uid, component);
		}

		// Token: 0x06001C45 RID: 7237 RVA: 0x00096872 File Offset: 0x00094A72
		public void SetBoltsDown(EntityUid uid, AirlockComponent component, bool value)
		{
			if (component.BoltsDown == value)
			{
				return;
			}
			component.BoltsDown = value;
			this.UpdateBoltLightStatus(uid, component);
		}

		// Token: 0x04001231 RID: 4657
		[Dependency]
		private readonly WiresSystem _wiresSystem;

		// Token: 0x04001232 RID: 4658
		[Dependency]
		private readonly PowerReceiverSystem _power;
	}
}
