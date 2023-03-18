using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Pointing.Components;
using Content.Shared.Pointing.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Pointing.EntitySystems
{
	// Token: 0x020002CC RID: 716
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class RoguePointingSystem : EntitySystem
	{
		// Token: 0x06000E71 RID: 3697 RVA: 0x00049890 File Offset: 0x00047A90
		[NullableContext(2)]
		private EntityUid? RandomNearbyPlayer(EntityUid uid, RoguePointingArrowComponent component = null, TransformComponent transform = null)
		{
			if (!base.Resolve<RoguePointingArrowComponent, TransformComponent>(uid, ref component, ref transform, true))
			{
				return null;
			}
			List<PointingArrowAngeringComponent> targets = base.EntityQuery<PointingArrowAngeringComponent>(false).ToList<PointingArrowAngeringComponent>();
			if (targets.Count == 0)
			{
				return null;
			}
			PointingArrowAngeringComponent pointingArrowAngeringComponent = RandomExtensions.Pick<PointingArrowAngeringComponent>(this._random, targets);
			pointingArrowAngeringComponent.RemainingAnger--;
			if (pointingArrowAngeringComponent.RemainingAnger <= 0)
			{
				base.RemComp<PointingArrowAngeringComponent>(uid);
			}
			return new EntityUid?(pointingArrowAngeringComponent.Owner);
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x0004990C File Offset: 0x00047B0C
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, RoguePointingArrowComponent component = null, TransformComponent transform = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<RoguePointingArrowComponent, TransformComponent, AppearanceComponent>(uid, ref component, ref transform, ref appearance, true) || component.Chasing == null)
			{
				return;
			}
			this._appearance.SetData(uid, RoguePointingArrowVisuals.Rotation, transform.LocalRotation.Degrees, appearance);
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x0004995E File Offset: 0x00047B5E
		[NullableContext(2)]
		public void SetTarget(EntityUid arrow, EntityUid target, RoguePointingArrowComponent component = null)
		{
			if (!base.Resolve<RoguePointingArrowComponent>(arrow, ref component, true))
			{
				throw new ArgumentException("Input was not a rogue pointing arrow!", "arrow");
			}
			component.Chasing = new EntityUid?(target);
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x00049988 File Offset: 0x00047B88
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<RoguePointingArrowComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<RoguePointingArrowComponent, TransformComponent>(false))
			{
				RoguePointingArrowComponent component = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				EntityUid uid = component.Owner;
				RoguePointingArrowComponent roguePointingArrowComponent = component;
				EntityUid? chasing2 = roguePointingArrowComponent.Chasing;
				if (chasing2 == null)
				{
					roguePointingArrowComponent.Chasing = this.RandomNearbyPlayer(uid, component, transform);
				}
				chasing2 = component.Chasing;
				if (chasing2 != null)
				{
					EntityUid chasing = chasing2.GetValueOrDefault();
					if (chasing.Valid && !base.Deleted(chasing, null))
					{
						component.TurningDelay -= frameTime;
						if (component.TurningDelay > 0f)
						{
							Angle newAngle = Angle.FromDegrees(DirectionExtensions.ToAngle(this.EntityManager.GetComponent<TransformComponent>(chasing).WorldPosition - transform.WorldPosition).Degrees + 90.0);
							transform.WorldRotation = newAngle;
							this.UpdateAppearance(uid, component, transform, null);
							continue;
						}
						transform.WorldRotation += Angle.FromDegrees(20.0);
						this.UpdateAppearance(uid, component, transform, null);
						Vector2 toChased = this.EntityManager.GetComponent<TransformComponent>(chasing).WorldPosition - transform.WorldPosition;
						transform.WorldPosition += toChased * frameTime * component.ChasingSpeed;
						component.ChasingTime -= frameTime;
						if (component.ChasingTime <= 0f)
						{
							this._explosion.QueueExplosion(uid, "Default", 50f, 3f, 10f, 1f, int.MaxValue, true, false, null, true);
							this.EntityManager.QueueDeleteEntity(uid);
							continue;
						}
						continue;
					}
				}
				this.EntityManager.QueueDeleteEntity(uid);
			}
		}

		// Token: 0x04000888 RID: 2184
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000889 RID: 2185
		[Dependency]
		private readonly ExplosionSystem _explosion;

		// Token: 0x0400088A RID: 2186
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
