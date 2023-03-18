using System;
using System.Runtime.CompilerServices;
using Content.Shared.Ghost;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.EntitySystems
{
	// Token: 0x0200019D RID: 413
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedEventHorizonSystem : EntitySystem
	{
		// Token: 0x060004D8 RID: 1240 RVA: 0x00012988 File Offset: 0x00010B88
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EventHorizonComponent, ComponentStartup>(new ComponentEventHandler<EventHorizonComponent, ComponentStartup>(this.OnEventHorizonStartup), null, null);
			base.SubscribeLocalEvent<EventHorizonComponent, PreventCollideEvent>(new ComponentEventRefHandler<EventHorizonComponent, PreventCollideEvent>(this.OnPreventCollide), null, null);
			ViewVariablesTypeHandler<EventHorizonComponent> typeHandler = this.Vvm.GetTypeHandler<EventHorizonComponent>();
			typeHandler.AddPath<float>("Radius", (EntityUid _, EventHorizonComponent comp) => comp.Radius, delegate(EntityUid uid, float value, EventHorizonComponent comp)
			{
				this.SetRadius(uid, value, true, comp);
			});
			typeHandler.AddPath<bool>("CanBreachContainment", (EntityUid _, EventHorizonComponent comp) => comp.CanBreachContainment, delegate(EntityUid uid, bool value, EventHorizonComponent comp)
			{
				this.SetCanBreachContainment(uid, value, true, comp);
			});
			typeHandler.AddPath<string>("HorizonFixtureId", (EntityUid _, [Nullable(1)] EventHorizonComponent comp) => comp.HorizonFixtureId, delegate(EntityUid uid, string value, [Nullable(2)] EventHorizonComponent comp)
			{
				this.SetHorizonFixtureId(uid, value, true, comp);
			});
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00012A72 File Offset: 0x00010C72
		public override void Shutdown()
		{
			ViewVariablesTypeHandler<EventHorizonComponent> typeHandler = this.Vvm.GetTypeHandler<EventHorizonComponent>();
			typeHandler.RemovePath("Radius");
			typeHandler.RemovePath("CanBreachContainment");
			typeHandler.RemovePath("HorizonFixtureId");
			base.Shutdown();
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x00012AA8 File Offset: 0x00010CA8
		[NullableContext(2)]
		public void SetRadius(EntityUid uid, float value, bool updateFixture = true, EventHorizonComponent eventHorizon = null)
		{
			if (!base.Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
			{
				return;
			}
			float oldValue = eventHorizon.Radius;
			if (value == oldValue)
			{
				return;
			}
			eventHorizon.Radius = value;
			base.Dirty(eventHorizon, null);
			if (updateFixture)
			{
				this.UpdateEventHorizonFixture(uid, null, eventHorizon);
			}
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x00012AF0 File Offset: 0x00010CF0
		[NullableContext(2)]
		public void SetCanBreachContainment(EntityUid uid, bool value, bool updateFixture = true, EventHorizonComponent eventHorizon = null)
		{
			if (!base.Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
			{
				return;
			}
			bool oldValue = eventHorizon.CanBreachContainment;
			if (value == oldValue)
			{
				return;
			}
			eventHorizon.CanBreachContainment = value;
			base.Dirty(eventHorizon, null);
			if (updateFixture)
			{
				this.UpdateEventHorizonFixture(uid, null, eventHorizon);
			}
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x00012B38 File Offset: 0x00010D38
		[NullableContext(2)]
		public void SetHorizonFixtureId(EntityUid uid, string value, bool updateFixture = true, EventHorizonComponent eventHorizon = null)
		{
			if (!base.Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
			{
				return;
			}
			string oldValue = eventHorizon.HorizonFixtureId;
			if (value == oldValue)
			{
				return;
			}
			eventHorizon.HorizonFixtureId = value;
			base.Dirty(eventHorizon, null);
			if (updateFixture)
			{
				this.UpdateEventHorizonFixture(uid, null, eventHorizon);
			}
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x00012B84 File Offset: 0x00010D84
		[NullableContext(2)]
		public void UpdateEventHorizonFixture(EntityUid uid, PhysicsComponent physics = null, EventHorizonComponent eventHorizon = null)
		{
			if (!base.Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
			{
				return;
			}
			string fixtureId = eventHorizon.HorizonFixtureId;
			FixturesComponent manager = null;
			if (fixtureId == null || !base.Resolve<FixturesComponent, PhysicsComponent>(uid, ref manager, ref physics, false))
			{
				return;
			}
			Fixture fixture = this._fixtures.GetFixtureOrNull(uid, fixtureId, manager);
			if (fixture == null)
			{
				return;
			}
			PhysShapeCircle shape = (PhysShapeCircle)fixture.Shape;
			this._physics.SetRadius(uid, fixture, shape, eventHorizon.Radius, manager, physics, null);
			this._physics.SetHard(uid, fixture, true, manager);
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x00012BFE File Offset: 0x00010DFE
		private void OnEventHorizonStartup(EntityUid uid, EventHorizonComponent comp, ComponentStartup args)
		{
			this.UpdateEventHorizonFixture(uid, null, comp);
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x00012C09 File Offset: 0x00010E09
		private void OnPreventCollide(EntityUid uid, EventHorizonComponent comp, ref PreventCollideEvent args)
		{
			if (!args.Cancelled)
			{
				this.PreventCollide(uid, comp, ref args);
			}
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x00012C20 File Offset: 0x00010E20
		protected virtual bool PreventCollide(EntityUid uid, EventHorizonComponent comp, ref PreventCollideEvent args)
		{
			EntityUid otherUid = args.BodyB.Owner;
			if (base.HasComp<MapGridComponent>(otherUid) || base.HasComp<SharedGhostComponent>(otherUid))
			{
				args.Cancelled = true;
				return true;
			}
			if (base.HasComp<ContainmentFieldComponent>(otherUid) || base.HasComp<ContainmentFieldGeneratorComponent>(otherUid))
			{
				if (comp.CanBreachContainment)
				{
					args.Cancelled = true;
				}
				return true;
			}
			return false;
		}

		// Token: 0x04000481 RID: 1153
		[Dependency]
		private readonly FixtureSystem _fixtures;

		// Token: 0x04000482 RID: 1154
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000483 RID: 1155
		[Dependency]
		protected readonly IViewVariablesManager Vvm;
	}
}
