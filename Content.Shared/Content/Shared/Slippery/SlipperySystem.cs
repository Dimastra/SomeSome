using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Inventory;
using Content.Shared.StatusEffect;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Slippery
{
	// Token: 0x02000198 RID: 408
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SlipperySystem : EntitySystem
	{
		// Token: 0x060004CA RID: 1226 RVA: 0x00012684 File Offset: 0x00010884
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SlipperyComponent, StepTriggerAttemptEvent>(new ComponentEventRefHandler<SlipperyComponent, StepTriggerAttemptEvent>(this.HandleAttemptCollide), null, null);
			base.SubscribeLocalEvent<SlipperyComponent, StepTriggeredEvent>(new ComponentEventRefHandler<SlipperyComponent, StepTriggeredEvent>(this.HandleStepTrigger), null, null);
			ComponentEventHandler<NoSlipComponent, SlipAttemptEvent> componentEventHandler;
			if ((componentEventHandler = SlipperySystem.<>O.<0>__OnNoSlipAttempt) == null)
			{
				componentEventHandler = (SlipperySystem.<>O.<0>__OnNoSlipAttempt = new ComponentEventHandler<NoSlipComponent, SlipAttemptEvent>(SlipperySystem.OnNoSlipAttempt));
			}
			base.SubscribeLocalEvent<NoSlipComponent, SlipAttemptEvent>(componentEventHandler, null, null);
			base.SubscribeLocalEvent<NoSlipComponent, InventoryRelayedEvent<SlipAttemptEvent>>(delegate(EntityUid e, NoSlipComponent c, InventoryRelayedEvent<SlipAttemptEvent> ev)
			{
				SlipperySystem.OnNoSlipAttempt(e, c, ev.Args);
			}, null, null);
			base.SubscribeLocalEvent<SlipperyComponent, ComponentGetState>(new ComponentEventRefHandler<SlipperyComponent, ComponentGetState>(this.OnSlipperyGetState), null, null);
			base.SubscribeLocalEvent<SlipperyComponent, ComponentHandleState>(new ComponentEventRefHandler<SlipperyComponent, ComponentHandleState>(this.OnSlipperyHandleState), null, null);
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00012734 File Offset: 0x00010934
		private void OnSlipperyHandleState(EntityUid uid, SlipperyComponent component, ref ComponentHandleState args)
		{
			SlipperyComponentState state = args.Current as SlipperyComponentState;
			if (state == null)
			{
				return;
			}
			component.ParalyzeTime = state.ParalyzeTime;
			component.LaunchForwardsMultiplier = state.LaunchForwardsMultiplier;
			component.SlipSound = new SoundPathSpecifier(state.SlipSound, null);
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00012783 File Offset: 0x00010983
		private void OnSlipperyGetState(EntityUid uid, SlipperyComponent component, ref ComponentGetState args)
		{
			args.State = new SlipperyComponentState(component.ParalyzeTime, component.LaunchForwardsMultiplier, this._audio.GetSound(component.SlipSound));
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x000127AD File Offset: 0x000109AD
		private void HandleStepTrigger(EntityUid uid, SlipperyComponent component, ref StepTriggeredEvent args)
		{
			this.TrySlip(component, args.Tripper);
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x000127BC File Offset: 0x000109BC
		private void HandleAttemptCollide(EntityUid uid, SlipperyComponent component, ref StepTriggerAttemptEvent args)
		{
			args.Continue |= this.CanSlip(uid, args.Tripper);
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x000127D5 File Offset: 0x000109D5
		private static void OnNoSlipAttempt(EntityUid uid, NoSlipComponent component, SlipAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x000127DD File Offset: 0x000109DD
		private bool CanSlip(EntityUid uid, EntityUid toSlip)
		{
			return !this._container.IsEntityInContainer(uid, null) && this._statusEffectsSystem.CanApplyEffect(toSlip, "Stun", null);
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00012804 File Offset: 0x00010A04
		private void TrySlip(SlipperyComponent component, EntityUid other)
		{
			if (base.HasComp<KnockedDownComponent>(other))
			{
				return;
			}
			SlipAttemptEvent ev = new SlipAttemptEvent();
			base.RaiseLocalEvent<SlipAttemptEvent>(other, ev, false);
			if (ev.Cancelled)
			{
				return;
			}
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(other, ref physics))
			{
				this._physics.SetLinearVelocity(other, physics.LinearVelocity * component.LaunchForwardsMultiplier, true, true, null, physics);
			}
			bool flag = !this._statusEffectsSystem.HasStatusEffect(other, "KnockedDown", null);
			this._stunSystem.TryParalyze(other, TimeSpan.FromSeconds((double)component.ParalyzeTime), true, null);
			if (flag)
			{
				this._audio.PlayPredicted(component.SlipSound, other, new EntityUid?(other), null);
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Slip;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(27, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(other), "mob", "ToPrettyString(other)");
			logStringHandler.AppendLiteral(" slipped on collision with ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "entity", "ToPrettyString(component.Owner)");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001290D File Offset: 0x00010B0D
		public void CopyConstruct(EntityUid destUid, SlipperyComponent srcSlip)
		{
			SlipperyComponent slipperyComponent = this.EntityManager.EnsureComponent<SlipperyComponent>(destUid);
			slipperyComponent.SlipSound = srcSlip.SlipSound;
			slipperyComponent.ParalyzeTime = srcSlip.ParalyzeTime;
			slipperyComponent.LaunchForwardsMultiplier = srcSlip.LaunchForwardsMultiplier;
		}

		// Token: 0x04000475 RID: 1141
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000476 RID: 1142
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000477 RID: 1143
		[Dependency]
		private readonly SharedStunSystem _stunSystem;

		// Token: 0x04000478 RID: 1144
		[Dependency]
		private readonly StatusEffectsSystem _statusEffectsSystem;

		// Token: 0x04000479 RID: 1145
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x0400047A RID: 1146
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x020007A6 RID: 1958
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040017C5 RID: 6085
			[Nullable(new byte[]
			{
				0,
				1,
				1
			})]
			public static ComponentEventHandler<NoSlipComponent, SlipAttemptEvent> <0>__OnNoSlipAttempt;
		}
	}
}
