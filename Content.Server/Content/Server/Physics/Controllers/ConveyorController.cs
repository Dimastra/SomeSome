using System;
using System.Runtime.CompilerServices;
using Content.Server.MachineLinking.Events;
using Content.Server.MachineLinking.System;
using Content.Server.Power.Components;
using Content.Server.Recycling;
using Content.Server.Recycling.Components;
using Content.Shared.Conveyor;
using Content.Shared.Maps;
using Content.Shared.Physics.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Physics.Controllers
{
	// Token: 0x020002D9 RID: 729
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConveyorController : SharedConveyorController
	{
		// Token: 0x06000ED2 RID: 3794 RVA: 0x0004B10C File Offset: 0x0004930C
		public override void Initialize()
		{
			base.UpdatesAfter.Add(typeof(MoverController));
			base.SubscribeLocalEvent<ConveyorComponent, ComponentInit>(new ComponentEventHandler<ConveyorComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ConveyorComponent, ComponentShutdown>(new ComponentEventHandler<ConveyorComponent, ComponentShutdown>(this.OnConveyorShutdown), null, null);
			base.SubscribeLocalEvent<ConveyorComponent, SignalReceivedEvent>(new ComponentEventHandler<ConveyorComponent, SignalReceivedEvent>(this.OnSignalReceived), null, null);
			base.SubscribeLocalEvent<ConveyorComponent, PowerChangedEvent>(new ComponentEventRefHandler<ConveyorComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.Initialize();
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x0004B184 File Offset: 0x00049384
		private void OnInit(EntityUid uid, ConveyorComponent component, ComponentInit args)
		{
			this._signalSystem.EnsureReceiverPorts(uid, new string[]
			{
				component.ReversePort,
				component.ForwardPort,
				component.OffPort
			});
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(uid, ref physics))
			{
				PolygonShape shape = new PolygonShape();
				shape.SetAsBox(0.55f, 0.55f);
				this._fixtures.TryCreateFixture(uid, shape, "conveyor", 1f, false, 22, 0, 0.4f, 0f, true, null, physics, null);
			}
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x0004B20C File Offset: 0x0004940C
		private void OnConveyorShutdown(EntityUid uid, ConveyorComponent component, ComponentShutdown args)
		{
			if (base.MetaData(uid).EntityLifeStage >= 4)
			{
				return;
			}
			base.RemComp<ActiveConveyorComponent>(uid);
			PhysicsComponent physics;
			if (!base.TryComp<PhysicsComponent>(uid, ref physics))
			{
				return;
			}
			this._fixtures.DestroyFixture(uid, "conveyor", true, physics, null, null);
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0004B252 File Offset: 0x00049452
		private void OnPowerChanged(EntityUid uid, ConveyorComponent component, ref PowerChangedEvent args)
		{
			component.Powered = args.Powered;
			this.UpdateAppearance(uid, component);
			base.Dirty(component, null);
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x0004B270 File Offset: 0x00049470
		private void UpdateAppearance(EntityUid uid, ConveyorComponent component)
		{
			this._appearance.SetData(uid, ConveyorVisuals.State, component.Powered ? component.State : ConveyorState.Off, null);
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x0004B29C File Offset: 0x0004949C
		private void OnSignalReceived(EntityUid uid, ConveyorComponent component, SignalReceivedEvent args)
		{
			if (args.Port == component.OffPort)
			{
				this.SetState(uid, ConveyorState.Off, component);
				return;
			}
			if (args.Port == component.ForwardPort)
			{
				this.AwakenEntities(uid, component);
				this.SetState(uid, ConveyorState.Forward, component);
				return;
			}
			if (args.Port == component.ReversePort)
			{
				this.AwakenEntities(uid, component);
				this.SetState(uid, ConveyorState.Reverse, component);
			}
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x0004B310 File Offset: 0x00049510
		[NullableContext(2)]
		private void SetState(EntityUid uid, ConveyorState state, ConveyorComponent component = null)
		{
			if (!base.Resolve<ConveyorComponent>(uid, ref component, true))
			{
				return;
			}
			component.State = state;
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(uid, ref physics))
			{
				this._broadphase.RegenerateContacts(physics, null, null);
			}
			RecyclerComponent recycler;
			if (base.TryComp<RecyclerComponent>(uid, ref recycler))
			{
				if (component.State != ConveyorState.Off)
				{
					this._recycler.EnableRecycler(recycler);
				}
				else
				{
					this._recycler.DisableRecycler(recycler);
				}
			}
			this.UpdateAppearance(uid, component);
			base.Dirty(component, null);
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x0004B388 File Offset: 0x00049588
		private void AwakenEntities(EntityUid uid, ConveyorComponent component)
		{
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<PhysicsComponent> bodyQuery = base.GetEntityQuery<PhysicsComponent>();
			TransformComponent xform;
			if (!xformQuery.TryGetComponent(uid, ref xform))
			{
				return;
			}
			TileRef? beltTileRef = xform.Coordinates.GetTileRef(this.EntityManager, this._mapManager);
			if (beltTileRef != null)
			{
				foreach (EntityUid entity in this._lookup.GetEntitiesIntersecting(beltTileRef.Value, 46))
				{
					PhysicsComponent physics;
					if (bodyQuery.TryGetComponent(entity, ref physics) && physics.BodyType != 4)
					{
						this._physics.WakeBody(entity, false, null, physics);
					}
				}
			}
		}

		// Token: 0x040008AF RID: 2223
		[Dependency]
		private readonly FixtureSystem _fixtures;

		// Token: 0x040008B0 RID: 2224
		[Dependency]
		private readonly RecyclerSystem _recycler;

		// Token: 0x040008B1 RID: 2225
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x040008B2 RID: 2226
		[Dependency]
		private readonly SharedBroadphaseSystem _broadphase;

		// Token: 0x040008B3 RID: 2227
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
