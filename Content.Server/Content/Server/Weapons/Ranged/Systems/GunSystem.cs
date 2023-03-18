using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Cargo.Systems;
using Content.Server.Examine;
using Content.Server.Interaction;
using Content.Server.Power.Components;
using Content.Server.Stunnable;
using Content.Server.Weapons.Ranged.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Weapons.Ranged.Systems
{
	// Token: 0x020000B0 RID: 176
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GunSystem : SharedGunSystem
	{
		// Token: 0x060002B9 RID: 697 RVA: 0x0000E1A8 File Offset: 0x0000C3A8
		protected override void Cycle(BallisticAmmoProviderComponent component, MapCoordinates coordinates)
		{
			EntityUid? ent = null;
			if (component.Entities.Count > 0)
			{
				List<EntityUid> entities = component.Entities;
				EntityUid existing = entities[entities.Count - 1];
				component.Entities.RemoveAt(component.Entities.Count - 1);
				component.Container.Remove(existing, null, null, null, true, false, null, null);
				base.EnsureComp<AmmoComponent>(existing);
			}
			else if (component.UnspawnedCount > 0)
			{
				component.UnspawnedCount--;
				ent = new EntityUid?(base.Spawn(component.FillProto, coordinates));
				base.EnsureComp<AmmoComponent>(ent.Value);
			}
			if (ent != null)
			{
				base.EjectCartridge(ent.Value, true);
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000E278 File Offset: 0x0000C478
		protected override void InitializeBattery()
		{
			base.InitializeBattery();
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ComponentStartup>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, ComponentStartup>(this.OnBatteryStartup), null, null);
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ChargeChangedEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, ChargeChangedEvent>(this.OnBatteryChargeChange), null, null);
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, GetVerbsEvent<ExamineVerb>>(this.OnBatteryExaminableVerb), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ComponentStartup>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, ComponentStartup>(this.OnBatteryStartup), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ChargeChangedEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, ChargeChangedEvent>(this.OnBatteryChargeChange), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, GetVerbsEvent<ExamineVerb>>(this.OnBatteryExaminableVerb), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, ComponentStartup>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, ComponentStartup>(this.OnBatteryStartup), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, ChargeChangedEvent>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, ChargeChangedEvent>(this.OnBatteryChargeChange), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, GetVerbsEvent<ExamineVerb>>(this.OnBatteryExaminableVerb), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, UseInHandEvent>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, UseInHandEvent>(this.OnBatteryModeChange), null, null);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000E354 File Offset: 0x0000C554
		private void OnBatteryModeChange(EntityUid uid, TwoModeEnergyAmmoProviderComponent component, UseInHandEvent args)
		{
			GunComponent gun;
			if (!base.TryComp<GunComponent>(uid, ref gun))
			{
				return;
			}
			if (component.CurrentMode == EnergyModes.Stun)
			{
				component.InStun = false;
				gun.SoundGunshot = component.HitscanSound;
				component.CurrentMode = EnergyModes.Laser;
				component.FireCost = component.HitscanFireCost;
				this._audio.PlayPvs(component.ToggleSound, args.User, null);
			}
			else if (component.CurrentMode == EnergyModes.Laser)
			{
				component.InStun = true;
				gun.SoundGunshot = component.ProjSound;
				component.CurrentMode = EnergyModes.Stun;
				component.FireCost = component.ProjFireCost;
				this._audio.PlayPvs(component.ToggleSound, args.User, null);
			}
			this.UpdateShots(uid, component);
			base.UpdateTwoModeAppearance(uid, component);
			base.UpdateBatteryAppearance(uid, component);
			this.UpdateAmmoCount(uid);
			base.Dirty(gun, null);
			base.Dirty(component, null);
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000E43D File Offset: 0x0000C63D
		private void OnBatteryStartup(EntityUid uid, BatteryAmmoProviderComponent component, ComponentStartup args)
		{
			this.UpdateShots(uid, component);
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000E447 File Offset: 0x0000C647
		private void OnBatteryChargeChange(EntityUid uid, BatteryAmmoProviderComponent component, ChargeChangedEvent args)
		{
			this.UpdateShots(uid, component);
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000E454 File Offset: 0x0000C654
		private void UpdateShots(EntityUid uid, BatteryAmmoProviderComponent component)
		{
			BatteryComponent battery;
			if (!base.TryComp<BatteryComponent>(uid, ref battery))
			{
				return;
			}
			this.UpdateShots(uid, component, battery);
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000E478 File Offset: 0x0000C678
		private void UpdateShots(EntityUid uid, BatteryAmmoProviderComponent component, BatteryComponent battery)
		{
			int shots = (int)(battery.CurrentCharge / component.FireCost);
			int maxShots = (int)(battery.MaxCharge / component.FireCost);
			if (component.Shots != shots || component.Capacity != maxShots)
			{
				base.Dirty(component, null);
			}
			component.Shots = shots;
			component.Capacity = maxShots;
			base.UpdateBatteryAppearance(uid, component);
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000E4D4 File Offset: 0x0000C6D4
		private void OnBatteryExaminableVerb(EntityUid uid, BatteryAmmoProviderComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			DamageSpecifier damageSpec = this.GetDamage(component);
			if (damageSpec == null)
			{
				return;
			}
			string damageType;
			if (!(component is HitscanBatteryAmmoProviderComponent))
			{
				if (!(component is ProjectileBatteryAmmoProviderComponent))
				{
					TwoModeEnergyAmmoProviderComponent twoMode = component as TwoModeEnergyAmmoProviderComponent;
					if (twoMode == null)
					{
						throw new ArgumentOutOfRangeException();
					}
					if (twoMode.CurrentMode == EnergyModes.Stun)
					{
						damageType = Loc.GetString("damage-projectile");
					}
					else
					{
						damageType = Loc.GetString("damage-hitscan");
					}
				}
				else
				{
					damageType = Loc.GetString("damage-projectile");
				}
			}
			else
			{
				damageType = Loc.GetString("damage-hitscan");
			}
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate()
				{
					FormattedMessage markup = this.Damageable.GetDamageExamine(damageSpec, damageType);
					this.Examine.SendExamineTooltip(args.User, uid, markup, false, false);
				},
				Text = Loc.GetString("damage-examinable-verb-text"),
				Message = Loc.GetString("damage-examinable-verb-message"),
				Category = VerbCategory.Examine,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/smite.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0000E610 File Offset: 0x0000C810
		[return: Nullable(2)]
		private DamageSpecifier GetDamage(BatteryAmmoProviderComponent component)
		{
			ProjectileBatteryAmmoProviderComponent battery = component as ProjectileBatteryAmmoProviderComponent;
			if (battery != null)
			{
				EntityPrototype.ComponentRegistryEntry projectile;
				if (this.ProtoManager.Index<EntityPrototype>(battery.Prototype).Components.TryGetValue(this._factory.GetComponentName(typeof(ProjectileComponent)), out projectile))
				{
					ProjectileComponent p = (ProjectileComponent)projectile.Component;
					if (p.Damage.Total > FixedPoint2.Zero)
					{
						return p.Damage;
					}
				}
				return null;
			}
			HitscanBatteryAmmoProviderComponent hitscan = component as HitscanBatteryAmmoProviderComponent;
			if (hitscan != null)
			{
				return this.ProtoManager.Index<HitscanPrototype>(hitscan.Prototype).Damage;
			}
			TwoModeEnergyAmmoProviderComponent twoMode = component as TwoModeEnergyAmmoProviderComponent;
			if (twoMode == null)
			{
				return null;
			}
			if (twoMode.CurrentMode == EnergyModes.Stun)
			{
				EntityPrototype.ComponentRegistryEntry projectile2;
				if (this.ProtoManager.Index<EntityPrototype>(twoMode.ProjectilePrototype).Components.TryGetValue(this._factory.GetComponentName(typeof(ProjectileComponent)), out projectile2))
				{
					ProjectileComponent p2 = (ProjectileComponent)projectile2.Component;
					if (p2.Damage.Total > FixedPoint2.Zero)
					{
						return p2.Damage;
					}
				}
				return null;
			}
			return this.ProtoManager.Index<HitscanPrototype>(twoMode.HitscanPrototype).Damage;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000E73C File Offset: 0x0000C93C
		protected override void TakeCharge(EntityUid uid, BatteryAmmoProviderComponent component)
		{
			BatteryComponent battery;
			if (!base.TryComp<BatteryComponent>(uid, ref battery))
			{
				return;
			}
			battery.CurrentCharge -= component.FireCost;
			this.UpdateShots(uid, component, battery);
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000E771 File Offset: 0x0000C971
		protected override void InitializeCartridge()
		{
			base.InitializeCartridge();
			base.SubscribeLocalEvent<CartridgeAmmoComponent, ExaminedEvent>(new ComponentEventHandler<CartridgeAmmoComponent, ExaminedEvent>(this.OnCartridgeExamine), null, null);
			base.SubscribeLocalEvent<CartridgeAmmoComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<CartridgeAmmoComponent, GetVerbsEvent<ExamineVerb>>(this.OnCartridgeVerbExamine), null, null);
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000E7A4 File Offset: 0x0000C9A4
		private void OnCartridgeVerbExamine(EntityUid uid, CartridgeAmmoComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			DamageSpecifier damageSpec = this.GetProjectileDamage(component.Prototype);
			if (damageSpec == null)
			{
				return;
			}
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate()
				{
					FormattedMessage markup = this.Damageable.GetDamageExamine(damageSpec, Loc.GetString("damage-projectile"));
					this._examine.SendExamineTooltip(args.User, uid, markup, false, false);
				},
				Text = Loc.GetString("damage-examinable-verb-text"),
				Message = Loc.GetString("damage-examinable-verb-message"),
				Category = VerbCategory.Examine,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/smite.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000E874 File Offset: 0x0000CA74
		[return: Nullable(2)]
		private DamageSpecifier GetProjectileDamage(string proto)
		{
			EntityPrototype entityProto;
			if (!this.ProtoManager.TryIndex<EntityPrototype>(proto, ref entityProto))
			{
				return null;
			}
			EntityPrototype.ComponentRegistryEntry projectile;
			if (entityProto.Components.TryGetValue(this._factory.GetComponentName(typeof(ProjectileComponent)), out projectile))
			{
				ProjectileComponent p = (ProjectileComponent)projectile.Component;
				if (p.Damage.Total > FixedPoint2.Zero)
				{
					return p.Damage;
				}
			}
			return null;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000E8E2 File Offset: 0x0000CAE2
		private void OnCartridgeExamine(EntityUid uid, CartridgeAmmoComponent component, ExaminedEvent args)
		{
			if (component.Spent)
			{
				args.PushMarkup(Loc.GetString("gun-cartridge-spent"));
				return;
			}
			args.PushMarkup(Loc.GetString("gun-cartridge-unspent"));
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000E90D File Offset: 0x0000CB0D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, PriceCalculationEvent>(new ComponentEventRefHandler<BallisticAmmoProviderComponent, PriceCalculationEvent>(this.OnBallisticPrice), null, null);
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000E92C File Offset: 0x0000CB2C
		private void OnBallisticPrice(EntityUid uid, BallisticAmmoProviderComponent component, ref PriceCalculationEvent args)
		{
			if (string.IsNullOrEmpty(component.FillProto) || component.UnspawnedCount == 0)
			{
				return;
			}
			EntityPrototype proto;
			if (!this.ProtoManager.TryIndex<EntityPrototype>(component.FillProto, ref proto))
			{
				ISawmill sawmill = this.Sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Unable to find fill prototype for price on ");
				defaultInterpolatedStringHandler.AppendFormatted(component.FillProto);
				defaultInterpolatedStringHandler.AppendLiteral(" on ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			double price = this._pricing.GetEstimatedPrice(proto, null);
			args.Price += price * (double)component.UnspawnedCount;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000E9D8 File Offset: 0x0000CBD8
		public override void Shoot(GunComponent gun, List<IShootable> ammo, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, EntityUid? user = null)
		{
			ClumsyComponent clumsy;
			if (base.TryComp<ClumsyComponent>(user, ref clumsy))
			{
				for (int i = 0; i < ammo.Count; i++)
				{
					if (this._interaction.TryRollClumsy(user.Value, 0.5f, clumsy))
					{
						this.Damageable.TryChangeDamage(user, clumsy.ClumsyDamage, false, true, null, user);
						this._stun.TryParalyze(user.Value, TimeSpan.FromSeconds(3.0), true, null);
						this.Audio.PlayPvs(new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/bang.ogg", null), gun.Owner, null);
						this.Audio.PlayPvs(new SoundPathSpecifier("/Audio/Items/bikehorn.ogg", null), gun.Owner, null);
						this.PopupSystem.PopupEntity(Loc.GetString("gun-clumsy"), user.Value, PopupType.Small);
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.EntityDelete;
						LogImpact impact = LogImpact.Medium;
						LogStringHandler logStringHandler = new LogStringHandler(24, 2);
						logStringHandler.AppendLiteral("Clumsy fire by ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "ToPrettyString(user.Value)");
						logStringHandler.AppendLiteral(" deleted ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(gun.Owner), "ToPrettyString(gun.Owner)");
						adminLogger.Add(type, impact, ref logStringHandler);
						base.Del(gun.Owner);
						return;
					}
				}
			}
			MapCoordinates fromMap = fromCoordinates.ToMap(this.EntityManager);
			Vector2 mapDirection = toCoordinates.ToMapPos(this.EntityManager) - fromMap.Position;
			Angle mapAngle = DirectionExtensions.ToAngle(mapDirection);
			Angle angle = this.GetRecoilAngle(this.Timing.CurTime, gun, DirectionExtensions.ToAngle(mapDirection));
			MapGridComponent grid;
			EntityCoordinates fromEnt = this.MapManager.TryFindGridAt(fromMap, ref grid) ? fromCoordinates.WithEntityId(grid.Owner, this.EntityManager) : new EntityCoordinates(this.MapManager.GetMapEntityId(fromMap.MapId), fromMap.Position);
			mapDirection = fromMap.Position + angle.ToVec() * mapDirection.Length - fromMap.Position;
			Vector2 gunVelocity = this.Physics.GetMapLinearVelocity(gun.Owner, null, null, null, null);
			List<EntityUid> shotProjectiles = new List<EntityUid>(ammo.Count);
			foreach (IShootable shootable in ammo)
			{
				CartridgeAmmoComponent cartridge = shootable as CartridgeAmmoComponent;
				if (cartridge == null)
				{
					AmmoComponent newAmmo = shootable as AmmoComponent;
					if (newAmmo == null)
					{
						HitscanPrototype hitscan = shootable as HitscanPrototype;
						if (hitscan == null)
						{
							throw new ArgumentOutOfRangeException();
						}
						CollisionRay ray;
						ray..ctor(fromMap.Position, mapDirection.Normalized, hitscan.CollisionMask);
						List<RayCastResults> rayCastResults = this.Physics.IntersectRay(fromMap.MapId, ray, hitscan.MaxLength, user, false).ToList<RayCastResults>();
						if (rayCastResults.Count >= 1)
						{
							RayCastResults result = rayCastResults[0];
							EntityUid hitEntity = result.HitEntity;
							HitScanShotEvent ev = new HitScanShotEvent(user, hitEntity);
							base.RaiseLocalEvent<HitScanShotEvent>(ref ev);
							hitEntity = ev.Target;
							float distance = result.Distance;
							this.FireEffects(fromCoordinates, distance, DirectionExtensions.ToAngle(mapDirection), hitscan, new EntityUid?(hitEntity));
							if (hitscan.StaminaDamage > 0f)
							{
								this._stamina.TakeStaminaDamage(hitEntity, hitscan.StaminaDamage, null, user, null);
							}
							DamageSpecifier dmg = hitscan.Damage;
							string hitName = base.ToPrettyString(hitEntity);
							if (dmg != null)
							{
								dmg = this.Damageable.TryChangeDamage(new EntityUid?(hitEntity), dmg, false, true, null, user);
							}
							if (dmg != null)
							{
								if (!base.Deleted(hitEntity, null))
								{
									if (dmg.Total > FixedPoint2.Zero)
									{
										base.RaiseNetworkEvent(new DamageEffectEvent(Color.Red, new List<EntityUid>
										{
											hitEntity
										}), Filter.Pvs(hitEntity, 2f, this.EntityManager, null, null), true);
									}
									this.PlayImpactSound(hitEntity, dmg, hitscan.Sound, hitscan.ForceSound);
								}
								if (user != null)
								{
									ISharedAdminLogManager logs = this.Logs;
									LogType type2 = LogType.HitScanHit;
									LogStringHandler logStringHandler = new LogStringHandler(37, 3);
									logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
									logStringHandler.AppendLiteral(" hit ");
									logStringHandler.AppendFormatted(hitName, 0, "target");
									logStringHandler.AppendLiteral(" using hitscan and dealt ");
									logStringHandler.AppendFormatted<FixedPoint2>(dmg.Total, "damage", "dmg.Total");
									logStringHandler.AppendLiteral(" damage");
									logs.Add(type2, ref logStringHandler);
								}
								else
								{
									ISharedAdminLogManager logs2 = this.Logs;
									LogType type3 = LogType.HitScanHit;
									LogStringHandler logStringHandler = new LogStringHandler(36, 2);
									logStringHandler.AppendLiteral("Hit ");
									logStringHandler.AppendFormatted(hitName, 0, "target");
									logStringHandler.AppendLiteral(" using hitscan and dealt ");
									logStringHandler.AppendFormatted<FixedPoint2>(dmg.Total, "damage", "dmg.Total");
									logStringHandler.AppendLiteral(" damage");
									logs2.Add(type3, ref logStringHandler);
								}
							}
						}
						else
						{
							this.FireEffects(fromCoordinates, hitscan.MaxLength, DirectionExtensions.ToAngle(mapDirection), hitscan, null);
						}
						this.Audio.PlayPredicted(gun.SoundGunshot, gun.Owner, user, null);
					}
					else
					{
						shotProjectiles.Add(newAmmo.Owner);
						base.MuzzleFlash(gun.Owner, newAmmo, user);
						this.Audio.PlayPredicted(gun.SoundGunshot, gun.Owner, user, null);
						if (!base.HasComp<ProjectileComponent>(newAmmo.Owner))
						{
							base.RemComp<AmmoComponent>(newAmmo.Owner);
							this.ThrowingSystem.TryThrow(newAmmo.Owner, mapDirection, gun.ProjectileSpeed, user, 5f, null, null, null, null);
						}
						else
						{
							this.ShootProjectile(newAmmo.Owner, mapDirection, gunVelocity, user, gun.ProjectileSpeed);
						}
					}
				}
				else
				{
					if (!cartridge.Spent)
					{
						if (cartridge.Count > 1)
						{
							Angle[] angles = this.LinearSpread(mapAngle - cartridge.Spread / 2.0, mapAngle + cartridge.Spread / 2.0, cartridge.Count);
							for (int j = 0; j < cartridge.Count; j++)
							{
								EntityUid uid = base.Spawn(cartridge.Prototype, fromEnt);
								this.ShootProjectile(uid, angles[j].ToVec(), gunVelocity, user, gun.ProjectileSpeed);
								shotProjectiles.Add(uid);
							}
						}
						else
						{
							EntityUid uid2 = base.Spawn(cartridge.Prototype, fromEnt);
							this.ShootProjectile(uid2, mapDirection, gunVelocity, user, gun.ProjectileSpeed);
							shotProjectiles.Add(uid2);
						}
						base.RaiseLocalEvent<AmmoShotEvent>(cartridge.Owner, new AmmoShotEvent
						{
							FiredProjectiles = shotProjectiles
						}, false);
						base.SetCartridgeSpent(cartridge, true);
						base.MuzzleFlash(gun.Owner, cartridge, user);
						this.Audio.PlayPredicted(gun.SoundGunshot, gun.Owner, user, null);
						if (cartridge.DeleteOnSpawn)
						{
							base.Del(cartridge.Owner);
						}
					}
					else
					{
						this.Audio.PlayPredicted(gun.SoundEmpty, gun.Owner, user, null);
					}
					if (!cartridge.DeleteOnSpawn && !this.Containers.IsEntityInContainer(cartridge.Owner, null))
					{
						base.EjectCartridge(cartridge.Owner, true);
					}
					base.Dirty(cartridge, null);
				}
			}
			base.RaiseLocalEvent<AmmoShotEvent>(gun.Owner, new AmmoShotEvent
			{
				FiredProjectiles = shotProjectiles
			}, false);
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0000F200 File Offset: 0x0000D400
		public void ShootProjectile(EntityUid uid, Vector2 direction, Vector2 gunVelocity, EntityUid? user = null, float speed = 20f)
		{
			PhysicsComponent physics = base.EnsureComp<PhysicsComponent>(uid);
			this.Physics.SetBodyStatus(physics, 1, true);
			Vector2 targetMapVelocity = gunVelocity + direction.Normalized * speed;
			Vector2 currentMapVelocity = this.Physics.GetMapLinearVelocity(uid, physics, null, null, null);
			Vector2 finalLinear = physics.LinearVelocity + targetMapVelocity - currentMapVelocity;
			this.Physics.SetLinearVelocity(uid, finalLinear, true, true, null, physics);
			if (user != null)
			{
				ProjectileComponent projectile = base.EnsureComp<ProjectileComponent>(uid);
				this.Projectiles.SetShooter(projectile, user.Value);
			}
			base.Transform(uid).WorldRotation = DirectionExtensions.ToWorldAngle(direction);
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000F2B8 File Offset: 0x0000D4B8
		private Angle[] LinearSpread(Angle start, Angle end, int intervals)
		{
			Angle[] angles = new Angle[intervals];
			for (int i = 0; i <= intervals - 1; i++)
			{
				angles[i] = new Angle(start + (end - start) * (double)i / (double)(intervals - 1));
			}
			return angles;
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000F30C File Offset: 0x0000D50C
		private Angle GetRecoilAngle(TimeSpan curTime, GunComponent component, Angle direction)
		{
			double timeSinceLastFire = (curTime - component.LastFire).TotalSeconds;
			double newTheta = MathHelper.Clamp(component.CurrentAngle.Theta + component.AngleIncrease.Theta - component.AngleDecay.Theta * timeSinceLastFire, component.MinAngle.Theta, component.MaxAngle.Theta);
			component.CurrentAngle = new Angle(newTheta);
			component.LastFire = component.NextFire;
			float random = this.Random.NextFloat(-0.5f, 0.5f);
			Angle currentAngle = component.CurrentAngle;
			return new Angle(direction.Theta + component.CurrentAngle.Theta * (double)random);
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000F3BE File Offset: 0x0000D5BE
		protected override void Popup(string message, EntityUid? uid, EntityUid? user)
		{
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0000F3C0 File Offset: 0x0000D5C0
		protected override void CreateEffect(EntityUid uid, MuzzleFlashEvent message, EntityUid? user = null)
		{
			Filter filter = Filter.Pvs(uid, 2f, this.EntityManager, null, null);
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(user, ref actor))
			{
				filter.RemovePlayer(actor.PlayerSession);
			}
			base.RaiseNetworkEvent(message, filter, true);
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000F404 File Offset: 0x0000D604
		[NullableContext(2)]
		public void PlayImpactSound(EntityUid otherEntity, DamageSpecifier modifiedDamage, SoundSpecifier weaponSound, bool forceWeaponSound)
		{
			bool playedSound = false;
			RangedDamageSoundComponent rangedSound;
			if (!forceWeaponSound && modifiedDamage != null && modifiedDamage.Total > 0 && base.TryComp<RangedDamageSoundComponent>(otherEntity, ref rangedSound))
			{
				string type = SharedMeleeWeaponSystem.GetHighestDamageSound(modifiedDamage, this.ProtoManager);
				if (type != null)
				{
					Dictionary<string, SoundSpecifier> soundTypes = rangedSound.SoundTypes;
					SoundSpecifier damageSoundType;
					if (soundTypes != null && soundTypes.TryGetValue(type, out damageSoundType))
					{
						this.Audio.PlayPvs(damageSoundType, otherEntity, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
						playedSound = true;
						goto IL_C4;
					}
				}
				if (type != null)
				{
					Dictionary<string, SoundSpecifier> soundGroups = rangedSound.SoundGroups;
					SoundSpecifier damageSoundGroup;
					if (soundGroups != null && soundGroups.TryGetValue(type, out damageSoundGroup))
					{
						this.Audio.PlayPvs(damageSoundGroup, otherEntity, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
						playedSound = true;
					}
				}
			}
			IL_C4:
			if (!playedSound && weaponSound != null)
			{
				this.Audio.PlayPvs(weaponSound, otherEntity, null);
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0000F4F4 File Offset: 0x0000D6F4
		private void FireEffects(EntityCoordinates fromCoordinates, float distance, Angle mapDirection, HitscanPrototype hitscan, EntityUid? hitEntity = null)
		{
			List<ValueTuple<EntityCoordinates, Angle, SpriteSpecifier, float>> sprites = new List<ValueTuple<EntityCoordinates, Angle, SpriteSpecifier, float>>();
			EntityUid? gridUid = fromCoordinates.GetGridUid(this.EntityManager);
			Angle angle = mapDirection;
			TransformComponent gridXform;
			if (base.TryComp<TransformComponent>(gridUid, ref gridXform))
			{
				ValueTuple<Vector2, Angle, Matrix3> worldPositionRotationInvMatrix = gridXform.GetWorldPositionRotationInvMatrix();
				Angle gridRot = worldPositionRotationInvMatrix.Item2;
				Matrix3 gridInvMatrix = worldPositionRotationInvMatrix.Item3;
				fromCoordinates..ctor(gridUid.Value, gridInvMatrix.Transform(fromCoordinates.ToMapPos(this.EntityManager)));
				angle -= gridRot;
			}
			if (distance >= 1f)
			{
				if (hitscan.MuzzleFlash != null)
				{
					sprites.Add(new ValueTuple<EntityCoordinates, Angle, SpriteSpecifier, float>(fromCoordinates.Offset(angle.ToVec().Normalized / 2f), angle, hitscan.MuzzleFlash, 1f));
				}
				if (hitscan.TravelFlash != null)
				{
					sprites.Add(new ValueTuple<EntityCoordinates, Angle, SpriteSpecifier, float>(fromCoordinates.Offset(angle.ToVec() * (distance + 0.5f) / 2f), angle, hitscan.TravelFlash, distance - 1.5f));
				}
			}
			if (hitscan.ImpactFlash != null)
			{
				sprites.Add(new ValueTuple<EntityCoordinates, Angle, SpriteSpecifier, float>(fromCoordinates.Offset(angle.ToVec() * distance), angle.FlipPositive(), hitscan.ImpactFlash, 1f));
			}
			if (sprites.Count > 0)
			{
				base.RaiseNetworkEvent(new SharedGunSystem.HitscanEvent
				{
					Sprites = sprites
				}, Filter.Pvs(fromCoordinates, 2f, this.EntityManager, null), true);
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000F660 File Offset: 0x0000D860
		protected override void SpinRevolver(RevolverAmmoProviderComponent component, EntityUid? user = null)
		{
			base.SpinRevolver(component, user);
			int index = this.Random.Next(component.Capacity);
			if (component.CurrentIndex == index)
			{
				return;
			}
			component.CurrentIndex = index;
			base.Dirty(component, null);
		}

		// Token: 0x040001E1 RID: 481
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040001E2 RID: 482
		[Dependency]
		private readonly IComponentFactory _factory;

		// Token: 0x040001E3 RID: 483
		[Dependency]
		private readonly ExamineSystem _examine;

		// Token: 0x040001E4 RID: 484
		[Dependency]
		private readonly InteractionSystem _interaction;

		// Token: 0x040001E5 RID: 485
		[Dependency]
		private readonly PricingSystem _pricing;

		// Token: 0x040001E6 RID: 486
		[Dependency]
		private readonly StaminaSystem _stamina;

		// Token: 0x040001E7 RID: 487
		[Dependency]
		private readonly StunSystem _stun;

		// Token: 0x040001E8 RID: 488
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040001E9 RID: 489
		public const float DamagePitchVariation = 0.05f;

		// Token: 0x040001EA RID: 490
		public const float GunClumsyChance = 0.5f;
	}
}
