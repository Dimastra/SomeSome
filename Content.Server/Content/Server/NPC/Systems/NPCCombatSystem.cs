using System;
using System.Runtime.CompilerServices;
using Content.Server.CombatMode;
using Content.Server.Interaction;
using Content.Server.NPC.Components;
using Content.Server.NPC.Events;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.CombatMode;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.NPC.Systems
{
	// Token: 0x02000333 RID: 819
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NPCCombatSystem : EntitySystem
	{
		// Token: 0x060010F3 RID: 4339 RVA: 0x00057324 File Offset: 0x00055524
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeMelee();
			this.InitializeRanged();
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00057338 File Offset: 0x00055538
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateMelee(frameTime);
			this.UpdateRanged(frameTime);
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x0005734F File Offset: 0x0005554F
		private void InitializeMelee()
		{
			base.SubscribeLocalEvent<NPCMeleeCombatComponent, ComponentStartup>(new ComponentEventHandler<NPCMeleeCombatComponent, ComponentStartup>(this.OnMeleeStartup), null, null);
			base.SubscribeLocalEvent<NPCMeleeCombatComponent, ComponentShutdown>(new ComponentEventHandler<NPCMeleeCombatComponent, ComponentShutdown>(this.OnMeleeShutdown), null, null);
			base.SubscribeLocalEvent<NPCMeleeCombatComponent, NPCSteeringEvent>(new ComponentEventRefHandler<NPCMeleeCombatComponent, NPCSteeringEvent>(this.OnMeleeSteering), null, null);
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x00057390 File Offset: 0x00055590
		private void OnMeleeSteering(EntityUid uid, NPCMeleeCombatComponent component, ref NPCSteeringEvent args)
		{
			args.Steering.CanSeek = true;
			MeleeWeaponComponent weapon;
			if (base.TryComp<MeleeWeaponComponent>(component.Weapon, ref weapon))
			{
				if (weapon.NextAttack - this._timing.CurTime < TimeSpan.FromSeconds((double)(1f / weapon.AttackRate)) * 0.5)
				{
					return;
				}
				Vector2 pointA;
				Vector2 pointB;
				if (!this._physics.TryGetNearestPoints(uid, component.Target, ref pointA, ref pointB, null, null, null, null, null, null))
				{
					return;
				}
				float idealDistance = weapon.Range * 1.5f;
				Vector2 obstacleDirection = pointB - args.WorldPosition;
				float obstacleDistance = obstacleDirection.Length;
				if (obstacleDistance > idealDistance || obstacleDistance == 0f)
				{
					return;
				}
				args.Steering.CanSeek = false;
				obstacleDirection = args.OffsetRotation.RotateVec(ref obstacleDirection);
				Vector2 norm = obstacleDirection.Normalized;
				float weight = (obstacleDistance <= args.AgentRadius) ? 1f : ((idealDistance - obstacleDistance) / idealDistance);
				for (int i = 0; i < 12; i++)
				{
					float result = -Vector2.Dot(norm, NPCSteeringSystem.Directions[i]) * weight;
					if (result >= 0f)
					{
						args.Interest[i] = MathF.Max(args.Interest[i], result);
					}
				}
			}
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x000574D8 File Offset: 0x000556D8
		private void OnMeleeShutdown(EntityUid uid, NPCMeleeCombatComponent component, ComponentShutdown args)
		{
			CombatModeComponent combatMode;
			if (base.TryComp<CombatModeComponent>(uid, ref combatMode))
			{
				combatMode.IsInCombatMode = false;
			}
			this._steering.Unregister(component.Owner, null);
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x0005750C File Offset: 0x0005570C
		private void OnMeleeStartup(EntityUid uid, NPCMeleeCombatComponent component, ComponentStartup args)
		{
			CombatModeComponent combatMode;
			if (base.TryComp<CombatModeComponent>(uid, ref combatMode))
			{
				combatMode.IsInCombatMode = true;
			}
			component.Weapon = uid;
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00057534 File Offset: 0x00055734
		private void UpdateMelee(float frameTime)
		{
			EntityQuery<CombatModeComponent> combatQuery = base.GetEntityQuery<CombatModeComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<PhysicsComponent> physicsQuery = base.GetEntityQuery<PhysicsComponent>();
			TimeSpan curTime = this._timing.CurTime;
			foreach (ValueTuple<NPCMeleeCombatComponent, ActiveNPCComponent> valueTuple in base.EntityQuery<NPCMeleeCombatComponent, ActiveNPCComponent>(false))
			{
				NPCMeleeCombatComponent comp = valueTuple.Item1;
				EntityUid uid = comp.Owner;
				CombatModeComponent combat;
				if (!combatQuery.TryGetComponent(uid, ref combat) || !combat.IsInCombatMode)
				{
					base.RemComp<NPCMeleeCombatComponent>(uid);
				}
				else
				{
					this.Attack(uid, comp, curTime, physicsQuery, xformQuery);
				}
			}
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x000575E0 File Offset: 0x000557E0
		private void Attack(EntityUid uid, NPCMeleeCombatComponent component, TimeSpan curTime, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> physicsQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			component.Status = CombatStatus.Normal;
			MeleeWeaponComponent weapon;
			if (!base.TryComp<MeleeWeaponComponent>(component.Weapon, ref weapon))
			{
				component.Status = CombatStatus.NoWeapon;
				return;
			}
			TransformComponent xform;
			TransformComponent targetXform;
			if (!xformQuery.TryGetComponent(uid, ref xform) || !xformQuery.TryGetComponent(component.Target, ref targetXform))
			{
				component.Status = CombatStatus.TargetUnreachable;
				return;
			}
			float distance;
			if (!xform.Coordinates.TryDistance(this.EntityManager, targetXform.Coordinates, ref distance))
			{
				component.Status = CombatStatus.TargetUnreachable;
				return;
			}
			if (distance > 14f)
			{
				component.Status = CombatStatus.TargetUnreachable;
				return;
			}
			NPCSteeringComponent steering;
			if (base.TryComp<NPCSteeringComponent>(uid, ref steering) && steering.Status == SteeringStatus.NoPath)
			{
				component.Status = CombatStatus.TargetUnreachable;
				return;
			}
			if (distance > weapon.Range)
			{
				component.Status = CombatStatus.TargetOutOfRange;
				return;
			}
			steering = base.EnsureComp<NPCSteeringComponent>(uid);
			steering.Range = MathF.Max(0.2f, weapon.Range - 0.4f);
			this._steering.TryRegister(uid, new EntityCoordinates(component.Target, Vector2.Zero), steering);
			if (weapon.NextAttack > curTime || !this.Enabled)
			{
				return;
			}
			PhysicsComponent targetPhysics;
			if (RandomExtensions.Prob(this._random, component.MissChance) && physicsQuery.TryGetComponent(component.Target, ref targetPhysics) && targetPhysics.LinearVelocity.LengthSquared != 0f)
			{
				this._melee.AttemptLightAttackMiss(uid, component.Weapon, weapon, targetXform.Coordinates.Offset(this._random.NextVector2(0.5f)));
				return;
			}
			this._melee.AttemptLightAttack(uid, component.Weapon, weapon, component.Target);
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x00057774 File Offset: 0x00055974
		private void InitializeRanged()
		{
			base.SubscribeLocalEvent<NPCRangedCombatComponent, ComponentStartup>(new ComponentEventHandler<NPCRangedCombatComponent, ComponentStartup>(this.OnRangedStartup), null, null);
			base.SubscribeLocalEvent<NPCRangedCombatComponent, ComponentShutdown>(new ComponentEventHandler<NPCRangedCombatComponent, ComponentShutdown>(this.OnRangedShutdown), null, null);
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x000577A0 File Offset: 0x000559A0
		private void OnRangedStartup(EntityUid uid, NPCRangedCombatComponent component, ComponentStartup args)
		{
			SharedCombatModeComponent combat;
			if (base.TryComp<SharedCombatModeComponent>(uid, ref combat))
			{
				combat.IsInCombatMode = true;
				return;
			}
			component.Status = CombatStatus.Unspecified;
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x000577C8 File Offset: 0x000559C8
		private void OnRangedShutdown(EntityUid uid, NPCRangedCombatComponent component, ComponentShutdown args)
		{
			SharedCombatModeComponent combat;
			if (base.TryComp<SharedCombatModeComponent>(uid, ref combat))
			{
				combat.IsInCombatMode = false;
			}
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x000577E8 File Offset: 0x000559E8
		private void UpdateRanged(float frameTime)
		{
			EntityQuery<PhysicsComponent> bodyQuery = base.GetEntityQuery<PhysicsComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<SharedCombatModeComponent> combatQuery = base.GetEntityQuery<SharedCombatModeComponent>();
			foreach (ValueTuple<NPCRangedCombatComponent, TransformComponent> valueTuple in base.EntityQuery<NPCRangedCombatComponent, TransformComponent>(false))
			{
				NPCRangedCombatComponent comp = valueTuple.Item1;
				TransformComponent xform = valueTuple.Item2;
				if (comp.Status != CombatStatus.Unspecified)
				{
					TransformComponent targetXform;
					PhysicsComponent targetBody;
					if (!xformQuery.TryGetComponent(comp.Target, ref targetXform) || !bodyQuery.TryGetComponent(comp.Target, ref targetBody))
					{
						comp.Status = CombatStatus.TargetUnreachable;
						comp.ShootAccumulator = 0f;
					}
					else if (targetXform.MapID != xform.MapID)
					{
						comp.Status = CombatStatus.TargetUnreachable;
						comp.ShootAccumulator = 0f;
					}
					else
					{
						SharedCombatModeComponent combatMode;
						if (combatQuery.TryGetComponent(comp.Owner, ref combatMode))
						{
							combatMode.IsInCombatMode = true;
						}
						GunComponent gun = this._gun.GetGun(comp.Owner);
						if (gun == null)
						{
							comp.Status = CombatStatus.NoWeapon;
							comp.ShootAccumulator = 0f;
						}
						else
						{
							comp.LOSAccumulator -= frameTime;
							Vector2 worldPos = this._transform.GetWorldPositionRotation(xform, xformQuery).Item1;
							Vector2 targetPos = this._transform.GetWorldPositionRotation(targetXform, xformQuery).Item1;
							float distance = (targetPos - worldPos).Length;
							bool oldInLos = comp.TargetInLOS;
							if (comp.LOSAccumulator < 0f)
							{
								comp.LOSAccumulator += 0.2f;
								comp.TargetInLOS = this._interaction.InRangeUnobstructed(comp.Owner, comp.Target, distance + 0.1f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false);
							}
							if (!comp.TargetInLOS)
							{
								comp.ShootAccumulator = 0f;
								comp.Status = CombatStatus.TargetUnreachable;
							}
							else
							{
								if (!oldInLos && comp.SoundTargetInLOS != null)
								{
									this._audio.PlayPvs(comp.SoundTargetInLOS, comp.Owner, null);
								}
								comp.ShootAccumulator += frameTime;
								if (comp.ShootAccumulator >= comp.ShootDelay)
								{
									Vector2 mapVelocity = targetBody.LinearVelocity;
									Vector2 targetSpot = targetPos + mapVelocity * distance / 20f;
									Angle goalRotation = DirectionExtensions.ToWorldAngle(targetSpot - worldPos);
									Angle? rotationSpeed = comp.RotationSpeed;
									if (this._rotate.TryRotateTo(comp.Owner, goalRotation, frameTime, comp.AccuracyThreshold, (rotationSpeed != null) ? rotationSpeed.GetValueOrDefault().Theta : 1.7976931348623157E+308, xform) && this.Enabled && this._gun.CanShoot(gun))
									{
										MapGridComponent mapGrid;
										EntityCoordinates targetCordinates;
										if (this._mapManager.TryFindGridAt(xform.MapID, targetPos, ref mapGrid))
										{
											targetCordinates..ctor(mapGrid.Owner, mapGrid.WorldToLocal(targetSpot));
										}
										else
										{
											targetCordinates..ctor(xform.MapUid.Value, targetSpot);
										}
										this._gun.AttemptShoot(comp.Owner, gun, targetCordinates);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04000A0D RID: 2573
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000A0E RID: 2574
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000A0F RID: 2575
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000A10 RID: 2576
		[Dependency]
		private readonly GunSystem _gun;

		// Token: 0x04000A11 RID: 2577
		[Dependency]
		private readonly InteractionSystem _interaction;

		// Token: 0x04000A12 RID: 2578
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000A13 RID: 2579
		[Dependency]
		private readonly NPCSteeringSystem _steering;

		// Token: 0x04000A14 RID: 2580
		[Dependency]
		private readonly SharedMeleeWeaponSystem _melee;

		// Token: 0x04000A15 RID: 2581
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000A16 RID: 2582
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000A17 RID: 2583
		public bool Enabled = true;

		// Token: 0x04000A18 RID: 2584
		private const float TargetMeleeLostRange = 14f;

		// Token: 0x04000A19 RID: 2585
		[Dependency]
		private readonly RotateToFaceSystem _rotate;

		// Token: 0x04000A1A RID: 2586
		private const float ShootSpeed = 20f;

		// Token: 0x04000A1B RID: 2587
		public const float UnoccludedCooldown = 0.2f;
	}
}
