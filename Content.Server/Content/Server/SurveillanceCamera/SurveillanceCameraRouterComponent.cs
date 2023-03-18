using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.DeviceNetwork;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x0200013F RID: 319
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SurveillanceCameraRouterComponent : Component
	{
		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x0001C495 File Offset: 0x0001A695
		// (set) Token: 0x060005E7 RID: 1511 RVA: 0x0001C49D File Offset: 0x0001A69D
		[ViewVariables]
		public bool Active { get; set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x0001C4A6 File Offset: 0x0001A6A6
		// (set) Token: 0x060005E9 RID: 1513 RVA: 0x0001C4AE File Offset: 0x0001A6AE
		[DataField("subnetName", false, 1, false, false, null)]
		public string SubnetName { get; set; } = string.Empty;

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x0001C4B7 File Offset: 0x0001A6B7
		[ViewVariables]
		public HashSet<string> MonitorRoutes { get; } = new HashSet<string>();

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x0001C4BF File Offset: 0x0001A6BF
		// (set) Token: 0x060005EC RID: 1516 RVA: 0x0001C4C7 File Offset: 0x0001A6C7
		[Nullable(2)]
		[DataField("subnetFrequency", false, 1, false, false, typeof(PrototypeIdSerializer<DeviceFrequencyPrototype>))]
		public string SubnetFrequencyId { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x0001C4D0 File Offset: 0x0001A6D0
		[DataField("setupAvailableNetworks", false, 1, false, false, typeof(PrototypeIdListSerializer<DeviceFrequencyPrototype>))]
		public List<string> AvailableNetworks { get; } = new List<string>();

		// Token: 0x04000380 RID: 896
		[ViewVariables]
		public uint SubnetFrequency;
	}
}
