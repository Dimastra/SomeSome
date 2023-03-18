using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Content.Server.Power.EntitySystems;
using Robust.Shared.GameObjects;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000582 RID: 1410
	public sealed class DeviceNetworkRequiresPowerSystem : EntitySystem
	{
		// Token: 0x06001D88 RID: 7560 RVA: 0x0009D551 File Offset: 0x0009B751
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DeviceNetworkRequiresPowerComponent, BeforePacketSentEvent>(new ComponentEventHandler<DeviceNetworkRequiresPowerComponent, BeforePacketSentEvent>(this.OnBeforePacketSent), null, null);
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x0009D567 File Offset: 0x0009B767
		[NullableContext(1)]
		private void OnBeforePacketSent(EntityUid uid, DeviceNetworkRequiresPowerComponent component, BeforePacketSentEvent args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				args.Cancel();
			}
		}
	}
}
