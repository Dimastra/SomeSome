using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Disposal.Components;
using Content.Client.Disposal.UI;
using Content.Shared.Disposal;
using Content.Shared.Disposal.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Disposal.Systems
{
	// Token: 0x02000353 RID: 851
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DisposalUnitSystem : SharedDisposalUnitSystem
	{
		// Token: 0x0600151D RID: 5405 RVA: 0x0007C1A6 File Offset: 0x0007A3A6
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DisposalUnitComponent, ComponentInit>(new ComponentEventHandler<DisposalUnitComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<DisposalUnitComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<DisposalUnitComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x0007C1D6 File Offset: 0x0007A3D6
		public void UpdateActive(EntityUid disposalEntity, bool active)
		{
			if (active)
			{
				if (!this.PressuringDisposals.Contains(disposalEntity))
				{
					this.PressuringDisposals.Add(disposalEntity);
					return;
				}
			}
			else
			{
				this.PressuringDisposals.Remove(disposalEntity);
			}
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x0007C204 File Offset: 0x0007A404
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			for (int i = this.PressuringDisposals.Count - 1; i >= 0; i--)
			{
				EntityUid disposalUnit = this.PressuringDisposals[i];
				if (this.UpdateInterface(disposalUnit))
				{
					this.PressuringDisposals.RemoveAt(i);
				}
			}
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x0007C254 File Offset: 0x0007A454
		private bool UpdateInterface(EntityUid disposalUnit)
		{
			DisposalUnitComponent disposalUnitComponent;
			if (!base.TryComp<DisposalUnitComponent>(disposalUnit, ref disposalUnitComponent) || disposalUnitComponent.Deleted)
			{
				return true;
			}
			if (disposalUnitComponent.Deleted)
			{
				return true;
			}
			ClientUserInterfaceComponent clientUserInterfaceComponent;
			if (!base.TryComp<ClientUserInterfaceComponent>(disposalUnit, ref clientUserInterfaceComponent))
			{
				return true;
			}
			SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState uiState = disposalUnitComponent.UiState;
			if (uiState == null)
			{
				return true;
			}
			foreach (BoundUserInterface boundUserInterface in clientUserInterfaceComponent.Interfaces)
			{
				DisposalUnitBoundUserInterface disposalUnitBoundUserInterface = boundUserInterface as DisposalUnitBoundUserInterface;
				if (disposalUnitBoundUserInterface != null)
				{
					bool? flag = disposalUnitBoundUserInterface.UpdateWindowState(uiState);
					bool flag2 = false;
					return !(flag.GetValueOrDefault() == flag2 & flag != null);
				}
			}
			return true;
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x0007C308 File Offset: 0x0007A508
		private void OnComponentInit(EntityUid uid, DisposalUnitComponent disposalUnit, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			int num;
			if (!spriteComponent.LayerMapTryGet(DisposalUnitVisualLayers.Base, ref num, false))
			{
				return;
			}
			int num2;
			if (!spriteComponent.LayerMapTryGet(DisposalUnitVisualLayers.BaseFlush, ref num2, false))
			{
				return;
			}
			RSI.StateId stateId = spriteComponent.LayerGetState(num);
			RSI.StateId stateId2 = spriteComponent.LayerGetState(num2);
			disposalUnit.FlushAnimation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)disposalUnit.FlushTime),
				AnimationTracks = 
				{
					new AnimationTrackSpriteFlick
					{
						LayerKey = DisposalUnitVisualLayers.BaseFlush,
						KeyFrames = 
						{
							new AnimationTrackSpriteFlick.KeyFrame(stateId2, 0f),
							new AnimationTrackSpriteFlick.KeyFrame(stateId, disposalUnit.FlushTime)
						}
					}
				}
			};
			if (disposalUnit.FlushSound != null)
			{
				disposalUnit.FlushAnimation.AnimationTracks.Add(new AnimationTrackPlaySound
				{
					KeyFrames = 
					{
						new AnimationTrackPlaySound.KeyFrame(this.SoundSystem.GetSound(disposalUnit.FlushSound), 0f, null)
					}
				});
			}
			base.EnsureComp<AnimationPlayerComponent>(uid);
			this.UpdateState(uid, disposalUnit, spriteComponent);
		}

		// Token: 0x06001522 RID: 5410 RVA: 0x0007C412 File Offset: 0x0007A612
		private void OnAppearanceChange(EntityUid uid, DisposalUnitComponent unit, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			this.UpdateState(uid, unit, args.Sprite);
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x0007C42C File Offset: 0x0007A62C
		private void UpdateState(EntityUid uid, DisposalUnitComponent unit, SpriteComponent sprite)
		{
			SharedDisposalUnitComponent.VisualState visualState;
			if (!this.AppearanceSystem.TryGetData<SharedDisposalUnitComponent.VisualState>(uid, SharedDisposalUnitComponent.Visuals.VisualState, ref visualState, null))
			{
				return;
			}
			sprite.LayerSetVisible(DisposalUnitVisualLayers.Unanchored, visualState == SharedDisposalUnitComponent.VisualState.UnAnchored);
			sprite.LayerSetVisible(DisposalUnitVisualLayers.Base, visualState == SharedDisposalUnitComponent.VisualState.Anchored);
			sprite.LayerSetVisible(DisposalUnitVisualLayers.BaseCharging, visualState == SharedDisposalUnitComponent.VisualState.Charging);
			sprite.LayerSetVisible(DisposalUnitVisualLayers.BaseFlush, visualState == SharedDisposalUnitComponent.VisualState.Flushing);
			if (visualState == SharedDisposalUnitComponent.VisualState.Flushing && !this.AnimationSystem.HasRunningAnimation(uid, "disposal_unit_animation"))
			{
				this.AnimationSystem.Play(uid, unit.FlushAnimation, "disposal_unit_animation");
			}
			SharedDisposalUnitComponent.HandleState handleState;
			if (!this.AppearanceSystem.TryGetData<SharedDisposalUnitComponent.HandleState>(uid, SharedDisposalUnitComponent.Visuals.Handle, ref handleState, null))
			{
				handleState = SharedDisposalUnitComponent.HandleState.Normal;
			}
			sprite.LayerSetVisible(DisposalUnitVisualLayers.OverlayEngaged, handleState > SharedDisposalUnitComponent.HandleState.Normal);
			SharedDisposalUnitComponent.LightStates lightStates;
			if (!this.AppearanceSystem.TryGetData<SharedDisposalUnitComponent.LightStates>(uid, SharedDisposalUnitComponent.Visuals.Light, ref lightStates, null))
			{
				lightStates = SharedDisposalUnitComponent.LightStates.Off;
			}
			sprite.LayerSetVisible(DisposalUnitVisualLayers.OverlayCharging, (lightStates & SharedDisposalUnitComponent.LightStates.Charging) > SharedDisposalUnitComponent.LightStates.Off);
			sprite.LayerSetVisible(DisposalUnitVisualLayers.OverlayReady, (lightStates & SharedDisposalUnitComponent.LightStates.Ready) > SharedDisposalUnitComponent.LightStates.Off);
			sprite.LayerSetVisible(DisposalUnitVisualLayers.OverlayFull, (lightStates & SharedDisposalUnitComponent.LightStates.Full) > SharedDisposalUnitComponent.LightStates.Off);
		}

		// Token: 0x04000AE7 RID: 2791
		[Dependency]
		private readonly AppearanceSystem AppearanceSystem;

		// Token: 0x04000AE8 RID: 2792
		[Dependency]
		private readonly AnimationPlayerSystem AnimationSystem;

		// Token: 0x04000AE9 RID: 2793
		[Dependency]
		private readonly SharedAudioSystem SoundSystem;

		// Token: 0x04000AEA RID: 2794
		private const string AnimationKey = "disposal_unit_animation";

		// Token: 0x04000AEB RID: 2795
		private List<EntityUid> PressuringDisposals = new List<EntityUid>();
	}
}
