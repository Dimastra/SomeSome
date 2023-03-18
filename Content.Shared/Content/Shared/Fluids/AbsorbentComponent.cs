using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Fluids
{
	// Token: 0x02000481 RID: 1153
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class AbsorbentComponent : Component
	{
		// Token: 0x04000D35 RID: 3381
		public float Progress;

		// Token: 0x04000D36 RID: 3382
		public const string SolutionName = "absorbed";

		// Token: 0x04000D37 RID: 3383
		[DataField("pickupAmount", false, 1, false, false, null)]
		public FixedPoint2 PickupAmount = FixedPoint2.New(10);

		// Token: 0x04000D38 RID: 3384
		[DataField("residueAmount", false, 1, false, false, null)]
		public FixedPoint2 ResidueAmount = FixedPoint2.New(10);

		// Token: 0x04000D39 RID: 3385
		[DataField("lowerLimit", false, 1, false, false, null)]
		public FixedPoint2 LowerLimit = FixedPoint2.New(5);

		// Token: 0x04000D3A RID: 3386
		[DataField("pickupSound", false, 1, false, false, null)]
		public SoundSpecifier PickupSound = new SoundPathSpecifier("/Audio/Effects/Fluids/slosh.ogg", null);

		// Token: 0x04000D3B RID: 3387
		[DataField("transferSound", false, 1, false, false, null)]
		public SoundSpecifier TransferSound = new SoundPathSpecifier("/Audio/Effects/Fluids/watersplash.ogg", null);

		// Token: 0x04000D3C RID: 3388
		[DataField("speed", false, 1, false, false, null)]
		public float Speed = 10f;

		// Token: 0x04000D3D RID: 3389
		[DataField("maxEntities", false, 1, false, false, null)]
		public int MaxInteractingEntities = 1;

		// Token: 0x04000D3E RID: 3390
		[ViewVariables]
		public HashSet<EntityUid> InteractingEntities = new HashSet<EntityUid>();
	}
}
