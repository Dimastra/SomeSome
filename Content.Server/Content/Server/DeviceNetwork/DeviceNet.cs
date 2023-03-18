using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.DeviceNetwork
{
	// Token: 0x0200057D RID: 1405
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DeviceNet
	{
		// Token: 0x06001D71 RID: 7537 RVA: 0x0009CCB8 File Offset: 0x0009AEB8
		public DeviceNet(int netId, IRobustRandom random)
		{
			this._random = random;
			this.NetId = netId;
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x0009CCF0 File Offset: 0x0009AEF0
		public bool Add(DeviceNetworkComponent device)
		{
			if (device.CustomAddress)
			{
				if (!this.Devices.TryAdd(device.Address, device))
				{
					return false;
				}
			}
			else
			{
				if (string.IsNullOrWhiteSpace(device.Address) || this.Devices.ContainsKey(device.Address))
				{
					device.Address = this.GenerateValidAddress(device.Prefix);
				}
				this.Devices[device.Address] = device;
			}
			uint? receiveFrequency = device.ReceiveFrequency;
			if (receiveFrequency == null)
			{
				return true;
			}
			uint freq = receiveFrequency.GetValueOrDefault();
			HashSet<DeviceNetworkComponent> devices;
			if (!this.ListeningDevices.TryGetValue(freq, out devices))
			{
				devices = (this.ListeningDevices[freq] = new HashSet<DeviceNetworkComponent>());
			}
			devices.Add(device);
			if (!device.ReceiveAll)
			{
				return true;
			}
			HashSet<DeviceNetworkComponent> receiveAlldevices;
			if (!this.ReceiveAllDevices.TryGetValue(freq, out receiveAlldevices))
			{
				receiveAlldevices = (this.ReceiveAllDevices[freq] = new HashSet<DeviceNetworkComponent>());
			}
			receiveAlldevices.Add(device);
			return true;
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x0009CDDC File Offset: 0x0009AFDC
		public bool Remove(DeviceNetworkComponent device)
		{
			if (device.Address == null || !this.Devices.Remove(device.Address))
			{
				return false;
			}
			uint? receiveFrequency = device.ReceiveFrequency;
			if (receiveFrequency != null)
			{
				uint freq = receiveFrequency.GetValueOrDefault();
				HashSet<DeviceNetworkComponent> listening;
				if (this.ListeningDevices.TryGetValue(freq, out listening))
				{
					listening.Remove(device);
					if (listening.Count == 0)
					{
						this.ListeningDevices.Remove(freq);
					}
				}
				HashSet<DeviceNetworkComponent> receiveAll;
				if (device.ReceiveAll && this.ReceiveAllDevices.TryGetValue(freq, out receiveAll))
				{
					receiveAll.Remove(device);
					if (receiveAll.Count == 0)
					{
						this.ListeningDevices.Remove(freq);
					}
				}
				return true;
			}
			return true;
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x0009CE88 File Offset: 0x0009B088
		public bool RandomizeAddress(string oldAddress, [Nullable(2)] string prefix = null)
		{
			DeviceNetworkComponent device;
			if (!this.Devices.Remove(oldAddress, out device))
			{
				return false;
			}
			device.Address = this.GenerateValidAddress(prefix ?? device.Prefix);
			device.CustomAddress = false;
			this.Devices[device.Address] = device;
			return true;
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x0009CED8 File Offset: 0x0009B0D8
		public bool UpdateAddress(string oldAddress, string newAddress)
		{
			if (this.Devices.ContainsKey(newAddress))
			{
				return false;
			}
			DeviceNetworkComponent device;
			if (!this.Devices.Remove(oldAddress, out device))
			{
				return false;
			}
			device.Address = newAddress;
			device.CustomAddress = true;
			this.Devices[newAddress] = device;
			return true;
		}

		// Token: 0x06001D76 RID: 7542 RVA: 0x0009CF24 File Offset: 0x0009B124
		public bool UpdateReceiveFrequency(string address, uint? newFrequency)
		{
			DeviceNetworkComponent device;
			if (!this.Devices.TryGetValue(address, out device))
			{
				return false;
			}
			uint? receiveFrequency = device.ReceiveFrequency;
			uint? num = newFrequency;
			if (receiveFrequency.GetValueOrDefault() == num.GetValueOrDefault() & receiveFrequency != null == (num != null))
			{
				return true;
			}
			num = device.ReceiveFrequency;
			if (num != null)
			{
				uint freq = num.GetValueOrDefault();
				HashSet<DeviceNetworkComponent> listening;
				if (this.ListeningDevices.TryGetValue(freq, out listening))
				{
					listening.Remove(device);
					if (listening.Count == 0)
					{
						this.ListeningDevices.Remove(freq);
					}
				}
				HashSet<DeviceNetworkComponent> receiveAll;
				if (device.ReceiveAll && this.ReceiveAllDevices.TryGetValue(freq, out receiveAll))
				{
					receiveAll.Remove(device);
					if (receiveAll.Count == 0)
					{
						this.ListeningDevices.Remove(freq);
					}
				}
			}
			device.ReceiveFrequency = newFrequency;
			if (newFrequency == null)
			{
				return true;
			}
			HashSet<DeviceNetworkComponent> devices;
			if (!this.ListeningDevices.TryGetValue(newFrequency.Value, out devices))
			{
				devices = (this.ListeningDevices[newFrequency.Value] = new HashSet<DeviceNetworkComponent>());
			}
			devices.Add(device);
			if (!device.ReceiveAll)
			{
				return true;
			}
			HashSet<DeviceNetworkComponent> receiveAlldevices;
			if (!this.ReceiveAllDevices.TryGetValue(newFrequency.Value, out receiveAlldevices))
			{
				receiveAlldevices = (this.ReceiveAllDevices[newFrequency.Value] = new HashSet<DeviceNetworkComponent>());
			}
			receiveAlldevices.Add(device);
			return true;
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x0009D080 File Offset: 0x0009B280
		public bool UpdateReceiveAll(string address, bool receiveAll)
		{
			DeviceNetworkComponent device;
			if (!this.Devices.TryGetValue(address, out device))
			{
				return false;
			}
			if (device.ReceiveAll == receiveAll)
			{
				return true;
			}
			device.ReceiveAll = receiveAll;
			uint? receiveFrequency = device.ReceiveFrequency;
			if (receiveFrequency != null)
			{
				uint freq = receiveFrequency.GetValueOrDefault();
				HashSet<DeviceNetworkComponent> devices;
				if (receiveAll)
				{
					if (!this.ReceiveAllDevices.TryGetValue(freq, out devices))
					{
						devices = (this.ReceiveAllDevices[freq] = new HashSet<DeviceNetworkComponent>());
					}
					devices.Add(device);
				}
				else if (this.ReceiveAllDevices.TryGetValue(freq, out devices))
				{
					devices.Remove(device);
					if (devices.Count == 0)
					{
						this.ReceiveAllDevices.Remove(freq);
					}
				}
				return true;
			}
			return true;
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x0009D12C File Offset: 0x0009B32C
		private string GenerateValidAddress([Nullable(2)] string prefix)
		{
			prefix = (string.IsNullOrWhiteSpace(prefix) ? null : Loc.GetString(prefix));
			string address;
			do
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
				defaultInterpolatedStringHandler.AppendFormatted(prefix);
				defaultInterpolatedStringHandler.AppendFormatted<int>(this._random.Next(), "x");
				address = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			while (this.Devices.ContainsKey(address));
			return address;
		}

		// Token: 0x040012EB RID: 4843
		public readonly Dictionary<string, DeviceNetworkComponent> Devices = new Dictionary<string, DeviceNetworkComponent>();

		// Token: 0x040012EC RID: 4844
		public readonly Dictionary<uint, HashSet<DeviceNetworkComponent>> ListeningDevices = new Dictionary<uint, HashSet<DeviceNetworkComponent>>();

		// Token: 0x040012ED RID: 4845
		public readonly Dictionary<uint, HashSet<DeviceNetworkComponent>> ReceiveAllDevices = new Dictionary<uint, HashSet<DeviceNetworkComponent>>();

		// Token: 0x040012EE RID: 4846
		private readonly IRobustRandom _random;

		// Token: 0x040012EF RID: 4847
		public readonly int NetId;
	}
}
