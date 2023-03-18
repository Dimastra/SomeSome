using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.SubFloor
{
	// Token: 0x02000107 RID: 263
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedSubFloorHideSystem)
	})]
	public sealed class SubFloorHideComponent : Component
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x0000D15C File Offset: 0x0000B35C
		// (set) Token: 0x060002E3 RID: 739 RVA: 0x0000D164 File Offset: 0x0000B364
		[ViewVariables]
		public bool IsUnderCover { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x0000D16D File Offset: 0x0000B36D
		// (set) Token: 0x060002E5 RID: 741 RVA: 0x0000D175 File Offset: 0x0000B375
		[DataField("blockInteractions", false, 1, false, false, null)]
		public bool BlockInteractions { get; set; } = true;

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x0000D17E File Offset: 0x0000B37E
		// (set) Token: 0x060002E7 RID: 743 RVA: 0x0000D186 File Offset: 0x0000B386
		[DataField("blockAmbience", false, 1, false, false, null)]
		public bool BlockAmbience { get; set; } = true;

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x0000D18F File Offset: 0x0000B38F
		// (set) Token: 0x060002E9 RID: 745 RVA: 0x0000D197 File Offset: 0x0000B397
		[ViewVariables]
		[Access]
		public HashSet<EntityUid> RevealedBy { get; set; } = new HashSet<EntityUid>();

		// Token: 0x04000335 RID: 821
		[DataField("scannerTransparency", false, 1, false, false, null)]
		public float ScannerTransparency = 0.8f;

		// Token: 0x04000336 RID: 822
		[DataField("visibleLayers", false, 1, false, false, null)]
		public HashSet<Enum> VisibleLayers = new HashSet<Enum>
		{
			SubfloorLayers.FirstLayer
		};
	}
}
