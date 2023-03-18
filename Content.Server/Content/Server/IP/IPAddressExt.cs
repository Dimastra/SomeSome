using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Content.Server.IP
{
	// Token: 0x02000440 RID: 1088
	[NullableContext(1)]
	[Nullable(0)]
	public static class IPAddressExt
	{
		// Token: 0x060015FE RID: 5630 RVA: 0x000743D8 File Offset: 0x000725D8
		public static bool IsInSubnet(this IPAddress address, string subnetMask)
		{
			int slashIdx = subnetMask.IndexOf("/", StringComparison.Ordinal);
			if (slashIdx == -1)
			{
				throw new NotSupportedException("Only SubNetMasks with a given prefix length are supported.");
			}
			IPAddress maskAddress = IPAddress.Parse(subnetMask.Substring(0, slashIdx));
			if (maskAddress.AddressFamily != address.AddressFamily)
			{
				return false;
			}
			int num = slashIdx + 1;
			int maskLength = int.Parse(subnetMask.Substring(num, subnetMask.Length - num));
			return address.IsInSubnet(maskAddress, maskLength);
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x00074445 File Offset: 0x00072645
		public static bool IsInSubnet(this IPAddress address, [TupleElementNames(new string[]
		{
			"maskAddress",
			"maskLength"
		})] [Nullable(new byte[]
		{
			0,
			1
		})] ValueTuple<IPAddress, int> tuple)
		{
			return address.IsInSubnet(tuple.Item1, tuple.Item2);
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x0007445C File Offset: 0x0007265C
		public static bool IsInSubnet(this IPAddress address, IPAddress maskAddress, int maskLength)
		{
			if (maskAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				uint num = BitConverter.ToUInt32(maskAddress.GetAddressBytes().Reverse<byte>().ToArray<byte>(), 0);
				uint ipAddressBits = BitConverter.ToUInt32(address.GetAddressBytes().Reverse<byte>().ToArray<byte>(), 0);
				uint mask = uint.MaxValue << 32 - maskLength;
				return (num & mask) == (ipAddressBits & mask);
			}
			if (maskAddress.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("Only InterNetworkV6 or InterNetwork address families are supported.");
			}
			BitArray maskAddressBits = new BitArray(maskAddress.GetAddressBytes());
			BitArray ipAddressBits2 = new BitArray(address.GetAddressBytes());
			if (maskAddressBits.Length != ipAddressBits2.Length)
			{
				throw new ArgumentException("Length of IP Address and Subnet Mask do not match.");
			}
			for (int maskIndex = 0; maskIndex < maskLength; maskIndex++)
			{
				if (ipAddressBits2[maskIndex] != maskAddressBits[maskIndex])
				{
					return false;
				}
			}
			return true;
		}
	}
}
