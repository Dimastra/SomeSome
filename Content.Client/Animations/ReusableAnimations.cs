using System;
using System.Runtime.CompilerServices;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Animations
{
	// Token: 0x02000471 RID: 1137
	public static class ReusableAnimations
	{
		// Token: 0x06001C30 RID: 7216 RVA: 0x000A390C File Offset: 0x000A1B0C
		[NullableContext(2)]
		public static void AnimateEntityPickup(EntityUid entity, EntityCoordinates initialPosition, Vector2 finalPosition, IEntityManager entMan = null)
		{
			IoCManager.Resolve<IEntityManager>(ref entMan);
			if (entMan.Deleted(entity) || !initialPosition.IsValid(entMan))
			{
				return;
			}
			EntityUid animatableClone = entMan.SpawnEntity("clientsideclone", initialPosition);
			string entityName = entMan.GetComponent<MetaDataComponent>(entity).EntityName;
			entMan.GetComponent<MetaDataComponent>(animatableClone).EntityName = entityName;
			SpriteComponent spriteComponent;
			if (!entMan.TryGetComponent<SpriteComponent>(entity, ref spriteComponent))
			{
				Logger.Error("Entity ({0}) couldn't be animated for pickup since it doesn't have a {1}!", new object[]
				{
					entMan.GetComponent<MetaDataComponent>(entity).EntityName,
					"SpriteComponent"
				});
				return;
			}
			SpriteComponent component = entMan.GetComponent<SpriteComponent>(animatableClone);
			component.CopyFrom(spriteComponent);
			component.Visible = true;
			AnimationPlayerComponent component2 = entMan.GetComponent<AnimationPlayerComponent>(animatableClone);
			component2.AnimationCompleted += delegate(string _)
			{
				entMan.DeleteEntity(animatableClone);
			};
			component2.Play(new Animation
			{
				Length = TimeSpan.FromMilliseconds(125.0),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(TransformComponent),
						Property = "LocalPosition",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(initialPosition.Position, 0f),
							new AnimationTrackProperty.KeyFrame(finalPosition, 0.125f)
						}
					}
				}
			}, "fancy_pickup_anim");
		}
	}
}
