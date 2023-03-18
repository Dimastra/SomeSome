using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x0200013E RID: 318
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SurveillanceCameraMonitorSystem)
	})]
	public sealed class SurveillanceCameraMonitorComponent : Component
	{
		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x0001C3E3 File Offset: 0x0001A5E3
		// (set) Token: 0x060005D7 RID: 1495 RVA: 0x0001C3EB File Offset: 0x0001A5EB
		[ViewVariables]
		public EntityUid? ActiveCamera { get; set; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060005D8 RID: 1496 RVA: 0x0001C3F4 File Offset: 0x0001A5F4
		// (set) Token: 0x060005D9 RID: 1497 RVA: 0x0001C3FC File Offset: 0x0001A5FC
		[ViewVariables]
		public string ActiveCameraAddress { get; set; } = string.Empty;

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060005DA RID: 1498 RVA: 0x0001C405 File Offset: 0x0001A605
		// (set) Token: 0x060005DB RID: 1499 RVA: 0x0001C40D File Offset: 0x0001A60D
		[ViewVariables]
		public float LastHeartbeat { get; set; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060005DC RID: 1500 RVA: 0x0001C416 File Offset: 0x0001A616
		// (set) Token: 0x060005DD RID: 1501 RVA: 0x0001C41E File Offset: 0x0001A61E
		[ViewVariables]
		public float LastHeartbeatSent { get; set; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x0001C427 File Offset: 0x0001A627
		// (set) Token: 0x060005DF RID: 1503 RVA: 0x0001C42F File Offset: 0x0001A62F
		[Nullable(2)]
		[ViewVariables]
		public string NextCameraAddress { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x0001C438 File Offset: 0x0001A638
		[ViewVariables]
		public HashSet<EntityUid> Viewers { get; } = new HashSet<EntityUid>();

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x0001C440 File Offset: 0x0001A640
		// (set) Token: 0x060005E2 RID: 1506 RVA: 0x0001C448 File Offset: 0x0001A648
		[ViewVariables]
		public string ActiveSubnet { get; set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060005E3 RID: 1507 RVA: 0x0001C451 File Offset: 0x0001A651
		[ViewVariables]
		public Dictionary<string, string> KnownCameras { get; } = new Dictionary<string, string>();

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060005E4 RID: 1508 RVA: 0x0001C459 File Offset: 0x0001A659
		[ViewVariables]
		public Dictionary<string, string> KnownSubnets { get; } = new Dictionary<string, string>();
	}
}
