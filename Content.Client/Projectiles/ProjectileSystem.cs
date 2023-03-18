using System;
using System.Runtime.CompilerServices;
using Content.Shared.Projectiles;
using Content.Shared.Spawners.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Client.Projectiles
{
	// Token: 0x0200017F RID: 383
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ProjectileSystem : SharedProjectileSystem
	{
		// Token: 0x060009F3 RID: 2547 RVA: 0x00039F28 File Offset: 0x00038128
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ProjectileComponent, ComponentHandleState>(new ComponentEventRefHandler<ProjectileComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeNetworkEvent<SharedProjectileSystem.ImpactEffectEvent>(new EntityEventHandler<SharedProjectileSystem.ImpactEffectEvent>(this.OnProjectileImpact), null, null);
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x00039F58 File Offset: 0x00038158
		private void OnProjectileImpact(SharedProjectileSystem.ImpactEffectEvent ev)
		{
			if (base.Deleted(ev.Coordinates.EntityId, null))
			{
				return;
			}
			EntityUid entityUid = base.Spawn(ev.Prototype, ev.Coordinates);
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(entityUid, ref spriteComponent))
			{
				spriteComponent[EffectLayers.Unshaded].AutoAnimated = false;
				int num;
				spriteComponent.LayerMapTryGet(EffectLayers.Unshaded, ref num, false);
				RSI.StateId stateId = spriteComponent.LayerGetState(num);
				float num2 = 0.5f;
				TimedDespawnComponent timedDespawnComponent;
				if (base.TryComp<TimedDespawnComponent>(entityUid, ref timedDespawnComponent))
				{
					num2 = timedDespawnComponent.Lifetime;
				}
				Animation animation = new Animation
				{
					Length = TimeSpan.FromSeconds((double)num2),
					AnimationTracks = 
					{
						new AnimationTrackSpriteFlick
						{
							LayerKey = EffectLayers.Unshaded,
							KeyFrames = 
							{
								new AnimationTrackSpriteFlick.KeyFrame(stateId.Name, 0f)
							}
						}
					}
				};
				this._player.Play(entityUid, animation, "impact-effect");
			}
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x0003A044 File Offset: 0x00038244
		private void OnHandleState(EntityUid uid, ProjectileComponent component, ref ComponentHandleState args)
		{
			SharedProjectileSystem.ProjectileComponentState projectileComponentState = args.Current as SharedProjectileSystem.ProjectileComponentState;
			if (projectileComponentState == null)
			{
				return;
			}
			component.Shooter = projectileComponentState.Shooter;
			component.IgnoreShooter = projectileComponentState.IgnoreShooter;
		}

		// Token: 0x040004F1 RID: 1265
		[Dependency]
		private readonly AnimationPlayerSystem _player;
	}
}
