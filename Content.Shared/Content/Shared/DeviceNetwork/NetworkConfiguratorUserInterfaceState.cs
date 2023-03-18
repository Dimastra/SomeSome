using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x02000517 RID: 1303
	[NetSerializable]
	[Serializable]
	public sealed class NetworkConfiguratorUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06000FCB RID: 4043 RVA: 0x00032F0C File Offset: 0x0003110C
		public NetworkConfiguratorUserInterfaceState([Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})] HashSet<ValueTuple<string, string>> deviceList)
		{
			this.DeviceList = deviceList;
		}

		// Token: 0x04000F05 RID: 3845
		[TupleElementNames(new string[]
		{
			"address",
			"name"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		public readonly HashSet<ValueTuple<string, string>> DeviceList;
	}
}
