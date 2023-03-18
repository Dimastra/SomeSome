using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.CombatMode;
using Content.Client.Gameplay;
using Content.Client.Hands;
using Content.Client.Weapons.Melee.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Weapons;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Weapons.Melee
{
	// Token: 0x0200003D RID: 61
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeleeWeaponSystem : SharedMeleeWeaponSystem
	{
		// Token: 0x06000106 RID: 262 RVA: 0x0000912C File Offset: 0x0000732C
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeEffect();
			this._overlayManager.AddOverlay(new MeleeWindupOverlay(this.EntityManager, this._timing, this._player, this._protoManager));
			base.SubscribeAllEvent<DamageEffectEvent>(new EntityEventHandler<DamageEffectEvent>(this.OnDamageEffect), null, null);
			base.SubscribeNetworkEvent<MeleeLungeEvent>(new EntityEventHandler<MeleeLungeEvent>(this.OnMeleeLunge), null, null);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00009196 File Offset: 0x00007396
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlayManager.RemoveOverlay<MeleeWindupOverlay>();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x000091AC File Offset: 0x000073AC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			LocalPlayer localPlayer = this._player.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			EntityUid value = entityUid.Value;
			MeleeWeaponComponent weapon = base.GetWeapon(value);
			if (weapon == null)
			{
				return;
			}
			if (!this.CombatMode.IsInCombatMode(new EntityUid?(value), null) || !this.Blocker.CanAttack(value, null))
			{
				weapon.Attacking = false;
				if (weapon.WindUpStart != null)
				{
					this.EntityManager.RaisePredictiveEvent<StopHeavyAttackEvent>(new StopHeavyAttackEvent(weapon.Owner));
				}
				return;
			}
			BoundKeyState state = this._inputSystem.CmdStates.GetState(EngineKeyFunctions.Use);
			int state2 = this._inputSystem.CmdStates.GetState(EngineKeyFunctions.UseSecondary);
			TimeSpan curTime = this.Timing.CurTime;
			if (state2 == 1 && weapon.CanHeavyAttack)
			{
				if (weapon.Attacking)
				{
					return;
				}
				if (weapon.Owner == value)
				{
					EntityUid? target = null;
					MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(this._inputManager.MouseScreenPosition);
					MapGridComponent mapGridComponent;
					EntityCoordinates coordinates;
					if (this.MapManager.TryFindGridAt(mapCoordinates, ref mapGridComponent))
					{
						coordinates = EntityCoordinates.FromMap(mapGridComponent.Owner, mapCoordinates, this.EntityManager);
					}
					else
					{
						coordinates = EntityCoordinates.FromMap(this.MapManager.GetMapEntityId(mapCoordinates.MapId), mapCoordinates, this.EntityManager);
					}
					GameplayStateBase gameplayStateBase = this._stateManager.CurrentState as GameplayStateBase;
					if (gameplayStateBase != null)
					{
						target = gameplayStateBase.GetClickedEntity(mapCoordinates);
					}
					this.EntityManager.RaisePredictiveEvent<DisarmAttackEvent>(new DisarmAttackEvent(target, coordinates));
					return;
				}
				if (weapon.WindUpStart == null)
				{
					this.EntityManager.RaisePredictiveEvent<StartHeavyAttackEvent>(new StartHeavyAttackEvent(weapon.Owner));
					weapon.WindUpStart = new TimeSpan?(curTime);
				}
				if (state == 1)
				{
					MapCoordinates mapCoordinates2 = this._eyeManager.ScreenToMap(this._inputManager.MouseScreenPosition);
					MapGridComponent mapGridComponent2;
					EntityCoordinates coordinates2;
					if (this.MapManager.TryFindGridAt(mapCoordinates2, ref mapGridComponent2))
					{
						coordinates2 = EntityCoordinates.FromMap(mapGridComponent2.Owner, mapCoordinates2, this.EntityManager);
					}
					else
					{
						coordinates2 = EntityCoordinates.FromMap(this.MapManager.GetMapEntityId(mapCoordinates2.MapId), mapCoordinates2, this.EntityManager);
					}
					this.EntityManager.RaisePredictiveEvent<HeavyAttackEvent>(new HeavyAttackEvent(weapon.Owner, coordinates2));
				}
				return;
			}
			else
			{
				if (weapon.WindUpStart != null)
				{
					this.EntityManager.RaisePredictiveEvent<StopHeavyAttackEvent>(new StopHeavyAttackEvent(weapon.Owner));
				}
				if (state != 1)
				{
					if (weapon.Attacking)
					{
						base.RaisePredictiveEvent<StopAttackEvent>(new StopAttackEvent(weapon.Owner));
					}
					return;
				}
				if (weapon.Attacking || weapon.NextAttack > this.Timing.CurTime)
				{
					return;
				}
				MapCoordinates mapCoordinates3 = this._eyeManager.ScreenToMap(this._inputManager.MouseScreenPosition);
				MapCoordinates mapPosition = base.Transform(value).MapPosition;
				if (mapCoordinates3.MapId != mapPosition.MapId || (mapPosition.Position - mapCoordinates3.Position).Length > weapon.Range)
				{
					return;
				}
				MapGridComponent mapGridComponent3;
				EntityCoordinates coordinates3;
				if (this.MapManager.TryFindGridAt(mapCoordinates3, ref mapGridComponent3))
				{
					coordinates3 = EntityCoordinates.FromMap(mapGridComponent3.Owner, mapCoordinates3, this.EntityManager);
				}
				else
				{
					coordinates3 = EntityCoordinates.FromMap(this.MapManager.GetMapEntityId(mapCoordinates3.MapId), mapCoordinates3, this.EntityManager);
				}
				EntityUid? target2 = null;
				GameplayStateBase gameplayStateBase2 = this._stateManager.CurrentState as GameplayStateBase;
				if (gameplayStateBase2 != null)
				{
					target2 = gameplayStateBase2.GetClickedEntity(mapCoordinates3);
				}
				base.RaisePredictiveEvent<LightAttackEvent>(new LightAttackEvent(target2, weapon.Owner, coordinates3));
				return;
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00009564 File Offset: 0x00007764
		[NullableContext(2)]
		protected override bool InRange(EntityUid user, EntityUid target, float range, ICommonSession session)
		{
			TransformComponent transformComponent = base.Transform(target);
			EntityCoordinates coordinates = transformComponent.Coordinates;
			Angle localRotation = transformComponent.LocalRotation;
			return this.Interaction.InRangeUnobstructed(user, target, coordinates, localRotation, range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000959C File Offset: 0x0000779C
		protected override void DoDamageEffect(List<EntityUid> targets, EntityUid? user, TransformComponent targetXform)
		{
			if (this._timing.IsFirstTimePredicted)
			{
				base.RaiseLocalEvent<DamageEffectEvent>(new DamageEffectEvent(Color.Red, targets));
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000095BC File Offset: 0x000077BC
		protected override bool DoDisarm(EntityUid user, DisarmAttackEvent ev, EntityUid meleeUid, MeleeWeaponComponent component, [Nullable(2)] ICommonSession session)
		{
			if (!base.DoDisarm(user, ev, meleeUid, component, session))
			{
				return false;
			}
			CombatModeComponent combatModeComponent;
			if (base.TryComp<CombatModeComponent>(user, ref combatModeComponent))
			{
				bool? canDisarm = combatModeComponent.CanDisarm;
				bool flag = true;
				if (canDisarm.GetValueOrDefault() == flag & canDisarm != null)
				{
					if (base.HasComp<HandsComponent>(ev.Target.Value))
					{
						return true;
					}
					StatusEffectsComponent statusEffectsComponent;
					if (base.TryComp<StatusEffectsComponent>(ev.Target.Value, ref statusEffectsComponent) && statusEffectsComponent.AllowedEffects.Contains("KnockedDown"))
					{
						return true;
					}
					if (this.Timing.IsFirstTimePredicted && base.HasComp<MobStateComponent>(ev.Target.Value))
					{
						this.PopupSystem.PopupEntity(Loc.GetString("disarm-action-disarmable", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("targetName", ev.Target.Value)
						}), ev.Target.Value, PopupType.Small);
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000096AE File Offset: 0x000078AE
		protected override void Popup(string message, EntityUid? uid, EntityUid? user)
		{
			if (!this.Timing.IsFirstTimePredicted || uid == null)
			{
				return;
			}
			this.PopupSystem.PopupEntity(message, uid.Value, PopupType.Small);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000096DB File Offset: 0x000078DB
		private void OnMeleeLunge(MeleeLungeEvent ev)
		{
			if (base.Exists(ev.Entity))
			{
				this.DoLunge(ev.Entity, ev.Angle, ev.LocalPos, ev.Animation);
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00009709 File Offset: 0x00007909
		private void InitializeEffect()
		{
			base.SubscribeLocalEvent<DamageEffectComponent, AnimationCompletedEvent>(new ComponentEventHandler<DamageEffectComponent, AnimationCompletedEvent>(this.OnEffectAnimation), null, null);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00009720 File Offset: 0x00007920
		private void OnEffectAnimation(EntityUid uid, DamageEffectComponent component, AnimationCompletedEvent args)
		{
			if (args.Key != "damage-effect")
			{
				return;
			}
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.Color = component.Color;
			}
			base.RemCompDeferred<DamageEffectComponent>(uid);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00009760 File Offset: 0x00007960
		[NullableContext(2)]
		private Animation GetDamageAnimation(EntityUid uid, Color color, SpriteComponent sprite = null)
		{
			if (!base.Resolve<SpriteComponent>(uid, ref sprite, false))
			{
				return null;
			}
			return new Animation
			{
				Length = TimeSpan.FromSeconds(0.30000001192092896),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Color",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(color, 0f),
							new AnimationTrackProperty.KeyFrame(sprite.Color, 0.3f)
						}
					}
				}
			};
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00009804 File Offset: 0x00007A04
		private void OnDamageEffect(DamageEffectEvent ev)
		{
			Color color = ev.Color;
			foreach (EntityUid entityUid in ev.Entities)
			{
				if (!base.Deleted(entityUid, null))
				{
					AnimationPlayerComponent animationPlayerComponent = base.EnsureComp<AnimationPlayerComponent>(entityUid);
					animationPlayerComponent.NetSyncEnabled = false;
					if (this._animation.HasRunningAnimation(entityUid, animationPlayerComponent, "damage-effect"))
					{
						this._animation.Stop(entityUid, animationPlayerComponent, "damage-effect");
					}
					SpriteComponent spriteComponent;
					if (base.TryComp<SpriteComponent>(entityUid, ref spriteComponent))
					{
						DamageEffectComponent damageEffectComponent;
						if (base.TryComp<DamageEffectComponent>(entityUid, ref damageEffectComponent))
						{
							spriteComponent.Color = damageEffectComponent.Color;
						}
						Animation damageAnimation = this.GetDamageAnimation(entityUid, color, spriteComponent);
						if (damageAnimation != null)
						{
							DamageEffectComponent damageEffectComponent2 = base.EnsureComp<DamageEffectComponent>(entityUid);
							damageEffectComponent2.NetSyncEnabled = false;
							damageEffectComponent2.Color = spriteComponent.Color;
							this._animation.Play(animationPlayerComponent, damageAnimation, "damage-effect");
						}
					}
				}
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00009900 File Offset: 0x00007B00
		[NullableContext(2)]
		public override void DoLunge(EntityUid user, Angle angle, Vector2 localPos, string animation)
		{
			if (!this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			Animation lungeAnimation = this.GetLungeAnimation(localPos);
			this._animation.Stop(user, "melee-lunge");
			this._animation.Play(user, lungeAnimation, "melee-lunge");
			if (localPos == Vector2.Zero || animation == null)
			{
				return;
			}
			TransformComponent transformComponent;
			if (!base.TryComp<TransformComponent>(user, ref transformComponent) || transformComponent.MapID == MapId.Nullspace)
			{
				return;
			}
			EntityUid entityUid = base.Spawn(animation, transformComponent.Coordinates);
			SpriteComponent spriteComponent;
			WeaponArcVisualsComponent weaponArcVisualsComponent;
			if (!base.TryComp<SpriteComponent>(entityUid, ref spriteComponent) || !base.TryComp<WeaponArcVisualsComponent>(entityUid, ref weaponArcVisualsComponent))
			{
				return;
			}
			spriteComponent.NoRotation = true;
			spriteComponent.Rotation = DirectionExtensions.ToWorldAngle(localPos);
			float distance = Math.Clamp(localPos.Length / 2f, 0.2f, 1f);
			switch (weaponArcVisualsComponent.Animation)
			{
			case WeaponArcAnimation.None:
			{
				Vector2 worldPosition = transformComponent.WorldPosition;
				TransformComponent transformComponent2 = base.Transform(entityUid);
				transformComponent2.AttachToGridOrMap();
				transformComponent2.WorldPosition = worldPosition + (transformComponent.WorldRotation - transformComponent.LocalRotation).RotateVec(ref localPos);
				if (weaponArcVisualsComponent.Fadeout)
				{
					this._animation.Play(entityUid, this.GetFadeAnimation(spriteComponent, 0f, 0.15f), "melee-fade");
				}
				break;
			}
			case WeaponArcAnimation.Thrust:
				this._animation.Play(entityUid, this.GetThrustAnimation(spriteComponent, distance), "melee-thrust");
				if (weaponArcVisualsComponent.Fadeout)
				{
					this._animation.Play(entityUid, this.GetFadeAnimation(spriteComponent, 0.05f, 0.15f), "melee-fade");
					return;
				}
				break;
			case WeaponArcAnimation.Slash:
				this._animation.Play(entityUid, this.GetSlashAnimation(spriteComponent, angle), "melee-slash");
				if (weaponArcVisualsComponent.Fadeout)
				{
					this._animation.Play(entityUid, this.GetFadeAnimation(spriteComponent, 0.065f, 0.114999995f), "melee-fade");
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00009AE4 File Offset: 0x00007CE4
		private Animation GetSlashAnimation(SpriteComponent sprite, Angle arc)
		{
			Angle angle = sprite.Rotation - arc / 2.0;
			Angle angle2 = sprite.Rotation + arc / 2.0;
			sprite.NoRotation = true;
			Animation animation = new Animation();
			animation.Length = TimeSpan.FromSeconds(0.11499999463558197);
			animation.AnimationTracks.Add(new AnimationTrackComponentProperty
			{
				ComponentType = typeof(SpriteComponent),
				Property = "Rotation",
				KeyFrames = 
				{
					new AnimationTrackProperty.KeyFrame(angle, 0f),
					new AnimationTrackProperty.KeyFrame(angle, 0.03f),
					new AnimationTrackProperty.KeyFrame(angle2, 0.065f)
				}
			});
			List<AnimationTrack> animationTracks = animation.AnimationTracks;
			AnimationTrackComponentProperty animationTrackComponentProperty = new AnimationTrackComponentProperty();
			animationTrackComponentProperty.ComponentType = typeof(SpriteComponent);
			animationTrackComponentProperty.Property = "Offset";
			List<AnimationTrackProperty.KeyFrame> keyFrames = animationTrackComponentProperty.KeyFrames;
			Vector2 vector = new Vector2(0f, -1f);
			keyFrames.Add(new AnimationTrackProperty.KeyFrame(angle.RotateVec(ref vector), 0f));
			List<AnimationTrackProperty.KeyFrame> keyFrames2 = animationTrackComponentProperty.KeyFrames;
			Vector2 vector2 = new Vector2(0f, -1f);
			keyFrames2.Add(new AnimationTrackProperty.KeyFrame(angle.RotateVec(ref vector2), 0.03f));
			List<AnimationTrackProperty.KeyFrame> keyFrames3 = animationTrackComponentProperty.KeyFrames;
			Vector2 vector3 = new Vector2(0f, -1f);
			keyFrames3.Add(new AnimationTrackProperty.KeyFrame(angle2.RotateVec(ref vector3), 0.065f));
			animationTracks.Add(animationTrackComponentProperty);
			return animation;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00009C94 File Offset: 0x00007E94
		private Animation GetThrustAnimation(SpriteComponent sprite, float distance)
		{
			Animation animation = new Animation();
			animation.Length = TimeSpan.FromSeconds(0.15000000596046448);
			List<AnimationTrack> animationTracks = animation.AnimationTracks;
			AnimationTrackComponentProperty animationTrackComponentProperty = new AnimationTrackComponentProperty();
			animationTrackComponentProperty.ComponentType = typeof(SpriteComponent);
			animationTrackComponentProperty.Property = "Offset";
			List<AnimationTrackProperty.KeyFrame> keyFrames = animationTrackComponentProperty.KeyFrames;
			Angle rotation = sprite.Rotation;
			Vector2 vector = new Vector2(0f, -distance / 5f);
			keyFrames.Add(new AnimationTrackProperty.KeyFrame(rotation.RotateVec(ref vector), 0f));
			List<AnimationTrackProperty.KeyFrame> keyFrames2 = animationTrackComponentProperty.KeyFrames;
			rotation = sprite.Rotation;
			Vector2 vector2 = new Vector2(0f, -distance);
			keyFrames2.Add(new AnimationTrackProperty.KeyFrame(rotation.RotateVec(ref vector2), 0.05f));
			List<AnimationTrackProperty.KeyFrame> keyFrames3 = animationTrackComponentProperty.KeyFrames;
			rotation = sprite.Rotation;
			Vector2 vector3 = new Vector2(0f, -distance);
			keyFrames3.Add(new AnimationTrackProperty.KeyFrame(rotation.RotateVec(ref vector3), 0.15f));
			animationTracks.Add(animationTrackComponentProperty);
			return animation;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00009D90 File Offset: 0x00007F90
		private Animation GetFadeAnimation(SpriteComponent sprite, float start, float end)
		{
			return new Animation
			{
				Length = TimeSpan.FromSeconds((double)end),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Color",
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(sprite.Color, start),
							new AnimationTrackProperty.KeyFrame(sprite.Color.WithAlpha(0f), end)
						}
					}
				}
			};
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00009E20 File Offset: 0x00008020
		private Animation GetLungeAnimation(Vector2 direction)
		{
			return new Animation
			{
				Length = TimeSpan.FromSeconds(0.10000000149011612),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Offset",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(direction.Normalized * 0.15f, 0f),
							new AnimationTrackProperty.KeyFrame(Vector2.Zero, 0.1f)
						}
					}
				}
			};
		}

		// Token: 0x040000B0 RID: 176
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x040000B1 RID: 177
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040000B2 RID: 178
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x040000B3 RID: 179
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x040000B4 RID: 180
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x040000B5 RID: 181
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x040000B6 RID: 182
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x040000B7 RID: 183
		[Dependency]
		private readonly AnimationPlayerSystem _animation;

		// Token: 0x040000B8 RID: 184
		[Dependency]
		private readonly InputSystem _inputSystem;

		// Token: 0x040000B9 RID: 185
		private const string MeleeLungeKey = "melee-lunge";

		// Token: 0x040000BA RID: 186
		private const float DamageAnimationLength = 0.3f;

		// Token: 0x040000BB RID: 187
		private const string AnimationKey = "melee-animation";

		// Token: 0x040000BC RID: 188
		private const string DamageAnimationKey = "damage-effect";

		// Token: 0x040000BD RID: 189
		private const string FadeAnimationKey = "melee-fade";

		// Token: 0x040000BE RID: 190
		private const string SlashAnimationKey = "melee-slash";

		// Token: 0x040000BF RID: 191
		private const string ThrustAnimationKey = "melee-thrust";
	}
}
