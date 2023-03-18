using System;
using System.Runtime.CompilerServices;
using Content.Server.Beam.Components;
using Content.Shared.Beam;
using Content.Shared.Beam.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Beam
{
	// Token: 0x0200072A RID: 1834
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BeamSystem : SharedBeamSystem
	{
		// Token: 0x06002680 RID: 9856 RVA: 0x000CBAE4 File Offset: 0x000C9CE4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BeamComponent, CreateBeamSuccessEvent>(new ComponentEventHandler<BeamComponent, CreateBeamSuccessEvent>(this.OnBeamCreationSuccess), null, null);
			base.SubscribeLocalEvent<BeamComponent, BeamControllerCreatedEvent>(new ComponentEventHandler<BeamComponent, BeamControllerCreatedEvent>(this.OnControllerCreated), null, null);
			base.SubscribeLocalEvent<BeamComponent, BeamFiredEvent>(new ComponentEventHandler<BeamComponent, BeamFiredEvent>(this.OnBeamFired), null, null);
			base.SubscribeLocalEvent<BeamComponent, ComponentRemove>(new ComponentEventHandler<BeamComponent, ComponentRemove>(this.OnRemove), null, null);
		}

		// Token: 0x06002681 RID: 9857 RVA: 0x000CBB47 File Offset: 0x000C9D47
		private void OnBeamCreationSuccess(EntityUid uid, BeamComponent component, CreateBeamSuccessEvent args)
		{
			component.BeamShooter = args.User;
		}

		// Token: 0x06002682 RID: 9858 RVA: 0x000CBB55 File Offset: 0x000C9D55
		private void OnControllerCreated(EntityUid uid, BeamComponent component, BeamControllerCreatedEvent args)
		{
			component.OriginBeam = args.OriginBeam;
		}

		// Token: 0x06002683 RID: 9859 RVA: 0x000CBB63 File Offset: 0x000C9D63
		private void OnBeamFired(EntityUid uid, BeamComponent component, BeamFiredEvent args)
		{
			component.CreatedBeams.Add(args.CreatedBeam);
		}

		// Token: 0x06002684 RID: 9860 RVA: 0x000CBB78 File Offset: 0x000C9D78
		private void OnRemove(EntityUid uid, BeamComponent component, ComponentRemove args)
		{
			if (component.VirtualBeamController == null)
			{
				return;
			}
			if (component.CreatedBeams.Count == 0 && component.VirtualBeamController.Value.Valid)
			{
				base.QueueDel(component.VirtualBeamController.Value);
			}
		}

		// Token: 0x06002685 RID: 9861 RVA: 0x000CBBC8 File Offset: 0x000C9DC8
		private void CreateBeam(string prototype, Angle userAngle, Vector2 calculatedDistance, MapCoordinates beamStartPos, Vector2 distanceCorrection, EntityUid? controller, [Nullable(2)] string bodyState = null, string shader = "unshaded")
		{
			MapCoordinates beamSpawnPos = beamStartPos;
			EntityUid ent = base.Spawn(prototype, beamSpawnPos);
			EdgeShape shape = new EdgeShape(distanceCorrection, new Vector2(0f, 0f));
			float distanceLength = distanceCorrection.Length;
			PhysicsComponent physics;
			BeamComponent beam;
			if (!base.TryComp<PhysicsComponent>(ent, ref physics) || !base.TryComp<BeamComponent>(ent, ref beam))
			{
				return;
			}
			FixturesComponent manager = null;
			this._fixture.TryCreateFixture(ent, shape, "BeamBody", 1f, false, 65, 10, 0.4f, 0f, true, manager, physics, null);
			this._physics.SetBodyType(ent, 8, manager, physics, null);
			this._physics.SetCanCollide(ent, true, true, false, manager, physics);
			this._broadphase.RegenerateContacts(physics, manager, null);
			BeamVisualizerEvent beamVisualizerEvent = new BeamVisualizerEvent(ent, distanceLength, userAngle, bodyState, shader);
			base.RaiseNetworkEvent(beamVisualizerEvent);
			if (controller != null)
			{
				beam.VirtualBeamController = controller;
			}
			else
			{
				EntityUid controllerEnt = base.Spawn("VirtualBeamEntityController", beamSpawnPos);
				beam.VirtualBeamController = new EntityUid?(controllerEnt);
				this._audio.PlayPvs(beam.Sound, beam.Owner, null);
				BeamControllerCreatedEvent beamControllerCreatedEvent = new BeamControllerCreatedEvent(ent, controllerEnt);
				base.RaiseLocalEvent<BeamControllerCreatedEvent>(controllerEnt, beamControllerCreatedEvent, false);
			}
			int i = 0;
			while ((float)i < distanceLength - 1f)
			{
				beamSpawnPos = beamSpawnPos.Offset(calculatedDistance.Normalized);
				BeamVisualizerEvent ev = new BeamVisualizerEvent(base.Spawn(prototype, beamSpawnPos), distanceLength, userAngle, bodyState, shader);
				base.RaiseNetworkEvent(ev);
				i++;
			}
			BeamFiredEvent beamFiredEvent = new BeamFiredEvent(ent);
			base.RaiseLocalEvent<BeamFiredEvent>(beam.VirtualBeamController.Value, beamFiredEvent, false);
		}

		// Token: 0x06002686 RID: 9862 RVA: 0x000CBD5C File Offset: 0x000C9F5C
		public void TryCreateBeam(EntityUid user, EntityUid target, string bodyPrototype, [Nullable(2)] string bodyState = null, string shader = "unshaded", EntityUid? controller = null)
		{
			if (base.Deleted(user, null) || base.Deleted(target, null))
			{
				return;
			}
			MapCoordinates userMapPos = base.Transform(user).MapPosition;
			MapCoordinates targetMapPos = base.Transform(target).MapPosition;
			Vector2 calculatedDistance = targetMapPos.Position - userMapPos.Position;
			Angle userAngle = DirectionExtensions.ToWorldAngle(calculatedDistance);
			if (userMapPos.MapId != targetMapPos.MapId)
			{
				return;
			}
			MapCoordinates beamStartPos = userMapPos.Offset(calculatedDistance.Normalized);
			if (calculatedDistance.Length == 0f)
			{
				return;
			}
			BeamComponent controllerBeamComp;
			if (controller != null && base.TryComp<BeamComponent>(controller, ref controllerBeamComp))
			{
				controllerBeamComp.HitTargets.Add(user);
				controllerBeamComp.HitTargets.Add(target);
			}
			Vector2 distanceCorrection = calculatedDistance - calculatedDistance.Normalized;
			this.CreateBeam(bodyPrototype, userAngle, calculatedDistance, beamStartPos, distanceCorrection, controller, bodyState, shader);
			CreateBeamSuccessEvent ev = new CreateBeamSuccessEvent(user, target);
			base.RaiseLocalEvent<CreateBeamSuccessEvent>(user, ev, false);
		}

		// Token: 0x040017F6 RID: 6134
		[Dependency]
		private readonly FixtureSystem _fixture;

		// Token: 0x040017F7 RID: 6135
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040017F8 RID: 6136
		[Dependency]
		private readonly SharedBroadphaseSystem _broadphase;

		// Token: 0x040017F9 RID: 6137
		[Dependency]
		private readonly SharedPhysicsSystem _physics;
	}
}
