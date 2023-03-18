using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x0200051C RID: 1308
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedNetworkConfiguratorSystem : EntitySystem
	{
		// Token: 0x06000FD6 RID: 4054 RVA: 0x0003307F File Offset: 0x0003127F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, ComponentGetState>(new ComponentEventRefHandler<NetworkConfiguratorComponent, ComponentGetState>(this.GetNetworkConfiguratorState), null, null);
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, ComponentHandleState>(new ComponentEventRefHandler<NetworkConfiguratorComponent, ComponentHandleState>(this.HandleNetworkConfiguratorState), null, null);
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x000330AF File Offset: 0x000312AF
		private void GetNetworkConfiguratorState(EntityUid uid, NetworkConfiguratorComponent comp, ref ComponentGetState args)
		{
			args.State = new NetworkConfiguratorComponentState(comp.ActiveDeviceList);
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x000330C4 File Offset: 0x000312C4
		private void HandleNetworkConfiguratorState(EntityUid uid, NetworkConfiguratorComponent comp, ref ComponentHandleState args)
		{
			NetworkConfiguratorComponentState state = args.Current as NetworkConfiguratorComponentState;
			if (state == null)
			{
				return;
			}
			comp.ActiveDeviceList = state.ActiveDeviceList;
		}
	}
}
