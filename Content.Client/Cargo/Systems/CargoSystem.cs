using System;
using System.Runtime.CompilerServices;
using Content.Shared.Cargo;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Cargo.Systems
{
	// Token: 0x0200040A RID: 1034
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CargoSystem : SharedCargoSystem
	{
		// Token: 0x06001980 RID: 6528 RVA: 0x000928CA File Offset: 0x00090ACA
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeCargoTelepad();
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x000928D8 File Offset: 0x00090AD8
		private void InitializeCargoTelepad()
		{
			base.SubscribeLocalEvent<CargoTelepadComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<CargoTelepadComponent, AppearanceChangeEvent>(this.OnCargoAppChange), null, null);
			base.SubscribeLocalEvent<CargoTelepadComponent, AnimationCompletedEvent>(new ComponentEventHandler<CargoTelepadComponent, AnimationCompletedEvent>(this.OnCargoAnimComplete), null, null);
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x00092902 File Offset: 0x00090B02
		private void OnCargoAppChange(EntityUid uid, CargoTelepadComponent component, ref AppearanceChangeEvent args)
		{
			this.OnChangeData(args.Component, args.Sprite);
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x00092918 File Offset: 0x00090B18
		private void OnCargoAnimComplete(EntityUid uid, CargoTelepadComponent component, AnimationCompletedEvent args)
		{
			AppearanceComponent component2;
			if (!base.TryComp<AppearanceComponent>(uid, ref component2))
			{
				return;
			}
			this.OnChangeData(component2, null);
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x0009293C File Offset: 0x00090B3C
		private void OnChangeData(AppearanceComponent component, [Nullable(2)] SpriteComponent sprite = null)
		{
			if (!base.Resolve<SpriteComponent>(component.Owner, ref sprite, true))
			{
				return;
			}
			CargoTelepadState? cargoTelepadState;
			this._appearance.TryGetData<CargoTelepadState?>(component.Owner, CargoTelepadVisuals.State, ref cargoTelepadState, null);
			AnimationPlayerComponent animationPlayerComponent = null;
			if (cargoTelepadState != null)
			{
				CargoTelepadState valueOrDefault = cargoTelepadState.GetValueOrDefault();
				if (valueOrDefault == CargoTelepadState.Unpowered)
				{
					sprite.LayerSetVisible(CargoSystem.CargoTelepadLayers.Beam, false);
					this._player.Stop(component.Owner, animationPlayerComponent, "cargo-telepad-beam");
					this._player.Stop(component.Owner, animationPlayerComponent, "cargo-telepad-idle");
					return;
				}
				if (valueOrDefault == CargoTelepadState.Teleporting)
				{
					if (this._player.HasRunningAnimation(component.Owner, "cargo-telepad-beam"))
					{
						return;
					}
					this._player.Stop(component.Owner, animationPlayerComponent, "cargo-telepad-idle");
					this._player.Play(component.Owner, animationPlayerComponent, CargoSystem.CargoTelepadBeamAnimation, "cargo-telepad-beam");
					return;
				}
			}
			sprite.LayerSetVisible(CargoSystem.CargoTelepadLayers.Beam, true);
			if (this._player.HasRunningAnimation(component.Owner, animationPlayerComponent, "cargo-telepad-idle") || this._player.HasRunningAnimation(component.Owner, animationPlayerComponent, "cargo-telepad-beam"))
			{
				return;
			}
			this._player.Play(component.Owner, animationPlayerComponent, CargoSystem.CargoTelepadIdleAnimation, "cargo-telepad-idle");
		}

		// Token: 0x04000CEB RID: 3307
		[Dependency]
		private readonly AnimationPlayerSystem _player;

		// Token: 0x04000CEC RID: 3308
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000CED RID: 3309
		private static readonly Animation CargoTelepadBeamAnimation = new Animation
		{
			Length = TimeSpan.FromSeconds(0.5),
			AnimationTracks = 
			{
				new AnimationTrackSpriteFlick
				{
					LayerKey = CargoSystem.CargoTelepadLayers.Beam,
					KeyFrames = 
					{
						new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("beam"), 0f)
					}
				}
			}
		};

		// Token: 0x04000CEE RID: 3310
		private static readonly Animation CargoTelepadIdleAnimation = new Animation
		{
			Length = TimeSpan.FromSeconds(0.8),
			AnimationTracks = 
			{
				new AnimationTrackSpriteFlick
				{
					LayerKey = CargoSystem.CargoTelepadLayers.Beam,
					KeyFrames = 
					{
						new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("idle"), 0f)
					}
				}
			}
		};

		// Token: 0x04000CEF RID: 3311
		private const string TelepadBeamKey = "cargo-telepad-beam";

		// Token: 0x04000CF0 RID: 3312
		private const string TelepadIdleKey = "cargo-telepad-idle";

		// Token: 0x0200040B RID: 1035
		[NullableContext(0)]
		private enum CargoTelepadLayers : byte
		{
			// Token: 0x04000CF2 RID: 3314
			Base,
			// Token: 0x04000CF3 RID: 3315
			Beam
		}
	}
}
