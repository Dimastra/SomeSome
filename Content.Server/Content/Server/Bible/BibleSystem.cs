using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Bible.Components;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Ghost.Roles.Events;
using Content.Server.Popups;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Bible
{
	// Token: 0x0200071E RID: 1822
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BibleSystem : EntitySystem
	{
		// Token: 0x06002653 RID: 9811 RVA: 0x000CA444 File Offset: 0x000C8644
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BibleComponent, AfterInteractEvent>(new ComponentEventHandler<BibleComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<SummonableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<SummonableComponent, GetVerbsEvent<AlternativeVerb>>(this.AddSummonVerb), null, null);
			base.SubscribeLocalEvent<SummonableComponent, GetItemActionsEvent>(new ComponentEventHandler<SummonableComponent, GetItemActionsEvent>(this.GetSummonAction), null, null);
			base.SubscribeLocalEvent<SummonableComponent, SummonActionEvent>(new ComponentEventHandler<SummonableComponent, SummonActionEvent>(this.OnSummon), null, null);
			base.SubscribeLocalEvent<FamiliarComponent, MobStateChangedEvent>(new ComponentEventHandler<FamiliarComponent, MobStateChangedEvent>(this.OnFamiliarDeath), null, null);
			base.SubscribeLocalEvent<FamiliarComponent, GhostRoleSpawnerUsedEvent>(new ComponentEventHandler<FamiliarComponent, GhostRoleSpawnerUsedEvent>(this.OnSpawned), null, null);
		}

		// Token: 0x06002654 RID: 9812 RVA: 0x000CA4D0 File Offset: 0x000C86D0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (EntityUid entity in this._addQueue)
			{
				base.EnsureComp<SummonableRespawningComponent>(entity);
			}
			this._addQueue.Clear();
			foreach (EntityUid entity2 in this._remQueue)
			{
				base.RemComp<SummonableRespawningComponent>(entity2);
			}
			this._remQueue.Clear();
			foreach (ValueTuple<SummonableRespawningComponent, SummonableComponent> valueTuple in base.EntityQuery<SummonableRespawningComponent, SummonableComponent>(false))
			{
				SummonableRespawningComponent respawning = valueTuple.Item1;
				SummonableComponent summonableComp = valueTuple.Item2;
				summonableComp.Accumulator += frameTime;
				if (summonableComp.Accumulator >= summonableComp.RespawnTime)
				{
					if (summonableComp.Summon != null)
					{
						this.EntityManager.DeleteEntity(summonableComp.Summon.Value);
						summonableComp.Summon = null;
					}
					summonableComp.AlreadySummoned = false;
					this._popupSystem.PopupEntity(Loc.GetString("bible-summon-respawn-ready", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("book", summonableComp.Owner)
					}), summonableComp.Owner, PopupType.Medium);
					SoundSystem.Play("/Audio/Effects/radpulse9.ogg", Filter.Pvs(summonableComp.Owner, 2f, null, null, null), summonableComp.Owner, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
					summonableComp.Accumulator = 0f;
					this._remQueue.Enqueue(respawning.Owner);
				}
			}
		}

		// Token: 0x06002655 RID: 9813 RVA: 0x000CA6E8 File Offset: 0x000C88E8
		private void OnAfterInteract(EntityUid uid, BibleComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach)
			{
				return;
			}
			UseDelayComponent delay = null;
			if (this._delay.ActiveDelay(uid, delay))
			{
				return;
			}
			EntityUid? target = args.Target;
			if (target != null)
			{
				target = args.Target;
				EntityUid user = args.User;
				if (target == null || (target != null && !(target.GetValueOrDefault() == user)))
				{
					MobStateSystem mobStateSystem = this._mobStateSystem;
					target = args.Target;
					if (mobStateSystem.IsAlive(target.Value, null))
					{
						if (!base.HasComp<BibleUserComponent>(args.User))
						{
							this._popupSystem.PopupEntity(Loc.GetString("bible-sizzle"), args.User, args.User, PopupType.Small);
							SoundSystem.Play(component.SizzleSoundPath.GetSound(null, null), Filter.Pvs(args.User, 2f, null, null, null), args.User, null);
							this._damageableSystem.TryChangeDamage(new EntityUid?(args.User), component.DamageOnUntrainedUse, true, true, null, new EntityUid?(uid));
							this._delay.BeginDelay(uid, delay);
							return;
						}
						if (!this._invSystem.TryGetSlotEntity(args.Target.Value, "head", out target, null, null))
						{
							target = args.Target;
							if (!base.HasComp<FamiliarComponent>(target.Value) && RandomExtensions.Prob(this._random, component.FailChance))
							{
								string text = component.LocPrefix + "-heal-fail-others";
								ValueTuple<string, object>[] array = new ValueTuple<string, object>[3];
								array[0] = new ValueTuple<string, object>("user", Identity.Entity(args.User, this.EntityManager));
								int num = 1;
								string item = "target";
								target = args.Target;
								array[num] = new ValueTuple<string, object>(item, Identity.Entity(target.Value, this.EntityManager));
								array[2] = new ValueTuple<string, object>("bible", uid);
								string othersFailMessage = Loc.GetString(text, array);
								this._popupSystem.PopupEntity(othersFailMessage, args.User, Filter.PvsExcept(args.User, 2f, null), true, PopupType.SmallCaution);
								string text2 = component.LocPrefix + "-heal-fail-self";
								ValueTuple<string, object>[] array2 = new ValueTuple<string, object>[2];
								int num2 = 0;
								string item2 = "target";
								target = args.Target;
								array2[num2] = new ValueTuple<string, object>(item2, Identity.Entity(target.Value, this.EntityManager));
								array2[1] = new ValueTuple<string, object>("bible", uid);
								string selfFailMessage = Loc.GetString(text2, array2);
								this._popupSystem.PopupEntity(selfFailMessage, args.User, args.User, PopupType.MediumCaution);
								string text3 = "/Audio/Effects/hit_kick.ogg";
								target = args.Target;
								SoundSystem.Play(text3, Filter.Pvs(target.Value, 2f, null, null, null), args.User, null);
								DamageableSystem damageableSystem = this._damageableSystem;
								target = args.Target;
								damageableSystem.TryChangeDamage(new EntityUid?(target.Value), component.DamageOnFail, true, true, null, new EntityUid?(uid));
								this._delay.BeginDelay(uid, delay);
								return;
							}
						}
						DamageableSystem damageableSystem2 = this._damageableSystem;
						target = args.Target;
						DamageSpecifier damage = damageableSystem2.TryChangeDamage(new EntityUid?(target.Value), component.Damage, true, true, null, new EntityUid?(uid));
						if (damage == null || damage.Total == 0)
						{
							string text4 = component.LocPrefix + "-heal-success-none-others";
							ValueTuple<string, object>[] array3 = new ValueTuple<string, object>[3];
							array3[0] = new ValueTuple<string, object>("user", Identity.Entity(args.User, this.EntityManager));
							int num3 = 1;
							string item3 = "target";
							target = args.Target;
							array3[num3] = new ValueTuple<string, object>(item3, Identity.Entity(target.Value, this.EntityManager));
							array3[2] = new ValueTuple<string, object>("bible", uid);
							string othersMessage = Loc.GetString(text4, array3);
							this._popupSystem.PopupEntity(othersMessage, args.User, Filter.PvsExcept(args.User, 2f, null), true, PopupType.Medium);
							string text5 = component.LocPrefix + "-heal-success-none-self";
							ValueTuple<string, object>[] array4 = new ValueTuple<string, object>[2];
							int num4 = 0;
							string item4 = "target";
							target = args.Target;
							array4[num4] = new ValueTuple<string, object>(item4, Identity.Entity(target.Value, this.EntityManager));
							array4[1] = new ValueTuple<string, object>("bible", uid);
							string selfMessage = Loc.GetString(text5, array4);
							this._popupSystem.PopupEntity(selfMessage, args.User, args.User, PopupType.Large);
							return;
						}
						string text6 = component.LocPrefix + "-heal-success-others";
						ValueTuple<string, object>[] array5 = new ValueTuple<string, object>[3];
						array5[0] = new ValueTuple<string, object>("user", Identity.Entity(args.User, this.EntityManager));
						int num5 = 1;
						string item5 = "target";
						target = args.Target;
						array5[num5] = new ValueTuple<string, object>(item5, Identity.Entity(target.Value, this.EntityManager));
						array5[2] = new ValueTuple<string, object>("bible", uid);
						string othersMessage2 = Loc.GetString(text6, array5);
						this._popupSystem.PopupEntity(othersMessage2, args.User, Filter.PvsExcept(args.User, 2f, null), true, PopupType.Medium);
						string text7 = component.LocPrefix + "-heal-success-self";
						ValueTuple<string, object>[] array6 = new ValueTuple<string, object>[2];
						int num6 = 0;
						string item6 = "target";
						target = args.Target;
						array6[num6] = new ValueTuple<string, object>(item6, Identity.Entity(target.Value, this.EntityManager));
						array6[1] = new ValueTuple<string, object>("bible", uid);
						string selfMessage2 = Loc.GetString(text7, array6);
						this._popupSystem.PopupEntity(selfMessage2, args.User, args.User, PopupType.Large);
						string sound = component.HealSoundPath.GetSound(null, null);
						target = args.Target;
						SoundSystem.Play(sound, Filter.Pvs(target.Value, 2f, null, null, null), args.User, null);
						this._delay.BeginDelay(uid, delay);
						return;
					}
				}
			}
		}

		// Token: 0x06002656 RID: 9814 RVA: 0x000CACF0 File Offset: 0x000C8EF0
		private void AddSummonVerb(EntityUid uid, SummonableComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess || component.AlreadySummoned || component.SpecialItemPrototype == null)
			{
				return;
			}
			if (component.RequiresBibleUser && !base.HasComp<BibleUserComponent>(args.User))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					TransformComponent userXform;
					if (!this.TryComp<TransformComponent>(args.User, ref userXform))
					{
						return;
					}
					this.AttemptSummon(component, args.User, userXform);
				},
				Text = Loc.GetString("bible-summon-verb"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06002657 RID: 9815 RVA: 0x000CADAF File Offset: 0x000C8FAF
		private void GetSummonAction(EntityUid uid, SummonableComponent component, GetItemActionsEvent args)
		{
			if (component.AlreadySummoned)
			{
				return;
			}
			args.Actions.Add(component.SummonAction);
		}

		// Token: 0x06002658 RID: 9816 RVA: 0x000CADCC File Offset: 0x000C8FCC
		private void OnSummon(EntityUid uid, SummonableComponent component, SummonActionEvent args)
		{
			this.AttemptSummon(component, args.Performer, base.Transform(args.Performer));
		}

		// Token: 0x06002659 RID: 9817 RVA: 0x000CADE8 File Offset: 0x000C8FE8
		private void OnFamiliarDeath(EntityUid uid, FamiliarComponent component, MobStateChangedEvent args)
		{
			if (args.NewMobState != MobState.Dead || component.Source == null)
			{
				return;
			}
			EntityUid? source = component.Source;
			SummonableComponent summonable;
			if (source != null && base.TryComp<SummonableComponent>(source, ref summonable))
			{
				this._addQueue.Enqueue(summonable.Owner);
			}
		}

		// Token: 0x0600265A RID: 9818 RVA: 0x000CAE3C File Offset: 0x000C903C
		private void OnSpawned(EntityUid uid, FamiliarComponent component, GhostRoleSpawnerUsedEvent args)
		{
			SummonableComponent summonable;
			if (!base.TryComp<SummonableComponent>(base.Transform(args.Spawner).ParentUid, ref summonable))
			{
				return;
			}
			component.Source = new EntityUid?(summonable.Owner);
			summonable.Summon = new EntityUid?(uid);
		}

		// Token: 0x0600265B RID: 9819 RVA: 0x000CAE84 File Offset: 0x000C9084
		private void AttemptSummon(SummonableComponent component, EntityUid user, [Nullable(2)] TransformComponent position)
		{
			if (component.AlreadySummoned || component.SpecialItemPrototype == null)
			{
				return;
			}
			if (component.RequiresBibleUser && !base.HasComp<BibleUserComponent>(user))
			{
				return;
			}
			if (!base.Resolve<TransformComponent>(user, ref position, true))
			{
				return;
			}
			if (component.Deleted || base.Deleted(component.Owner, null))
			{
				return;
			}
			if (!this._blocker.CanInteract(user, new EntityUid?(component.Owner)))
			{
				return;
			}
			EntityUid familiar = this.EntityManager.SpawnEntity(component.SpecialItemPrototype, position.Coordinates);
			component.Summon = new EntityUid?(familiar);
			if (base.HasComp<GhostRoleMobSpawnerComponent>(familiar))
			{
				this._popupSystem.PopupEntity(Loc.GetString("bible-summon-requested"), user, PopupType.Medium);
				base.Transform(familiar).AttachParent(component.Owner);
			}
			component.AlreadySummoned = true;
			this._actionsSystem.RemoveAction(user, component.SummonAction, null);
		}

		// Token: 0x040017C9 RID: 6089
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040017CA RID: 6090
		[Dependency]
		private readonly ActionBlockerSystem _blocker;

		// Token: 0x040017CB RID: 6091
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x040017CC RID: 6092
		[Dependency]
		private readonly InventorySystem _invSystem;

		// Token: 0x040017CD RID: 6093
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040017CE RID: 6094
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040017CF RID: 6095
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x040017D0 RID: 6096
		[Dependency]
		private readonly UseDelaySystem _delay;

		// Token: 0x040017D1 RID: 6097
		private readonly Queue<EntityUid> _addQueue = new Queue<EntityUid>();

		// Token: 0x040017D2 RID: 6098
		private readonly Queue<EntityUid> _remQueue = new Queue<EntityUid>();
	}
}
