using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Components.Devices;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.DeviceNetwork.Systems.Devices
{
	// Token: 0x0200058B RID: 1419
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ApcNetSwitchSystem : EntitySystem
	{
		// Token: 0x06001DC3 RID: 7619 RVA: 0x0009E85B File Offset: 0x0009CA5B
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ApcNetSwitchComponent, InteractHandEvent>(new ComponentEventHandler<ApcNetSwitchComponent, InteractHandEvent>(this.OnInteracted), null, null);
			base.SubscribeLocalEvent<ApcNetSwitchComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<ApcNetSwitchComponent, DeviceNetworkPacketEvent>(this.OnPackedReceived), null, null);
		}

		// Token: 0x06001DC4 RID: 7620 RVA: 0x0009E88C File Offset: 0x0009CA8C
		private void OnInteracted(EntityUid uid, ApcNetSwitchComponent component, InteractHandEvent args)
		{
			DeviceNetworkComponent networkComponent;
			if (!this.EntityManager.TryGetComponent<DeviceNetworkComponent>(uid, ref networkComponent))
			{
				return;
			}
			component.State = !component.State;
			if (networkComponent.TransmitFrequency == null)
			{
				return;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "set_state";
			networkPayload["state_enabled"] = component.State;
			NetworkPayload payload = networkPayload;
			DeviceNetworkSystem deviceNetworkSystem = this._deviceNetworkSystem;
			string address = null;
			NetworkPayload data = payload;
			DeviceNetworkComponent device = networkComponent;
			deviceNetworkSystem.QueuePacket(uid, address, data, null, device);
			args.Handled = true;
		}

		// Token: 0x06001DC5 RID: 7621 RVA: 0x0009E918 File Offset: 0x0009CB18
		private void OnPackedReceived(EntityUid uid, ApcNetSwitchComponent component, DeviceNetworkPacketEvent args)
		{
			DeviceNetworkComponent networkComponent;
			if (!this.EntityManager.TryGetComponent<DeviceNetworkComponent>(uid, ref networkComponent) || args.SenderAddress == networkComponent.Address)
			{
				return;
			}
			string command;
			if (!args.Data.TryGetValue<string>("command", out command) || command != "set_state")
			{
				return;
			}
			bool enabled;
			if (!args.Data.TryGetValue<bool>("state_enabled", out enabled))
			{
				return;
			}
			component.State = enabled;
		}

		// Token: 0x0400130A RID: 4874
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetworkSystem;
	}
}
