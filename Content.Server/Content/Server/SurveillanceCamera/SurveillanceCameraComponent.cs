using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.DeviceNetwork;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x0200013C RID: 316
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SurveillanceCameraSystem)
	})]
	public sealed class SurveillanceCameraComponent : Component
	{
		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x0001C309 File Offset: 0x0001A509
		[ViewVariables]
		public HashSet<EntityUid> ActiveViewers { get; } = new HashSet<EntityUid>();

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x0001C311 File Offset: 0x0001A511
		[ViewVariables]
		public HashSet<EntityUid> ActiveMonitors { get; } = new HashSet<EntityUid>();

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x0001C319 File Offset: 0x0001A519
		// (set) Token: 0x060005C8 RID: 1480 RVA: 0x0001C321 File Offset: 0x0001A521
		[ViewVariables]
		public bool Active { get; set; } = true;

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x0001C32A File Offset: 0x0001A52A
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x0001C332 File Offset: 0x0001A532
		[ViewVariables]
		[DataField("id", false, 1, false, false, null)]
		public string CameraId { get; set; } = "camera";

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x0001C33B File Offset: 0x0001A53B
		// (set) Token: 0x060005CC RID: 1484 RVA: 0x0001C343 File Offset: 0x0001A543
		[ViewVariables]
		[DataField("nameSet", false, 1, false, false, null)]
		public bool NameSet { get; set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x0001C34C File Offset: 0x0001A54C
		// (set) Token: 0x060005CE RID: 1486 RVA: 0x0001C354 File Offset: 0x0001A554
		[ViewVariables]
		[DataField("networkSet", false, 1, false, false, null)]
		public bool NetworkSet { get; set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060005CF RID: 1487 RVA: 0x0001C35D File Offset: 0x0001A55D
		[DataField("setupAvailableNetworks", false, 1, false, false, typeof(PrototypeIdListSerializer<DeviceFrequencyPrototype>))]
		public List<string> AvailableNetworks { get; } = new List<string>();
	}
}
