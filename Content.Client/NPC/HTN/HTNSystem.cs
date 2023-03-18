using System;
using System.Runtime.CompilerServices;
using Content.Shared.NPC;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.NPC.HTN
{
	// Token: 0x02000215 RID: 533
	public sealed class HTNSystem : EntitySystem
	{
		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06000DE7 RID: 3559 RVA: 0x00054820 File Offset: 0x00052A20
		// (set) Token: 0x06000DE8 RID: 3560 RVA: 0x00054828 File Offset: 0x00052A28
		public bool EnableOverlay
		{
			get
			{
				return this._enableOverlay;
			}
			set
			{
				IOverlayManager overlayManager = IoCManager.Resolve<IOverlayManager>();
				this._enableOverlay = value;
				if (this._enableOverlay)
				{
					overlayManager.AddOverlay(new HTNOverlay(this.EntityManager, IoCManager.Resolve<IResourceCache>()));
					base.RaiseNetworkEvent(new RequestHTNMessage
					{
						Enabled = true
					});
					return;
				}
				overlayManager.RemoveOverlay<HTNOverlay>();
				base.RaiseNetworkEvent(new RequestHTNMessage
				{
					Enabled = false
				});
			}
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x0005488D File Offset: 0x00052A8D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<HTNMessage>(new EntityEventHandler<HTNMessage>(this.OnHTNMessage), null, null);
		}

		// Token: 0x06000DEA RID: 3562 RVA: 0x000548AC File Offset: 0x00052AAC
		[NullableContext(1)]
		private void OnHTNMessage(HTNMessage ev)
		{
			HTNComponent htncomponent;
			if (!base.TryComp<HTNComponent>(ev.Uid, ref htncomponent))
			{
				return;
			}
			htncomponent.DebugText = ev.Text;
		}

		// Token: 0x040006DB RID: 1755
		private bool _enableOverlay;
	}
}
