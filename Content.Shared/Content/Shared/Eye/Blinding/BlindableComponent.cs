using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Eye.Blinding
{
	// Token: 0x02000495 RID: 1173
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class BlindableComponent : Component
	{
		// Token: 0x04000D64 RID: 3428
		[DataField("sources", false, 1, false, false, null)]
		public int Sources;

		// Token: 0x04000D65 RID: 3429
		[DataField("blindResistance", false, 1, false, false, null)]
		public float BlindResistance;

		// Token: 0x04000D66 RID: 3430
		[ViewVariables]
		public int EyeDamage;

		// Token: 0x04000D67 RID: 3431
		[ViewVariables]
		public bool EyeTooDamaged;

		// Token: 0x04000D68 RID: 3432
		public bool LightSetup;

		// Token: 0x04000D69 RID: 3433
		public bool GraceFrame;
	}
}
