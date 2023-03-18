using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Camera;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;

namespace Content.Server.Projectiles
{
	// Token: 0x0200026B RID: 619
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ProjectileSystem : SharedProjectileSystem
	{
		// Token: 0x06000C4F RID: 3151 RVA: 0x0004096E File Offset: 0x0003EB6E
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ProjectileComponent, StartCollideEvent>(new ComponentEventRefHandler<ProjectileComponent, StartCollideEvent>(this.OnStartCollide), null, null);
			base.SubscribeLocalEvent<ProjectileComponent, ComponentGetState>(new ComponentEventRefHandler<ProjectileComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x0004099E File Offset: 0x0003EB9E
		private void OnGetState(EntityUid uid, ProjectileComponent component, ref ComponentGetState args)
		{
			args.State = new SharedProjectileSystem.ProjectileComponentState(component.Shooter, component.IgnoreShooter);
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x000409B8 File Offset: 0x0003EBB8
		private void OnStartCollide(EntityUid uid, ProjectileComponent component, ref StartCollideEvent args)
		{
			if (args.OurFixture.ID != "projectile" || !args.OtherFixture.Hard || component.DamagedEntity)
			{
				return;
			}
			EntityUid otherEntity = args.OtherFixture.Body.Owner;
			ProjectileCollideAttemptEvent attemptEv = new ProjectileCollideAttemptEvent(otherEntity, false);
			base.RaiseLocalEvent<ProjectileCollideAttemptEvent>(uid, ref attemptEv, false);
			if (attemptEv.Cancelled)
			{
				return;
			}
			EntityStringRepresentation otherName = base.ToPrettyString(otherEntity);
			Vector2 direction = args.OurFixture.Body.LinearVelocity.Normalized;
			DamageSpecifier modifiedDamage = this._damageableSystem.TryChangeDamage(new EntityUid?(otherEntity), component.Damage, component.IgnoreResistances, true, null, new EntityUid?(component.Shooter));
			component.DamagedEntity = true;
			bool deleted = base.Deleted(otherEntity, null);
			if (modifiedDamage != null && this.EntityManager.EntityExists(component.Shooter))
			{
				if (modifiedDamage.Total > FixedPoint2.Zero && !deleted)
				{
					base.RaiseNetworkEvent(new DamageEffectEvent(Color.Red, new List<EntityUid>
					{
						otherEntity
					}), Filter.Pvs(otherEntity, 2f, this.EntityManager, null, null), true);
				}
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.BulletHit;
				LogImpact impact = base.HasComp<ActorComponent>(otherEntity) ? LogImpact.Extreme : LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(43, 4);
				logStringHandler.AppendLiteral("Projectile ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "projectile", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" shot by ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Shooter), "user", "ToPrettyString(component.Shooter)");
				logStringHandler.AppendLiteral(" hit ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(otherName, "target", "otherName");
				logStringHandler.AppendLiteral(" and dealt ");
				logStringHandler.AppendFormatted<FixedPoint2>(modifiedDamage.Total, "damage", "modifiedDamage.Total");
				logStringHandler.AppendLiteral(" damage");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			if (!deleted)
			{
				this._guns.PlayImpactSound(otherEntity, modifiedDamage, component.SoundHit, component.ForceSound);
				this._sharedCameraRecoil.KickCamera(otherEntity, direction, null);
			}
			if (component.DeleteOnCollide)
			{
				base.QueueDel(uid);
				TransformComponent xform;
				if (component.ImpactEffect != null && base.TryComp<TransformComponent>(component.Owner, ref xform))
				{
					base.RaiseNetworkEvent(new SharedProjectileSystem.ImpactEffectEvent(component.ImpactEffect, xform.Coordinates), Filter.Pvs(xform.Coordinates, 2f, this.EntityManager, null), true);
				}
			}
		}

		// Token: 0x040007A0 RID: 1952
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040007A1 RID: 1953
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x040007A2 RID: 1954
		[Dependency]
		private readonly GunSystem _guns;

		// Token: 0x040007A3 RID: 1955
		[Dependency]
		private readonly SharedCameraRecoilSystem _sharedCameraRecoil;
	}
}
