using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Weapons.Melee
{
	// Token: 0x02000070 RID: 112
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedMeleeWeaponSystem : EntitySystem
	{
		// Token: 0x06000150 RID: 336 RVA: 0x00007298 File Offset: 0x00005498
		public override void Initialize()
		{
			base.Initialize();
			this.Sawmill = Logger.GetSawmill("melee");
			base.SubscribeLocalEvent<MeleeWeaponComponent, ComponentGetState>(new ComponentEventRefHandler<MeleeWeaponComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<MeleeWeaponComponent, ComponentHandleState>(new ComponentEventRefHandler<MeleeWeaponComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<MeleeWeaponComponent, HandDeselectedEvent>(new ComponentEventHandler<MeleeWeaponComponent, HandDeselectedEvent>(this.OnMeleeDropped), null, null);
			base.SubscribeLocalEvent<MeleeWeaponComponent, HandSelectedEvent>(new ComponentEventHandler<MeleeWeaponComponent, HandSelectedEvent>(this.OnMeleeSelected), null, null);
			base.SubscribeAllEvent<LightAttackEvent>(new EntitySessionEventHandler<LightAttackEvent>(this.OnLightAttack), null, null);
			base.SubscribeAllEvent<StartHeavyAttackEvent>(new EntitySessionEventHandler<StartHeavyAttackEvent>(this.OnStartHeavyAttack), null, null);
			base.SubscribeAllEvent<StopHeavyAttackEvent>(new EntitySessionEventHandler<StopHeavyAttackEvent>(this.OnStopHeavyAttack), null, null);
			base.SubscribeAllEvent<HeavyAttackEvent>(new EntitySessionEventHandler<HeavyAttackEvent>(this.OnHeavyAttack), null, null);
			base.SubscribeAllEvent<DisarmAttackEvent>(new EntitySessionEventHandler<DisarmAttackEvent>(this.OnDisarmAttack), null, null);
			base.SubscribeAllEvent<StopAttackEvent>(new EntitySessionEventHandler<StopAttackEvent>(this.OnStopAttack), null, null);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00007384 File Offset: 0x00005584
		private void OnMeleeSelected(EntityUid uid, MeleeWeaponComponent component, HandSelectedEvent args)
		{
			if (component.AttackRate.Equals(0f))
			{
				return;
			}
			if (!component.ResetOnHandSelected)
			{
				return;
			}
			TimeSpan minimum = this.Timing.CurTime + TimeSpan.FromSeconds((double)(1f / component.AttackRate));
			if (minimum < component.NextAttack)
			{
				return;
			}
			component.NextAttack = minimum;
			base.Dirty(component, null);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x000073EE File Offset: 0x000055EE
		private void OnMeleeDropped(EntityUid uid, MeleeWeaponComponent component, HandDeselectedEvent args)
		{
			if (component.WindUpStart == null)
			{
				return;
			}
			component.WindUpStart = null;
			base.Dirty(component, null);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007414 File Offset: 0x00005614
		private void OnStopAttack(StopAttackEvent msg, EntitySessionEventArgs args)
		{
			EntityUid? user = args.SenderSession.AttachedEntity;
			if (user == null)
			{
				return;
			}
			MeleeWeaponComponent weapon = this.GetWeapon(user.Value);
			EntityUid? entityUid = (weapon != null) ? new EntityUid?(weapon.Owner) : null;
			EntityUid weapon2 = msg.Weapon;
			if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != weapon2))
			{
				return;
			}
			if (!weapon.Attacking)
			{
				return;
			}
			weapon.Attacking = false;
			base.Dirty(weapon, null);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x000074A8 File Offset: 0x000056A8
		private void OnStartHeavyAttack(StartHeavyAttackEvent msg, EntitySessionEventArgs args)
		{
			EntityUid? user = args.SenderSession.AttachedEntity;
			if (user == null)
			{
				return;
			}
			MeleeWeaponComponent weapon = this.GetWeapon(user.Value);
			EntityUid? entityUid = (weapon != null) ? new EntityUid?(weapon.Owner) : null;
			EntityUid weapon2 = msg.Weapon;
			if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != weapon2))
			{
				return;
			}
			weapon.WindUpStart = new TimeSpan?(this.Timing.CurTime);
			base.Dirty(weapon, null);
		}

		// Token: 0x06000155 RID: 341
		protected abstract void Popup(string message, EntityUid? uid, EntityUid? user);

		// Token: 0x06000156 RID: 342 RVA: 0x00007544 File Offset: 0x00005744
		private void OnLightAttack(LightAttackEvent msg, EntitySessionEventArgs args)
		{
			EntityUid? user = args.SenderSession.AttachedEntity;
			if (user == null)
			{
				return;
			}
			MeleeWeaponComponent weapon = this.GetWeapon(user.Value);
			EntityUid? entityUid = (weapon != null) ? new EntityUid?(weapon.Owner) : null;
			EntityUid weapon2 = msg.Weapon;
			if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != weapon2))
			{
				return;
			}
			this.AttemptAttack(args.SenderSession.AttachedEntity.Value, msg.Weapon, weapon, msg, args.SenderSession);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x000075EC File Offset: 0x000057EC
		private void OnStopHeavyAttack(StopHeavyAttackEvent msg, EntitySessionEventArgs args)
		{
			MeleeWeaponComponent weapon;
			if (args.SenderSession.AttachedEntity == null || !base.TryComp<MeleeWeaponComponent>(msg.Weapon, ref weapon))
			{
				return;
			}
			if (this.GetWeapon(args.SenderSession.AttachedEntity.Value) != weapon)
			{
				return;
			}
			if (weapon.WindUpStart.Equals(null))
			{
				return;
			}
			weapon.WindUpStart = null;
			base.Dirty(weapon, null);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00007668 File Offset: 0x00005868
		private void OnHeavyAttack(HeavyAttackEvent msg, EntitySessionEventArgs args)
		{
			MeleeWeaponComponent weapon;
			if (args.SenderSession.AttachedEntity == null || !base.TryComp<MeleeWeaponComponent>(msg.Weapon, ref weapon))
			{
				return;
			}
			if (this.GetWeapon(args.SenderSession.AttachedEntity.Value) != weapon)
			{
				return;
			}
			this.AttemptAttack(args.SenderSession.AttachedEntity.Value, msg.Weapon, weapon, msg, args.SenderSession);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x000076E4 File Offset: 0x000058E4
		private void OnDisarmAttack(DisarmAttackEvent msg, EntitySessionEventArgs args)
		{
			if (args.SenderSession.AttachedEntity == null)
			{
				return;
			}
			MeleeWeaponComponent userWeapon = this.GetWeapon(args.SenderSession.AttachedEntity.Value);
			if (userWeapon == null)
			{
				return;
			}
			this.AttemptAttack(args.SenderSession.AttachedEntity.Value, userWeapon.Owner, userWeapon, msg, args.SenderSession);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007750 File Offset: 0x00005950
		private void OnGetState(EntityUid uid, MeleeWeaponComponent component, ref ComponentGetState args)
		{
			args.State = new MeleeWeaponComponentState(component.AttackRate, component.Attacking, component.NextAttack, component.WindUpStart, component.ClickAnimation, component.WideAnimation, component.Range);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00007788 File Offset: 0x00005988
		private void OnHandleState(EntityUid uid, MeleeWeaponComponent component, ref ComponentHandleState args)
		{
			MeleeWeaponComponentState state = args.Current as MeleeWeaponComponentState;
			if (state == null)
			{
				return;
			}
			component.Attacking = state.Attacking;
			component.AttackRate = state.AttackRate;
			component.NextAttack = state.NextAttack;
			component.WindUpStart = state.WindUpStart;
			component.ClickAnimation = state.ClickAnimation;
			component.WideAnimation = state.WideAnimation;
			component.Range = state.Range;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000077FC File Offset: 0x000059FC
		[NullableContext(2)]
		public MeleeWeaponComponent GetWeapon(EntityUid entity)
		{
			GetMeleeWeaponEvent ev = new GetMeleeWeaponEvent();
			base.RaiseLocalEvent<GetMeleeWeaponEvent>(entity, ev, false);
			if (ev.Handled)
			{
				return EntityManagerExt.GetComponentOrNull<MeleeWeaponComponent>(this.EntityManager, ev.Weapon);
			}
			SharedHandsComponent hands;
			MeleeWeaponComponent melee;
			if (this.EntityManager.TryGetComponent<SharedHandsComponent>(entity, ref hands))
			{
				EntityUid? activeHandEntity = hands.ActiveHandEntity;
				if (activeHandEntity != null)
				{
					EntityUid held = activeHandEntity.GetValueOrDefault();
					if (this.EntityManager.TryGetComponent<MeleeWeaponComponent>(held, ref melee))
					{
						return melee;
					}
					return null;
				}
			}
			EntityUid? gloves;
			MeleeWeaponComponent glovesMelee;
			if (this.Inventory.TryGetSlotEntity(entity, "gloves", out gloves, null, null) && base.TryComp<MeleeWeaponComponent>(gloves, ref glovesMelee))
			{
				return glovesMelee;
			}
			if (base.TryComp<MeleeWeaponComponent>(entity, ref melee))
			{
				return melee;
			}
			return null;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x000078A4 File Offset: 0x00005AA4
		public void AttemptLightAttackMiss(EntityUid user, EntityUid weaponUid, MeleeWeaponComponent weapon, EntityCoordinates coordinates)
		{
			this.AttemptAttack(user, weaponUid, weapon, new LightAttackEvent(null, weaponUid, coordinates), null);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000078CC File Offset: 0x00005ACC
		public void AttemptLightAttack(EntityUid user, EntityUid weaponUid, MeleeWeaponComponent weapon, EntityUid target)
		{
			TransformComponent targetXform;
			if (!base.TryComp<TransformComponent>(target, ref targetXform))
			{
				return;
			}
			this.AttemptAttack(user, weaponUid, weapon, new LightAttackEvent(new EntityUid?(target), weaponUid, targetXform.Coordinates), null);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00007904 File Offset: 0x00005B04
		public void AttemptDisarmAttack(EntityUid user, EntityUid weaponUid, MeleeWeaponComponent weapon, EntityUid target)
		{
			TransformComponent targetXform;
			if (!base.TryComp<TransformComponent>(target, ref targetXform))
			{
				return;
			}
			this.AttemptAttack(user, weaponUid, weapon, new DisarmAttackEvent(new EntityUid?(target), targetXform.Coordinates), null);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000793C File Offset: 0x00005B3C
		private void AttemptAttack(EntityUid user, EntityUid weaponUid, MeleeWeaponComponent weapon, AttackEvent attack, [Nullable(2)] ICommonSession session)
		{
			TimeSpan curTime = this.Timing.CurTime;
			if (weapon.NextAttack > curTime)
			{
				return;
			}
			if (!this.CombatMode.IsInCombatMode(new EntityUid?(user), null))
			{
				return;
			}
			LightAttackEvent light = attack as LightAttackEvent;
			if (light == null)
			{
				DisarmAttackEvent disarm = attack as DisarmAttackEvent;
				if (disarm == null)
				{
					if (!this.Blocker.CanAttack(user, null))
					{
						return;
					}
				}
				else if (!this.Blocker.CanAttack(user, disarm.Target))
				{
					return;
				}
			}
			else if (!this.Blocker.CanAttack(user, light.Target))
			{
				return;
			}
			if (weapon.NextAttack < curTime)
			{
				weapon.NextAttack = curTime;
			}
			weapon.NextAttack += TimeSpan.FromSeconds((double)(1f / weapon.AttackRate));
			LightAttackEvent light2 = attack as LightAttackEvent;
			string animation;
			if (light2 == null)
			{
				DisarmAttackEvent disarm2 = attack as DisarmAttackEvent;
				if (disarm2 == null)
				{
					HeavyAttackEvent heavy = attack as HeavyAttackEvent;
					if (heavy == null)
					{
						throw new NotImplementedException();
					}
					this.DoHeavyAttack(user, heavy, weaponUid, weapon, session);
					animation = weapon.WideAnimation;
				}
				else
				{
					if (!this.DoDisarm(user, disarm2, weaponUid, weapon, session))
					{
						return;
					}
					animation = weapon.ClickAnimation;
				}
			}
			else
			{
				this.DoLightAttack(user, light2, weaponUid, weapon, session);
				animation = weapon.ClickAnimation;
			}
			this.DoLungeAnimation(user, weapon.Angle, attack.Coordinates.ToMap(this.EntityManager), weapon.Range, animation);
			weapon.Attacking = true;
			base.Dirty(weapon, null);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00007AB8 File Offset: 0x00005CB8
		public float GetModifier(MeleeWeaponComponent component, bool lightAttack)
		{
			if (lightAttack)
			{
				return 1f;
			}
			TimeSpan? windup = component.WindUpStart;
			if (windup == null)
			{
				return 0f;
			}
			double totalSeconds = (this.Timing.CurTime - windup.Value).TotalSeconds;
			double windupTime = component.WindupTime.TotalSeconds;
			double releaseDiff = Math.Abs(totalSeconds % (2.0 * windupTime) - windupTime);
			if (releaseDiff < 0.0)
			{
				releaseDiff = Math.Min(0.0, releaseDiff + 0.05000000074505806);
			}
			else
			{
				releaseDiff = Math.Max(0.0, releaseDiff - 0.05000000074505806);
			}
			double fraction = (windupTime - releaseDiff) / windupTime;
			if (fraction < 0.4)
			{
				fraction = 0.0;
			}
			return (float)fraction * component.HeavyDamageModifier.Float();
		}

		// Token: 0x06000162 RID: 354
		[NullableContext(2)]
		protected abstract bool InRange(EntityUid user, EntityUid target, float range, ICommonSession session);

		// Token: 0x06000163 RID: 355 RVA: 0x00007B94 File Offset: 0x00005D94
		protected virtual void DoLightAttack(EntityUid user, LightAttackEvent ev, EntityUid meleeUid, MeleeWeaponComponent component, [Nullable(2)] ICommonSession session)
		{
			DamageSpecifier damage = component.Damage * this.GetModifier(component, true);
			TransformComponent targetXform;
			if (user == ev.Target || base.Deleted(ev.Target) || !base.HasComp<DamageableComponent>(ev.Target) || !base.TryComp<TransformComponent>(ev.Target, ref targetXform) || !this.InRange(user, ev.Target.Value, component.Range, session))
			{
				MeleeHitEvent missEvent = new MeleeHitEvent(new List<EntityUid>(), user, damage);
				base.RaiseLocalEvent<MeleeHitEvent>(meleeUid, missEvent, false);
				this.Audio.PlayPredicted(component.SwingSound, meleeUid, new EntityUid?(user), null);
				return;
			}
			MeleeHitEvent hitEvent = new MeleeHitEvent(new List<EntityUid>
			{
				ev.Target.Value
			}, user, damage);
			base.RaiseLocalEvent<MeleeHitEvent>(meleeUid, hitEvent, false);
			if (hitEvent.Handled)
			{
				return;
			}
			List<EntityUid> targets = new List<EntityUid>(1)
			{
				ev.Target.Value
			};
			this.Interaction.DoContactInteraction(ev.Weapon, ev.Target, null);
			this.Interaction.DoContactInteraction(user, new EntityUid?(ev.Weapon), null);
			this.Interaction.DoContactInteraction(user, ev.Target, null);
			base.RaiseLocalEvent<AttackedEvent>(ev.Target.Value, new AttackedEvent(meleeUid, user, targetXform.Coordinates), false);
			DamageSpecifier modifiedDamage = DamageSpecifier.ApplyModifierSets(damage + hitEvent.BonusDamage, hitEvent.ModifiersList);
			DamageSpecifier damageResult = this.Damageable.TryChangeDamage(ev.Target, modifiedDamage, false, true, null, new EntityUid?(user));
			if (damageResult != null && damageResult.Total > FixedPoint2.Zero)
			{
				FixedPoint2 bluntDamage;
				if (damageResult.DamageDict.TryGetValue("Blunt", out bluntDamage))
				{
					this._stamina.TakeStaminaDamage(ev.Target.Value, (bluntDamage * component.BluntStaminaDamageFactor).Float(), null, new EntityUid?(user), (component.Owner == user) ? null : new EntityUid?(component.Owner));
				}
				if (meleeUid == user)
				{
					ISharedAdminLogManager adminLogger = this.AdminLogger;
					LogType type = LogType.MeleeHit;
					LogStringHandler logStringHandler = new LogStringHandler(52, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
					logStringHandler.AppendLiteral(" melee attacked ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(ev.Target.Value), "target", "ToPrettyString(ev.Target.Value)");
					logStringHandler.AppendLiteral(" using their hands and dealt ");
					logStringHandler.AppendFormatted<FixedPoint2>(damageResult.Total, "damage", "damageResult.Total");
					logStringHandler.AppendLiteral(" damage");
					adminLogger.Add(type, ref logStringHandler);
				}
				else
				{
					ISharedAdminLogManager adminLogger2 = this.AdminLogger;
					LogType type2 = LogType.MeleeHit;
					LogStringHandler logStringHandler = new LogStringHandler(41, 4);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
					logStringHandler.AppendLiteral(" melee attacked ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(ev.Target.Value), "target", "ToPrettyString(ev.Target.Value)");
					logStringHandler.AppendLiteral(" using ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "used", "ToPrettyString(component.Owner)");
					logStringHandler.AppendLiteral(" and dealt ");
					logStringHandler.AppendFormatted<FixedPoint2>(damageResult.Total, "damage", "damageResult.Total");
					logStringHandler.AppendLiteral(" damage");
					adminLogger2.Add(type2, ref logStringHandler);
				}
				this.PlayHitSound(ev.Target.Value, new EntityUid?(user), SharedMeleeWeaponSystem.GetHighestDamageSound(modifiedDamage, this._protoManager), hitEvent.HitSoundOverride, component.HitSound);
			}
			else if (hitEvent.HitSoundOverride != null)
			{
				this.Audio.PlayPredicted(hitEvent.HitSoundOverride, meleeUid, new EntityUid?(user), null);
			}
			else
			{
				this.Audio.PlayPredicted(component.NoDamageSound, meleeUid, new EntityUid?(user), null);
			}
			if (((damageResult != null) ? new FixedPoint2?(damageResult.Total) : null) > FixedPoint2.Zero)
			{
				this.DoDamageEffect(targets, new EntityUid?(user), targetXform);
			}
		}

		// Token: 0x06000164 RID: 356
		protected abstract void DoDamageEffect(List<EntityUid> targets, EntityUid? user, TransformComponent targetXform);

		// Token: 0x06000165 RID: 357 RVA: 0x00008000 File Offset: 0x00006200
		protected virtual void DoHeavyAttack(EntityUid user, HeavyAttackEvent ev, EntityUid meleeUid, MeleeWeaponComponent component, [Nullable(2)] ICommonSession session)
		{
			TransformComponent userXform;
			if (!base.TryComp<TransformComponent>(user, ref userXform))
			{
				return;
			}
			MapCoordinates targetMap = ev.Coordinates.ToMap(this.EntityManager);
			if (targetMap.MapId != userXform.MapID)
			{
				return;
			}
			Vector2 userPos = this._transform.GetWorldPosition(userXform);
			Vector2 direction = targetMap.Position - userPos;
			float distance = Math.Min(component.Range, direction.Length);
			DamageSpecifier damage = component.Damage * this.GetModifier(component, false);
			HashSet<EntityUid> entities = this.ArcRayCast(userPos, DirectionExtensions.ToWorldAngle(direction), component.Angle, distance, userXform.MapID, user);
			if (entities.Count == 0)
			{
				MeleeHitEvent missEvent = new MeleeHitEvent(new List<EntityUid>(), user, damage);
				base.RaiseLocalEvent<MeleeHitEvent>(meleeUid, missEvent, false);
				this.Audio.PlayPredicted(component.SwingSound, meleeUid, new EntityUid?(user), null);
				return;
			}
			List<EntityUid> targets = new List<EntityUid>();
			EntityQuery<DamageableComponent> damageQuery = base.GetEntityQuery<DamageableComponent>();
			foreach (EntityUid entity in entities)
			{
				if (!(entity == user) && damageQuery.HasComponent(entity))
				{
					targets.Add(entity);
				}
			}
			MeleeHitEvent hitEvent = new MeleeHitEvent(targets, user, damage);
			base.RaiseLocalEvent<MeleeHitEvent>(meleeUid, hitEvent, false);
			if (hitEvent.Handled)
			{
				return;
			}
			this.Interaction.DoContactInteraction(user, new EntityUid?(ev.Weapon), null);
			foreach (EntityUid target in targets)
			{
				this.Interaction.DoContactInteraction(ev.Weapon, new EntityUid?(target), null);
				this.Interaction.DoContactInteraction(user, new EntityUid?(target), null);
				base.RaiseLocalEvent<AttackedEvent>(target, new AttackedEvent(meleeUid, user, base.Transform(target).Coordinates), false);
			}
			DamageSpecifier modifiedDamage = DamageSpecifier.ApplyModifierSets(damage + hitEvent.BonusDamage, hitEvent.ModifiersList);
			DamageSpecifier appliedDamage = new DamageSpecifier();
			foreach (EntityUid entity2 in targets)
			{
				base.RaiseLocalEvent<AttackedEvent>(entity2, new AttackedEvent(meleeUid, user, ev.Coordinates), false);
				DamageSpecifier damageResult = this.Damageable.TryChangeDamage(new EntityUid?(entity2), modifiedDamage, false, true, null, new EntityUid?(user));
				if (damageResult != null && damageResult.Total > FixedPoint2.Zero)
				{
					appliedDamage += damageResult;
					if (meleeUid == user)
					{
						ISharedAdminLogManager adminLogger = this.AdminLogger;
						LogType type = LogType.MeleeHit;
						LogStringHandler logStringHandler = new LogStringHandler(52, 3);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
						logStringHandler.AppendLiteral(" melee attacked ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity2), "target", "ToPrettyString(entity)");
						logStringHandler.AppendLiteral(" using their hands and dealt ");
						logStringHandler.AppendFormatted<FixedPoint2>(damageResult.Total, "damage", "damageResult.Total");
						logStringHandler.AppendLiteral(" damage");
						adminLogger.Add(type, ref logStringHandler);
					}
					else
					{
						ISharedAdminLogManager adminLogger2 = this.AdminLogger;
						LogType type2 = LogType.MeleeHit;
						LogStringHandler logStringHandler = new LogStringHandler(41, 4);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
						logStringHandler.AppendLiteral(" melee attacked ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity2), "target", "ToPrettyString(entity)");
						logStringHandler.AppendLiteral(" using ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "used", "ToPrettyString(component.Owner)");
						logStringHandler.AppendLiteral(" and dealt ");
						logStringHandler.AppendFormatted<FixedPoint2>(damageResult.Total, "damage", "damageResult.Total");
						logStringHandler.AppendLiteral(" damage");
						adminLogger2.Add(type2, ref logStringHandler);
					}
				}
			}
			if (entities.Count != 0)
			{
				if (appliedDamage.Total > FixedPoint2.Zero)
				{
					EntityUid target2 = entities.First<EntityUid>();
					this.PlayHitSound(target2, new EntityUid?(user), SharedMeleeWeaponSystem.GetHighestDamageSound(modifiedDamage, this._protoManager), hitEvent.HitSoundOverride, component.HitSound);
				}
				else if (hitEvent.HitSoundOverride != null)
				{
					this.Audio.PlayPredicted(hitEvent.HitSoundOverride, meleeUid, new EntityUid?(user), null);
				}
				else
				{
					this.Audio.PlayPredicted(component.NoDamageSound, meleeUid, new EntityUid?(user), null);
				}
			}
			if (appliedDamage.Total > FixedPoint2.Zero)
			{
				this.DoDamageEffect(targets, new EntityUid?(user), base.Transform(targets[0]));
			}
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00008510 File Offset: 0x00006710
		private HashSet<EntityUid> ArcRayCast(Vector2 position, Angle angle, Angle arcWidth, float range, MapId mapId, EntityUid ignore)
		{
			int increments = 1 + 35 * (int)Math.Ceiling(arcWidth / 6.283185307179586);
			double increment = arcWidth / (double)increments;
			Angle baseAngle = angle - arcWidth / 2.0;
			HashSet<EntityUid> resSet = new HashSet<EntityUid>();
			for (int i = 0; i < increments; i++)
			{
				Angle castAngle;
				castAngle..ctor(baseAngle + increment * (double)i);
				List<RayCastResults> res = this._physics.IntersectRay(mapId, new CollisionRay(position, castAngle.ToWorldVec(), 31), range, new EntityUid?(ignore), false).ToList<RayCastResults>();
				if (res.Count != 0)
				{
					resSet.Add(res[0].HitEntity);
				}
			}
			return resSet;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x000085E8 File Offset: 0x000067E8
		[NullableContext(2)]
		private void PlayHitSound(EntityUid target, EntityUid? user, string type, SoundSpecifier hitSoundOverride, SoundSpecifier hitSound)
		{
			bool playedSound = false;
			MeleeSoundComponent damageSoundComp;
			if (base.TryComp<MeleeSoundComponent>(target, ref damageSoundComp))
			{
				if (type == null && damageSoundComp.NoDamageSound != null)
				{
					this.Audio.PlayPredicted(damageSoundComp.NoDamageSound, target, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
					playedSound = true;
				}
				else
				{
					if (type != null)
					{
						Dictionary<string, SoundSpecifier> soundTypes = damageSoundComp.SoundTypes;
						SoundSpecifier damageSoundType;
						if (soundTypes != null && soundTypes.TryGetValue(type, out damageSoundType))
						{
							this.Audio.PlayPredicted(damageSoundType, target, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
							playedSound = true;
							goto IL_D9;
						}
					}
					if (type != null)
					{
						Dictionary<string, SoundSpecifier> soundGroups = damageSoundComp.SoundGroups;
						SoundSpecifier damageSoundGroup;
						if (soundGroups != null && soundGroups.TryGetValue(type, out damageSoundGroup))
						{
							this.Audio.PlayPredicted(damageSoundGroup, target, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
							playedSound = true;
						}
					}
				}
			}
			IL_D9:
			if (!playedSound)
			{
				if (hitSoundOverride != null)
				{
					this.Audio.PlayPredicted(hitSoundOverride, target, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
					playedSound = true;
				}
				else if (hitSound != null)
				{
					this.Audio.PlayPredicted(hitSound, target, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
					playedSound = true;
				}
			}
			if (!playedSound)
			{
				if (type == "Burn" || type == "Heat" || type == "Cold")
				{
					this.Audio.PlayPredicted(new SoundPathSpecifier("/Audio/Items/welder.ogg", null), target, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
					return;
				}
				if (type == null)
				{
					this.Audio.PlayPredicted(new SoundPathSpecifier("/Audio/Weapons/tap.ogg", null), target, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
					return;
				}
				if (!(type == "Brute"))
				{
					return;
				}
				this.Audio.PlayPredicted(new SoundPathSpecifier("/Audio/Weapons/smash.ogg", null), target, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00008824 File Offset: 0x00006A24
		[return: Nullable(2)]
		public static string GetHighestDamageSound(DamageSpecifier modifiedDamage, IPrototypeManager protoManager)
		{
			Dictionary<string, FixedPoint2> groups = modifiedDamage.GetDamagePerGroup(protoManager);
			if (groups.Count == 1)
			{
				return groups.Keys.First<string>();
			}
			FixedPoint2 highestDamage = FixedPoint2.Zero;
			string highestDamageType = null;
			foreach (KeyValuePair<string, FixedPoint2> keyValuePair in modifiedDamage.DamageDict)
			{
				string text;
				FixedPoint2 a;
				keyValuePair.Deconstruct(out text, out a);
				string type = text;
				if (!(a <= highestDamage))
				{
					highestDamageType = type;
				}
			}
			return highestDamageType;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x000088B4 File Offset: 0x00006AB4
		protected virtual bool DoDisarm(EntityUid user, DisarmAttackEvent ev, EntityUid meleeUid, MeleeWeaponComponent component, [Nullable(2)] ICommonSession session)
		{
			if (base.Deleted(ev.Target) || user == ev.Target)
			{
				return false;
			}
			this.Audio.PlayPredicted(component.SwingSound, meleeUid, new EntityUid?(user), null);
			return true;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000891C File Offset: 0x00006B1C
		[NullableContext(2)]
		private void DoLungeAnimation(EntityUid user, Angle angle, MapCoordinates coordinates, float length, string animation)
		{
			TransformComponent userXform;
			if (!base.TryComp<TransformComponent>(user, ref userXform))
			{
				return;
			}
			Vector2 localPos = userXform.InvWorldMatrix.Transform(coordinates.Position);
			if (localPos.LengthSquared <= 0f)
			{
				return;
			}
			localPos = userXform.LocalRotation.RotateVec(ref localPos);
			float visualLength = length - 0.2f;
			if (localPos.Length > visualLength)
			{
				localPos = localPos.Normalized * visualLength;
			}
			this.DoLunge(user, angle, localPos, animation);
		}

		// Token: 0x0600016B RID: 363
		[NullableContext(2)]
		public abstract void DoLunge(EntityUid user, Angle angle, Vector2 localPos, string animation);

		// Token: 0x04000174 RID: 372
		[Dependency]
		protected readonly IGameTiming Timing;

		// Token: 0x04000175 RID: 373
		[Dependency]
		protected readonly IMapManager MapManager;

		// Token: 0x04000176 RID: 374
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x04000177 RID: 375
		[Dependency]
		protected readonly ISharedAdminLogManager AdminLogger;

		// Token: 0x04000178 RID: 376
		[Dependency]
		protected readonly ActionBlockerSystem Blocker;

		// Token: 0x04000179 RID: 377
		[Dependency]
		protected readonly DamageableSystem Damageable;

		// Token: 0x0400017A RID: 378
		[Dependency]
		protected readonly InventorySystem Inventory;

		// Token: 0x0400017B RID: 379
		[Dependency]
		protected readonly SharedAudioSystem Audio;

		// Token: 0x0400017C RID: 380
		[Dependency]
		protected readonly SharedCombatModeSystem CombatMode;

		// Token: 0x0400017D RID: 381
		[Dependency]
		protected readonly SharedInteractionSystem Interaction;

		// Token: 0x0400017E RID: 382
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x0400017F RID: 383
		[Dependency]
		protected readonly SharedPopupSystem PopupSystem;

		// Token: 0x04000180 RID: 384
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000181 RID: 385
		[Dependency]
		private readonly StaminaSystem _stamina;

		// Token: 0x04000182 RID: 386
		protected ISawmill Sawmill;

		// Token: 0x04000183 RID: 387
		public const float DamagePitchVariation = 0.05f;

		// Token: 0x04000184 RID: 388
		private const int AttackMask = 31;

		// Token: 0x04000185 RID: 389
		public const float GracePeriod = 0.05f;
	}
}
