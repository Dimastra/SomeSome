using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Physics.Events;
using Robust.Shared.Serialization;

namespace Content.Shared.Projectiles
{
	// Token: 0x02000242 RID: 578
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedProjectileSystem : EntitySystem
	{
		// Token: 0x06000689 RID: 1673 RVA: 0x000172EB File Offset: 0x000154EB
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ProjectileComponent, PreventCollideEvent>(new ComponentEventRefHandler<ProjectileComponent, PreventCollideEvent>(this.PreventCollision), null, null);
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x00017307 File Offset: 0x00015507
		private void PreventCollision(EntityUid uid, ProjectileComponent component, ref PreventCollideEvent args)
		{
			if (component.IgnoreShooter && args.BodyB.Owner == component.Shooter)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x00017330 File Offset: 0x00015530
		public void SetShooter(ProjectileComponent component, EntityUid uid)
		{
			if (component.Shooter == uid)
			{
				return;
			}
			component.Shooter = uid;
			base.Dirty(component, null);
		}

		// Token: 0x0400067C RID: 1660
		public const string ProjectileFixture = "projectile";

		// Token: 0x020007B4 RID: 1972
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ProjectileComponentState : ComponentState
		{
			// Token: 0x06001807 RID: 6151 RVA: 0x0004D452 File Offset: 0x0004B652
			public ProjectileComponentState(EntityUid shooter, bool ignoreShooter)
			{
				this.Shooter = shooter;
				this.IgnoreShooter = ignoreShooter;
			}

			// Token: 0x170004F5 RID: 1269
			// (get) Token: 0x06001808 RID: 6152 RVA: 0x0004D468 File Offset: 0x0004B668
			public EntityUid Shooter { get; }

			// Token: 0x170004F6 RID: 1270
			// (get) Token: 0x06001809 RID: 6153 RVA: 0x0004D470 File Offset: 0x0004B670
			public bool IgnoreShooter { get; }
		}

		// Token: 0x020007B5 RID: 1973
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ImpactEffectEvent : EntityEventArgs
		{
			// Token: 0x0600180A RID: 6154 RVA: 0x0004D478 File Offset: 0x0004B678
			public ImpactEffectEvent(string prototype, EntityCoordinates coordinates)
			{
				this.Prototype = prototype;
				this.Coordinates = coordinates;
			}

			// Token: 0x040017DA RID: 6106
			public string Prototype;

			// Token: 0x040017DB RID: 6107
			public EntityCoordinates Coordinates;
		}
	}
}
