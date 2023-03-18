using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Body.Part
{
	// Token: 0x02000658 RID: 1624
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedBodySystem)
	})]
	public sealed class BodyPartComponent : Component
	{
		// Token: 0x0400138E RID: 5006
		[DataField("body", false, 1, false, false, null)]
		public EntityUid? Body;

		// Token: 0x0400138F RID: 5007
		[Nullable(2)]
		[DataField("parent", false, 1, false, false, null)]
		public BodyPartSlot ParentSlot;

		// Token: 0x04001390 RID: 5008
		[DataField("children", false, 1, false, false, null)]
		public Dictionary<string, BodyPartSlot> Children = new Dictionary<string, BodyPartSlot>();

		// Token: 0x04001391 RID: 5009
		[DataField("organs", false, 1, false, false, null)]
		public Dictionary<string, OrganSlot> Organs = new Dictionary<string, OrganSlot>();

		// Token: 0x04001392 RID: 5010
		[DataField("partType", false, 1, false, false, null)]
		public BodyPartType PartType;

		// Token: 0x04001393 RID: 5011
		[DataField("vital", false, 1, false, false, null)]
		public bool IsVital;

		// Token: 0x04001394 RID: 5012
		[DataField("symmetry", false, 1, false, false, null)]
		public BodyPartSymmetry Symmetry;
	}
}
