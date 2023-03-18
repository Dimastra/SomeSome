using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Beam;
using Content.Server.Beam.Components;
using Content.Server.Lightning.Components;
using Content.Shared.Lightning;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;

namespace Content.Server.Lightning
{
	// Token: 0x02000407 RID: 1031
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LightningSystem : SharedLightningSystem
	{
		// Token: 0x060014DA RID: 5338 RVA: 0x0006D1F7 File Offset: 0x0006B3F7
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LightningComponent, StartCollideEvent>(new ComponentEventRefHandler<LightningComponent, StartCollideEvent>(this.OnCollide), null, null);
			base.SubscribeLocalEvent<LightningComponent, ComponentRemove>(new ComponentEventHandler<LightningComponent, ComponentRemove>(this.OnRemove), null, null);
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0006D228 File Offset: 0x0006B428
		private void OnRemove(EntityUid uid, LightningComponent component, ComponentRemove args)
		{
			BeamComponent lightningBeam;
			BeamComponent beamController;
			if (!base.TryComp<BeamComponent>(uid, ref lightningBeam) || !base.TryComp<BeamComponent>(lightningBeam.VirtualBeamController, ref beamController))
			{
				return;
			}
			beamController.CreatedBeams.Remove(uid);
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x0006D260 File Offset: 0x0006B460
		private void OnCollide(EntityUid uid, LightningComponent component, ref StartCollideEvent args)
		{
			BeamComponent lightningBeam;
			BeamComponent beamController;
			if (!base.TryComp<BeamComponent>(uid, ref lightningBeam) || !base.TryComp<BeamComponent>(lightningBeam.VirtualBeamController, ref beamController))
			{
				return;
			}
			if (!component.CanArc || beamController.CreatedBeams.Count >= component.MaxTotalArcs)
			{
				return;
			}
			this.Arc(component, args.OtherFixture.Body.Owner, lightningBeam.VirtualBeamController.Value);
			string spriteState = base.LightningRandomizer();
			component.ArcTargets.Add(args.OtherFixture.Body.Owner);
			component.ArcTargets.Add(component.ArcTarget);
			this._beam.TryCreateBeam(args.OtherFixture.Body.Owner, component.ArcTarget, component.LightningPrototype, spriteState, "unshaded", new EntityUid?(lightningBeam.VirtualBeamController.Value));
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x0006D344 File Offset: 0x0006B544
		public void ShootLightning(EntityUid user, EntityUid target, string lightningPrototype = "Lightning")
		{
			string spriteState = base.LightningRandomizer();
			this._beam.TryCreateBeam(user, target, lightningPrototype, spriteState, "unshaded", null);
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x0006D378 File Offset: 0x0006B578
		private void Arc(LightningComponent component, EntityUid target, EntityUid controllerEntity)
		{
			BeamComponent controller;
			if (!base.TryComp<BeamComponent>(controllerEntity, ref controller))
			{
				return;
			}
			TransformComponent targetXForm = base.Transform(target);
			int directions = Enum.GetValues<Direction>().Length;
			EntityQuery<LightningComponent> lightningQuery = base.GetEntityQuery<LightningComponent>();
			EntityQuery<BeamComponent> beamQuery = base.GetEntityQuery<BeamComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			Dictionary<Direction, EntityUid> arcDirections = new Dictionary<Direction, EntityUid>();
			for (int i = 0; i < directions; i++)
			{
				Direction direction = (sbyte)i;
				ValueTuple<Vector2, Angle> worldPositionRotation = targetXForm.GetWorldPositionRotation(xformQuery);
				Vector2 targetPos = worldPositionRotation.Item1;
				Angle targetRot = worldPositionRotation.Item2;
				CollisionRay ray;
				ray..ctor(targetPos, (DirectionExtensions.ToAngle(direction) + targetRot).ToVec(), component.CollisionMask);
				List<RayCastResults> list = this._physics.IntersectRay(targetXForm.MapID, ray, component.MaxLength, new EntityUid?(target), false).ToList<RayCastResults>();
				RayCastResults? closestResult = null;
				foreach (RayCastResults result in list)
				{
					if (!lightningQuery.HasComponent(result.HitEntity) && !beamQuery.HasComponent(result.HitEntity) && !component.ArcTargets.Contains(result.HitEntity) && !controller.HitTargets.Contains(result.HitEntity) && !(controller.BeamShooter == result.HitEntity))
					{
						closestResult = new RayCastResults?(result);
						break;
					}
				}
				if (closestResult != null)
				{
					arcDirections.Add(direction, closestResult.Value.HitEntity);
				}
			}
			Direction randomDirection = (sbyte)this._random.Next(0, 7);
			if (arcDirections.ContainsKey(randomDirection))
			{
				component.ArcTarget = arcDirections.GetValueOrDefault(randomDirection);
				arcDirections.Clear();
			}
		}

		// Token: 0x04000CFB RID: 3323
		[Dependency]
		private readonly PhysicsSystem _physics;

		// Token: 0x04000CFC RID: 3324
		[Dependency]
		private readonly BeamSystem _beam;

		// Token: 0x04000CFD RID: 3325
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
