using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Storage.Components
{
	// Token: 0x02000167 RID: 359
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BluespaceLockerComponent : Component
	{
		// Token: 0x1700012C RID: 300
		// (get) Token: 0x0600070D RID: 1805 RVA: 0x00023870 File Offset: 0x00021A70
		// (set) Token: 0x0600070E RID: 1806 RVA: 0x00023878 File Offset: 0x00021A78
		[DataField("bluespaceEffectMinInterval", false, 1, false, false, null)]
		[ViewVariables]
		public uint BluespaceEffectNextTime { get; set; }

		// Token: 0x04000409 RID: 1033
		[DataField("bluespaceLinks", false, 1, false, false, null)]
		[ViewVariables]
		public HashSet<EntityUid> BluespaceLinks = new HashSet<EntityUid>();

		// Token: 0x0400040A RID: 1034
		[DataField("minBluespaceLinks", false, 1, false, false, null)]
		[ViewVariables]
		public uint MinBluespaceLinks;

		// Token: 0x0400040B RID: 1035
		[DataField("pickLinksFromSameMap", false, 1, false, false, null)]
		[ViewVariables]
		public bool PickLinksFromSameMap;

		// Token: 0x0400040C RID: 1036
		[DataField("pickLinksFromResistLockers", false, 1, false, false, null)]
		[ViewVariables]
		public bool PickLinksFromResistLockers = true;

		// Token: 0x0400040D RID: 1037
		[DataField("pickLinksFromStationGrids", false, 1, false, false, null)]
		[ViewVariables]
		public bool PickLinksFromStationGrids = true;

		// Token: 0x0400040E RID: 1038
		[DataField("pickLinksFromSameAccess", false, 1, false, false, null)]
		[ViewVariables]
		public bool PickLinksFromSameAccess = true;

		// Token: 0x0400040F RID: 1039
		[DataField("pickLinksFromBluespaceLockers", false, 1, false, false, null)]
		[ViewVariables]
		public bool PickLinksFromBluespaceLockers;

		// Token: 0x04000410 RID: 1040
		[DataField("pickLinksFromNonBluespaceLockers", false, 1, false, false, null)]
		[ViewVariables]
		public bool PickLinksFromNonBluespaceLockers = true;

		// Token: 0x04000411 RID: 1041
		[DataField("autoLinksBidirectional", false, 1, false, false, null)]
		[ViewVariables]
		public bool AutoLinksBidirectional;

		// Token: 0x04000412 RID: 1042
		[DataField("autoLinksUseProperties", false, 1, false, false, null)]
		[ViewVariables]
		public bool AutoLinksUseProperties;

		// Token: 0x04000413 RID: 1043
		[DataField("usesSinceLinkClear", false, 1, false, false, null)]
		[ViewVariables]
		public int UsesSinceLinkClear;

		// Token: 0x04000415 RID: 1045
		[DataField("autoLinkProperties", false, 1, false, false, null)]
		[ViewVariables]
		public BluespaceLockerBehaviorProperties AutoLinkProperties = new BluespaceLockerBehaviorProperties();

		// Token: 0x04000416 RID: 1046
		[DataField("behaviorProperties", false, 1, false, false, null)]
		[ViewVariables]
		public BluespaceLockerBehaviorProperties BehaviorProperties = new BluespaceLockerBehaviorProperties();
	}
}
