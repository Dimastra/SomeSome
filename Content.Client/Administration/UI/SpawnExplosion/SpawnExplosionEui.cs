using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Client.Administration.UI.SpawnExplosion
{
	// Token: 0x020004B2 RID: 1202
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpawnExplosionEui : BaseEui
	{
		// Token: 0x06001DFD RID: 7677 RVA: 0x000B01FD File Offset: 0x000AE3FD
		public SpawnExplosionEui()
		{
			IoCManager.InjectDependencies<SpawnExplosionEui>(this);
			this._window = new SpawnExplosionWindow(this);
			this._window.OnClose += this.SendClosedMessage;
		}

		// Token: 0x06001DFE RID: 7678 RVA: 0x000B022F File Offset: 0x000AE42F
		public override void Opened()
		{
			base.Opened();
			this._window.OpenCentered();
		}

		// Token: 0x06001DFF RID: 7679 RVA: 0x000B0242 File Offset: 0x000AE442
		public override void Closed()
		{
			base.Closed();
			this._window.OnClose -= this.SendClosedMessage;
			this._window.Close();
			this.ClearOverlay();
		}

		// Token: 0x06001E00 RID: 7680 RVA: 0x000B0272 File Offset: 0x000AE472
		public void SendClosedMessage()
		{
			base.SendMessage(new SpawnExplosionEuiMsg.Close());
		}

		// Token: 0x06001E01 RID: 7681 RVA: 0x000B027F File Offset: 0x000AE47F
		public void ClearOverlay()
		{
			if (this._overlayManager.HasOverlay<ExplosionDebugOverlay>())
			{
				this._overlayManager.RemoveOverlay<ExplosionDebugOverlay>();
			}
			this._debugOverlay = null;
		}

		// Token: 0x06001E02 RID: 7682 RVA: 0x000B02A4 File Offset: 0x000AE4A4
		public void RequestPreviewData(MapCoordinates epicenter, string typeId, float totalIntensity, float intensitySlope, float maxIntensity)
		{
			SpawnExplosionEuiMsg.PreviewRequest msg = new SpawnExplosionEuiMsg.PreviewRequest(epicenter, typeId, totalIntensity, intensitySlope, maxIntensity);
			base.SendMessage(msg);
		}

		// Token: 0x06001E03 RID: 7683 RVA: 0x000B02C8 File Offset: 0x000AE4C8
		public override void HandleMessage(EuiMessageBase msg)
		{
			SpawnExplosionEuiMsg.PreviewData previewData = msg as SpawnExplosionEuiMsg.PreviewData;
			if (previewData == null)
			{
				return;
			}
			if (this._debugOverlay == null)
			{
				this._debugOverlay = new ExplosionDebugOverlay();
				this._overlayManager.AddOverlay(this._debugOverlay);
			}
			this._debugOverlay.Tiles = previewData.Explosion.Tiles;
			this._debugOverlay.SpaceTiles = previewData.Explosion.SpaceTiles;
			this._debugOverlay.Intensity = previewData.Explosion.Intensity;
			this._debugOverlay.Slope = previewData.Slope;
			this._debugOverlay.TotalIntensity = previewData.TotalIntensity;
			this._debugOverlay.Map = previewData.Explosion.Epicenter.MapId;
			this._debugOverlay.SpaceMatrix = previewData.Explosion.SpaceMatrix;
			this._debugOverlay.SpaceTileSize = previewData.Explosion.SpaceTileSize;
		}

		// Token: 0x04000EB2 RID: 3762
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x04000EB3 RID: 3763
		private readonly SpawnExplosionWindow _window;

		// Token: 0x04000EB4 RID: 3764
		[Nullable(2)]
		private ExplosionDebugOverlay _debugOverlay;
	}
}
