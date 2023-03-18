using System;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.Events;
using Content.Shared.Shuttles.Systems;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Shuttles.Systems
{
	// Token: 0x02000155 RID: 341
	public sealed class ShuttleSystem : SharedShuttleSystem
	{
		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000905 RID: 2309 RVA: 0x00035766 File Offset: 0x00033966
		// (set) Token: 0x06000906 RID: 2310 RVA: 0x00035770 File Offset: 0x00033970
		public bool EnableShuttlePosition
		{
			get
			{
				return this._enableShuttlePosition;
			}
			set
			{
				if (this._enableShuttlePosition == value)
				{
					return;
				}
				this._enableShuttlePosition = value;
				IOverlayManager overlayManager = IoCManager.Resolve<IOverlayManager>();
				if (this._enableShuttlePosition)
				{
					this._overlay = new EmergencyShuttleOverlay(this.EntityManager);
					overlayManager.AddOverlay(this._overlay);
					base.RaiseNetworkEvent(new EmergencyShuttleRequestPositionMessage());
					return;
				}
				overlayManager.RemoveOverlay(this._overlay);
				this._overlay = null;
			}
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x000357DA File Offset: 0x000339DA
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<EmergencyShuttlePositionMessage>(new EntityEventHandler<EmergencyShuttlePositionMessage>(this.OnShuttlePosMessage), null, null);
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x000357F6 File Offset: 0x000339F6
		[NullableContext(1)]
		private void OnShuttlePosMessage(EmergencyShuttlePositionMessage ev)
		{
			if (this._overlay == null)
			{
				return;
			}
			this._overlay.StationUid = ev.StationUid;
			this._overlay.Position = ev.Position;
		}

		// Token: 0x0400048A RID: 1162
		private bool _enableShuttlePosition;

		// Token: 0x0400048B RID: 1163
		[Nullable(2)]
		private EmergencyShuttleOverlay _overlay;
	}
}
