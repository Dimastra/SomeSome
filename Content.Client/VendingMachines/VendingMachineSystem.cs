using System;
using System.Runtime.CompilerServices;
using Content.Shared.VendingMachines;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.VendingMachines
{
	// Token: 0x02000065 RID: 101
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VendingMachineSystem : SharedVendingMachineSystem
	{
		// Token: 0x060001DA RID: 474 RVA: 0x0000D43A File Offset: 0x0000B63A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<VendingMachineComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<VendingMachineComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, AnimationCompletedEvent>(new ComponentEventHandler<VendingMachineComponent, AnimationCompletedEvent>(this.OnAnimationCompleted), null, null);
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000D46C File Offset: 0x0000B66C
		private void OnAnimationCompleted(EntityUid uid, VendingMachineComponent component, AnimationCompletedEvent args)
		{
			SpriteComponent sprite;
			if (!base.TryComp<SpriteComponent>(uid, ref sprite))
			{
				return;
			}
			AppearanceComponent appearanceComponent;
			VendingMachineVisualState visualState;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !this._appearanceSystem.TryGetData<VendingMachineVisualState>(uid, VendingMachineVisuals.VisualState, ref visualState, appearanceComponent))
			{
				visualState = VendingMachineVisualState.Normal;
			}
			this.UpdateAppearance(uid, visualState, component, sprite);
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000D4B4 File Offset: 0x0000B6B4
		private void OnAppearanceChange(EntityUid uid, VendingMachineComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			object obj;
			VendingMachineVisualState visualState;
			if (args.AppearanceData.TryGetValue(VendingMachineVisuals.VisualState, out obj) && obj is VendingMachineVisualState)
			{
				visualState = (VendingMachineVisualState)obj;
			}
			else
			{
				visualState = VendingMachineVisualState.Normal;
			}
			this.UpdateAppearance(uid, visualState, component, args.Sprite);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000D504 File Offset: 0x0000B704
		private void UpdateAppearance(EntityUid uid, VendingMachineVisualState visualState, VendingMachineComponent component, SpriteComponent sprite)
		{
			VendingMachineSystem.SetLayerState(VendingMachineVisualLayers.Base, component.OffState, sprite);
			switch (visualState)
			{
			case VendingMachineVisualState.Normal:
				VendingMachineSystem.SetLayerState(VendingMachineVisualLayers.BaseUnshaded, component.NormalState, sprite);
				VendingMachineSystem.SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, sprite);
				return;
			case VendingMachineVisualState.Off:
				VendingMachineSystem.HideLayers(sprite);
				return;
			case VendingMachineVisualState.Broken:
				VendingMachineSystem.HideLayers(sprite);
				VendingMachineSystem.SetLayerState(VendingMachineVisualLayers.Base, component.BrokenState, sprite);
				return;
			case VendingMachineVisualState.Eject:
				this.PlayAnimation(uid, VendingMachineVisualLayers.BaseUnshaded, component.EjectState, component.EjectDelay, sprite);
				VendingMachineSystem.SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, sprite);
				return;
			case VendingMachineVisualState.Deny:
				if (component.LoopDenyAnimation)
				{
					VendingMachineSystem.SetLayerState(VendingMachineVisualLayers.BaseUnshaded, component.DenyState, sprite);
				}
				else
				{
					this.PlayAnimation(uid, VendingMachineVisualLayers.BaseUnshaded, component.DenyState, component.DenyDelay, sprite);
				}
				VendingMachineSystem.SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, sprite);
				return;
			default:
				return;
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000D5D6 File Offset: 0x0000B7D6
		private static void SetLayerState(VendingMachineVisualLayers layer, [Nullable(2)] string state, SpriteComponent sprite)
		{
			if (string.IsNullOrEmpty(state))
			{
				return;
			}
			sprite.LayerSetVisible(layer, true);
			sprite.LayerSetAutoAnimated(layer, true);
			sprite.LayerSetState(layer, state);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000D610 File Offset: 0x0000B810
		private void PlayAnimation(EntityUid uid, VendingMachineVisualLayers layer, [Nullable(2)] string state, float animationTime, SpriteComponent sprite)
		{
			if (string.IsNullOrEmpty(state))
			{
				return;
			}
			if (!this._animationPlayer.HasRunningAnimation(uid, state))
			{
				Animation animation = VendingMachineSystem.GetAnimation(layer, state, animationTime);
				sprite.LayerSetVisible(layer, true);
				this._animationPlayer.Play(uid, animation, state);
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000D65C File Offset: 0x0000B85C
		private static Animation GetAnimation(VendingMachineVisualLayers layer, string state, float animationTime)
		{
			return new Animation
			{
				Length = TimeSpan.FromSeconds((double)animationTime),
				AnimationTracks = 
				{
					new AnimationTrackSpriteFlick
					{
						LayerKey = layer,
						KeyFrames = 
						{
							new AnimationTrackSpriteFlick.KeyFrame(state, 0f)
						}
					}
				}
			};
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000D6B2 File Offset: 0x0000B8B2
		private static void HideLayers(SpriteComponent sprite)
		{
			VendingMachineSystem.HideLayer(VendingMachineVisualLayers.BaseUnshaded, sprite);
			VendingMachineSystem.HideLayer(VendingMachineVisualLayers.Screen, sprite);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000D6C4 File Offset: 0x0000B8C4
		private static void HideLayer(VendingMachineVisualLayers layer, SpriteComponent sprite)
		{
			int num;
			if (!sprite.LayerMapTryGet(layer, ref num, false))
			{
				return;
			}
			sprite.LayerSetVisible(num, false);
		}

		// Token: 0x0400013D RID: 317
		[Dependency]
		private readonly AnimationPlayerSystem _animationPlayer;

		// Token: 0x0400013E RID: 318
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;
	}
}
