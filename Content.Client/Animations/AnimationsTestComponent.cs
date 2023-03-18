using System;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Animations
{
	// Token: 0x02000470 RID: 1136
	[RegisterComponent]
	public sealed class AnimationsTestComponent : Component
	{
		// Token: 0x06001C2E RID: 7214 RVA: 0x000A37B8 File Offset: 0x000A19B8
		protected override void Initialize()
		{
			base.Initialize();
			IoCManager.Resolve<IEntityManager>().GetComponent<AnimationPlayerComponent>(base.Owner).Play(new Animation
			{
				Length = TimeSpan.FromSeconds(20.0),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(TransformComponent),
						Property = "LocalRotation",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(Angle.Zero, 0f),
							new AnimationTrackProperty.KeyFrame(Angle.FromDegrees(1440.0), 20f)
						}
					},
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "layer/0/texture",
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame("Objects/toolbox_r.png", 0f),
							new AnimationTrackProperty.KeyFrame("Objects/Toolbox_b.png", 5f),
							new AnimationTrackProperty.KeyFrame("Objects/Toolbox_y.png", 5f),
							new AnimationTrackProperty.KeyFrame("Objects/toolbox_r.png", 5f)
						}
					}
				}
			}, "yes");
		}
	}
}
