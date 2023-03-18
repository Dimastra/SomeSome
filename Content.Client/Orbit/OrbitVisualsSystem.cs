using System;
using System.Runtime.CompilerServices;
using Content.Shared.Follower.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Orbit
{
	// Token: 0x020001F6 RID: 502
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class OrbitVisualsSystem : EntitySystem
	{
		// Token: 0x06000CCD RID: 3277 RVA: 0x0004AE98 File Offset: 0x00049098
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<OrbitVisualsComponent, ComponentInit>(new ComponentEventHandler<OrbitVisualsComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<OrbitVisualsComponent, ComponentRemove>(new ComponentEventHandler<OrbitVisualsComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<OrbitVisualsComponent, AnimationCompletedEvent>(new ComponentEventHandler<OrbitVisualsComponent, AnimationCompletedEvent>(this.OnAnimationCompleted), null, null);
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x0004AEE8 File Offset: 0x000490E8
		private void OnComponentInit(EntityUid uid, OrbitVisualsComponent component, ComponentInit args)
		{
			component.OrbitDistance = this._robustRandom.NextFloat(0.75f * component.OrbitDistance, 1.25f * component.OrbitDistance);
			component.OrbitLength = this._robustRandom.NextFloat(0.5f * component.OrbitLength, 1.5f * component.OrbitLength);
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.EnableDirectionOverride = true;
				spriteComponent.DirectionOverride = 0;
			}
			AnimationPlayerComponent animationPlayerComponent = this.EntityManager.EnsureComponent<AnimationPlayerComponent>(uid);
			if (animationPlayerComponent.HasRunningAnimation(this._orbitAnimationKey))
			{
				return;
			}
			if (animationPlayerComponent.HasRunningAnimation(this._orbitStopKey))
			{
				animationPlayerComponent.Stop(this._orbitStopKey);
			}
			animationPlayerComponent.Play(this.GetOrbitAnimation(component), this._orbitAnimationKey);
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x0004AFAC File Offset: 0x000491AC
		private void OnComponentRemove(EntityUid uid, OrbitVisualsComponent component, ComponentRemove args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			spriteComponent.EnableDirectionOverride = false;
			AnimationPlayerComponent animationPlayerComponent = this.EntityManager.EnsureComponent<AnimationPlayerComponent>(uid);
			if (animationPlayerComponent.HasRunningAnimation(this._orbitAnimationKey))
			{
				animationPlayerComponent.Stop(this._orbitAnimationKey);
			}
			if (!animationPlayerComponent.HasRunningAnimation(this._orbitStopKey))
			{
				animationPlayerComponent.Play(this.GetStopAnimation(component, spriteComponent), this._orbitStopKey);
			}
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x0004B018 File Offset: 0x00049218
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			foreach (ValueTuple<OrbitVisualsComponent, SpriteComponent> valueTuple in this.EntityManager.EntityQuery<OrbitVisualsComponent, SpriteComponent>(false))
			{
				OrbitVisualsComponent item = valueTuple.Item1;
				SpriteComponent item2 = valueTuple.Item2;
				Angle rotation;
				rotation..ctor(6.283185307179586 * (double)item.Orbit);
				Vector2 vector = new Vector2(item.OrbitDistance, 0f);
				Vector2 offset = rotation.RotateVec(ref vector);
				item2.Rotation = rotation;
				item2.Offset = offset;
			}
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x0004B0B8 File Offset: 0x000492B8
		private void OnAnimationCompleted(EntityUid uid, OrbitVisualsComponent component, AnimationCompletedEvent args)
		{
			AnimationPlayerComponent animationPlayerComponent;
			if (args.Key == this._orbitAnimationKey && this.EntityManager.TryGetComponent<AnimationPlayerComponent>(uid, ref animationPlayerComponent))
			{
				animationPlayerComponent.Play(this.GetOrbitAnimation(component), this._orbitAnimationKey);
			}
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x0004B0FC File Offset: 0x000492FC
		private Animation GetOrbitAnimation(OrbitVisualsComponent component)
		{
			float orbitLength = component.OrbitLength;
			return new Animation
			{
				Length = TimeSpan.FromSeconds((double)orbitLength),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(OrbitVisualsComponent),
						Property = "Orbit",
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(0f, 0f),
							new AnimationTrackProperty.KeyFrame(1f, orbitLength)
						},
						InterpolationMode = 0
					}
				}
			};
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x0004B190 File Offset: 0x00049390
		private Animation GetStopAnimation(OrbitVisualsComponent component, SpriteComponent sprite)
		{
			float orbitStopLength = component.OrbitStopLength;
			return new Animation
			{
				Length = TimeSpan.FromSeconds((double)orbitStopLength),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Offset",
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(sprite.Offset, 0f),
							new AnimationTrackProperty.KeyFrame(Vector2.Zero, orbitStopLength)
						},
						InterpolationMode = 0
					},
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Rotation",
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(sprite.Rotation.Reduced(), 0f),
							new AnimationTrackProperty.KeyFrame(Angle.Zero, orbitStopLength)
						},
						InterpolationMode = 0
					}
				}
			};
		}

		// Token: 0x04000695 RID: 1685
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000696 RID: 1686
		private readonly string _orbitAnimationKey = "orbiting";

		// Token: 0x04000697 RID: 1687
		private readonly string _orbitStopKey = "orbiting_stop";
	}
}
