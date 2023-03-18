using System;
using System.Runtime.CompilerServices;
using Content.Shared.EntityHealthBar;
using Content.Shared.GameTicking;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.EntityHealthBar
{
	// Token: 0x02000027 RID: 39
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShowHealthBarsSystem : EntitySystem
	{
		// Token: 0x0600009B RID: 155 RVA: 0x00005A24 File Offset: 0x00003C24
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ShowHealthBarsComponent, ComponentInit>(new ComponentEventHandler<ShowHealthBarsComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ShowHealthBarsComponent, ComponentRemove>(new ComponentEventHandler<ShowHealthBarsComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<ShowHealthBarsComponent, PlayerAttachedEvent>(new ComponentEventHandler<ShowHealthBarsComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<ShowHealthBarsComponent, PlayerDetachedEvent>(new ComponentEventHandler<ShowHealthBarsComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
			this._overlay = new EntityHealthBarOverlay(this.EntityManager, this._protoMan);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005AB4 File Offset: 0x00003CB4
		private void OnInit(EntityUid uid, ShowHealthBarsComponent component, ComponentInit args)
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
				this._overlay.DamageContainer = component.DamageContainer;
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005B28 File Offset: 0x00003D28
		private void OnRemove(EntityUid uid, ShowHealthBarsComponent component, ComponentRemove args)
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

		// Token: 0x0600009E RID: 158 RVA: 0x00005B88 File Offset: 0x00003D88
		private void OnPlayerAttached(EntityUid uid, ShowHealthBarsComponent component, PlayerAttachedEvent args)
		{
			this._overlayMan.AddOverlay(this._overlay);
			this._overlay.DamageContainer = component.DamageContainer;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005BAD File Offset: 0x00003DAD
		private void OnPlayerDetached(EntityUid uid, ShowHealthBarsComponent component, PlayerDetachedEvent args)
		{
			this._overlayMan.RemoveOverlay(this._overlay);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005BAD File Offset: 0x00003DAD
		private void OnRoundRestart(RoundRestartCleanupEvent args)
		{
			this._overlayMan.RemoveOverlay(this._overlay);
		}

		// Token: 0x0400005B RID: 91
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x0400005C RID: 92
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x0400005D RID: 93
		[Dependency]
		private readonly IOverlayManager _overlayMan;

		// Token: 0x0400005E RID: 94
		private EntityHealthBarOverlay _overlay;
	}
}
