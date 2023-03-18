using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.Miasma;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Borgs;
using Content.Server.Chat.Managers;
using Content.Server.CombatMode;
using Content.Server.Disease.Components;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Hands.Components;
using Content.Server.Humanoid;
using Content.Server.IdentityManagement;
using Content.Server.Inventory;
using Content.Server.Mind.Commands;
using Content.Server.Mind.Components;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Server.Speech.Components;
using Content.Server.Temperature.Components;
using Content.Server.Traitor;
using Content.Server.Traits.Assorted;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Damage;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.Weapons.Melee;
using Content.Shared.Zombies;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Server.Zombies
{
	// Token: 0x0200001B RID: 27
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ZombifyOnDeathSystem : EntitySystem
	{
		// Token: 0x06000055 RID: 85 RVA: 0x000035B6 File Offset: 0x000017B6
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ZombifyOnDeathComponent, MobStateChangedEvent>(new ComponentEventHandler<ZombifyOnDeathComponent, MobStateChangedEvent>(this.OnDamageChanged), null, null);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000035D2 File Offset: 0x000017D2
		private void OnDamageChanged(EntityUid uid, ZombifyOnDeathComponent component, MobStateChangedEvent args)
		{
			if (args.NewMobState == MobState.Dead || args.NewMobState == MobState.Critical)
			{
				this.ZombifyEntity(uid);
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000035F0 File Offset: 0x000017F0
		public void ZombifyEntity(EntityUid target)
		{
			if (base.HasComp<ZombieComponent>(target))
			{
				return;
			}
			if (base.HasComp<BorgComponent>(target))
			{
				return;
			}
			ZombieComponent zombiecomp = base.AddComp<ZombieComponent>(target);
			base.RemComp<DiseaseCarrierComponent>(target);
			base.RemComp<RespiratorComponent>(target);
			base.RemComp<BarotraumaComponent>(target);
			base.RemComp<HungerComponent>(target);
			base.RemComp<ThirstComponent>(target);
			base.EnsureComp<ReplacementAccentComponent>(target).Accent = "zombie";
			base.EnsureComp<RottingComponent>(target).DealDamage = false;
			base.RemComp<CombatModeComponent>(target);
			base.AddComp<CombatModeComponent>(target).IsInCombatMode = true;
			base.RemComp<PacifistComponent>(target);
			base.RemComp<PacifiedComponent>(target);
			MeleeWeaponComponent melee = base.EnsureComp<MeleeWeaponComponent>(target);
			melee.ClickAnimation = zombiecomp.AttackAnimation;
			melee.WideAnimation = zombiecomp.AttackAnimation;
			melee.Range = 1.5f;
			base.Dirty(melee, null);
			HumanoidAppearanceComponent huApComp;
			if (base.TryComp<HumanoidAppearanceComponent>(target, ref huApComp))
			{
				zombiecomp.BeforeZombifiedSkinColor = huApComp.SkinColor;
				zombiecomp.BeforeZombifiedCustomBaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo>(huApComp.CustomBaseLayers);
				this._sharedHuApp.SetSkinColor(target, zombiecomp.SkinColor, true, huApComp);
				this._sharedHuApp.SetBaseLayerColor(target, HumanoidVisualLayers.Eyes, new Color?(zombiecomp.EyeColor), true, huApComp);
				this._sharedHuApp.SetBaseLayerId(target, HumanoidVisualLayers.Tail, zombiecomp.BaseLayerExternal, true, huApComp);
				this._sharedHuApp.SetBaseLayerId(target, HumanoidVisualLayers.HeadSide, zombiecomp.BaseLayerExternal, true, huApComp);
				this._sharedHuApp.SetBaseLayerId(target, HumanoidVisualLayers.HeadTop, zombiecomp.BaseLayerExternal, true, huApComp);
				this._sharedHuApp.SetBaseLayerId(target, HumanoidVisualLayers.Snout, zombiecomp.BaseLayerExternal, true, huApComp);
				melee.Damage = new DamageSpecifier
				{
					DamageDict = 
					{
						{
							"Slash",
							13
						},
						{
							"Piercing",
							7
						},
						{
							"Structural",
							10
						}
					}
				};
			}
			this._damageable.SetDamageModifierSetId(target, "Zombie", null);
			this._bloodstream.SetBloodLossThreshold(target, 0f, null);
			this._serverInventory.TryUnequip(target, "gloves", true, true, false, null, null);
			this._popupSystem.PopupEntity(Loc.GetString("zombie-transform", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", target)
			}), target, PopupType.LargeCaution);
			if (!base.HasComp<InputMoverComponent>(target))
			{
				MakeSentientCommand.MakeSentient(target, this.EntityManager, true, true);
			}
			TemperatureComponent tempComp;
			if (base.TryComp<TemperatureComponent>(target, ref tempComp))
			{
				tempComp.ColdDamage.ClampMax(0);
			}
			DamageableComponent damageablecomp;
			if (base.TryComp<DamageableComponent>(target, ref damageablecomp))
			{
				this._damageable.SetAllDamage(damageablecomp, 0);
			}
			MetaDataComponent meta = base.MetaData(target);
			zombiecomp.BeforeZombifiedEntityName = meta.EntityName;
			meta.EntityName = Loc.GetString("zombie-name-prefix", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", meta.EntityName)
			});
			this._identity.QueueIdentityUpdate(target);
			MindComponent mindcomp = base.EnsureComp<MindComponent>(target);
			IPlayerSession session;
			if (mindcomp.Mind != null && mindcomp.Mind.TryGetSession(out session))
			{
				mindcomp.Mind.AddRole(new TraitorRole(mindcomp.Mind, this._proto.Index<AntagPrototype>(zombiecomp.ZombieRoleId)));
				this._chatMan.DispatchServerMessage(session, Loc.GetString("zombie-infection-greeting"), false);
			}
			if (!base.HasComp<GhostRoleMobSpawnerComponent>(target) && !mindcomp.HasMind)
			{
				GhostTakeoverAvailableComponent ghostcomp;
				this.EntityManager.EnsureComponent<GhostTakeoverAvailableComponent>(target, ref ghostcomp);
				ghostcomp.RoleName = Loc.GetString("zombie-generic");
				ghostcomp.RoleDescription = Loc.GetString("zombie-role-desc");
				ghostcomp.RoleRules = Loc.GetString("zombie-role-rules");
			}
			foreach (Hand hand in this._sharedHands.EnumerateHands(target, null))
			{
				this._sharedHands.SetActiveHand(target, hand, null);
				this._sharedHands.DoDrop(target, hand, true, null);
				this._sharedHands.RemoveHand(target, hand.Name, null);
			}
			base.RemComp<HandsComponent>(target);
			base.RaiseLocalEvent<EntityZombifiedEvent>(new EntityZombifiedEvent(target));
			this._movementSpeedModifier.RefreshMovementSpeedModifiers(target, null);
		}

		// Token: 0x04000037 RID: 55
		[Dependency]
		private readonly SharedHandsSystem _sharedHands;

		// Token: 0x04000038 RID: 56
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000039 RID: 57
		[Dependency]
		private readonly BloodstreamSystem _bloodstream;

		// Token: 0x0400003A RID: 58
		[Dependency]
		private readonly ServerInventorySystem _serverInventory;

		// Token: 0x0400003B RID: 59
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x0400003C RID: 60
		[Dependency]
		private readonly HumanoidAppearanceSystem _sharedHuApp;

		// Token: 0x0400003D RID: 61
		[Dependency]
		private readonly IdentitySystem _identity;

		// Token: 0x0400003E RID: 62
		[Dependency]
		private readonly MovementSpeedModifierSystem _movementSpeedModifier;

		// Token: 0x0400003F RID: 63
		[Dependency]
		private readonly IChatManager _chatMan;

		// Token: 0x04000040 RID: 64
		[Dependency]
		private readonly IPrototypeManager _proto;
	}
}
