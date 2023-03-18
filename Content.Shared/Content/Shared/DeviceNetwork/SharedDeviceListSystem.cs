using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x02000519 RID: 1305
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedDeviceListSystem : EntitySystem
	{
		// Token: 0x06000FCD RID: 4045 RVA: 0x00032F2A File Offset: 0x0003112A
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DeviceListComponent, ComponentGetState>(new ComponentEventRefHandler<DeviceListComponent, ComponentGetState>(this.GetDeviceListState), null, null);
			base.SubscribeLocalEvent<DeviceListComponent, ComponentHandleState>(new ComponentEventRefHandler<DeviceListComponent, ComponentHandleState>(this.HandleDeviceListState), null, null);
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x00032F54 File Offset: 0x00031154
		public DeviceListUpdateResult UpdateDeviceList(EntityUid uid, IEnumerable<EntityUid> devices, bool merge = false, [Nullable(2)] DeviceListComponent deviceList = null)
		{
			if (!base.Resolve<DeviceListComponent>(uid, ref deviceList, true))
			{
				return DeviceListUpdateResult.NoComponent;
			}
			List<EntityUid> oldDevices = deviceList.Devices.ToList<EntityUid>();
			HashSet<EntityUid> newDevices = merge ? new HashSet<EntityUid>(deviceList.Devices) : new HashSet<EntityUid>();
			List<EntityUid> devicesList = devices.ToList<EntityUid>();
			newDevices.UnionWith(devicesList);
			if (newDevices.Count > deviceList.DeviceLimit)
			{
				return DeviceListUpdateResult.TooManyDevices;
			}
			deviceList.Devices = newDevices;
			base.RaiseLocalEvent<DeviceListUpdateEvent>(uid, new DeviceListUpdateEvent(oldDevices, devicesList), false);
			base.Dirty(deviceList, null);
			return DeviceListUpdateResult.UpdateOk;
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x00032FD3 File Offset: 0x000311D3
		public IEnumerable<EntityUid> GetAllDevices(EntityUid uid, [Nullable(2)] DeviceListComponent component = null)
		{
			if (!base.Resolve<DeviceListComponent>(uid, ref component, true))
			{
				return new EntityUid[0];
			}
			return component.Devices;
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x00032FEE File Offset: 0x000311EE
		private void GetDeviceListState(EntityUid uid, DeviceListComponent comp, ref ComponentGetState args)
		{
			args.State = new DeviceListComponentState(comp.Devices, comp.IsAllowList, comp.HandleIncomingPackets);
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x00033010 File Offset: 0x00031210
		private void HandleDeviceListState(EntityUid uid, DeviceListComponent comp, ref ComponentHandleState args)
		{
			DeviceListComponentState state = args.Current as DeviceListComponentState;
			if (state == null)
			{
				return;
			}
			comp.Devices = state.Devices;
			comp.HandleIncomingPackets = state.HandleIncomingPackets;
			comp.IsAllowList = state.IsAllowList;
		}
	}
}
