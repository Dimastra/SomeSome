using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eye.Blinding;
using Content.Shared.GameTicking;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Eye.Blinding
{
	// Token: 0x0200031C RID: 796
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BlindingSystem : EntitySystem
	{
		// Token: 0x0600141B RID: 5147 RVA: 0x0007625C File Offset: 0x0007445C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BlindableComponent, ComponentInit>(new ComponentEventHandler<BlindableComponent, ComponentInit>(this.OnBlindInit), null, null);
			base.SubscribeLocalEvent<BlindableComponent, ComponentShutdown>(new ComponentEventHandler<BlindableComponent, ComponentShutdown>(this.OnBlindShutdown), null, null);
			base.SubscribeLocalEvent<BlindableComponent, PlayerAttachedEvent>(new ComponentEventHandler<BlindableComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<BlindableComponent, PlayerDetachedEvent>(new ComponentEventHandler<BlindableComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.RoundRestartCleanup), null, null);
			this._overlay = new BlindOverlay();
		}

		// Token: 0x0600141C RID: 5148 RVA: 0x000762DE File Offset: 0x000744DE
		private void OnPlayerAttached(EntityUid uid, BlindableComponent component, PlayerAttachedEvent args)
		{
			this._overlayMan.AddOverlay(this._overlay);
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x000762F2 File Offset: 0x000744F2
		private void OnPlayerDetached(EntityUid uid, BlindableComponent component, PlayerDetachedEvent args)
		{
			this._overlayMan.RemoveOverlay(this._overlay);
			this._lightManager.Enabled = true;
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x00076314 File Offset: 0x00074514
		private void OnBlindInit(EntityUid uid, BlindableComponent component, ComponentInit args)
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

		// Token: 0x0600141F RID: 5151 RVA: 0x00076374 File Offset: 0x00074574
		private void OnBlindShutdown(EntityUid uid, BlindableComponent component, ComponentShutdown args)
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

		// Token: 0x06001420 RID: 5152 RVA: 0x000763D4 File Offset: 0x000745D4
		private void RoundRestartCleanup(RoundRestartCleanupEvent ev)
		{
			this._lightManager.Enabled = true;
		}

		// Token: 0x04000A13 RID: 2579
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x04000A14 RID: 2580
		[Dependency]
		private readonly IOverlayManager _overlayMan;

		// Token: 0x04000A15 RID: 2581
		[Dependency]
		private ILightManager _lightManager;

		// Token: 0x04000A16 RID: 2582
		private BlindOverlay _overlay;
	}
}
