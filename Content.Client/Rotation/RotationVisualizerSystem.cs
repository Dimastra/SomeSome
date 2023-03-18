using System;
using System.Runtime.CompilerServices;
using Content.Shared.Rotation;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Rotation
{
	// Token: 0x02000165 RID: 357
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class RotationVisualizerSystem : VisualizerSystem<RotationVisualsComponent>
	{
		// Token: 0x06000953 RID: 2387 RVA: 0x000369D7 File Offset: 0x00034BD7
		[NullableContext(2)]
		public void SetHorizontalAngle(EntityUid uid, Angle angle, RotationVisualsComponent component = null)
		{
			if (!base.Resolve<RotationVisualsComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.HorizontalRotation.Equals(angle))
			{
				return;
			}
			component.HorizontalRotation = angle;
			base.Dirty(component, null);
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00036A04 File Offset: 0x00034C04
		protected override void OnAppearanceChange(EntityUid uid, RotationVisualsComponent component, ref AppearanceChangeEvent args)
		{
			base.OnAppearanceChange(uid, component, ref args);
			RotationState rotationState;
			if (!this.AppearanceSystem.TryGetData<RotationState>(uid, RotationVisuals.RotationState, ref rotationState, args.Component) || args.Sprite == null)
			{
				return;
			}
			if (rotationState == RotationState.Vertical)
			{
				this.AnimateSpriteRotation(uid, args.Sprite, component.VerticalRotation, component.AnimationTime);
				return;
			}
			if (rotationState != RotationState.Horizontal)
			{
				return;
			}
			this.AnimateSpriteRotation(uid, args.Sprite, component.HorizontalRotation, component.AnimationTime);
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00036A7C File Offset: 0x00034C7C
		public void AnimateSpriteRotation(EntityUid uid, SpriteComponent spriteComp, Angle rotation, float animationTime)
		{
			if (spriteComp.Rotation.Equals(rotation))
			{
				return;
			}
			AnimationPlayerComponent animationPlayerComponent = base.EnsureComp<AnimationPlayerComponent>(uid);
			if (this.AnimationSystem.HasRunningAnimation(animationPlayerComponent, "rotate"))
			{
				this.AnimationSystem.Stop(animationPlayerComponent, "rotate");
			}
			Animation animation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)animationTime),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Rotation",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(spriteComp.Rotation, 0f),
							new AnimationTrackProperty.KeyFrame(rotation, animationTime)
						}
					}
				}
			};
			this.AnimationSystem.Play(animationPlayerComponent, animation, "rotate");
		}
	}
}
