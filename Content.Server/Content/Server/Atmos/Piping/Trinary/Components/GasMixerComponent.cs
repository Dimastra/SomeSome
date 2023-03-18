using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Piping.Trinary.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Trinary.Components
{
	// Token: 0x0200075A RID: 1882
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GasMixerSystem)
	})]
	public sealed class GasMixerComponent : Component
	{
		// Token: 0x040018CC RID: 6348
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;

		// Token: 0x040018CD RID: 6349
		[ViewVariables]
		[DataField("inletOne", false, 1, false, false, null)]
		public string InletOneName = "inletOne";

		// Token: 0x040018CE RID: 6350
		[ViewVariables]
		[DataField("inletTwo", false, 1, false, false, null)]
		public string InletTwoName = "inletTwo";

		// Token: 0x040018CF RID: 6351
		[ViewVariables]
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName = "outlet";

		// Token: 0x040018D0 RID: 6352
		[ViewVariables]
		[DataField("targetPressure", false, 1, false, false, null)]
		public float TargetPressure = 101.325f;

		// Token: 0x040018D1 RID: 6353
		[ViewVariables]
		[DataField("maxTargetPressure", false, 1, false, false, null)]
		public float MaxTargetPressure = 4500f;

		// Token: 0x040018D2 RID: 6354
		[ViewVariables]
		[DataField("inletOneConcentration", false, 1, false, false, null)]
		public float InletOneConcentration = 0.5f;

		// Token: 0x040018D3 RID: 6355
		[ViewVariables]
		[DataField("inletTwoConcentration", false, 1, false, false, null)]
		public float InletTwoConcentration = 0.5f;
	}
}
