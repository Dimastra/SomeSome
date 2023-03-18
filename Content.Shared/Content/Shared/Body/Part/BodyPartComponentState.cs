using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Organ;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Body.Part
{
	// Token: 0x02000659 RID: 1625
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class BodyPartComponentState : ComponentState
	{
		// Token: 0x060013D0 RID: 5072 RVA: 0x00042B16 File Offset: 0x00040D16
		public BodyPartComponentState(EntityUid? body, [Nullable(2)] BodyPartSlot parentSlot, Dictionary<string, BodyPartSlot> children, Dictionary<string, OrganSlot> organs, BodyPartType partType, bool isVital, BodyPartSymmetry symmetry)
		{
			this.ParentSlot = parentSlot;
			this.Children = children;
			this.Organs = organs;
			this.PartType = partType;
			this.IsVital = isVital;
			this.Symmetry = symmetry;
			this.Body = body;
		}

		// Token: 0x04001395 RID: 5013
		public readonly EntityUid? Body;

		// Token: 0x04001396 RID: 5014
		[Nullable(2)]
		public readonly BodyPartSlot ParentSlot;

		// Token: 0x04001397 RID: 5015
		public readonly Dictionary<string, BodyPartSlot> Children;

		// Token: 0x04001398 RID: 5016
		public readonly Dictionary<string, OrganSlot> Organs;

		// Token: 0x04001399 RID: 5017
		public readonly BodyPartType PartType;

		// Token: 0x0400139A RID: 5018
		public readonly bool IsVital;

		// Token: 0x0400139B RID: 5019
		public readonly BodyPartSymmetry Symmetry;
	}
}
