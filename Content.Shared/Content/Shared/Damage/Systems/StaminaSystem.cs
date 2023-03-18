using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.CombatMode;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Database;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Rounding;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems
{
	// Token: 0x02000539 RID: 1337
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StaminaSystem : EntitySystem
	{
		// Token: 0x06001048 RID: 4168 RVA: 0x0003511C File Offset: 0x0003331C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StaminaComponent, ComponentStartup>(new ComponentEventHandler<StaminaComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<StaminaComponent, ComponentShutdown>(new ComponentEventHandler<StaminaComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<StaminaComponent, ComponentGetState>(new ComponentEventRefHandler<StaminaComponent, ComponentGetState>(this.OnStamGetState), null, null);
			base.SubscribeLocalEvent<StaminaComponent, ComponentHandleState>(new ComponentEventRefHandler<StaminaComponent, ComponentHandleState>(this.OnStamHandleState), null, null);
			base.SubscribeLocalEvent<StaminaComponent, DisarmedEvent>(new ComponentEventHandler<StaminaComponent, DisarmedEvent>(this.OnDisarmed), null, null);
			base.SubscribeLocalEvent<StaminaDamageOnCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<StaminaDamageOnCollideComponent, StartCollideEvent>(this.OnCollide), null, null);
			base.SubscribeLocalEvent<StaminaDamageOnHitComponent, MeleeHitEvent>(new ComponentEventHandler<StaminaDamageOnHitComponent, MeleeHitEvent>(this.OnHit), null, null);
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x000351BC File Offset: 0x000333BC
		private void OnStamGetState(EntityUid uid, StaminaComponent component, ref ComponentGetState args)
		{
			args.State = new StaminaSystem.StaminaComponentState
			{
				Critical = component.Critical,
				Decay = component.Decay,
				CritThreshold = component.CritThreshold,
				DecayCooldown = component.DecayCooldown,
				LastUpdate = component.NextUpdate,
				StaminaDamage = component.StaminaDamage
			};
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x0003521C File Offset: 0x0003341C
		private void OnStamHandleState(EntityUid uid, StaminaComponent component, ref ComponentHandleState args)
		{
			StaminaSystem.StaminaComponentState state = args.Current as StaminaSystem.StaminaComponentState;
			if (state == null)
			{
				return;
			}
			component.Critical = state.Critical;
			component.Decay = state.Decay;
			component.CritThreshold = state.CritThreshold;
			component.DecayCooldown = state.DecayCooldown;
			component.NextUpdate = state.LastUpdate;
			component.StaminaDamage = state.StaminaDamage;
			if (component.Critical)
			{
				this.EnterStamCrit(uid, component);
				return;
			}
			if (component.StaminaDamage > 0f)
			{
				base.EnsureComp<ActiveStaminaComponent>(uid);
			}
			this.ExitStamCrit(uid, component);
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x000352AF File Offset: 0x000334AF
		private void OnShutdown(EntityUid uid, StaminaComponent component, ComponentShutdown args)
		{
			if (base.MetaData(uid).EntityLifeStage < 4)
			{
				base.RemCompDeferred<ActiveStaminaComponent>(uid);
			}
			this.SetStaminaAlert(uid, null);
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x000352D0 File Offset: 0x000334D0
		private void OnStartup(EntityUid uid, StaminaComponent component, ComponentStartup args)
		{
			this.SetStaminaAlert(uid, component);
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x000352DC File Offset: 0x000334DC
		[NullableContext(2)]
		public float GetStaminaDamage(EntityUid uid, StaminaComponent component = null)
		{
			if (!base.Resolve<StaminaComponent>(uid, ref component, true))
			{
				return 0f;
			}
			TimeSpan curTime = this._timing.CurTime;
			TimeSpan pauseTime = this._metadata.GetPauseTime(uid, null);
			return MathF.Max(0f, component.StaminaDamage - MathF.Max(0f, (float)(curTime - (component.NextUpdate + pauseTime)).TotalSeconds * component.Decay));
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00035354 File Offset: 0x00033554
		private void OnDisarmed(EntityUid uid, StaminaComponent component, DisarmedEvent args)
		{
			if (args.Handled || !RandomExtensions.Prob(this._random, args.PushProbability))
			{
				return;
			}
			if (component.Critical)
			{
				return;
			}
			float damage = args.PushProbability * component.CritThreshold;
			this.TakeStaminaDamage(uid, damage, component, new EntityUid?(args.Source), null);
			if (!component.Critical)
			{
				return;
			}
			EntityUid targetEnt = Identity.Entity(args.Target, this.EntityManager);
			EntityUid sourceEnt = Identity.Entity(args.Source, this.EntityManager);
			this._popup.PopupEntity(Loc.GetString("stunned-component-disarm-success-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("source", sourceEnt),
				new ValueTuple<string, object>("target", targetEnt)
			}), targetEnt, Filter.PvsExcept(args.Source, 2f, null), true, PopupType.LargeCaution);
			this._popup.PopupCursor(Loc.GetString("stunned-component-disarm-success", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", targetEnt)
			}), args.Source, PopupType.Large);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.DisarmedKnockdown;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(14, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Source), "user", "ToPrettyString(args.Source)");
			logStringHandler.AppendLiteral(" knocked down ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target), "target", "ToPrettyString(args.Target)");
			adminLogger.Add(type, impact, ref logStringHandler);
			args.Handled = true;
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x000354DC File Offset: 0x000336DC
		private void OnHit(EntityUid uid, StaminaDamageOnHitComponent component, MeleeHitEvent args)
		{
			if (!args.IsHit || !args.HitEntities.Any<EntityUid>() || component.Damage <= 0f)
			{
				return;
			}
			StaminaDamageOnHitAttemptEvent ev = default(StaminaDamageOnHitAttemptEvent);
			base.RaiseLocalEvent<StaminaDamageOnHitAttemptEvent>(uid, ref ev, false);
			if (ev.Cancelled)
			{
				return;
			}
			args.HitSoundOverride = ev.HitSoundOverride;
			EntityQuery<StaminaComponent> stamQuery = base.GetEntityQuery<StaminaComponent>();
			List<StaminaComponent> toHit = new List<StaminaComponent>();
			foreach (EntityUid ent in args.HitEntities)
			{
				StaminaComponent stam;
				if (stamQuery.TryGetComponent(ent, ref stam))
				{
					toHit.Add(stam);
				}
			}
			StaminaMeleeHitEvent hitEvent = new StaminaMeleeHitEvent(toHit);
			base.RaiseLocalEvent<StaminaMeleeHitEvent>(uid, hitEvent, false);
			if (hitEvent.Handled)
			{
				return;
			}
			float damage = component.Damage;
			damage *= hitEvent.Multiplier;
			damage += hitEvent.FlatModifier;
			foreach (StaminaComponent comp in toHit)
			{
				float oldDamage = comp.StaminaDamage;
				this.TakeStaminaDamage(comp.Owner, damage / (float)toHit.Count, comp, new EntityUid?(args.User), new EntityUid?(component.Owner));
				if (comp.StaminaDamage.Equals(oldDamage))
				{
					this._popup.PopupEntity(Loc.GetString("stamina-resist"), comp.Owner, args.User, PopupType.Small);
				}
			}
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x00035670 File Offset: 0x00033870
		private void OnCollide(EntityUid uid, StaminaDamageOnCollideComponent component, ref StartCollideEvent args)
		{
			if (!args.OurFixture.ID.Equals("projectile"))
			{
				return;
			}
			this.TakeStaminaDamage(args.OtherFixture.Body.Owner, component.Damage, null, new EntityUid?(args.OurFixture.Body.Owner), null);
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x000356D0 File Offset: 0x000338D0
		[NullableContext(2)]
		private void SetStaminaAlert(EntityUid uid, StaminaComponent component = null)
		{
			if (!base.Resolve<StaminaComponent>(uid, ref component, false) || component.Deleted)
			{
				this._alerts.ClearAlert(uid, AlertType.Stamina);
				return;
			}
			int severity = ContentHelpers.RoundToLevels((double)MathF.Max(0f, component.CritThreshold - component.StaminaDamage), (double)component.CritThreshold, 7);
			this._alerts.ShowAlert(uid, AlertType.Stamina, new short?((short)severity), null);
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x00035744 File Offset: 0x00033944
		[NullableContext(2)]
		public void TakeStaminaDamage(EntityUid uid, float value, StaminaComponent component = null, EntityUid? source = null, EntityUid? with = null)
		{
			if (!base.Resolve<StaminaComponent>(uid, ref component, false) || component.Critical)
			{
				return;
			}
			float staminaDamage = component.StaminaDamage;
			component.StaminaDamage = MathF.Max(0f, component.StaminaDamage + value);
			if (staminaDamage < component.StaminaDamage)
			{
				TimeSpan nextUpdate = this._timing.CurTime + TimeSpan.FromSeconds((double)component.DecayCooldown);
				if (component.NextUpdate < nextUpdate)
				{
					component.NextUpdate = nextUpdate;
				}
			}
			float slowdownThreshold = component.CritThreshold / 2f;
			if (staminaDamage < slowdownThreshold && component.StaminaDamage > slowdownThreshold)
			{
				this._stunSystem.TrySlowdown(uid, TimeSpan.FromSeconds(3.0), true, 0.8f, 0.8f, null);
			}
			this.SetStaminaAlert(uid, component);
			if (!component.Critical)
			{
				if (component.StaminaDamage >= component.CritThreshold)
				{
					this.EnterStamCrit(uid, component);
				}
			}
			else if (component.StaminaDamage < component.CritThreshold)
			{
				this.ExitStamCrit(uid, component);
			}
			base.EnsureComp<ActiveStaminaComponent>(uid);
			base.Dirty(component, null);
			if (value <= 0f)
			{
				return;
			}
			LogStringHandler logStringHandler;
			if (source != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Stamina;
				logStringHandler = new LogStringHandler(27, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(source.Value), "user", "ToPrettyString(source.Value)");
				logStringHandler.AppendLiteral(" caused ");
				logStringHandler.AppendFormatted<float>(value, "value");
				logStringHandler.AppendLiteral(" stamina damage to ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
				string value2;
				if (with == null)
				{
					value2 = "";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
					defaultInterpolatedStringHandler.AppendLiteral(" using ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(with.Value), "using");
					value2 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				logStringHandler.AppendFormatted(value2);
				adminLogger.Add(type, ref logStringHandler);
				return;
			}
			ISharedAdminLogManager adminLogger2 = this._adminLogger;
			LogType type2 = LogType.Stamina;
			logStringHandler = new LogStringHandler(21, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" took ");
			logStringHandler.AppendFormatted<float>(value, "value");
			logStringHandler.AppendLiteral(" stamina damage");
			adminLogger2.Add(type2, ref logStringHandler);
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x0003597C File Offset: 0x00033B7C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			base.GetEntityQuery<MetaDataComponent>();
			EntityQuery<StaminaComponent> stamQuery = base.GetEntityQuery<StaminaComponent>();
			TimeSpan curTime = this._timing.CurTime;
			foreach (ActiveStaminaComponent active in base.EntityQuery<ActiveStaminaComponent>(false))
			{
				StaminaComponent comp;
				if (!stamQuery.TryGetComponent(active.Owner, ref comp) || (comp.StaminaDamage <= 0f && !comp.Critical))
				{
					base.RemComp<ActiveStaminaComponent>(active.Owner);
				}
				else if (!(comp.NextUpdate > curTime))
				{
					if (comp.Critical)
					{
						this.ExitStamCrit(active.Owner, comp);
					}
					else
					{
						comp.NextUpdate += TimeSpan.FromSeconds(1.0);
						this.TakeStaminaDamage(comp.Owner, -comp.Decay, comp, null, null);
						base.Dirty(comp, null);
					}
				}
			}
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x00035AAC File Offset: 0x00033CAC
		[NullableContext(2)]
		private void EnterStamCrit(EntityUid uid, StaminaComponent component = null)
		{
			if (!base.Resolve<StaminaComponent>(uid, ref component, true) || component.Critical)
			{
				return;
			}
			component.Critical = true;
			component.StaminaDamage = component.CritThreshold;
			TimeSpan stunTime = TimeSpan.FromSeconds(6.0);
			this._stunSystem.TryParalyze(uid, stunTime, true, null);
			component.NextUpdate = this._timing.CurTime + stunTime + StaminaSystem.StamCritBufferTime;
			base.EnsureComp<ActiveStaminaComponent>(uid);
			base.Dirty(component, null);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Stamina;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(21, 1);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "user", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" entered stamina crit");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x00035B70 File Offset: 0x00033D70
		[NullableContext(2)]
		private void ExitStamCrit(EntityUid uid, StaminaComponent component = null)
		{
			if (!base.Resolve<StaminaComponent>(uid, ref component, true) || !component.Critical)
			{
				return;
			}
			component.Critical = false;
			component.StaminaDamage = 0f;
			component.NextUpdate = this._timing.CurTime;
			this.SetStaminaAlert(uid, component);
			base.RemComp<ActiveStaminaComponent>(uid);
			base.Dirty(component, null);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Stamina;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(28, 1);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "user", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" recovered from stamina crit");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x04000F53 RID: 3923
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000F54 RID: 3924
		[Dependency]
		private readonly AlertsSystem _alerts;

		// Token: 0x04000F55 RID: 3925
		[Dependency]
		private readonly MetaDataSystem _metadata;

		// Token: 0x04000F56 RID: 3926
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x04000F57 RID: 3927
		[Dependency]
		private readonly SharedStunSystem _stunSystem;

		// Token: 0x04000F58 RID: 3928
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000F59 RID: 3929
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000F5A RID: 3930
		private const string CollideFixture = "projectile";

		// Token: 0x04000F5B RID: 3931
		private static readonly TimeSpan StamCritBufferTime = TimeSpan.FromSeconds(3.0);

		// Token: 0x02000837 RID: 2103
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class StaminaComponentState : ComponentState
		{
			// Token: 0x04001932 RID: 6450
			public bool Critical;

			// Token: 0x04001933 RID: 6451
			public float Decay;

			// Token: 0x04001934 RID: 6452
			public float DecayCooldown;

			// Token: 0x04001935 RID: 6453
			public float StaminaDamage;

			// Token: 0x04001936 RID: 6454
			public float CritThreshold;

			// Token: 0x04001937 RID: 6455
			public TimeSpan LastUpdate;
		}
	}
}
