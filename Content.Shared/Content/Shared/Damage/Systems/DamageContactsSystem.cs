using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems
{
	// Token: 0x02000538 RID: 1336
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageContactsSystem : EntitySystem
	{
		// Token: 0x06001043 RID: 4163 RVA: 0x00034F33 File Offset: 0x00033133
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DamageContactsComponent, StartCollideEvent>(new ComponentEventRefHandler<DamageContactsComponent, StartCollideEvent>(this.OnEntityEnter), null, null);
			base.SubscribeLocalEvent<DamageContactsComponent, EndCollideEvent>(new ComponentEventRefHandler<DamageContactsComponent, EndCollideEvent>(this.OnEntityExit), null, null);
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x00034F64 File Offset: 0x00033164
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (DamagedByContactComponent damaged in base.EntityQuery<DamagedByContactComponent>(false))
			{
				EntityUid ent = damaged.Owner;
				if (!(this._timing.CurTime < damaged.NextSecond))
				{
					damaged.NextSecond = this._timing.CurTime + TimeSpan.FromSeconds(1.0);
					if (damaged.Damage != null)
					{
						this._damageable.TryChangeDamage(new EntityUid?(ent), damaged.Damage, false, false, null, null);
					}
				}
			}
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x00035024 File Offset: 0x00033224
		private void OnEntityExit(EntityUid uid, DamageContactsComponent component, ref EndCollideEvent args)
		{
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			PhysicsComponent body;
			if (!base.TryComp<PhysicsComponent>(uid, ref body))
			{
				return;
			}
			EntityQuery<DamageContactsComponent> damageQuery = base.GetEntityQuery<DamageContactsComponent>();
			foreach (PhysicsComponent physicsComponent in this._physics.GetContactingEntities(body, false))
			{
				EntityUid ent = physicsComponent.Owner;
				if (!(ent == uid) && damageQuery.HasComponent(ent))
				{
					return;
				}
			}
			base.RemComp<DamagedByContactComponent>(otherUid);
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x000350C0 File Offset: 0x000332C0
		private void OnEntityEnter(EntityUid uid, DamageContactsComponent component, ref StartCollideEvent args)
		{
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			if (base.HasComp<DamagedByContactComponent>(otherUid))
			{
				return;
			}
			EntityWhitelist ignoreWhitelist = component.IgnoreWhitelist;
			if (ignoreWhitelist != null && ignoreWhitelist.IsValid(otherUid, null))
			{
				return;
			}
			base.EnsureComp<DamagedByContactComponent>(otherUid).Damage = component.Damage;
		}

		// Token: 0x04000F50 RID: 3920
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000F51 RID: 3921
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x04000F52 RID: 3922
		[Dependency]
		private readonly SharedPhysicsSystem _physics;
	}
}
