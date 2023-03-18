using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007AC RID: 1964
	[RegisterComponent]
	public sealed class MapAtmosphereComponent : Component
	{
		// Token: 0x04001A72 RID: 6770
		[Nullable(2)]
		[DataField("mixture", false, 1, false, false, null)]
		[ViewVariables]
		public GasMixture Mixture = GasMixture.SpaceGas;

		// Token: 0x04001A73 RID: 6771
		[DataField("space", false, 1, false, false, null)]
		[ViewVariables]
		public bool Space = true;
	}
}
