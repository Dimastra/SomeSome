using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Audio;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Player;

namespace Content.Server.Guardian
{
	// Token: 0x02000486 RID: 1158
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GuardianSystem : EntitySystem
	{
		// Token: 0x06001712 RID: 5906 RVA: 0x000794A0 File Offset: 0x000776A0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GuardianCreatorComponent, UseInHandEvent>(new ComponentEventHandler<GuardianCreatorComponent, UseInHandEvent>(this.OnCreatorUse), null, null);
			base.SubscribeLocalEvent<GuardianCreatorComponent, AfterInteractEvent>(new ComponentEventHandler<GuardianCreatorComponent, AfterInteractEvent>(this.OnCreatorInteract), null, null);
			base.SubscribeLocalEvent<GuardianCreatorComponent, ExaminedEvent>(new ComponentEventHandler<GuardianCreatorComponent, ExaminedEvent>(this.OnCreatorExamine), null, null);
			base.SubscribeLocalEvent<GuardianCreatorComponent, DoAfterEvent>(new ComponentEventHandler<GuardianCreatorComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<GuardianComponent, MoveEvent>(new ComponentEventRefHandler<GuardianComponent, MoveEvent>(this.OnGuardianMove), null, null);
			base.SubscribeLocalEvent<GuardianComponent, DamageChangedEvent>(new ComponentEventHandler<GuardianComponent, DamageChangedEvent>(this.OnGuardianDamaged), null, null);
			base.SubscribeLocalEvent<GuardianComponent, PlayerAttachedEvent>(new ComponentEventHandler<GuardianComponent, PlayerAttachedEvent>(this.OnGuardianPlayer), null, null);
			base.SubscribeLocalEvent<GuardianComponent, PlayerDetachedEvent>(new ComponentEventHandler<GuardianComponent, PlayerDetachedEvent>(this.OnGuardianUnplayer), null, null);
			base.SubscribeLocalEvent<GuardianHostComponent, ComponentInit>(new ComponentEventHandler<GuardianHostComponent, ComponentInit>(this.OnHostInit), null, null);
			base.SubscribeLocalEvent<GuardianHostComponent, MoveEvent>(new ComponentEventRefHandler<GuardianHostComponent, MoveEvent>(this.OnHostMove), null, null);
			base.SubscribeLocalEvent<GuardianHostComponent, MobStateChangedEvent>(new ComponentEventHandler<GuardianHostComponent, MobStateChangedEvent>(this.OnHostStateChange), null, null);
			base.SubscribeLocalEvent<GuardianHostComponent, ComponentShutdown>(new ComponentEventHandler<GuardianHostComponent, ComponentShutdown>(this.OnHostShutdown), null, null);
			base.SubscribeLocalEvent<GuardianHostComponent, GuardianToggleActionEvent>(new ComponentEventHandler<GuardianHostComponent, GuardianToggleActionEvent>(this.OnPerformAction), null, null);
			base.SubscribeLocalEvent<GuardianComponent, AttackAttemptEvent>(new ComponentEventHandler<GuardianComponent, AttackAttemptEvent>(this.OnGuardianAttackAttempt), null, null);
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x000795CB File Offset: 0x000777CB
		private void OnPerformAction(EntityUid uid, GuardianHostComponent component, GuardianToggleActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (component.HostedGuardian != null)
			{
				this.ToggleGuardian(uid, component);
			}
			args.Handled = true;
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x000795F4 File Offset: 0x000777F4
		private void OnGuardianUnplayer(EntityUid uid, GuardianComponent component, PlayerDetachedEvent args)
		{
			EntityUid host = component.Host;
			GuardianHostComponent hostComponent;
			if (!base.TryComp<GuardianHostComponent>(host, ref hostComponent) || base.LifeStage(host, null) >= 3)
			{
				return;
			}
			this.RetractGuardian(host, hostComponent, uid, component);
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x0007962C File Offset: 0x0007782C
		private void OnGuardianPlayer(EntityUid uid, GuardianComponent component, PlayerAttachedEvent args)
		{
			EntityUid host = component.Host;
			if (!base.HasComp<GuardianHostComponent>(host))
			{
				return;
			}
			this._popupSystem.PopupEntity(Loc.GetString("guardian-available"), host, host, PopupType.Small);
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x00079664 File Offset: 0x00077864
		private void OnHostInit(EntityUid uid, GuardianHostComponent component, ComponentInit args)
		{
			component.GuardianContainer = ContainerHelpers.EnsureContainer<ContainerSlot>(uid, "GuardianContainer", null);
			this._actionSystem.AddAction(uid, component.Action, null, null, true);
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x000796A0 File Offset: 0x000778A0
		private void OnHostShutdown(EntityUid uid, GuardianHostComponent component, ComponentShutdown args)
		{
			if (component.HostedGuardian == null)
			{
				return;
			}
			this.EntityManager.QueueDeleteEntity(component.HostedGuardian.Value);
			this._actionSystem.RemoveAction(uid, component.Action, null);
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x000796DC File Offset: 0x000778DC
		private void OnGuardianAttackAttempt(EntityUid uid, GuardianComponent component, AttackAttemptEvent args)
		{
			if (!args.Cancelled)
			{
				EntityUid? target = args.Target;
				EntityUid host = component.Host;
				if (target != null && (target == null || !(target.GetValueOrDefault() != host)))
				{
					this._popupSystem.PopupCursor(Loc.GetString("guardian-attack-host"), uid, PopupType.LargeCaution);
					args.Cancel();
					return;
				}
			}
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x00079744 File Offset: 0x00077944
		public void ToggleGuardian(EntityUid user, GuardianHostComponent hostComponent)
		{
			GuardianComponent guardianComponent;
			if (hostComponent.HostedGuardian == null || !base.TryComp<GuardianComponent>(hostComponent.HostedGuardian, ref guardianComponent))
			{
				return;
			}
			if (guardianComponent.GuardianLoose)
			{
				this.RetractGuardian(user, hostComponent, hostComponent.HostedGuardian.Value, guardianComponent);
				return;
			}
			this.ReleaseGuardian(user, hostComponent, hostComponent.HostedGuardian.Value, guardianComponent);
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x000797A0 File Offset: 0x000779A0
		private void OnCreatorUse(EntityUid uid, GuardianCreatorComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.UseCreator(args.User, args.User, uid, component);
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x000797C0 File Offset: 0x000779C0
		private void OnCreatorInteract(EntityUid uid, GuardianCreatorComponent component, AfterInteractEvent args)
		{
			if (args.Handled || args.Target == null || !args.CanReach)
			{
				return;
			}
			this.UseCreator(args.User, args.Target.Value, uid, component);
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x0007980C File Offset: 0x00077A0C
		private void UseCreator(EntityUid user, EntityUid target, EntityUid injector, GuardianCreatorComponent component)
		{
			if (component.Used)
			{
				this._popupSystem.PopupEntity(Loc.GetString("guardian-activator-empty-invalid-creation"), user, user, PopupType.Small);
				return;
			}
			if (!base.HasComp<CanHostGuardianComponent>(target))
			{
				this._popupSystem.PopupEntity(Loc.GetString("guardian-activator-invalid-target"), user, user, PopupType.Small);
				return;
			}
			if (base.HasComp<GuardianHostComponent>(target))
			{
				this._popupSystem.PopupEntity(Loc.GetString("guardian-already-present-invalid-creation"), user, user, PopupType.Small);
				return;
			}
			if (component.Injecting)
			{
				return;
			}
			component.Injecting = true;
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			float injectionDelay = component.InjectionDelay;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(injector);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, injectionDelay, default(CancellationToken), target2, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true
			});
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x000798D4 File Offset: 0x00077AD4
		private void OnDoAfter(EntityUid uid, GuardianCreatorComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Args.Target == null)
			{
				return;
			}
			Hand hand;
			if (args.Cancelled || component.Deleted || component.Used || !this._handsSystem.IsHolding(args.Args.User, new EntityUid?(uid), out hand, null) || base.HasComp<GuardianHostComponent>(args.Args.Target))
			{
				component.Injecting = false;
				return;
			}
			TransformComponent hostXform = base.Transform(args.Args.Target.Value);
			GuardianHostComponent guardianHostComponent = base.EnsureComp<GuardianHostComponent>(args.Args.Target.Value);
			EntityUid guardian = base.Spawn(component.GuardianProto, hostXform.MapPosition);
			guardianHostComponent.GuardianContainer.Insert(guardian, null, null, null, null, null);
			guardianHostComponent.HostedGuardian = new EntityUid?(guardian);
			GuardianComponent guardianComp;
			if (base.TryComp<GuardianComponent>(guardian, ref guardianComp))
			{
				guardianComp.Host = args.Args.Target.Value;
				this._audio.Play("/Audio/Effects/guardian_inject.ogg", Filter.Pvs(args.Args.Target.Value, 2f, null, null, null), args.Args.Target.Value, true, null);
				this._popupSystem.PopupEntity(Loc.GetString("guardian-created"), args.Args.Target.Value, args.Args.Target.Value, PopupType.Small);
				component.Used = true;
			}
			else
			{
				Logger.ErrorS("guardian", "Tried to spawn a guardian that doesn't have GuardianComponent");
				this.EntityManager.QueueDeleteEntity(guardian);
			}
			args.Handled = true;
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x00079A7C File Offset: 0x00077C7C
		private void OnHostStateChange(EntityUid uid, GuardianHostComponent component, MobStateChangedEvent args)
		{
			if (component.HostedGuardian == null)
			{
				return;
			}
			if (args.NewMobState == MobState.Critical)
			{
				this._popupSystem.PopupEntity(Loc.GetString("guardian-critical-warn"), component.HostedGuardian.Value, component.HostedGuardian.Value, PopupType.Small);
				this._audio.Play("/Audio/Effects/guardian_warn.ogg", Filter.Pvs(component.HostedGuardian.Value, 2f, null, null, null), component.HostedGuardian.Value, true, null);
				return;
			}
			if (args.NewMobState == MobState.Dead)
			{
				this._audio.Play("/Audio/Voice/Human/malescream_guardian.ogg", Filter.Pvs(uid, 2f, null, null, null), uid, true, new AudioParams?(AudioHelpers.WithVariation(0.2f)));
				this.EntityManager.RemoveComponent<GuardianHostComponent>(uid);
			}
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x00079B54 File Offset: 0x00077D54
		private void OnGuardianDamaged(EntityUid uid, GuardianComponent component, DamageChangedEvent args)
		{
			if (args.DamageDelta == null)
			{
				return;
			}
			this._damageSystem.TryChangeDamage(new EntityUid?(component.Host), args.DamageDelta * component.DamageShare, false, true, null, args.Origin);
			this._popupSystem.PopupEntity(Loc.GetString("guardian-entity-taking-damage"), component.Host, component.Host, PopupType.Small);
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x00079BBD File Offset: 0x00077DBD
		private void OnCreatorExamine(EntityUid uid, GuardianCreatorComponent component, ExaminedEvent args)
		{
			if (component.Used)
			{
				args.PushMarkup(Loc.GetString("guardian-activator-empty-examine"));
			}
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x00079BD8 File Offset: 0x00077DD8
		private void OnHostMove(EntityUid uid, GuardianHostComponent component, ref MoveEvent args)
		{
			GuardianComponent guardianComponent;
			if (component.HostedGuardian == null || !base.TryComp<GuardianComponent>(component.HostedGuardian, ref guardianComponent) || !guardianComponent.GuardianLoose)
			{
				return;
			}
			this.CheckGuardianMove(uid, component.HostedGuardian.Value, component, null, null, null);
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x00079C21 File Offset: 0x00077E21
		private void OnGuardianMove(EntityUid uid, GuardianComponent component, ref MoveEvent args)
		{
			if (!component.GuardianLoose)
			{
				return;
			}
			this.CheckGuardianMove(component.Host, uid, null, component, null, null);
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x00079C40 File Offset: 0x00077E40
		[NullableContext(2)]
		private void CheckGuardianMove(EntityUid hostUid, EntityUid guardianUid, GuardianHostComponent hostComponent = null, GuardianComponent guardianComponent = null, TransformComponent hostXform = null, TransformComponent guardianXform = null)
		{
			if (!base.Resolve<GuardianHostComponent, TransformComponent>(hostUid, ref hostComponent, ref hostXform, true) || !base.Resolve<GuardianComponent, TransformComponent>(guardianUid, ref guardianComponent, ref guardianXform, true))
			{
				return;
			}
			if (!guardianComponent.GuardianLoose)
			{
				return;
			}
			if (!guardianXform.Coordinates.InRange(this.EntityManager, hostXform.Coordinates, guardianComponent.DistanceAllowed))
			{
				this.RetractGuardian(hostUid, hostComponent, guardianUid, guardianComponent);
			}
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x00079CA4 File Offset: 0x00077EA4
		private bool CanRelease(EntityUid guardian)
		{
			return base.HasComp<ActorComponent>(guardian);
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x00079CB0 File Offset: 0x00077EB0
		private void ReleaseGuardian(EntityUid host, GuardianHostComponent hostComponent, EntityUid guardian, GuardianComponent guardianComponent)
		{
			if (guardianComponent.GuardianLoose)
			{
				return;
			}
			if (!this.CanRelease(guardian))
			{
				this._popupSystem.PopupEntity(Loc.GetString("guardian-no-soul"), host, host, PopupType.Small);
				return;
			}
			hostComponent.GuardianContainer.Remove(guardian, null, null, null, true, false, null, null);
			guardianComponent.GuardianLoose = true;
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x00079D15 File Offset: 0x00077F15
		private void RetractGuardian(EntityUid host, GuardianHostComponent hostComponent, EntityUid guardian, GuardianComponent guardianComponent)
		{
			if (!guardianComponent.GuardianLoose)
			{
				return;
			}
			hostComponent.GuardianContainer.Insert(guardian, null, null, null, null, null);
			this._popupSystem.PopupEntity(Loc.GetString("guardian-entity-recall"), host, PopupType.Small);
			guardianComponent.GuardianLoose = false;
		}

		// Token: 0x04000E7C RID: 3708
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000E7D RID: 3709
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000E7E RID: 3710
		[Dependency]
		private readonly DamageableSystem _damageSystem;

		// Token: 0x04000E7F RID: 3711
		[Dependency]
		private readonly SharedActionsSystem _actionSystem;

		// Token: 0x04000E80 RID: 3712
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000E81 RID: 3713
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
