using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Explosion.Components;
using Content.Server.Flash;
using Content.Server.Flash.Components;
using Content.Server.MachineLinking.Events;
using Content.Server.MachineLinking.System;
using Content.Server.Speech;
using Content.Server.Speech.Components;
using Content.Server.Sticky.Events;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Payload.Components;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Trigger;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x02000512 RID: 1298
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TriggerSystem : EntitySystem
	{
		// Token: 0x06001B07 RID: 6919 RVA: 0x00090CCC File Offset: 0x0008EECC
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeProximity();
			this.InitializeOnUse();
			this.InitializeSignal();
			this.InitializeTimedCollide();
			this.InitializeVoice();
			this.InitializeMobstate();
			base.SubscribeLocalEvent<TriggerOnCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<TriggerOnCollideComponent, StartCollideEvent>(this.OnTriggerCollide), null, null);
			base.SubscribeLocalEvent<TriggerOnActivateComponent, ActivateInWorldEvent>(new ComponentEventHandler<TriggerOnActivateComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<TriggerImplantActionComponent, ActivateImplantEvent>(new ComponentEventHandler<TriggerImplantActionComponent, ActivateImplantEvent>(this.OnImplantTrigger), null, null);
			base.SubscribeLocalEvent<TriggerOnStepTriggerComponent, StepTriggeredEvent>(new ComponentEventRefHandler<TriggerOnStepTriggerComponent, StepTriggeredEvent>(this.OnStepTriggered), null, null);
			base.SubscribeLocalEvent<DeleteOnTriggerComponent, TriggerEvent>(new ComponentEventHandler<DeleteOnTriggerComponent, TriggerEvent>(this.HandleDeleteTrigger), null, null);
			base.SubscribeLocalEvent<ExplodeOnTriggerComponent, TriggerEvent>(new ComponentEventHandler<ExplodeOnTriggerComponent, TriggerEvent>(this.HandleExplodeTrigger), null, null);
			base.SubscribeLocalEvent<FlashOnTriggerComponent, TriggerEvent>(new ComponentEventHandler<FlashOnTriggerComponent, TriggerEvent>(this.HandleFlashTrigger), null, null);
			base.SubscribeLocalEvent<GibOnTriggerComponent, TriggerEvent>(new ComponentEventHandler<GibOnTriggerComponent, TriggerEvent>(this.HandleGibTrigger), null, null);
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x00090DA4 File Offset: 0x0008EFA4
		private void HandleExplodeTrigger(EntityUid uid, ExplodeOnTriggerComponent component, TriggerEvent args)
		{
			ExplosionSystem explosions = this._explosions;
			ExplosiveComponent explosive = null;
			bool delete = true;
			EntityUid? user = args.User;
			explosions.TriggerExplosive(uid, explosive, delete, null, null, user);
			args.Handled = true;
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x00090DE0 File Offset: 0x0008EFE0
		private void HandleFlashTrigger(EntityUid uid, FlashOnTriggerComponent component, TriggerEvent args)
		{
			this._flashSystem.FlashArea(uid, args.User, component.Range, component.Duration * 1000f, 0.8f, false, null);
			args.Handled = true;
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x00090E14 File Offset: 0x0008F014
		private void HandleDeleteTrigger(EntityUid uid, DeleteOnTriggerComponent component, TriggerEvent args)
		{
			this.EntityManager.QueueDeleteEntity(uid);
			args.Handled = true;
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x00090E2C File Offset: 0x0008F02C
		private void HandleGibTrigger(EntityUid uid, GibOnTriggerComponent component, TriggerEvent args)
		{
			TransformComponent xform;
			if (!base.TryComp<TransformComponent>(uid, ref xform))
			{
				return;
			}
			this._body.GibBody(new EntityUid?(xform.ParentUid), false, null, component.DeleteItems);
			args.Handled = true;
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x00090E6C File Offset: 0x0008F06C
		private void OnTriggerCollide(EntityUid uid, TriggerOnCollideComponent component, ref StartCollideEvent args)
		{
			if (args.OurFixture.ID == component.FixtureID && (!component.IgnoreOtherNonHard || args.OtherFixture.Hard))
			{
				this.Trigger(component.Owner, null);
			}
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x00090EBC File Offset: 0x0008F0BC
		private void OnActivate(EntityUid uid, TriggerOnActivateComponent component, ActivateInWorldEvent args)
		{
			this.Trigger(component.Owner, new EntityUid?(args.User));
			args.Handled = true;
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x00090EE0 File Offset: 0x0008F0E0
		private void OnImplantTrigger(EntityUid uid, TriggerImplantActionComponent component, ActivateImplantEvent args)
		{
			this.Trigger(uid, null);
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x00090EFE File Offset: 0x0008F0FE
		private void OnStepTriggered(EntityUid uid, TriggerOnStepTriggerComponent component, ref StepTriggeredEvent args)
		{
			this.Trigger(uid, new EntityUid?(args.Tripper));
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x00090F14 File Offset: 0x0008F114
		public bool Trigger(EntityUid trigger, EntityUid? user = null)
		{
			TriggerEvent triggerEvent = new TriggerEvent(trigger, user);
			this.EntityManager.EventBus.RaiseLocalEvent<TriggerEvent>(trigger, triggerEvent, true);
			return triggerEvent.Handled;
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x00090F44 File Offset: 0x0008F144
		[NullableContext(2)]
		public void HandleTimerTrigger(EntityUid uid, EntityUid? user, float delay, float beepInterval, float? initialBeepDelay, SoundSpecifier beepSound, AudioParams beepParams)
		{
			if (delay <= 0f)
			{
				base.RemComp<ActiveTimerTriggerComponent>(uid);
				this.Trigger(uid, user);
				return;
			}
			if (base.HasComp<ActiveTimerTriggerComponent>(uid))
			{
				return;
			}
			if (user != null)
			{
				IContainer container;
				ChemicalPayloadComponent chemicalPayloadComponent;
				if (this._container.TryGetContainer(uid, "payload", ref container, null) && container.ContainedEntities.Count > 0 && base.TryComp<ChemicalPayloadComponent>(container.ContainedEntities[0], ref chemicalPayloadComponent))
				{
					SolutionContainerManagerComponent beakerA;
					SolutionContainerManagerComponent beakerB;
					if (!base.TryComp<SolutionContainerManagerComponent>((chemicalPayloadComponent != null) ? chemicalPayloadComponent.BeakerSlotA.Item : null, ref beakerA) || !base.TryComp<SolutionContainerManagerComponent>((chemicalPayloadComponent != null) ? chemicalPayloadComponent.BeakerSlotB.Item : null, ref beakerB))
					{
						return;
					}
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Trigger;
					LogStringHandler logStringHandler = new LogStringHandler(97, 5);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
					logStringHandler.AppendLiteral(" started a ");
					logStringHandler.AppendFormatted<float>(delay, "delay");
					logStringHandler.AppendLiteral(" second timer trigger on entity ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "timer", "ToPrettyString(uid)");
					logStringHandler.AppendLiteral(", which contains [");
					logStringHandler.AppendFormatted(string.Join<Solution.ReagentQuantity>(", ", beakerA.Solutions.Values.First<Solution>()));
					logStringHandler.AppendLiteral("] in one beaker and [");
					logStringHandler.AppendFormatted(string.Join<Solution.ReagentQuantity>(", ", beakerB.Solutions.Values.First<Solution>()));
					logStringHandler.AppendLiteral("] in the other.");
					adminLogger.Add(type, ref logStringHandler);
				}
				else
				{
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.Trigger;
					LogStringHandler logStringHandler = new LogStringHandler(43, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
					logStringHandler.AppendLiteral(" started a ");
					logStringHandler.AppendFormatted<float>(delay, "delay");
					logStringHandler.AppendLiteral(" second timer trigger on entity ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "timer", "ToPrettyString(uid)");
					adminLogger2.Add(type2, ref logStringHandler);
				}
			}
			else
			{
				ISharedAdminLogManager adminLogger3 = this._adminLogger;
				LogType type3 = LogType.Trigger;
				LogStringHandler logStringHandler = new LogStringHandler(40, 2);
				logStringHandler.AppendFormatted<float>(delay, "delay");
				logStringHandler.AppendLiteral(" second timer trigger started on entity ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "timer", "ToPrettyString(uid)");
				adminLogger3.Add(type3, ref logStringHandler);
			}
			ActiveTimerTriggerComponent active = base.AddComp<ActiveTimerTriggerComponent>(uid);
			active.TimeRemaining = delay;
			active.User = user;
			active.BeepParams = beepParams;
			active.BeepSound = beepSound;
			active.BeepInterval = beepInterval;
			active.TimeUntilBeep = ((initialBeepDelay == null) ? active.BeepInterval : initialBeepDelay.Value);
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, TriggerVisuals.VisualState, TriggerVisualState.Primed, appearance);
			}
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x0009121E File Offset: 0x0008F41E
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateProximity(frameTime);
			this.UpdateTimer(frameTime);
			this.UpdateTimedCollide(frameTime);
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x0009123C File Offset: 0x0008F43C
		private void UpdateTimer(float frameTime)
		{
			HashSet<EntityUid> toRemove = new HashSet<EntityUid>();
			foreach (ActiveTimerTriggerComponent timer in base.EntityQuery<ActiveTimerTriggerComponent>(false))
			{
				timer.TimeRemaining -= frameTime;
				timer.TimeUntilBeep -= frameTime;
				if (timer.TimeRemaining <= 0f)
				{
					this.Trigger(timer.Owner, timer.User);
					toRemove.Add(timer.Owner);
				}
				else if (timer.BeepSound != null && timer.TimeUntilBeep <= 0f)
				{
					timer.TimeUntilBeep += timer.BeepInterval;
					Filter filter = Filter.Pvs(timer.Owner, 2f, this.EntityManager, null, null);
					SoundSystem.Play(timer.BeepSound.GetSound(null, null), filter, timer.Owner, new AudioParams?(timer.BeepParams));
				}
			}
			foreach (EntityUid uid in toRemove)
			{
				base.RemComp<ActiveTimerTriggerComponent>(uid);
				AppearanceComponent appearance;
				if (base.TryComp<AppearanceComponent>(uid, ref appearance))
				{
					this._appearance.SetData(uid, TriggerVisuals.VisualState, TriggerVisualState.Unprimed, appearance);
				}
			}
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x000913A8 File Offset: 0x0008F5A8
		private void InitializeMobstate()
		{
			base.SubscribeLocalEvent<TriggerOnMobstateChangeComponent, MobStateChangedEvent>(new ComponentEventHandler<TriggerOnMobstateChangeComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<TriggerOnMobstateChangeComponent, SuicideEvent>(new ComponentEventHandler<TriggerOnMobstateChangeComponent, SuicideEvent>(this.OnSuicide), null, null);
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x000913D4 File Offset: 0x0008F5D4
		private void OnMobStateChanged(EntityUid uid, TriggerOnMobstateChangeComponent component, MobStateChangedEvent args)
		{
			if (component.MobState < args.NewMobState)
			{
				return;
			}
			OnUseTimerTriggerComponent timerTrigger;
			if (base.TryComp<OnUseTimerTriggerComponent>(uid, ref timerTrigger))
			{
				this.HandleTimerTrigger(uid, args.Origin, timerTrigger.Delay, timerTrigger.BeepInterval, timerTrigger.InitialBeepDelay, timerTrigger.BeepSound, timerTrigger.BeepParams);
				return;
			}
			this.Trigger(uid, null);
		}

		// Token: 0x06001B16 RID: 6934 RVA: 0x0009143C File Offset: 0x0008F63C
		private void OnSuicide(EntityUid uid, TriggerOnMobstateChangeComponent component, SuicideEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (component.PreventSuicide)
			{
				this._popupSystem.PopupEntity(Loc.GetString("suicide-prevented"), args.Victim, args.Victim, PopupType.Small);
				args.BlockSuicideAttempt(component.PreventSuicide);
			}
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x00091488 File Offset: 0x0008F688
		private void InitializeOnUse()
		{
			base.SubscribeLocalEvent<OnUseTimerTriggerComponent, UseInHandEvent>(new ComponentEventHandler<OnUseTimerTriggerComponent, UseInHandEvent>(this.OnTimerUse), null, null);
			base.SubscribeLocalEvent<OnUseTimerTriggerComponent, ExaminedEvent>(new ComponentEventHandler<OnUseTimerTriggerComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<OnUseTimerTriggerComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<OnUseTimerTriggerComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetAltVerbs), null, null);
			base.SubscribeLocalEvent<OnUseTimerTriggerComponent, EntityStuckEvent>(new ComponentEventHandler<OnUseTimerTriggerComponent, EntityStuckEvent>(this.OnStuck), null, null);
		}

		// Token: 0x06001B18 RID: 6936 RVA: 0x000914E5 File Offset: 0x0008F6E5
		private void OnStuck(EntityUid uid, OnUseTimerTriggerComponent component, EntityStuckEvent args)
		{
			if (!component.StartOnStick)
			{
				return;
			}
			this.HandleTimerTrigger(uid, new EntityUid?(args.User), component.Delay, component.BeepInterval, component.InitialBeepDelay, component.BeepSound, component.BeepParams);
		}

		// Token: 0x06001B19 RID: 6937 RVA: 0x00091520 File Offset: 0x0008F720
		private void OnExamined(EntityUid uid, OnUseTimerTriggerComponent component, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				args.PushText(Loc.GetString("examine-trigger-timer", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("time", component.Delay)
				}));
			}
		}

		// Token: 0x06001B1A RID: 6938 RVA: 0x0009155C File Offset: 0x0008F75C
		private void OnGetAltVerbs(EntityUid uid, OnUseTimerTriggerComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			if (component.DelayOptions == null || component.DelayOptions.Count == 1)
			{
				return;
			}
			args.Verbs.Add(new AlternativeVerb
			{
				Category = TriggerSystem.TimerOptions,
				Text = Loc.GetString("verb-trigger-timer-cycle"),
				Act = delegate()
				{
					this.CycleDelay(component, args.User);
				},
				Priority = 1
			});
			using (List<float>.Enumerator enumerator = component.DelayOptions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					float option = enumerator.Current;
					if (MathHelper.CloseTo(option, component.Delay, 1E-07f))
					{
						args.Verbs.Add(new AlternativeVerb
						{
							Category = TriggerSystem.TimerOptions,
							Text = Loc.GetString("verb-trigger-timer-set-current", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("time", option)
							}),
							Disabled = true,
							Priority = (int)(-100f * option)
						});
					}
					else
					{
						args.Verbs.Add(new AlternativeVerb
						{
							Category = TriggerSystem.TimerOptions,
							Text = Loc.GetString("verb-trigger-timer-set", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("time", option)
							}),
							Priority = (int)(-100f * option),
							Act = delegate()
							{
								component.Delay = option;
								this._popupSystem.PopupEntity(Loc.GetString("popup-trigger-timer-set", new ValueTuple<string, object>[]
								{
									new ValueTuple<string, object>("time", option)
								}), args.User, args.User, PopupType.Small);
							}
						});
					}
				}
			}
			if (component.AllowToggleStartOnStick)
			{
				args.Verbs.Add(new AlternativeVerb
				{
					Text = Loc.GetString("verb-toggle-start-on-stick"),
					Act = delegate()
					{
						this.ToggleStartOnStick(uid, args.User, component);
					}
				});
			}
		}

		// Token: 0x06001B1B RID: 6939 RVA: 0x000917D8 File Offset: 0x0008F9D8
		private void CycleDelay(OnUseTimerTriggerComponent component, EntityUid user)
		{
			if (component.DelayOptions == null || component.DelayOptions.Count == 1)
			{
				return;
			}
			component.DelayOptions.Sort();
			List<float> delayOptions = component.DelayOptions;
			if (delayOptions[delayOptions.Count - 1] <= component.Delay)
			{
				component.Delay = component.DelayOptions[0];
				this._popupSystem.PopupEntity(Loc.GetString("popup-trigger-timer-set", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("time", component.Delay)
				}), user, user, PopupType.Small);
				return;
			}
			foreach (float option in component.DelayOptions)
			{
				if (option > component.Delay)
				{
					component.Delay = option;
					this._popupSystem.PopupEntity(Loc.GetString("popup-trigger-timer-set", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("time", option)
					}), user, user, PopupType.Small);
					break;
				}
			}
		}

		// Token: 0x06001B1C RID: 6940 RVA: 0x000918F8 File Offset: 0x0008FAF8
		private void ToggleStartOnStick(EntityUid grenade, EntityUid user, OnUseTimerTriggerComponent comp)
		{
			if (comp.StartOnStick)
			{
				comp.StartOnStick = false;
				this._popupSystem.PopupEntity(Loc.GetString("popup-start-on-stick-off"), grenade, user, PopupType.Small);
				return;
			}
			comp.StartOnStick = true;
			this._popupSystem.PopupEntity(Loc.GetString("popup-start-on-stick-on"), grenade, user, PopupType.Small);
		}

		// Token: 0x06001B1D RID: 6941 RVA: 0x0009194C File Offset: 0x0008FB4C
		private void OnTimerUse(EntityUid uid, OnUseTimerTriggerComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.HandleTimerTrigger(uid, new EntityUid?(args.User), component.Delay, component.BeepInterval, component.InitialBeepDelay, component.BeepSound, component.BeepParams);
			args.Handled = true;
		}

		// Token: 0x06001B1E RID: 6942 RVA: 0x0009199C File Offset: 0x0008FB9C
		private void InitializeProximity()
		{
			base.SubscribeLocalEvent<TriggerOnProximityComponent, StartCollideEvent>(new ComponentEventRefHandler<TriggerOnProximityComponent, StartCollideEvent>(this.OnProximityStartCollide), null, null);
			ComponentEventRefHandler<TriggerOnProximityComponent, EndCollideEvent> componentEventRefHandler;
			if ((componentEventRefHandler = TriggerSystem.<>O.<0>__OnProximityEndCollide) == null)
			{
				componentEventRefHandler = (TriggerSystem.<>O.<0>__OnProximityEndCollide = new ComponentEventRefHandler<TriggerOnProximityComponent, EndCollideEvent>(TriggerSystem.OnProximityEndCollide));
			}
			base.SubscribeLocalEvent<TriggerOnProximityComponent, EndCollideEvent>(componentEventRefHandler, null, null);
			base.SubscribeLocalEvent<TriggerOnProximityComponent, MapInitEvent>(new ComponentEventHandler<TriggerOnProximityComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<TriggerOnProximityComponent, ComponentShutdown>(new ComponentEventHandler<TriggerOnProximityComponent, ComponentShutdown>(this.OnProximityShutdown), null, null);
			base.SubscribeLocalEvent<TriggerOnProximityComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<TriggerOnProximityComponent, AnchorStateChangedEvent>(this.OnProximityAnchor), null, null);
		}

		// Token: 0x06001B1F RID: 6943 RVA: 0x00091A1C File Offset: 0x0008FC1C
		private void OnProximityAnchor(EntityUid uid, TriggerOnProximityComponent component, ref AnchorStateChangedEvent args)
		{
			component.Enabled = (!component.RequiresAnchored || args.Anchored);
			this.SetProximityAppearance(uid, component);
			if (!component.Enabled)
			{
				this._activeProximities.Remove(component);
				component.Colliding.Clear();
				return;
			}
			PhysicsComponent body;
			if (base.TryComp<PhysicsComponent>(uid, ref body))
			{
				this._broadphase.RegenerateContacts(body, null, null);
			}
		}

		// Token: 0x06001B20 RID: 6944 RVA: 0x00091A82 File Offset: 0x0008FC82
		private void OnProximityShutdown(EntityUid uid, TriggerOnProximityComponent component, ComponentShutdown args)
		{
			this._activeProximities.Remove(component);
			component.Colliding.Clear();
		}

		// Token: 0x06001B21 RID: 6945 RVA: 0x00091A9C File Offset: 0x0008FC9C
		private void OnMapInit(EntityUid uid, TriggerOnProximityComponent component, MapInitEvent args)
		{
			component.Enabled = (!component.RequiresAnchored || this.EntityManager.GetComponent<TransformComponent>(uid).Anchored);
			this.SetProximityAppearance(uid, component);
			PhysicsComponent body;
			if (!base.TryComp<PhysicsComponent>(uid, ref body))
			{
				return;
			}
			this._fixtures.TryCreateFixture(uid, component.Shape, "trigger-on-proximity-fixture", 1f, false, 28, 0, 0.4f, 0f, true, null, null, null);
		}

		// Token: 0x06001B22 RID: 6946 RVA: 0x00091B0E File Offset: 0x0008FD0E
		private void OnProximityStartCollide(EntityUid uid, TriggerOnProximityComponent component, ref StartCollideEvent args)
		{
			if (args.OurFixture.ID != "trigger-on-proximity-fixture")
			{
				return;
			}
			this._activeProximities.Add(component);
			component.Colliding.Add(args.OtherFixture.Body);
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x00091B4C File Offset: 0x0008FD4C
		private static void OnProximityEndCollide(EntityUid uid, TriggerOnProximityComponent component, ref EndCollideEvent args)
		{
			if (args.OurFixture.ID != "trigger-on-proximity-fixture")
			{
				return;
			}
			component.Colliding.Remove(args.OtherFixture.Body);
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x00091B80 File Offset: 0x0008FD80
		private void SetProximityAppearance(EntityUid uid, TriggerOnProximityComponent component)
		{
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, ProximityTriggerVisualState.State, component.Enabled ? ProximityTriggerVisuals.Inactive : ProximityTriggerVisuals.Off, appearance);
			}
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x00091BC4 File Offset: 0x0008FDC4
		private void Activate(TriggerOnProximityComponent component)
		{
			if (!component.Repeating)
			{
				component.Enabled = false;
				this._activeProximities.Remove(component);
				component.Colliding.Clear();
			}
			else
			{
				component.Accumulator += component.Cooldown;
			}
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(component.Owner, ref appearance))
			{
				this._appearance.SetData(appearance.Owner, ProximityTriggerVisualState.State, ProximityTriggerVisuals.Active, appearance);
			}
			this.Trigger(component.Owner, null);
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x00091C58 File Offset: 0x0008FE58
		private void UpdateProximity(float frameTime)
		{
			RemQueue<TriggerOnProximityComponent> toRemove = default(RemQueue<TriggerOnProximityComponent>);
			foreach (TriggerOnProximityComponent comp in this._activeProximities)
			{
				MetaDataComponent metadata = null;
				if (base.Deleted(comp.Owner, metadata))
				{
					toRemove.Add(comp);
				}
				else
				{
					this.SetProximityAppearance(comp.Owner, comp);
					if (!base.Paused(comp.Owner, metadata))
					{
						comp.Accumulator -= frameTime;
						if (comp.Accumulator <= 0f)
						{
							if (!comp.Enabled || comp.Colliding.Count == 0)
							{
								comp.Accumulator = 0f;
								toRemove.Add(comp);
							}
							else
							{
								foreach (PhysicsComponent colliding in comp.Colliding)
								{
									if (!base.Deleted(colliding.Owner, null) && colliding.LinearVelocity.Length >= comp.TriggerSpeed)
									{
										this.Activate(comp);
										break;
									}
								}
							}
						}
					}
				}
			}
			foreach (TriggerOnProximityComponent prox in toRemove)
			{
				this._activeProximities.Remove(prox);
			}
		}

		// Token: 0x06001B27 RID: 6951 RVA: 0x00091DE8 File Offset: 0x0008FFE8
		private void InitializeSignal()
		{
			base.SubscribeLocalEvent<TriggerOnSignalComponent, SignalReceivedEvent>(new ComponentEventHandler<TriggerOnSignalComponent, SignalReceivedEvent>(this.OnSignalReceived), null, null);
			base.SubscribeLocalEvent<TriggerOnSignalComponent, ComponentInit>(new ComponentEventHandler<TriggerOnSignalComponent, ComponentInit>(this.OnInit), null, null);
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x00091E12 File Offset: 0x00090012
		private void OnSignalReceived(EntityUid uid, TriggerOnSignalComponent component, SignalReceivedEvent args)
		{
			if (args.Port != component.Port)
			{
				return;
			}
			this.Trigger(uid, args.Trigger);
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x00091E36 File Offset: 0x00090036
		private void OnInit(EntityUid uid, TriggerOnSignalComponent component, ComponentInit args)
		{
			this._signalSystem.EnsureReceiverPorts(uid, new string[]
			{
				component.Port
			});
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x00091E53 File Offset: 0x00090053
		private void InitializeTimedCollide()
		{
			base.SubscribeLocalEvent<TriggerOnTimedCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<TriggerOnTimedCollideComponent, StartCollideEvent>(this.OnTimerCollide), null, null);
			base.SubscribeLocalEvent<TriggerOnTimedCollideComponent, EndCollideEvent>(new ComponentEventRefHandler<TriggerOnTimedCollideComponent, EndCollideEvent>(this.OnTimerEndCollide), null, null);
			base.SubscribeLocalEvent<TriggerOnTimedCollideComponent, ComponentRemove>(new ComponentEventHandler<TriggerOnTimedCollideComponent, ComponentRemove>(this.OnComponentRemove), null, null);
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x00091E94 File Offset: 0x00090094
		private void OnTimerCollide(EntityUid uid, TriggerOnTimedCollideComponent component, ref StartCollideEvent args)
		{
			base.EnsureComp<ActiveTriggerOnTimedCollideComponent>(uid);
			EntityUid otherUID = args.OtherFixture.Body.Owner;
			if (component.Colliding.ContainsKey(otherUID))
			{
				return;
			}
			component.Colliding.Add(otherUID, 0f);
		}

		// Token: 0x06001B2C RID: 6956 RVA: 0x00091EDC File Offset: 0x000900DC
		private void OnTimerEndCollide(EntityUid uid, TriggerOnTimedCollideComponent component, ref EndCollideEvent args)
		{
			EntityUid otherUID = args.OtherFixture.Body.Owner;
			component.Colliding.Remove(otherUID);
			if (component.Colliding.Count == 0 && base.HasComp<ActiveTriggerOnTimedCollideComponent>(uid))
			{
				base.RemComp<ActiveTriggerOnTimedCollideComponent>(uid);
			}
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x00091F25 File Offset: 0x00090125
		private void OnComponentRemove(EntityUid uid, TriggerOnTimedCollideComponent component, ComponentRemove args)
		{
			if (base.HasComp<ActiveTriggerOnTimedCollideComponent>(uid))
			{
				base.RemComp<ActiveTriggerOnTimedCollideComponent>(uid);
			}
		}

		// Token: 0x06001B2E RID: 6958 RVA: 0x00091F38 File Offset: 0x00090138
		private void UpdateTimedCollide(float frameTime)
		{
			foreach (ValueTuple<ActiveTriggerOnTimedCollideComponent, TriggerOnTimedCollideComponent> valueTuple in base.EntityQuery<ActiveTriggerOnTimedCollideComponent, TriggerOnTimedCollideComponent>(false))
			{
				ActiveTriggerOnTimedCollideComponent activeTrigger = valueTuple.Item1;
				TriggerOnTimedCollideComponent triggerOnTimedCollide = valueTuple.Item2;
				foreach (KeyValuePair<EntityUid, float> keyValuePair in triggerOnTimedCollide.Colliding)
				{
					EntityUid entityUid;
					float num;
					keyValuePair.Deconstruct(out entityUid, out num);
					EntityUid collidingEntity = entityUid;
					float num2 = num;
					Dictionary<EntityUid, float> colliding = triggerOnTimedCollide.Colliding;
					entityUid = collidingEntity;
					colliding[entityUid] += frameTime;
					if (num2 > triggerOnTimedCollide.Threshold)
					{
						base.RaiseLocalEvent<TriggerEvent>(activeTrigger.Owner, new TriggerEvent(activeTrigger.Owner, new EntityUid?(collidingEntity)), true);
						colliding = triggerOnTimedCollide.Colliding;
						entityUid = collidingEntity;
						colliding[entityUid] -= triggerOnTimedCollide.Threshold;
					}
				}
			}
		}

		// Token: 0x06001B2F RID: 6959 RVA: 0x00092050 File Offset: 0x00090250
		private void InitializeVoice()
		{
			base.SubscribeLocalEvent<TriggerOnVoiceComponent, ComponentInit>(new ComponentEventHandler<TriggerOnVoiceComponent, ComponentInit>(this.OnVoiceInit), null, null);
			base.SubscribeLocalEvent<TriggerOnVoiceComponent, ExaminedEvent>(new ComponentEventHandler<TriggerOnVoiceComponent, ExaminedEvent>(this.OnVoiceExamine), null, null);
			base.SubscribeLocalEvent<TriggerOnVoiceComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<TriggerOnVoiceComponent, GetVerbsEvent<AlternativeVerb>>(this.OnVoiceGetAltVerbs), null, null);
			base.SubscribeLocalEvent<TriggerOnVoiceComponent, ListenEvent>(new ComponentEventHandler<TriggerOnVoiceComponent, ListenEvent>(this.OnListen), null, null);
		}

		// Token: 0x06001B30 RID: 6960 RVA: 0x000920AD File Offset: 0x000902AD
		private void OnVoiceInit(EntityUid uid, TriggerOnVoiceComponent component, ComponentInit args)
		{
			if (component.IsListening)
			{
				base.EnsureComp<ActiveListenerComponent>(uid).Range = (float)component.ListenRange;
				return;
			}
			base.RemCompDeferred<ActiveListenerComponent>(uid);
		}

		// Token: 0x06001B31 RID: 6961 RVA: 0x000920D4 File Offset: 0x000902D4
		private void OnListen(EntityUid uid, TriggerOnVoiceComponent component, ListenEvent args)
		{
			string message = args.Message.Trim();
			if (component.IsRecording)
			{
				if (message.Length >= component.MinLength || message.Length <= component.MaxLength)
				{
					this.FinishRecording(component, args.Source, args.Message);
				}
				return;
			}
			if (!string.IsNullOrWhiteSpace(component.KeyPhrase) && message.Contains(component.KeyPhrase, StringComparison.InvariantCultureIgnoreCase))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Trigger;
				LogImpact impact = LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(63, 3);
				logStringHandler.AppendLiteral("A voice-trigger on ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" was triggered by ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Source), "speaker", "ToPrettyString(args.Source)");
				logStringHandler.AppendLiteral(" speaking the key-phrase ");
				logStringHandler.AppendFormatted(component.KeyPhrase);
				logStringHandler.AppendLiteral(".");
				adminLogger.Add(type, impact, ref logStringHandler);
				this.Trigger(uid, new EntityUid?(args.Source));
			}
		}

		// Token: 0x06001B32 RID: 6962 RVA: 0x000921E8 File Offset: 0x000903E8
		private void OnVoiceGetAltVerbs(EntityUid uid, TriggerOnVoiceComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			args.Verbs.Add(new AlternativeVerb
			{
				Text = Loc.GetString(component.IsRecording ? "verb-trigger-voice-record-stop" : "verb-trigger-voice-record"),
				Act = delegate()
				{
					if (component.IsRecording)
					{
						this.StopRecording(component);
						return;
					}
					this.StartRecording(component, args.User);
				},
				Priority = 1
			});
			if (string.IsNullOrWhiteSpace(component.KeyPhrase))
			{
				return;
			}
			args.Verbs.Add(new AlternativeVerb
			{
				Text = Loc.GetString("verb-trigger-voice-clear"),
				Act = delegate()
				{
					component.KeyPhrase = null;
					component.IsRecording = false;
					this.RemComp<ActiveListenerComponent>(uid);
				}
			});
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x000922D0 File Offset: 0x000904D0
		public void StartRecording(TriggerOnVoiceComponent component, EntityUid user)
		{
			component.IsRecording = true;
			base.EnsureComp<ActiveListenerComponent>(component.Owner).Range = (float)component.ListenRange;
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Trigger;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(49, 2);
			logStringHandler.AppendLiteral("A voice-trigger on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "entity", "ToPrettyString(component.Owner)");
			logStringHandler.AppendLiteral(" has started recording. User: ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
			adminLogger.Add(type, impact, ref logStringHandler);
			this._popupSystem.PopupEntity(Loc.GetString("popup-trigger-voice-start-recording"), component.Owner, PopupType.Small);
		}

		// Token: 0x06001B34 RID: 6964 RVA: 0x0009237F File Offset: 0x0009057F
		public void StopRecording(TriggerOnVoiceComponent component)
		{
			component.IsRecording = false;
			if (string.IsNullOrWhiteSpace(component.KeyPhrase))
			{
				base.RemComp<ActiveListenerComponent>(component.Owner);
			}
			this._popupSystem.PopupEntity(Loc.GetString("popup-trigger-voice-stop-recording"), component.Owner, PopupType.Small);
		}

		// Token: 0x06001B35 RID: 6965 RVA: 0x000923C0 File Offset: 0x000905C0
		public void FinishRecording(TriggerOnVoiceComponent component, EntityUid source, string message)
		{
			component.KeyPhrase = message;
			component.IsRecording = false;
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Trigger;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(68, 3);
			logStringHandler.AppendLiteral("A voice-trigger on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "entity", "ToPrettyString(component.Owner)");
			logStringHandler.AppendLiteral(" has recorded a new keyphrase: '");
			logStringHandler.AppendFormatted(component.KeyPhrase);
			logStringHandler.AppendLiteral("'. Recorded from ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source), "speaker", "ToPrettyString(source)");
			adminLogger.Add(type, impact, ref logStringHandler);
			this._popupSystem.PopupEntity(Loc.GetString("popup-trigger-voice-recorded", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("keyphrase", component.KeyPhrase)
			}), component.Owner, PopupType.Small);
		}

		// Token: 0x06001B36 RID: 6966 RVA: 0x00092494 File Offset: 0x00090694
		private void OnVoiceExamine(EntityUid uid, TriggerOnVoiceComponent component, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				args.PushText(string.IsNullOrWhiteSpace(component.KeyPhrase) ? Loc.GetString("trigger-voice-uninitialized") : Loc.GetString("examine-trigger-voice", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("keyphrase", component.KeyPhrase)
				}));
			}
		}

		// Token: 0x04001151 RID: 4433
		[Dependency]
		private readonly ExplosionSystem _explosions;

		// Token: 0x04001152 RID: 4434
		[Dependency]
		private readonly FixtureSystem _fixtures;

		// Token: 0x04001153 RID: 4435
		[Dependency]
		private readonly FlashSystem _flashSystem;

		// Token: 0x04001154 RID: 4436
		[Dependency]
		private readonly SharedBroadphaseSystem _broadphase;

		// Token: 0x04001155 RID: 4437
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001156 RID: 4438
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04001157 RID: 4439
		[Dependency]
		private readonly BodySystem _body;

		// Token: 0x04001158 RID: 4440
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04001159 RID: 4441
		public static VerbCategory TimerOptions = new VerbCategory("verb-categories-timer", "/Textures/Interface/VerbIcons/clock.svg.192dpi.png", false);

		// Token: 0x0400115A RID: 4442
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400115B RID: 4443
		private HashSet<TriggerOnProximityComponent> _activeProximities = new HashSet<TriggerOnProximityComponent>();

		// Token: 0x0400115C RID: 4444
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x02000A0A RID: 2570
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002321 RID: 8993
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<TriggerOnProximityComponent, EndCollideEvent> <0>__OnProximityEndCollide;
		}
	}
}
