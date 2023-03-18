using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork;
using Robust.Shared.GameObjects;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000581 RID: 1409
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DeviceListSystem : SharedDeviceListSystem
	{
		// Token: 0x06001D82 RID: 7554 RVA: 0x0009D374 File Offset: 0x0009B574
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DeviceListComponent, ComponentInit>(new ComponentEventHandler<DeviceListComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<DeviceListComponent, BeforeBroadcastAttemptEvent>(new ComponentEventHandler<DeviceListComponent, BeforeBroadcastAttemptEvent>(this.OnBeforeBroadcast), null, null);
			base.SubscribeLocalEvent<DeviceListComponent, BeforePacketSentEvent>(new ComponentEventHandler<DeviceListComponent, BeforePacketSentEvent>(this.OnBeforePacketSent), null, null);
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x0009D3C3 File Offset: 0x0009B5C3
		public void OnInit(EntityUid uid, DeviceListComponent component, ComponentInit args)
		{
			base.Dirty(component, null);
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x0009D3D0 File Offset: 0x0009B5D0
		public Dictionary<string, EntityUid> GetDeviceList(EntityUid uid, [Nullable(2)] DeviceListComponent deviceList = null)
		{
			if (!base.Resolve<DeviceListComponent>(uid, ref deviceList, true))
			{
				return new Dictionary<string, EntityUid>();
			}
			Dictionary<string, EntityUid> devices = new Dictionary<string, EntityUid>(deviceList.Devices.Count);
			foreach (EntityUid deviceUid in deviceList.Devices)
			{
				DeviceNetworkComponent deviceNet;
				if (base.TryComp<DeviceNetworkComponent>(deviceUid, ref deviceNet))
				{
					string address = (base.MetaData(deviceUid).EntityLifeStage == 3) ? deviceNet.Address : ("UID: " + deviceUid.ToString());
					devices.Add(address, deviceUid);
				}
			}
			return devices;
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x0009D484 File Offset: 0x0009B684
		private void OnBeforeBroadcast(EntityUid uid, DeviceListComponent component, BeforeBroadcastAttemptEvent args)
		{
			if (component.Devices.Count == 0)
			{
				if (component.IsAllowList)
				{
					args.Cancel();
				}
				return;
			}
			HashSet<DeviceNetworkComponent> filteredRecipients = new HashSet<DeviceNetworkComponent>(args.Recipients.Count);
			foreach (DeviceNetworkComponent recipient in args.Recipients)
			{
				if (component.Devices.Contains(recipient.Owner) == component.IsAllowList)
				{
					filteredRecipients.Add(recipient);
				}
			}
			args.ModifiedRecipients = filteredRecipients;
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x0009D520 File Offset: 0x0009B720
		private void OnBeforePacketSent(EntityUid uid, DeviceListComponent component, BeforePacketSentEvent args)
		{
			if (component.HandleIncomingPackets && component.Devices.Contains(args.Sender) != component.IsAllowList)
			{
				args.Cancel();
			}
		}
	}
}
