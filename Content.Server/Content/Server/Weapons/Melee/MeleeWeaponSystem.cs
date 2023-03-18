using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Actions.Events;
using Content.Server.Administration.Components;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.CombatMode;
using Content.Server.CombatMode.Disarm;
using Content.Server.Contests;
using Content.Server.Examine;
using Content.Server.Hands.Components;
using Content.Server.Movement.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Weapons.Melee
{
	// Token: 0x020000B7 RID: 183
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeleeWeaponSystem : SharedMeleeWeaponSystem
	{
		// Token: 0x060002EA RID: 746 RVA: 0x0000FECB File Offset: 0x0000E0CB
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MeleeChemicalInjectorComponent, MeleeHitEvent>(new ComponentEventHandler<MeleeChemicalInjectorComponent, MeleeHitEvent>(this.OnChemicalInjectorHit), null, null);
			base.SubscribeLocalEvent<MeleeWeaponComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<MeleeWeaponComponent, GetVerbsEvent<ExamineVerb>>(this.OnMeleeExaminableVerb), null, null);
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0000FEFC File Offset: 0x0000E0FC
		private void OnMeleeExaminableVerb(EntityUid uid, MeleeWeaponComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess || component.HideFromExamine)
			{
				return;
			}
			MeleeHitEvent getDamage = new MeleeHitEvent(new List<EntityUid>(), args.User, component.Damage);
			getDamage.IsHit = false;
			base.RaiseLocalEvent<MeleeHitEvent>(uid, getDamage, false);
			DamageSpecifier damageSpec = this.GetDamage(component);
			if (damageSpec == null)
			{
				damageSpec = new DamageSpecifier();
			}
			damageSpec += getDamage.BonusDamage;
			if (damageSpec.Total == FixedPoint2.Zero)
			{
				return;
			}
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate()
				{
					FormattedMessage markup = this.Damageable.GetDamageExamine(damageSpec, Loc.GetString("damage-melee"));
					this._examine.SendExamineTooltip(args.User, uid, markup, false, false);
				},
				Text = Loc.GetString("damage-examinable-verb-text"),
				Message = Loc.GetString("damage-examinable-verb-message"),
				Category = VerbCategory.Examine,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/smite.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00010036 File Offset: 0x0000E236
		[return: Nullable(2)]
		private DamageSpecifier GetDamage(MeleeWeaponComponent component)
		{
			if (!(component.Damage.Total > FixedPoint2.Zero))
			{
				return null;
			}
			return component.Damage;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00010058 File Offset: 0x0000E258
		protected override void Popup(string message, EntityUid? uid, EntityUid? user)
		{
			if (uid == null)
			{
				return;
			}
			if (user == null)
			{
				this.PopupSystem.PopupEntity(message, uid.Value, PopupType.Small);
				return;
			}
			this.PopupSystem.PopupEntity(message, uid.Value, Filter.PvsExcept(user.Value, 2f, this.EntityManager), true, PopupType.Small);
		}

		// Token: 0x060002EE RID: 750 RVA: 0x000100BC File Offset: 0x0000E2BC
		protected override bool DoDisarm(EntityUid user, DisarmAttackEvent ev, EntityUid meleeUid, MeleeWeaponComponent component, [Nullable(2)] ICommonSession session)
		{
			if (!base.DoDisarm(user, ev, meleeUid, component, session))
			{
				return false;
			}
			CombatModeComponent combatMode;
			if (base.TryComp<CombatModeComponent>(user, ref combatMode))
			{
				bool? canDisarm = combatMode.CanDisarm;
				bool flag = true;
				if (canDisarm.GetValueOrDefault() == flag & canDisarm != null)
				{
					EntityUid target = ev.Target.Value;
					HandsComponent targetHandsComponent;
					StatusEffectsComponent status;
					if (!base.TryComp<HandsComponent>(ev.Target.Value, ref targetHandsComponent) && (!base.TryComp<StatusEffectsComponent>(ev.Target.Value, ref status) || !status.AllowedEffects.Contains("KnockedDown")))
					{
						return false;
					}
					if (!this.InRange(user, ev.Target.Value, component.Range, session))
					{
						return false;
					}
					EntityUid? inTargetHand = null;
					Hand hand = (targetHandsComponent != null) ? targetHandsComponent.ActiveHand : null;
					if (hand != null && !hand.IsEmpty)
					{
						inTargetHand = new EntityUid?(targetHandsComponent.ActiveHand.HeldEntity.Value);
					}
					this.Interaction.DoContactInteraction(user, ev.Target, null);
					DisarmAttemptEvent attemptEvent = new DisarmAttemptEvent(target, user, inTargetHand);
					if (inTargetHand != null)
					{
						base.RaiseLocalEvent<DisarmAttemptEvent>(inTargetHand.Value, attemptEvent, false);
					}
					base.RaiseLocalEvent<DisarmAttemptEvent>(target, attemptEvent, false);
					if (attemptEvent.Cancelled)
					{
						return false;
					}
					float chance = this.CalculateDisarmChance(user, target, inTargetHand, combatMode);
					if (RandomExtensions.Prob(this._random, chance))
					{
						return false;
					}
					Filter filterOther = Filter.PvsExcept(user, 2f, this.EntityManager);
					string msgPrefix = "disarm-action-";
					if (inTargetHand == null)
					{
						msgPrefix = "disarm-action-shove-";
					}
					string msgOther = Loc.GetString(msgPrefix + "popup-message-other-clients", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("performerName", Identity.Entity(user, this.EntityManager)),
						new ValueTuple<string, object>("targetName", Identity.Entity(target, this.EntityManager))
					});
					string msgUser = Loc.GetString(msgPrefix + "popup-message-cursor", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("targetName", Identity.Entity(target, this.EntityManager))
					});
					this.PopupSystem.PopupEntity(msgOther, user, filterOther, true, PopupType.Small);
					this.PopupSystem.PopupEntity(msgUser, target, user, PopupType.Small);
					this.Audio.PlayPvs(combatMode.DisarmSuccessSound, user, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.025f)).WithVolume(5f)));
					ISharedAdminLogManager adminLogger = this.AdminLogger;
					LogType type = LogType.DisarmedAction;
					LogStringHandler logStringHandler = new LogStringHandler(16, 2);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
					logStringHandler.AppendLiteral(" used disarm on ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
					adminLogger.Add(type, ref logStringHandler);
					DisarmedEvent eventArgs = new DisarmedEvent
					{
						Target = target,
						Source = user,
						PushProbability = 1f - chance
					};
					base.RaiseLocalEvent<DisarmedEvent>(target, eventArgs, false);
					base.RaiseNetworkEvent(new DamageEffectEvent(Color.Aqua, new List<EntityUid>
					{
						target
					}));
					return true;
				}
			}
			return false;
		}

		// Token: 0x060002EF RID: 751 RVA: 0x000103D4 File Offset: 0x0000E5D4
		[NullableContext(2)]
		protected override bool InRange(EntityUid user, EntityUid target, float range, ICommonSession session)
		{
			IPlayerSession pSession = session as IPlayerSession;
			EntityCoordinates targetCoordinates;
			Angle targetLocalAngle;
			if (pSession != null)
			{
				ValueTuple<EntityCoordinates, Angle> coordinatesAngle = this._lag.GetCoordinatesAngle(target, pSession, null);
				targetCoordinates = coordinatesAngle.Item1;
				targetLocalAngle = coordinatesAngle.Item2;
			}
			else
			{
				TransformComponent transformComponent = base.Transform(target);
				targetCoordinates = transformComponent.Coordinates;
				targetLocalAngle = transformComponent.LocalRotation;
			}
			return this.Interaction.InRangeUnobstructed(user, target, targetCoordinates, targetLocalAngle, range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false);
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00010434 File Offset: 0x0000E634
		protected override void DoDamageEffect(List<EntityUid> targets, EntityUid? user, TransformComponent targetXform)
		{
			Filter filter = Filter.Pvs(targetXform.Coordinates, 2f, this.EntityManager, null).RemoveWhereAttachedEntity((EntityUid o) => o == user);
			base.RaiseNetworkEvent(new DamageEffectEvent(Color.Red, targets), filter, true);
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0001048C File Offset: 0x0000E68C
		private float CalculateDisarmChance(EntityUid disarmer, EntityUid disarmed, EntityUid? inTargetHand, SharedCombatModeComponent disarmerComp)
		{
			if (base.HasComp<DisarmProneComponent>(disarmer))
			{
				return 1f;
			}
			if (base.HasComp<DisarmProneComponent>(disarmed))
			{
				return 0f;
			}
			float contestResults = 1f - this._contests.OverallStrengthContest(disarmer, disarmed, 1f, 1f, 1f);
			float chance = disarmerComp.BaseDisarmFailChance + contestResults;
			DisarmMalusComponent malus;
			if (inTargetHand != null && base.TryComp<DisarmMalusComponent>(inTargetHand, ref malus))
			{
				chance += malus.Malus;
			}
			return Math.Clamp(chance, 0f, 1f);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00010511 File Offset: 0x0000E711
		[NullableContext(2)]
		public override void DoLunge(EntityUid user, Angle angle, Vector2 localPos, string animation)
		{
			base.RaiseNetworkEvent(new MeleeLungeEvent(user, angle, localPos, animation), Filter.PvsExcept(user, 2f, this.EntityManager), true);
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00010538 File Offset: 0x0000E738
		private void OnChemicalInjectorHit(EntityUid owner, MeleeChemicalInjectorComponent comp, MeleeHitEvent args)
		{
			Solution solutionContainer;
			if (!args.IsHit || !args.HitEntities.Any<EntityUid>() || !this._solutions.TryGetSolution(owner, comp.Solution, out solutionContainer, null))
			{
				return;
			}
			List<BloodstreamComponent> hitBloodstreams = new List<BloodstreamComponent>();
			EntityQuery<BloodstreamComponent> bloodQuery = base.GetEntityQuery<BloodstreamComponent>();
			foreach (EntityUid entity in args.HitEntities)
			{
				BloodstreamComponent bloodstream;
				if (!base.Deleted(entity, null) && bloodQuery.TryGetComponent(entity, ref bloodstream))
				{
					hitBloodstreams.Add(bloodstream);
				}
			}
			if (!hitBloodstreams.Any<BloodstreamComponent>())
			{
				return;
			}
			Solution solution = solutionContainer.SplitSolution(comp.TransferAmount * hitBloodstreams.Count);
			FixedPoint2 removedVol = solution.Volume;
			Solution solutionToInject = solution.SplitSolution(removedVol * comp.TransferEfficiency);
			FixedPoint2 volPerBloodstream = solutionToInject.Volume * (1 / hitBloodstreams.Count);
			foreach (BloodstreamComponent bloodstream2 in hitBloodstreams)
			{
				Solution individualInjection = solutionToInject.SplitSolution(volPerBloodstream);
				this._bloodstream.TryAddToChemicals(bloodstream2.Owner, individualInjection, bloodstream2);
			}
		}

		// Token: 0x040001FE RID: 510
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040001FF RID: 511
		[Dependency]
		private readonly BloodstreamSystem _bloodstream;

		// Token: 0x04000200 RID: 512
		[Dependency]
		private readonly ContestsSystem _contests;

		// Token: 0x04000201 RID: 513
		[Dependency]
		private readonly ExamineSystem _examine;

		// Token: 0x04000202 RID: 514
		[Dependency]
		private readonly LagCompensationSystem _lag;

		// Token: 0x04000203 RID: 515
		[Dependency]
		private readonly SolutionContainerSystem _solutions;
	}
}
