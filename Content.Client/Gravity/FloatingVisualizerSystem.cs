using System;
using System.Runtime.CompilerServices;
using Content.Shared.Gravity;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Gravity
{
	// Token: 0x020002F6 RID: 758
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FloatingVisualizerSystem : SharedFloatingVisualizerSystem
	{
		// Token: 0x06001310 RID: 4880 RVA: 0x00071724 File Offset: 0x0006F924
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FloatingVisualsComponent, AnimationCompletedEvent>(new ComponentEventHandler<FloatingVisualsComponent, AnimationCompletedEvent>(this.OnAnimationCompleted), null, null);
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x00071740 File Offset: 0x0006F940
		public override void FloatAnimation(EntityUid uid, Vector2 offset, string animationKey, float animationTime, bool stop = false)
		{
			if (stop)
			{
				this.AnimationSystem.Stop(uid, animationKey);
				return;
			}
			Animation animation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)(animationTime * 2f)),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Offset",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(Vector2.Zero, 0f),
							new AnimationTrackProperty.KeyFrame(offset, animationTime),
							new AnimationTrackProperty.KeyFrame(Vector2.Zero, animationTime)
						}
					}
				}
			};
			if (!this.AnimationSystem.HasRunningAnimation(uid, animationKey))
			{
				this.AnimationSystem.Play(uid, animation, animationKey);
			}
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x0007181B File Offset: 0x0006FA1B
		private void OnAnimationCompleted(EntityUid uid, FloatingVisualsComponent component, AnimationCompletedEvent args)
		{
			if (args.Key != component.AnimationKey)
			{
				return;
			}
			this.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime, !component.CanFloat);
		}

		// Token: 0x0400098B RID: 2443
		[Dependency]
		private readonly AnimationPlayerSystem AnimationSystem;
	}
}
