using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Markers
{
	// Token: 0x02000249 RID: 585
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MarkerSystem : EntitySystem
	{
		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000EC4 RID: 3780 RVA: 0x000591DD File Offset: 0x000573DD
		// (set) Token: 0x06000EC5 RID: 3781 RVA: 0x000591E5 File Offset: 0x000573E5
		public bool MarkersVisible
		{
			get
			{
				return this._markersVisible;
			}
			set
			{
				this._markersVisible = value;
				this.UpdateMarkers();
			}
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x000591F4 File Offset: 0x000573F4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MarkerComponent, ComponentStartup>(new ComponentEventHandler<MarkerComponent, ComponentStartup>(this.OnStartup), null, null);
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x00059210 File Offset: 0x00057410
		private void OnStartup(EntityUid uid, MarkerComponent marker, ComponentStartup args)
		{
			this.UpdateVisibility(marker);
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x0005921C File Offset: 0x0005741C
		private void UpdateVisibility(MarkerComponent marker)
		{
			SpriteComponent spriteComponent;
			if (this.EntityManager.TryGetComponent<SpriteComponent>(marker.Owner, ref spriteComponent))
			{
				spriteComponent.Visible = this.MarkersVisible;
			}
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x0005924C File Offset: 0x0005744C
		private void UpdateMarkers()
		{
			foreach (MarkerComponent marker in this.EntityManager.EntityQuery<MarkerComponent>(true))
			{
				this.UpdateVisibility(marker);
			}
		}

		// Token: 0x04000751 RID: 1873
		private bool _markersVisible;
	}
}
