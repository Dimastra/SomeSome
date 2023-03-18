using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A7 RID: 1959
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasMixtureHolderComponent : Component, IGasMixtureHolder
	{
		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06002A76 RID: 10870 RVA: 0x000DFC1D File Offset: 0x000DDE1D
		// (set) Token: 0x06002A77 RID: 10871 RVA: 0x000DFC25 File Offset: 0x000DDE25
		[DataField("air", false, 1, false, false, null)]
		public GasMixture Air { get; set; } = new GasMixture();
	}
}
