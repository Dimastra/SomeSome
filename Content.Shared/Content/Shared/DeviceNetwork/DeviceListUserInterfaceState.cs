using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x02000518 RID: 1304
	[NetSerializable]
	[Serializable]
	public sealed class DeviceListUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06000FCC RID: 4044 RVA: 0x00032F1B File Offset: 0x0003111B
		public DeviceListUserInterfaceState([TupleElementNames(new string[]
		{
			"address",
			"name"
		})] [Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})] HashSet<ValueTuple<string, string>> deviceList)
		{
			this.DeviceList = deviceList;
		}

		// Token: 0x04000F06 RID: 3846
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
