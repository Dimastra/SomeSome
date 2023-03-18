using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Actions;
using Content.Server.Disease;
using Content.Server.Disease.Components;
using Content.Server.DoAfter;
using Content.Server.GameTicking;
using Content.Server.Ghost;
using Content.Server.Light.Components;
using Content.Server.Maps;
using Content.Server.Revenant.Components;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Alert;
using Content.Shared.Bed.Sleep;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Revenant;
using Content.Shared.Revenant.Components;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Revenant.EntitySystems
{
	// Token: 0x02000233 RID: 563
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RevenantSystem : EntitySystem
	{
		// Token: 0x06000B2F RID: 2863 RVA: 0x0003A610 File Offset: 0x00038810
		private void InitializeAbilities()
		{
			base.SubscribeLocalEvent<RevenantComponent, InteractNoHandEvent>(new ComponentEventHandler<RevenantComponent, InteractNoHandEvent>(this.OnInteract), null, null);
			base.SubscribeLocalEvent<RevenantComponent, DoAfterEvent<RevenantSystem.SoulEvent>>(new ComponentEventHandler<RevenantComponent, DoAfterEvent<RevenantSystem.SoulEvent>>(this.OnSoulSearch), null, null);
			base.SubscribeLocalEvent<RevenantComponent, DoAfterEvent<RevenantSystem.HarvestEvent>>(new ComponentEventHandler<RevenantComponent, DoAfterEvent<RevenantSystem.HarvestEvent>>(this.OnHarvest), null, null);
			base.SubscribeLocalEvent<RevenantComponent, RevenantDefileActionEvent>(new ComponentEventHandler<RevenantComponent, RevenantDefileActionEvent>(this.OnDefileAction), null, null);
			base.SubscribeLocalEvent<RevenantComponent, RevenantOverloadLightsActionEvent>(new ComponentEventHandler<RevenantComponent, RevenantOverloadLightsActionEvent>(this.OnOverloadLightsAction), null, null);
			base.SubscribeLocalEvent<RevenantComponent, RevenantBlightActionEvent>(new ComponentEventHandler<RevenantComponent, RevenantBlightActionEvent>(this.OnBlightAction), null, null);
			base.SubscribeLocalEvent<RevenantComponent, RevenantMalfunctionActionEvent>(new ComponentEventHandler<RevenantComponent, RevenantMalfunctionActionEvent>(this.OnMalfunctionAction), null, null);
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x0003A6AC File Offset: 0x000388AC
		private void OnInteract(EntityUid uid, RevenantComponent component, InteractNoHandEvent args)
		{
			EntityUid? target2 = args.Target;
			EntityUid user = args.User;
			if ((target2 != null && (target2 == null || target2.GetValueOrDefault() == user)) || args.Target == null)
			{
				return;
			}
			EntityUid target = args.Target.Value;
			if (base.HasComp<PoweredLightComponent>(target))
			{
				args.Handled = this._ghost.DoGhostBooEvent(target);
				return;
			}
			if (!base.HasComp<MobStateComponent>(target) || !base.HasComp<HumanoidAppearanceComponent>(target) || base.HasComp<RevenantComponent>(target))
			{
				return;
			}
			args.Handled = true;
			EssenceComponent essence;
			if (!base.TryComp<EssenceComponent>(target, ref essence) || !essence.SearchComplete)
			{
				base.EnsureComp<EssenceComponent>(target);
				this.BeginSoulSearchDoAfter(uid, target, component);
				return;
			}
			this.BeginHarvestDoAfter(uid, target, component, essence);
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0003A778 File Offset: 0x00038978
		private void BeginSoulSearchDoAfter(EntityUid uid, EntityUid target, RevenantComponent revenant)
		{
			this._popup.PopupEntity(Loc.GetString("revenant-soul-searching", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", target)
			}), uid, uid, PopupType.Medium);
			RevenantSystem.SoulEvent soulSearchEvent = new RevenantSystem.SoulEvent();
			float soulSearchDuration = revenant.SoulSearchDuration;
			EntityUid? target2 = new EntityUid?(target);
			DoAfterEventArgs searchDoAfter = new DoAfterEventArgs(uid, soulSearchDuration, default(CancellationToken), target2, null)
			{
				BreakOnUserMove = true,
				DistanceThreshold = new float?((float)2)
			};
			this._doAfter.DoAfter<RevenantSystem.SoulEvent>(searchDoAfter, soulSearchEvent);
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0003A80C File Offset: 0x00038A0C
		private void OnSoulSearch(EntityUid uid, RevenantComponent component, DoAfterEvent<RevenantSystem.SoulEvent> args)
		{
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			EssenceComponent essence;
			if (!base.TryComp<EssenceComponent>(args.Args.Target, ref essence))
			{
				return;
			}
			essence.SearchComplete = true;
			float essenceAmount = essence.EssenceAmount;
			string message;
			if (essenceAmount > 45f)
			{
				if (essenceAmount < 90f)
				{
					message = "revenant-soul-yield-average";
				}
				else
				{
					message = "revenant-soul-yield-high";
				}
			}
			else
			{
				message = "revenant-soul-yield-low";
			}
			this._popup.PopupEntity(Loc.GetString(message, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", args.Args.Target)
			}), args.Args.Target.Value, uid, PopupType.Medium);
			args.Handled = true;
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x0003A8C8 File Offset: 0x00038AC8
		private void BeginHarvestDoAfter(EntityUid uid, EntityUid target, RevenantComponent revenant, EssenceComponent essence)
		{
			if (essence.Harvested)
			{
				this._popup.PopupEntity(Loc.GetString("revenant-soul-harvested"), target, uid, PopupType.SmallCaution);
				return;
			}
			MobStateComponent mobstate;
			if (base.TryComp<MobStateComponent>(target, ref mobstate) && mobstate.CurrentState == MobState.Alive && !base.HasComp<SleepingComponent>(target))
			{
				this._popup.PopupEntity(Loc.GetString("revenant-soul-too-powerful"), target, uid, PopupType.Small);
				return;
			}
			RevenantSystem.HarvestEvent harvestEvent = new RevenantSystem.HarvestEvent();
			float x = revenant.HarvestDebuffs.X;
			EntityUid? target2 = new EntityUid?(target);
			DoAfterEventArgs doAfter = new DoAfterEventArgs(uid, x, default(CancellationToken), target2, null)
			{
				DistanceThreshold = new float?((float)2),
				BreakOnUserMove = true,
				NeedHand = false
			};
			this._appearance.SetData(uid, RevenantVisuals.Harvesting, true, null);
			this._popup.PopupEntity(Loc.GetString("revenant-soul-begin-harvest", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", target)
			}), target, PopupType.Large);
			this.TryUseAbility(uid, revenant, 0, revenant.HarvestDebuffs);
			this._doAfter.DoAfter<RevenantSystem.HarvestEvent>(doAfter, harvestEvent);
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x0003A9EC File Offset: 0x00038BEC
		private void OnHarvest(EntityUid uid, RevenantComponent component, DoAfterEvent<RevenantSystem.HarvestEvent> args)
		{
			if (args.Cancelled)
			{
				this._appearance.SetData(uid, RevenantVisuals.Harvesting, false, null);
				return;
			}
			if (args.Handled || args.Args.Target == null)
			{
				return;
			}
			this._appearance.SetData(uid, RevenantVisuals.Harvesting, false, null);
			EssenceComponent essence;
			if (!base.TryComp<EssenceComponent>(args.Args.Target, ref essence))
			{
				return;
			}
			this._popup.PopupEntity(Loc.GetString("revenant-soul-finish-harvest", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", args.Args.Target)
			}), args.Args.Target.Value, PopupType.LargeCaution);
			essence.Harvested = true;
			this.ChangeEssenceAmount(uid, essence.EssenceAmount, component, true, false);
			this._store.TryAddCurrency(new Dictionary<string, FixedPoint2>
			{
				{
					component.StolenEssenceCurrencyPrototype,
					essence.EssenceAmount
				}
			}, uid, null);
			if (!base.HasComp<MobStateComponent>(args.Args.Target))
			{
				return;
			}
			if (this._mobState.IsAlive(args.Args.Target.Value, null) || this._mobState.IsCritical(args.Args.Target.Value, null))
			{
				this._popup.PopupEntity(Loc.GetString("revenant-max-essence-increased"), uid, uid, PopupType.Small);
				component.EssenceRegenCap += component.MaxEssenceUpgradeAmount;
			}
			FixedPoint2? damage;
			if (!this._mobThresholdSystem.TryGetThresholdForState(args.Args.Target.Value, MobState.Dead, out damage, null))
			{
				return;
			}
			DamageSpecifier dspec = new DamageSpecifier();
			dspec.DamageDict.Add("Poison", damage.Value);
			this._damage.TryChangeDamage(args.Args.Target, dspec, true, true, null, new EntityUid?(uid));
			args.Handled = true;
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x0003ABE4 File Offset: 0x00038DE4
		private void OnDefileAction(EntityUid uid, RevenantComponent component, RevenantDefileActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!this.TryUseAbility(uid, component, component.DefileCost, component.DefileDebuffs))
			{
				return;
			}
			args.Handled = true;
			TransformComponent xform = base.Transform(uid);
			MapGridComponent map;
			if (!this._mapManager.TryGetGrid(xform.GridUid, ref map))
			{
				return;
			}
			TileRef[] tiles = map.GetTilesIntersecting(Box2.CenteredAround(xform.WorldPosition, new ValueTuple<float, float>(component.DefileRadius * 2f, component.DefileRadius)), true, null).ToArray<TileRef>();
			this._random.Shuffle<TileRef>(tiles);
			for (int i = 0; i < component.DefileTilePryAmount; i++)
			{
				TileRef value;
				if (Extensions.TryGetValue<TileRef>(tiles, i, ref value))
				{
					this._tile.PryTile(value);
				}
			}
			HashSet<EntityUid> entitiesInRange = this._lookup.GetEntitiesInRange(uid, component.DefileRadius, 5);
			EntityQuery<TagComponent> tags = base.GetEntityQuery<TagComponent>();
			EntityQuery<EntityStorageComponent> entityStorage = base.GetEntityQuery<EntityStorageComponent>();
			EntityQuery<ItemComponent> items = base.GetEntityQuery<ItemComponent>();
			EntityQuery<PoweredLightComponent> lights = base.GetEntityQuery<PoweredLightComponent>();
			foreach (EntityUid ent in entitiesInRange)
			{
				if (tags.HasComponent(ent) && this._tag.HasAnyTag(ent, new string[]
				{
					"Window"
				}))
				{
					DamageSpecifier dspec = new DamageSpecifier();
					dspec.DamageDict.Add("Structural", 15);
					this._damage.TryChangeDamage(new EntityUid?(ent), dspec, false, true, null, new EntityUid?(uid));
				}
				if (RandomExtensions.Prob(this._random, component.DefileEffectChance))
				{
					EntityStorageComponent entstorecomp;
					if (entityStorage.TryGetComponent(ent, ref entstorecomp))
					{
						this._entityStorage.OpenStorage(ent, entstorecomp);
					}
					PhysicsComponent phys;
					if (items.HasComponent(ent) && base.TryComp<PhysicsComponent>(ent, ref phys) && phys.BodyType != 4)
					{
						this._throwing.TryThrow(ent, this._random.NextAngle().ToWorldVec(), 1f, null, 5f, null, null, null, null);
					}
					if (lights.HasComponent(ent))
					{
						this._ghost.DoGhostBooEvent(ent);
					}
				}
			}
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0003AE44 File Offset: 0x00039044
		private void OnOverloadLightsAction(EntityUid uid, RevenantComponent component, RevenantOverloadLightsActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!this.TryUseAbility(uid, component, component.OverloadCost, component.OverloadDebuffs))
			{
				return;
			}
			args.Handled = true;
			TransformComponent xform = base.Transform(uid);
			EntityQuery<PoweredLightComponent> poweredLights = base.GetEntityQuery<PoweredLightComponent>();
			EntityQuery<MobStateComponent> mobState = base.GetEntityQuery<MobStateComponent>();
			Func<EntityUid, bool> <>9__0;
			Func<EntityUid, float> <>9__1;
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, component.OverloadRadius, 46))
			{
				if (mobState.HasComponent(ent) && this._mobState.IsAlive(ent, null))
				{
					IEnumerable<EntityUid> entitiesInRange = this._lookup.GetEntitiesInRange(ent, component.OverloadZapRadius, 46);
					Func<EntityUid, bool> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((EntityUid e) => poweredLights.HasComponent(e) && !this.HasComp<RevenantOverloadedLightsComponent>(e) && this._interact.InRangeUnobstructed(e, uid, -1f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false)));
					}
					EntityUid[] nearbyLights = entitiesInRange.Where(predicate).ToArray<EntityUid>();
					if (nearbyLights.Any<EntityUid>())
					{
						IEnumerable<EntityUid> source = nearbyLights;
						Func<EntityUid, float> keySelector;
						if ((keySelector = <>9__1) == null)
						{
							keySelector = (<>9__1 = delegate(EntityUid e)
							{
								float dist;
								if (!this.Transform(e).Coordinates.TryDistance(this.EntityManager, xform.Coordinates, ref dist))
								{
									return dist;
								}
								return component.OverloadZapRadius;
							});
						}
						IOrderedEnumerable<EntityUid> allLight = source.OrderBy(keySelector);
						base.EnsureComp<RevenantOverloadedLightsComponent>(allLight.First<EntityUid>()).Target = new EntityUid?(ent);
					}
				}
			}
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0003AFDC File Offset: 0x000391DC
		private void OnBlightAction(EntityUid uid, RevenantComponent component, RevenantBlightActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!this.TryUseAbility(uid, component, component.BlightCost, component.BlightDebuffs))
			{
				return;
			}
			args.Handled = true;
			EntityQuery<DiseaseCarrierComponent> emo = base.GetEntityQuery<DiseaseCarrierComponent>();
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, component.BlightRadius, 46))
			{
				DiseaseCarrierComponent comp;
				if (emo.TryGetComponent(ent, ref comp))
				{
					this._disease.TryAddDisease(ent, component.BlightDiseasePrototypeId, comp);
				}
			}
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x0003B088 File Offset: 0x00039288
		private void OnMalfunctionAction(EntityUid uid, RevenantComponent component, RevenantMalfunctionActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!this.TryUseAbility(uid, component, component.MalfunctionCost, component.MalfunctionDebuffs))
			{
				return;
			}
			args.Handled = true;
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, component.MalfunctionRadius, 46))
			{
				this._emag.DoEmagEffect(ent, ent);
			}
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x0003B118 File Offset: 0x00039318
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RevenantComponent, ComponentStartup>(new ComponentEventHandler<RevenantComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<RevenantComponent, RevenantShopActionEvent>(new ComponentEventHandler<RevenantComponent, RevenantShopActionEvent>(this.OnShop), null, null);
			base.SubscribeLocalEvent<RevenantComponent, DamageChangedEvent>(new ComponentEventHandler<RevenantComponent, DamageChangedEvent>(this.OnDamage), null, null);
			base.SubscribeLocalEvent<RevenantComponent, ExaminedEvent>(new ComponentEventHandler<RevenantComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<RevenantComponent, StatusEffectAddedEvent>(new ComponentEventHandler<RevenantComponent, StatusEffectAddedEvent>(this.OnStatusAdded), null, null);
			base.SubscribeLocalEvent<RevenantComponent, StatusEffectEndedEvent>(new ComponentEventHandler<RevenantComponent, StatusEffectEndedEvent>(this.OnStatusEnded), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(delegate(RoundEndTextAppendEvent _)
			{
				this.MakeVisible(true);
			}, null, null);
			this.InitializeAbilities();
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x0003B1C0 File Offset: 0x000393C0
		private void OnStartup(EntityUid uid, RevenantComponent component, ComponentStartup args)
		{
			this.ChangeEssenceAmount(uid, 0, component, true, false);
			this._appearance.SetData(uid, RevenantVisuals.Corporeal, false, null);
			this._appearance.SetData(uid, RevenantVisuals.Harvesting, false, null);
			this._appearance.SetData(uid, RevenantVisuals.Stunned, false, null);
			VisibilityComponent visibility;
			if (this._ticker.RunLevel == GameRunLevel.PostRound && base.TryComp<VisibilityComponent>(uid, ref visibility))
			{
				this._visibility.AddLayer(visibility, 2, false);
				this._visibility.RemoveLayer(visibility, 1, false);
				this._visibility.RefreshVisibility(visibility);
			}
			EyeComponent eye;
			if (base.TryComp<EyeComponent>(uid, ref eye))
			{
				eye.VisibilityMask |= 2U;
			}
			InstantAction shopaction = new InstantAction(this._proto.Index<InstantActionPrototype>("RevenantShop"));
			this._action.AddAction(uid, shopaction, null, null, true);
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0003B2B1 File Offset: 0x000394B1
		private void OnStatusAdded(EntityUid uid, RevenantComponent component, StatusEffectAddedEvent args)
		{
			if (args.Key == "Stun")
			{
				this._appearance.SetData(uid, RevenantVisuals.Stunned, true, null);
			}
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x0003B2DE File Offset: 0x000394DE
		private void OnStatusEnded(EntityUid uid, RevenantComponent component, StatusEffectEndedEvent args)
		{
			if (args.Key == "Stun")
			{
				this._appearance.SetData(uid, RevenantVisuals.Stunned, false, null);
			}
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x0003B30C File Offset: 0x0003950C
		private void OnExamine(EntityUid uid, RevenantComponent component, ExaminedEvent args)
		{
			if (args.Examiner == args.Examined)
			{
				args.PushMarkup(Loc.GetString("revenant-essence-amount", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("current", component.Essence.Int()),
					new ValueTuple<string, object>("max", component.EssenceRegenCap.Int())
				}));
			}
		}

		// Token: 0x06000B3E RID: 2878 RVA: 0x0003B384 File Offset: 0x00039584
		private void OnDamage(EntityUid uid, RevenantComponent component, DamageChangedEvent args)
		{
			if (!base.HasComp<CorporealComponent>(uid) || args.DamageDelta == null)
			{
				return;
			}
			float essenceDamage = args.DamageDelta.Total.Float() * component.DamageToEssenceCoefficient * -1f;
			this.ChangeEssenceAmount(uid, essenceDamage, component, true, false);
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x0003B3D8 File Offset: 0x000395D8
		[NullableContext(2)]
		public bool ChangeEssenceAmount(EntityUid uid, FixedPoint2 amount, RevenantComponent component = null, bool allowDeath = true, bool regenCap = false)
		{
			if (!base.Resolve<RevenantComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!allowDeath && component.Essence + amount <= 0)
			{
				return false;
			}
			component.Essence += amount;
			if (regenCap)
			{
				FixedPoint2.Min(component.Essence, component.EssenceRegenCap);
			}
			StoreComponent store;
			if (base.TryComp<StoreComponent>(uid, ref store))
			{
				this._store.UpdateUserInterface(new EntityUid?(uid), uid, store, null);
			}
			this._alerts.ShowAlert(uid, AlertType.Essence, new short?((short)Math.Clamp(Math.Round((double)(component.Essence.Float() / 10f)), 0.0, 16.0)), null);
			if (component.Essence <= 0)
			{
				base.QueueDel(uid);
			}
			return true;
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x0003B4B8 File Offset: 0x000396B8
		private bool TryUseAbility(EntityUid uid, RevenantComponent component, FixedPoint2 abilityCost, Vector2 debuffs)
		{
			if (component.Essence <= abilityCost)
			{
				this._popup.PopupEntity(Loc.GetString("revenant-not-enough-essence"), uid, uid, PopupType.Small);
				return false;
			}
			if (base.Transform(uid).Coordinates.GetTileRef(null, null) != null && this._physics.GetEntitiesIntersectingBody(uid, 2, true, null, null, null).Count > 0)
			{
				this._popup.PopupEntity(Loc.GetString("revenant-in-solid"), uid, uid, PopupType.Small);
				return false;
			}
			this.ChangeEssenceAmount(uid, abilityCost, component, false, false);
			this._statusEffects.TryAddStatusEffect<CorporealComponent>(uid, "Corporeal", TimeSpan.FromSeconds((double)debuffs.Y), false, null);
			this._stun.TryStun(uid, TimeSpan.FromSeconds((double)debuffs.X), false, null);
			return true;
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x0003B588 File Offset: 0x00039788
		private void OnShop(EntityUid uid, RevenantComponent component, RevenantShopActionEvent args)
		{
			StoreComponent store;
			if (!base.TryComp<StoreComponent>(uid, ref store))
			{
				return;
			}
			this._store.ToggleUi(uid, uid, store);
		}

		// Token: 0x06000B42 RID: 2882 RVA: 0x0003B5B0 File Offset: 0x000397B0
		public void MakeVisible(bool visible)
		{
			foreach (ValueTuple<RevenantComponent, VisibilityComponent> valueTuple in base.EntityQuery<RevenantComponent, VisibilityComponent>(false))
			{
				VisibilityComponent vis = valueTuple.Item2;
				if (visible)
				{
					this._visibility.AddLayer(vis, 1, false);
					this._visibility.RemoveLayer(vis, 2, false);
				}
				else
				{
					this._visibility.AddLayer(vis, 2, false);
					this._visibility.RemoveLayer(vis, 1, false);
				}
				this._visibility.RefreshVisibility(vis);
			}
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x0003B648 File Offset: 0x00039848
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (RevenantComponent rev in base.EntityQuery<RevenantComponent>(false))
			{
				rev.Accumulator += frameTime;
				if (rev.Accumulator > 1f)
				{
					rev.Accumulator -= 1f;
					if (rev.Essence < rev.EssenceRegenCap)
					{
						this.ChangeEssenceAmount(rev.Owner, rev.EssencePerSecond, rev, true, true);
					}
				}
			}
		}

		// Token: 0x040006D3 RID: 1747
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040006D4 RID: 1748
		[Dependency]
		private readonly ThrowingSystem _throwing;

		// Token: 0x040006D5 RID: 1749
		[Dependency]
		private readonly EntityStorageSystem _entityStorage;

		// Token: 0x040006D6 RID: 1750
		[Dependency]
		private readonly DiseaseSystem _disease;

		// Token: 0x040006D7 RID: 1751
		[Dependency]
		private readonly EmagSystem _emag;

		// Token: 0x040006D8 RID: 1752
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040006D9 RID: 1753
		[Dependency]
		private readonly MobThresholdSystem _mobThresholdSystem;

		// Token: 0x040006DA RID: 1754
		[Dependency]
		private readonly GhostSystem _ghost;

		// Token: 0x040006DB RID: 1755
		[Dependency]
		private readonly TileSystem _tile;

		// Token: 0x040006DC RID: 1756
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040006DD RID: 1757
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x040006DE RID: 1758
		[Dependency]
		private readonly ActionsSystem _action;

		// Token: 0x040006DF RID: 1759
		[Dependency]
		private readonly AlertsSystem _alerts;

		// Token: 0x040006E0 RID: 1760
		[Dependency]
		private readonly DamageableSystem _damage;

		// Token: 0x040006E1 RID: 1761
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x040006E2 RID: 1762
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040006E3 RID: 1763
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x040006E4 RID: 1764
		[Dependency]
		private readonly PhysicsSystem _physics;

		// Token: 0x040006E5 RID: 1765
		[Dependency]
		private readonly StatusEffectsSystem _statusEffects;

		// Token: 0x040006E6 RID: 1766
		[Dependency]
		private readonly SharedInteractionSystem _interact;

		// Token: 0x040006E7 RID: 1767
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x040006E8 RID: 1768
		[Dependency]
		private readonly SharedStunSystem _stun;

		// Token: 0x040006E9 RID: 1769
		[Dependency]
		private readonly TagSystem _tag;

		// Token: 0x040006EA RID: 1770
		[Dependency]
		private readonly StoreSystem _store;

		// Token: 0x040006EB RID: 1771
		[Dependency]
		private readonly VisibilitySystem _visibility;

		// Token: 0x040006EC RID: 1772
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x02000918 RID: 2328
		[NullableContext(0)]
		private sealed class SoulEvent : EntityEventArgs
		{
		}

		// Token: 0x02000919 RID: 2329
		[NullableContext(0)]
		private sealed class HarvestEvent : EntityEventArgs
		{
		}
	}
}
