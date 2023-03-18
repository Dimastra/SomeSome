using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Systems;
using Content.Shared.DeviceNetwork;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.DeviceNetwork.Components
{
	// Token: 0x0200058D RID: 1421
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(DeviceNetworkSystem),
		typeof(DeviceNet)
	})]
	public sealed class DeviceNetworkComponent : Component
	{
		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001DC8 RID: 7624 RVA: 0x0009E997 File Offset: 0x0009CB97
		// (set) Token: 0x06001DC9 RID: 7625 RVA: 0x0009E99F File Offset: 0x0009CB9F
		[DataField("deviceNetId", false, 1, false, false, null)]
		public DeviceNetworkComponent.DeviceNetIdDefaults NetIdEnum { get; set; }

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001DCA RID: 7626 RVA: 0x0009E9A8 File Offset: 0x0009CBA8
		public int DeviceNetId
		{
			get
			{
				return (int)this.NetIdEnum;
			}
		}

		// Token: 0x0400130D RID: 4877
		[DataField("receiveFrequency", false, 1, false, false, null)]
		public uint? ReceiveFrequency;

		// Token: 0x0400130E RID: 4878
		[DataField("receiveFrequencyId", false, 1, false, false, typeof(PrototypeIdSerializer<DeviceFrequencyPrototype>))]
		public string ReceiveFrequencyId;

		// Token: 0x0400130F RID: 4879
		[ViewVariables]
		[DataField("transmitFrequency", false, 1, false, false, null)]
		public uint? TransmitFrequency;

		// Token: 0x04001310 RID: 4880
		[DataField("transmitFrequencyId", false, 1, false, false, typeof(PrototypeIdSerializer<DeviceFrequencyPrototype>))]
		public string TransmitFrequencyId;

		// Token: 0x04001311 RID: 4881
		[Nullable(1)]
		[DataField("address", false, 1, false, false, null)]
		public string Address = string.Empty;

		// Token: 0x04001312 RID: 4882
		[DataField("customAddress", false, 1, false, false, null)]
		public bool CustomAddress;

		// Token: 0x04001313 RID: 4883
		[ViewVariables]
		[DataField("prefix", false, 1, false, false, null)]
		public string Prefix;

		// Token: 0x04001314 RID: 4884
		[DataField("receiveAll", false, 1, false, false, null)]
		public bool ReceiveAll;

		// Token: 0x04001315 RID: 4885
		[DataField("examinableAddress", false, 1, false, false, null)]
		public bool ExaminableAddress;

		// Token: 0x04001316 RID: 4886
		[ViewVariables]
		[DataField("autoConnect", false, 1, false, false, null)]
		public bool AutoConnect = true;

		// Token: 0x04001317 RID: 4887
		[ViewVariables]
		[DataField("sendBroadcastAttemptEvent", false, 1, false, false, null)]
		public bool SendBroadcastAttemptEvent;

		// Token: 0x02000A23 RID: 2595
		[NullableContext(0)]
		public enum DeviceNetIdDefaults
		{
			// Token: 0x0400236F RID: 9071
			Private,
			// Token: 0x04002370 RID: 9072
			Wired,
			// Token: 0x04002371 RID: 9073
			Wireless,
			// Token: 0x04002372 RID: 9074
			Apc,
			// Token: 0x04002373 RID: 9075
			AtmosDevices,
			// Token: 0x04002374 RID: 9076
			Reserved = 100
		}
	}
}
