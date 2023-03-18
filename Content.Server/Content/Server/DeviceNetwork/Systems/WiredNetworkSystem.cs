using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000589 RID: 1417
	public sealed class WiredNetworkSystem : EntitySystem
	{
		// Token: 0x06001DBD RID: 7613 RVA: 0x0009E744 File Offset: 0x0009C944
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<WiredNetworkComponent, BeforePacketSentEvent>(new ComponentEventHandler<WiredNetworkComponent, BeforePacketSentEvent>(this.OnBeforePacketSent), null, null);
		}

		// Token: 0x06001DBE RID: 7614 RVA: 0x0009E760 File Offset: 0x0009C960
		[NullableContext(1)]
		private void OnBeforePacketSent(EntityUid uid, WiredNetworkComponent component, BeforePacketSentEvent args)
		{
			if (this.EntityManager.GetComponent<TransformComponent>(uid).GridUid != args.SenderTransform.GridUid)
			{
				args.Cancel();
			}
		}
	}
}
