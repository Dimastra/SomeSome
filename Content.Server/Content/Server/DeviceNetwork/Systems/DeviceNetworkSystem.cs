using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000583 RID: 1411
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DeviceNetworkSystem : EntitySystem
	{
		// Token: 0x06001D8B RID: 7563 RVA: 0x0009D587 File Offset: 0x0009B787
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DeviceNetworkComponent, MapInitEvent>(new ComponentEventHandler<DeviceNetworkComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<DeviceNetworkComponent, ComponentShutdown>(new ComponentEventHandler<DeviceNetworkComponent, ComponentShutdown>(this.OnNetworkShutdown), null, null);
			base.SubscribeLocalEvent<DeviceNetworkComponent, ExaminedEvent>(new ComponentEventHandler<DeviceNetworkComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x06001D8C RID: 7564 RVA: 0x0009D5C8 File Offset: 0x0009B7C8
		public override void Update(float frameTime)
		{
			DeviceNetworkPacketEvent packet;
			while (this._packets.TryDequeue(out packet))
			{
				this.SendPacket(packet);
			}
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x0009D5F0 File Offset: 0x0009B7F0
		[NullableContext(2)]
		public bool QueuePacket(EntityUid uid, string address, [Nullable(1)] NetworkPayload data, uint? frequency = null, DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<DeviceNetworkComponent>(uid, ref device, false))
			{
				return false;
			}
			if (device.Address == string.Empty)
			{
				return false;
			}
			uint? num = frequency;
			if (num == null)
			{
				frequency = device.TransmitFrequency;
			}
			if (frequency == null)
			{
				return false;
			}
			this._packets.Enqueue(new DeviceNetworkPacketEvent(device.DeviceNetId, address, frequency.Value, device.Address, uid, data));
			return true;
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x0009D669 File Offset: 0x0009B869
		private void OnExamine(EntityUid uid, DeviceNetworkComponent device, ExaminedEvent args)
		{
			if (device.ExaminableAddress)
			{
				args.PushText(Loc.GetString("device-address-examine-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("address", device.Address)
				}));
			}
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x0009D6A0 File Offset: 0x0009B8A0
		private void OnMapInit(EntityUid uid, DeviceNetworkComponent device, MapInitEvent args)
		{
			DeviceFrequencyPrototype receive;
			if (device.ReceiveFrequency == null && device.ReceiveFrequencyId != null && this._protoMan.TryIndex<DeviceFrequencyPrototype>(device.ReceiveFrequencyId, ref receive))
			{
				device.ReceiveFrequency = new uint?(receive.Frequency);
			}
			DeviceFrequencyPrototype xmit;
			if (device.TransmitFrequency == null && device.TransmitFrequencyId != null && this._protoMan.TryIndex<DeviceFrequencyPrototype>(device.TransmitFrequencyId, ref xmit))
			{
				device.TransmitFrequency = new uint?(xmit.Frequency);
			}
			if (device.AutoConnect)
			{
				this.ConnectDevice(uid, device);
			}
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x0009D734 File Offset: 0x0009B934
		private DeviceNet GetNetwork(int netId)
		{
			DeviceNet deviceNet;
			if (this._networks.TryGetValue(netId, out deviceNet))
			{
				return deviceNet;
			}
			DeviceNet newDeviceNet = new DeviceNet(netId, this._random);
			this._networks[netId] = newDeviceNet;
			return newDeviceNet;
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x0009D76E File Offset: 0x0009B96E
		private void OnNetworkShutdown(EntityUid uid, DeviceNetworkComponent component, ComponentShutdown args)
		{
			this.GetNetwork(component.DeviceNetId).Remove(component);
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x0009D783 File Offset: 0x0009B983
		[NullableContext(2)]
		public bool ConnectDevice(EntityUid uid, DeviceNetworkComponent device = null)
		{
			return base.Resolve<DeviceNetworkComponent>(uid, ref device, false) && this.GetNetwork(device.DeviceNetId).Add(device);
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x0009D7A5 File Offset: 0x0009B9A5
		[NullableContext(2)]
		public bool DisconnectDevice(EntityUid uid, DeviceNetworkComponent device, bool preventAutoConnect = true)
		{
			if (!base.Resolve<DeviceNetworkComponent>(uid, ref device, false))
			{
				return false;
			}
			if (preventAutoConnect)
			{
				device.AutoConnect = false;
			}
			return this.GetNetwork(device.DeviceNetId).Remove(device);
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x0009D7D4 File Offset: 0x0009B9D4
		[NullableContext(2)]
		public bool IsDeviceConnected(EntityUid uid, DeviceNetworkComponent device)
		{
			DeviceNet deviceNet;
			return base.Resolve<DeviceNetworkComponent>(uid, ref device, false) && this._networks.TryGetValue(device.DeviceNetId, out deviceNet) && deviceNet.Devices.ContainsValue(device);
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x0009D814 File Offset: 0x0009BA14
		[NullableContext(2)]
		public bool IsAddressPresent(int netId, string address)
		{
			DeviceNet network;
			return address != null && this._networks.TryGetValue(netId, out network) && network.Devices.ContainsKey(address);
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x0009D844 File Offset: 0x0009BA44
		[NullableContext(2)]
		public void SetReceiveFrequency(EntityUid uid, uint? frequency, DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<DeviceNetworkComponent>(uid, ref device, false))
			{
				return;
			}
			uint? receiveFrequency = device.ReceiveFrequency;
			uint? num = frequency;
			if (receiveFrequency.GetValueOrDefault() == num.GetValueOrDefault() & receiveFrequency != null == (num != null))
			{
				return;
			}
			DeviceNet network = this.GetNetwork(device.DeviceNetId);
			network.Remove(device);
			device.ReceiveFrequency = frequency;
			network.Add(device);
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x0009D8AD File Offset: 0x0009BAAD
		[NullableContext(2)]
		public void SetTransmitFrequency(EntityUid uid, uint? frequency, DeviceNetworkComponent device = null)
		{
			if (base.Resolve<DeviceNetworkComponent>(uid, ref device, false))
			{
				device.TransmitFrequency = frequency;
			}
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x0009D8C2 File Offset: 0x0009BAC2
		[NullableContext(2)]
		public void SetReceiveAll(EntityUid uid, bool receiveAll, DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<DeviceNetworkComponent>(uid, ref device, false))
			{
				return;
			}
			if (device.ReceiveAll == receiveAll)
			{
				return;
			}
			DeviceNet network = this.GetNetwork(device.DeviceNetId);
			network.Remove(device);
			device.ReceiveAll = receiveAll;
			network.Add(device);
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x0009D900 File Offset: 0x0009BB00
		public void SetAddress(EntityUid uid, string address, [Nullable(2)] DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<DeviceNetworkComponent>(uid, ref device, false))
			{
				return;
			}
			if (device.Address == address && device.CustomAddress)
			{
				return;
			}
			DeviceNet network = this.GetNetwork(device.DeviceNetId);
			network.Remove(device);
			device.CustomAddress = true;
			device.Address = address;
			network.Add(device);
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x0009D95A File Offset: 0x0009BB5A
		[NullableContext(2)]
		public void RandomizeAddress(EntityUid uid, DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<DeviceNetworkComponent>(uid, ref device, false))
			{
				return;
			}
			DeviceNet network = this.GetNetwork(device.DeviceNetId);
			network.Remove(device);
			device.CustomAddress = false;
			device.Address = "";
			network.Add(device);
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x0009D996 File Offset: 0x0009BB96
		private bool TryGetDevice(int netId, string address, [Nullable(2)] [NotNullWhen(true)] out DeviceNetworkComponent device)
		{
			return this.GetNetwork(netId).Devices.TryGetValue(address, out device);
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x0009D9AC File Offset: 0x0009BBAC
		private void SendPacket(DeviceNetworkPacketEvent packet)
		{
			DeviceNet network = this.GetNetwork(packet.NetId);
			if (packet.Address == null)
			{
				HashSet<DeviceNetworkComponent> devices;
				if (network.ListeningDevices.TryGetValue(packet.Frequency, out devices) && this.CheckRecipientsList(packet, ref devices))
				{
					DeviceNetworkComponent[] deviceCopy = ArrayPool<DeviceNetworkComponent>.Shared.Rent(devices.Count);
					devices.CopyTo(deviceCopy);
					this.SendToConnections(deviceCopy.AsSpan(0, devices.Count), packet);
					ArrayPool<DeviceNetworkComponent>.Shared.Return(deviceCopy, false);
					return;
				}
			}
			else
			{
				int totalDevices = 0;
				bool hasTargetedDevice = false;
				HashSet<DeviceNetworkComponent> devices2;
				if (network.ReceiveAllDevices.TryGetValue(packet.Frequency, out devices2))
				{
					totalDevices += devices2.Count;
				}
				DeviceNetworkComponent device;
				if (this.TryGetDevice(packet.NetId, packet.Address, out device) && !device.ReceiveAll)
				{
					uint? receiveFrequency = device.ReceiveFrequency;
					uint frequency = packet.Frequency;
					if (receiveFrequency.GetValueOrDefault() == frequency & receiveFrequency != null)
					{
						totalDevices++;
						hasTargetedDevice = true;
					}
				}
				DeviceNetworkComponent[] deviceCopy2 = ArrayPool<DeviceNetworkComponent>.Shared.Rent(totalDevices);
				if (devices2 != null)
				{
					devices2.CopyTo(deviceCopy2);
				}
				if (hasTargetedDevice)
				{
					deviceCopy2[totalDevices - 1] = device;
				}
				this.SendToConnections(deviceCopy2.AsSpan(0, totalDevices), packet);
				ArrayPool<DeviceNetworkComponent>.Shared.Return(deviceCopy2, false);
			}
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x0009DAEC File Offset: 0x0009BCEC
		private bool CheckRecipientsList(DeviceNetworkPacketEvent packet, ref HashSet<DeviceNetworkComponent> recipients)
		{
			if (!this._networks.ContainsKey(packet.NetId) || !this._networks[packet.NetId].Devices.ContainsKey(packet.SenderAddress))
			{
				return false;
			}
			if (!this._networks[packet.NetId].Devices[packet.SenderAddress].SendBroadcastAttemptEvent)
			{
				return true;
			}
			BeforeBroadcastAttemptEvent beforeBroadcastAttemptEvent = new BeforeBroadcastAttemptEvent(recipients);
			base.RaiseLocalEvent<BeforeBroadcastAttemptEvent>(packet.Sender, beforeBroadcastAttemptEvent, true);
			if (beforeBroadcastAttemptEvent.Cancelled || beforeBroadcastAttemptEvent.ModifiedRecipients == null)
			{
				return false;
			}
			recipients = beforeBroadcastAttemptEvent.ModifiedRecipients;
			return true;
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x0009DB8C File Offset: 0x0009BD8C
		private unsafe void SendToConnections([Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<DeviceNetworkComponent> connections, DeviceNetworkPacketEvent packet)
		{
			if (base.Deleted(packet.Sender, null))
			{
				return;
			}
			TransformComponent xform = base.Transform(packet.Sender);
			BeforePacketSentEvent beforeEv = new BeforePacketSentEvent(packet.Sender, xform, this._transformSystem.GetWorldPosition(xform));
			ReadOnlySpan<DeviceNetworkComponent> readOnlySpan = connections;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				DeviceNetworkComponent connection = *readOnlySpan[i];
				if (!(connection.Owner == packet.Sender))
				{
					base.RaiseLocalEvent<BeforePacketSentEvent>(connection.Owner, beforeEv, false);
					if (!beforeEv.Cancelled)
					{
						base.RaiseLocalEvent<DeviceNetworkPacketEvent>(connection.Owner, packet, false);
					}
					else
					{
						beforeEv.Uncancel();
					}
				}
			}
		}

		// Token: 0x040012F4 RID: 4852
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040012F5 RID: 4853
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x040012F6 RID: 4854
		[Dependency]
		private readonly SharedTransformSystem _transformSystem;

		// Token: 0x040012F7 RID: 4855
		private readonly Dictionary<int, DeviceNet> _networks = new Dictionary<int, DeviceNet>(4);

		// Token: 0x040012F8 RID: 4856
		private readonly Queue<DeviceNetworkPacketEvent> _packets = new Queue<DeviceNetworkPacketEvent>();
	}
}
