using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eye.Blinding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Client.Eye.Blinding
{
	// Token: 0x0200031F RID: 799
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BlurryVisionSystem : EntitySystem
	{
		// Token: 0x0600142C RID: 5164 RVA: 0x00076724 File Offset: 0x00074924
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BlurryVisionComponent, ComponentInit>(new ComponentEventHandler<BlurryVisionComponent, ComponentInit>(this.OnBlurryInit), null, null);
			base.SubscribeLocalEvent<BlurryVisionComponent, ComponentShutdown>(new ComponentEventHandler<BlurryVisionComponent, ComponentShutdown>(this.OnBlurryShutdown), null, null);
			base.SubscribeLocalEvent<BlurryVisionComponent, PlayerAttachedEvent>(new ComponentEventHandler<BlurryVisionComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<BlurryVisionComponent, PlayerDetachedEvent>(new ComponentEventHandler<BlurryVisionComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<BlurryVisionComponent, ComponentHandleState>(new ComponentEventRefHandler<BlurryVisionComponent, ComponentHandleState>(this.OnHandleState), null, null);
			this._overlay = new BlurryVisionOverlay();
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x000767A6 File Offset: 0x000749A6
		private void OnPlayerAttached(EntityUid uid, BlurryVisionComponent component, PlayerAttachedEvent args)
		{
			this._overlayMan.AddOverlay(this._overlay);
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x000767BA File Offset: 0x000749BA
		private void OnPlayerDetached(EntityUid uid, BlurryVisionComponent component, PlayerDetachedEvent args)
		{
			this._overlayMan.RemoveOverlay(this._overlay);
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x000767D0 File Offset: 0x000749D0
		private void OnBlurryInit(EntityUid uid, BlurryVisionComponent component, ComponentInit args)
		{
			LocalPlayer localPlayer = this._player.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == uid));
			}
			if (flag)
			{
				this._overlayMan.AddOverlay(this._overlay);
			}
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x00076830 File Offset: 0x00074A30
		private void OnBlurryShutdown(EntityUid uid, BlurryVisionComponent component, ComponentShutdown args)
		{
			LocalPlayer localPlayer = this._player.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == uid));
			}
			if (flag)
			{
				this._overlayMan.RemoveOverlay(this._overlay);
			}
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x00076890 File Offset: 0x00074A90
		private void OnHandleState(EntityUid uid, BlurryVisionComponent component, ref ComponentHandleState args)
		{
			BlurryVisionComponentState blurryVisionComponentState = args.Current as BlurryVisionComponentState;
			if (blurryVisionComponentState == null)
			{
				return;
			}
			component.Magnitude = blurryVisionComponentState.Magnitude;
		}

		// Token: 0x04000A23 RID: 2595
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x04000A24 RID: 2596
		[Dependency]
		private readonly IOverlayManager _overlayMan;

		// Token: 0x04000A25 RID: 2597
		private BlurryVisionOverlay _overlay;
	}
}
