using System;
using System.Runtime.CompilerServices;
using Content.Shared.Jittering;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Jittering
{
	// Token: 0x02000299 RID: 665
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class JitteringSystem : SharedJitteringSystem
	{
		// Token: 0x060010DF RID: 4319 RVA: 0x00064F20 File Offset: 0x00063120
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<JitteringComponent, ComponentStartup>(new ComponentEventHandler<JitteringComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<JitteringComponent, ComponentShutdown>(new ComponentEventHandler<JitteringComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<JitteringComponent, AnimationCompletedEvent>(new ComponentEventHandler<JitteringComponent, AnimationCompletedEvent>(this.OnAnimationCompleted), null, null);
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x00064F70 File Offset: 0x00063170
		private void OnStartup(EntityUid uid, JitteringComponent jittering, ComponentStartup args)
		{
			SpriteComponent sprite;
			if (!this.EntityManager.TryGetComponent<SpriteComponent>(uid, ref sprite))
			{
				return;
			}
			this.EntityManager.EnsureComponent<AnimationPlayerComponent>(uid).Play(this.GetAnimation(jittering, sprite), this._jitterAnimationKey);
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00064FB0 File Offset: 0x000631B0
		private void OnShutdown(EntityUid uid, JitteringComponent jittering, ComponentShutdown args)
		{
			AnimationPlayerComponent animationPlayerComponent;
			if (this.EntityManager.TryGetComponent<AnimationPlayerComponent>(uid, ref animationPlayerComponent))
			{
				animationPlayerComponent.Stop(this._jitterAnimationKey);
			}
			SpriteComponent spriteComponent;
			if (this.EntityManager.TryGetComponent<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.Offset = Vector2.Zero;
			}
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x00064FF4 File Offset: 0x000631F4
		private void OnAnimationCompleted(EntityUid uid, JitteringComponent jittering, AnimationCompletedEvent args)
		{
			if (args.Key != this._jitterAnimationKey)
			{
				return;
			}
			AnimationPlayerComponent animationPlayerComponent;
			SpriteComponent sprite;
			if (this.EntityManager.TryGetComponent<AnimationPlayerComponent>(uid, ref animationPlayerComponent) && this.EntityManager.TryGetComponent<SpriteComponent>(uid, ref sprite))
			{
				animationPlayerComponent.Play(this.GetAnimation(jittering, sprite), this._jitterAnimationKey);
			}
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x0006504C File Offset: 0x0006324C
		private Animation GetAnimation(JitteringComponent jittering, SpriteComponent sprite)
		{
			float num = MathF.Min(4f, jittering.Amplitude / 100f + 1f) / 10f;
			Vector2 vector;
			vector..ctor(this._random.NextFloat(num / 4f, num), this._random.NextFloat(num / 4f, num / 3f));
			vector.X *= RandomExtensions.Pick<float>(this._random, this._sign);
			vector.Y *= RandomExtensions.Pick<float>(this._random, this._sign);
			if (Math.Sign(vector.X) == Math.Sign(jittering.LastJitter.X) || Math.Sign(vector.Y) == Math.Sign(jittering.LastJitter.Y))
			{
				if (RandomExtensions.Prob(this._random, 0.5f))
				{
					vector.X *= -1f;
				}
				else
				{
					vector.Y *= -1f;
				}
			}
			float num2 = Math.Min(1f / jittering.Frequency, 2f);
			jittering.LastJitter = vector;
			return new Animation
			{
				Length = TimeSpan.FromSeconds((double)num2),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Offset",
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(sprite.Offset, 0f),
							new AnimationTrackProperty.KeyFrame(vector, num2)
						}
					}
				}
			};
		}

		// Token: 0x04000849 RID: 2121
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400084A RID: 2122
		private readonly float[] _sign = new float[]
		{
			-1f,
			1f
		};

		// Token: 0x0400084B RID: 2123
		private readonly string _jitterAnimationKey = "jittering";
	}
}
