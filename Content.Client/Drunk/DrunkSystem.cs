using System;
using System.Runtime.CompilerServices;
using Content.Shared.Drunk;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Drunk
{
	// Token: 0x02000337 RID: 823
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DrunkSystem : SharedDrunkSystem
	{
		// Token: 0x06001488 RID: 5256 RVA: 0x00078630 File Offset: 0x00076830
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DrunkComponent, ComponentInit>(new ComponentEventHandler<DrunkComponent, ComponentInit>(this.OnDrunkInit), null, null);
			base.SubscribeLocalEvent<DrunkComponent, ComponentShutdown>(new ComponentEventHandler<DrunkComponent, ComponentShutdown>(this.OnDrunkShutdown), null, null);
			base.SubscribeLocalEvent<DrunkComponent, PlayerAttachedEvent>(new ComponentEventHandler<DrunkComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<DrunkComponent, PlayerDetachedEvent>(new ComponentEventHandler<DrunkComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			this._overlay = new DrunkOverlay();
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x0007869E File Offset: 0x0007689E
		private void OnPlayerAttached(EntityUid uid, DrunkComponent component, PlayerAttachedEvent args)
		{
			this._overlayMan.AddOverlay(this._overlay);
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x000786B2 File Offset: 0x000768B2
		private void OnPlayerDetached(EntityUid uid, DrunkComponent component, PlayerDetachedEvent args)
		{
			this._overlay.CurrentBoozePower = 0f;
			this._overlayMan.RemoveOverlay(this._overlay);
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x000786D8 File Offset: 0x000768D8
		private void OnDrunkInit(EntityUid uid, DrunkComponent component, ComponentInit args)
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

		// Token: 0x0600148C RID: 5260 RVA: 0x00078738 File Offset: 0x00076938
		private void OnDrunkShutdown(EntityUid uid, DrunkComponent component, ComponentShutdown args)
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
				this._overlay.CurrentBoozePower = 0f;
				this._overlayMan.RemoveOverlay(this._overlay);
			}
		}

		// Token: 0x04000A85 RID: 2693
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x04000A86 RID: 2694
		[Dependency]
		private readonly IOverlayManager _overlayMan;

		// Token: 0x04000A87 RID: 2695
		private DrunkOverlay _overlay;
	}
}
