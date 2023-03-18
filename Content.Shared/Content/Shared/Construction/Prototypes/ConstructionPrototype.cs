using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Conditions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction.Prototypes
{
	// Token: 0x0200057A RID: 1402
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("construction", 1)]
	public sealed class ConstructionPrototype : IPrototype
	{
		// Token: 0x1700036A RID: 874
		// (get) Token: 0x0600112F RID: 4399 RVA: 0x00038B28 File Offset: 0x00036D28
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06001130 RID: 4400 RVA: 0x00038B30 File Offset: 0x00036D30
		[DataField("description", false, 1, false, false, null)]
		public string Description { get; } = string.Empty;

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06001131 RID: 4401 RVA: 0x00038B38 File Offset: 0x00036D38
		[DataField("graph", false, 1, false, false, typeof(PrototypeIdSerializer<ConstructionGraphPrototype>))]
		public string Graph { get; } = string.Empty;

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06001132 RID: 4402 RVA: 0x00038B40 File Offset: 0x00036D40
		[DataField("targetNode", false, 1, false, false, null)]
		public string TargetNode { get; } = string.Empty;

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06001133 RID: 4403 RVA: 0x00038B48 File Offset: 0x00036D48
		[DataField("startNode", false, 1, false, false, null)]
		public string StartNode { get; } = string.Empty;

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06001134 RID: 4404 RVA: 0x00038B50 File Offset: 0x00036D50
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon { get; } = SpriteSpecifier.Invalid;

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001135 RID: 4405 RVA: 0x00038B58 File Offset: 0x00036D58
		// (set) Token: 0x06001136 RID: 4406 RVA: 0x00038B60 File Offset: 0x00036D60
		[DataField("canBuildInImpassable", false, 1, false, false, null)]
		public bool CanBuildInImpassable { get; private set; }

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06001137 RID: 4407 RVA: 0x00038B69 File Offset: 0x00036D69
		// (set) Token: 0x06001138 RID: 4408 RVA: 0x00038B71 File Offset: 0x00036D71
		[DataField("category", false, 1, false, false, null)]
		public string Category { get; private set; } = "";

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06001139 RID: 4409 RVA: 0x00038B7A File Offset: 0x00036D7A
		// (set) Token: 0x0600113A RID: 4410 RVA: 0x00038B82 File Offset: 0x00036D82
		[DataField("objectType", false, 1, false, false, null)]
		public ConstructionType Type { get; private set; }

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x0600113B RID: 4411 RVA: 0x00038B8B File Offset: 0x00036D8B
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x0600113C RID: 4412 RVA: 0x00038B93 File Offset: 0x00036D93
		[DataField("placementMode", false, 1, false, false, null)]
		public string PlacementMode { get; } = "PlaceFree";

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x0600113D RID: 4413 RVA: 0x00038B9B File Offset: 0x00036D9B
		[DataField("canRotate", false, 1, false, false, null)]
		public bool CanRotate { get; } = 1;

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x0600113E RID: 4414 RVA: 0x00038BA3 File Offset: 0x00036DA3
		public IReadOnlyList<IConstructionCondition> Conditions
		{
			get
			{
				return this._conditions;
			}
		}

		// Token: 0x04000FEF RID: 4079
		[DataField("conditions", false, 1, false, false, null)]
		private List<IConstructionCondition> _conditions = new List<IConstructionCondition>();
	}
}
