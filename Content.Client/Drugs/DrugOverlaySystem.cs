using System;
using System.Runtime.CompilerServices;
using Content.Shared.Drugs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Drugs
{
	// Token: 0x02000338 RID: 824
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DrugOverlaySystem : EntitySystem
	{
		// Token: 0x0600148E RID: 5262 RVA: 0x000787B0 File Offset: 0x000769B0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SeeingRainbowsComponent, ComponentInit>(new ComponentEventHandler<SeeingRainbowsComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SeeingRainbowsComponent, ComponentShutdown>(new ComponentEventHandler<SeeingRainbowsComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<SeeingRainbowsComponent, PlayerAttachedEvent>(new ComponentEventHandler<SeeingRainbowsComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<SeeingRainbowsComponent, PlayerDetachedEvent>(new ComponentEventHandler<SeeingRainbowsComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			this._overlay = new RainbowOverlay();
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x0007881E File Offset: 0x00076A1E
		private void OnPlayerAttached(EntityUid uid, SeeingRainbowsComponent component, PlayerAttachedEvent args)
		{
			this._overlayMan.AddOverlay(this._overlay);
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x00078832 File Offset: 0x00076A32
		private void OnPlayerDetached(EntityUid uid, SeeingRainbowsComponent component, PlayerDetachedEvent args)
		{
			this._overlay.Intoxication = 0f;
			this._overlayMan.RemoveOverlay(this._overlay);
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x00078858 File Offset: 0x00076A58
		private void OnInit(EntityUid uid, SeeingRainbowsComponent component, ComponentInit args)
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

		// Token: 0x06001492 RID: 5266 RVA: 0x000788B8 File Offset: 0x00076AB8
		private void OnShutdown(EntityUid uid, SeeingRainbowsComponent component, ComponentShutdown args)
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
				this._overlay.Intoxication = 0f;
				this._overlayMan.RemoveOverlay(this._overlay);
			}
		}

		// Token: 0x04000A88 RID: 2696
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x04000A89 RID: 2697
		[Dependency]
		private readonly IOverlayManager _overlayMan;

		// Token: 0x04000A8A RID: 2698
		private RainbowOverlay _overlay;

		// Token: 0x04000A8B RID: 2699
		public static string RainbowKey = "SeeingRainbows";
	}
}
