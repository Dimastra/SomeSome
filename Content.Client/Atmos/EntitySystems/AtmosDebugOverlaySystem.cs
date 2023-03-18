using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.Overlays;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.GameTicking;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.EntitySystems
{
	// Token: 0x02000456 RID: 1110
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class AtmosDebugOverlaySystem : SharedAtmosDebugOverlaySystem
	{
		// Token: 0x06001BB5 RID: 7093 RVA: 0x000A04A8 File Offset: 0x0009E6A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeNetworkEvent<SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage>(new EntityEventHandler<SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage>(this.HandleAtmosDebugOverlayMessage), null, null);
			base.SubscribeNetworkEvent<SharedAtmosDebugOverlaySystem.AtmosDebugOverlayDisableMessage>(new EntityEventHandler<SharedAtmosDebugOverlaySystem.AtmosDebugOverlayDisableMessage>(this.HandleAtmosDebugOverlayDisableMessage), null, null);
			base.SubscribeLocalEvent<GridRemovalEvent>(new EntityEventHandler<GridRemovalEvent>(this.OnGridRemoved), null, null);
			IOverlayManager overlayManager = IoCManager.Resolve<IOverlayManager>();
			if (!overlayManager.HasOverlay<AtmosDebugOverlay>())
			{
				overlayManager.AddOverlay(new AtmosDebugOverlay(this));
			}
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x000A0526 File Offset: 0x0009E726
		private void OnGridRemoved(GridRemovalEvent ev)
		{
			if (this._tileData.ContainsKey(ev.EntityUid))
			{
				this._tileData.Remove(ev.EntityUid);
			}
		}

		// Token: 0x06001BB7 RID: 7095 RVA: 0x000A054D File Offset: 0x0009E74D
		private void HandleAtmosDebugOverlayMessage(SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage message)
		{
			this._tileData[message.GridId] = message;
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x000A0561 File Offset: 0x0009E761
		private void HandleAtmosDebugOverlayDisableMessage(SharedAtmosDebugOverlaySystem.AtmosDebugOverlayDisableMessage ev)
		{
			this._tileData.Clear();
		}

		// Token: 0x06001BB9 RID: 7097 RVA: 0x000A0570 File Offset: 0x0009E770
		public override void Shutdown()
		{
			base.Shutdown();
			IOverlayManager overlayManager = IoCManager.Resolve<IOverlayManager>();
			if (overlayManager.HasOverlay<AtmosDebugOverlay>())
			{
				overlayManager.RemoveOverlay<AtmosDebugOverlay>();
			}
		}

		// Token: 0x06001BBA RID: 7098 RVA: 0x000A0561 File Offset: 0x0009E761
		public void Reset(RoundRestartCleanupEvent ev)
		{
			this._tileData.Clear();
		}

		// Token: 0x06001BBB RID: 7099 RVA: 0x000A0598 File Offset: 0x0009E798
		public bool HasData(EntityUid gridId)
		{
			return this._tileData.ContainsKey(gridId);
		}

		// Token: 0x06001BBC RID: 7100 RVA: 0x000A05A8 File Offset: 0x0009E7A8
		public SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData? GetData(EntityUid gridIndex, Vector2i indices)
		{
			SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage atmosDebugOverlayMessage;
			if (!this._tileData.TryGetValue(gridIndex, out atmosDebugOverlayMessage))
			{
				return null;
			}
			Vector2i vector2i = indices - atmosDebugOverlayMessage.BaseIdx;
			if (vector2i.X < 0 || vector2i.Y < 0 || vector2i.X >= 16 || vector2i.Y >= 16)
			{
				return null;
			}
			return new SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData?(atmosDebugOverlayMessage.OverlayData[vector2i.X + vector2i.Y * 16]);
		}

		// Token: 0x04000DCB RID: 3531
		private readonly Dictionary<EntityUid, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage> _tileData = new Dictionary<EntityUid, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage>();

		// Token: 0x04000DCC RID: 3532
		public AtmosDebugOverlayMode CfgMode;

		// Token: 0x04000DCD RID: 3533
		public float CfgBase;

		// Token: 0x04000DCE RID: 3534
		public float CfgScale = 207.85599f;

		// Token: 0x04000DCF RID: 3535
		public int CfgSpecificGas;

		// Token: 0x04000DD0 RID: 3536
		public bool CfgCBM;
	}
}
