using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Hands.Components;
using Content.Server.Popups;
using Content.Server.Projectiles;
using Content.Server.Weapons.Melee.EnergySword;
using Content.Server.Weapons.Reflect;
using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;

namespace Server.Content.Weapons.Reflect
{
	// Token: 0x02000010 RID: 16
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReflectSystem : EntitySystem
	{
		// Token: 0x06000029 RID: 41 RVA: 0x000026E8 File Offset: 0x000008E8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ProjectileComponent, ProjectileCollideAttemptEvent>(new ComponentEventRefHandler<ProjectileComponent, ProjectileCollideAttemptEvent>(this.TryReflectProjectile), null, null);
			base.SubscribeLocalEvent<HitScanShotEvent>(new EntityEventRefHandler<HitScanShotEvent>(this.TryReflectHitScan), null, null);
			base.SubscribeLocalEvent<ReflectComponent, EnergySwordActivatedEvent>(new ComponentEventRefHandler<ReflectComponent, EnergySwordActivatedEvent>(this.EnableReflect), null, null);
			base.SubscribeLocalEvent<ReflectComponent, EnergySwordDeactivatedEvent>(new ComponentEventRefHandler<ReflectComponent, EnergySwordDeactivatedEvent>(this.DisableReflect), null, null);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x0000274C File Offset: 0x0000094C
		private void TryReflectProjectile(EntityUid uid, ProjectileComponent projComp, ref ProjectileCollideAttemptEvent args)
		{
			PhysicsComponent physicsComp;
			if (!base.TryComp<PhysicsComponent>(uid, ref physicsComp))
			{
				return;
			}
			HandsComponent hands;
			if (base.TryComp<HandsComponent>(args.Target, ref hands))
			{
				foreach (KeyValuePair<string, Hand> keyValuePair in hands.Hands)
				{
					string text;
					Hand hand2;
					keyValuePair.Deconstruct(out text, out hand2);
					Hand hand = hand2;
					ReflectComponent reflect;
					if (base.TryComp<ReflectComponent>(hand.HeldEntity, ref reflect) && reflect.Enabled && RandomExtensions.Prob(this._random, reflect.Chance))
					{
						Vector2 vel = this._physics.GetMapLinearVelocity(args.Target, null, null, null, null) - this._physics.GetMapLinearVelocity(uid, null, null, null, null);
						vel = this._random.NextAngle(-reflect.Spread / 2.0, reflect.Spread / 2.0).RotateVec(ref vel);
						this._physics.SetLinearVelocity(uid, vel, true, true, null, null);
						this._transform.SetWorldRotation(uid, DirectionExtensions.ToWorldAngle(vel));
						projComp.Shooter = args.Target;
						this._popup.PopupEntity(Loc.GetString("reflect-shot"), uid, PopupType.Small);
						this._audio.PlayPvs(reflect.OnReflect, uid, new AudioParams?(AudioHelpers.WithVariation(0.05f, this._random)));
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.ShotReflected;
						LogStringHandler logStringHandler = new LogStringHandler(22, 2);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target), "user", "ToPrettyString(args.Target)");
						logStringHandler.AppendLiteral(" reflected projectile ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "projectile", "ToPrettyString(uid)");
						adminLogger.Add(type, ref logStringHandler);
						args.Cancelled = true;
						break;
					}
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002988 File Offset: 0x00000B88
		private void TryReflectHitScan(ref HitScanShotEvent args)
		{
			if (args.User == null)
			{
				return;
			}
			HandsComponent hands;
			if (base.TryComp<HandsComponent>(args.Target, ref hands))
			{
				foreach (KeyValuePair<string, Hand> keyValuePair in hands.Hands)
				{
					string text;
					Hand hand2;
					keyValuePair.Deconstruct(out text, out hand2);
					Hand hand = hand2;
					ReflectComponent reflect;
					if (base.TryComp<ReflectComponent>(hand.HeldEntity, ref reflect) && reflect.Enabled && RandomExtensions.Prob(this._random, reflect.Chance))
					{
						this._popup.PopupEntity(Loc.GetString("reflect-shot"), args.Target, PopupType.Small);
						this._audio.PlayPvs(reflect.OnReflect, args.Target, new AudioParams?(AudioHelpers.WithVariation(0.05f, this._random)));
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.ShotReflected;
						LogStringHandler logStringHandler = new LogStringHandler(23, 1);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target), "entity", "ToPrettyString(args.Target)");
						logStringHandler.AppendLiteral(" reflected hitscan shot");
						adminLogger.Add(type, ref logStringHandler);
						args.Target = args.User.Value;
						break;
					}
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002AF4 File Offset: 0x00000CF4
		private void EnableReflect(EntityUid uid, ReflectComponent comp, ref EnergySwordActivatedEvent args)
		{
			comp.Enabled = true;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002AFD File Offset: 0x00000CFD
		private void DisableReflect(EntityUid uid, ReflectComponent comp, ref EnergySwordDeactivatedEvent args)
		{
			comp.Enabled = false;
		}

		// Token: 0x04000017 RID: 23
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000018 RID: 24
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000019 RID: 25
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x0400001A RID: 26
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x0400001B RID: 27
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x0400001C RID: 28
		[Dependency]
		private readonly SharedTransformSystem _transform;
	}
}
