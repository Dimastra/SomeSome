using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Fluids;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Fluids
{
	// Token: 0x02000310 RID: 784
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PuddleDebugOverlaySystem : SharedPuddleDebugOverlaySystem
	{
		// Token: 0x060013C9 RID: 5065 RVA: 0x000745EB File Offset: 0x000727EB
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<PuddleOverlayDisableMessage>(new EntityEventHandler<PuddleOverlayDisableMessage>(this.DisableOverlay), null, null);
			base.SubscribeNetworkEvent<PuddleOverlayDebugMessage>(new EntityEventHandler<PuddleOverlayDebugMessage>(this.RenderDebugData), null, null);
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x0007461B File Offset: 0x0007281B
		private void RenderDebugData(PuddleOverlayDebugMessage message)
		{
			this.TileData[message.GridUid] = message;
			if (this._overlay != null)
			{
				return;
			}
			this._overlay = new PuddleOverlay();
			this._overlayManager.AddOverlay(this._overlay);
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x00074655 File Offset: 0x00072855
		private void DisableOverlay(PuddleOverlayDisableMessage message)
		{
			this.TileData.Clear();
			if (this._overlay == null)
			{
				return;
			}
			this._overlayManager.RemoveOverlay(this._overlay);
			this._overlay = null;
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x00074684 File Offset: 0x00072884
		public PuddleDebugOverlayData[] GetData(EntityUid mapGridGridEntityId)
		{
			return this.TileData[mapGridGridEntityId].OverlayData;
		}

		// Token: 0x040009E7 RID: 2535
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x040009E8 RID: 2536
		public readonly Dictionary<EntityUid, PuddleOverlayDebugMessage> TileData = new Dictionary<EntityUid, PuddleOverlayDebugMessage>();

		// Token: 0x040009E9 RID: 2537
		[Nullable(2)]
		private PuddleOverlay _overlay;
	}
}
