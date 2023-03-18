using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Systems;
using Content.Shared.Revenant.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Revenant.EntitySystems
{
	// Token: 0x020001F6 RID: 502
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedCorporealSystem : EntitySystem
	{
		// Token: 0x06000592 RID: 1426 RVA: 0x00014358 File Offset: 0x00012558
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CorporealComponent, ComponentStartup>(new ComponentEventHandler<CorporealComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<CorporealComponent, ComponentShutdown>(new ComponentEventHandler<CorporealComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<CorporealComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<CorporealComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefresh), null, null);
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x000143A9 File Offset: 0x000125A9
		private void OnRefresh(EntityUid uid, CorporealComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			args.ModifySpeed(component.MovementSpeedDebuff, component.MovementSpeedDebuff);
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x000143C0 File Offset: 0x000125C0
		public virtual void OnStartup(EntityUid uid, CorporealComponent component, ComponentStartup args)
		{
			this._appearance.SetData(uid, RevenantVisuals.Corporeal, true, null);
			FixturesComponent fixtures;
			if (base.TryComp<FixturesComponent>(uid, ref fixtures) && fixtures.FixtureCount >= 1)
			{
				Fixture fixture = fixtures.Fixtures.Values.First<Fixture>();
				this._physics.SetCollisionMask(uid, fixture, 50, fixtures, null);
				this._physics.SetCollisionLayer(uid, fixture, 65, fixtures, null);
			}
			this._movement.RefreshMovementSpeedModifiers(uid, null);
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x0001443C File Offset: 0x0001263C
		public virtual void OnShutdown(EntityUid uid, CorporealComponent component, ComponentShutdown args)
		{
			this._appearance.SetData(uid, RevenantVisuals.Corporeal, false, null);
			FixturesComponent fixtures;
			if (base.TryComp<FixturesComponent>(uid, ref fixtures) && fixtures.FixtureCount >= 1)
			{
				Fixture fixture = fixtures.Fixtures.Values.First<Fixture>();
				this._physics.SetCollisionMask(uid, fixture, 32, fixtures, null);
				this._physics.SetCollisionLayer(uid, fixture, 0, fixtures, null);
			}
			component.MovementSpeedDebuff = 1f;
			this._movement.RefreshMovementSpeedModifiers(uid, null);
		}

		// Token: 0x0400059D RID: 1437
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400059E RID: 1438
		[Dependency]
		private readonly MovementSpeedModifierSystem _movement;

		// Token: 0x0400059F RID: 1439
		[Dependency]
		private readonly SharedPhysicsSystem _physics;
	}
}
