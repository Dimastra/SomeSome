using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Piping.Unary.Components
{
	// Token: 0x02000751 RID: 1873
	[RegisterComponent]
	public sealed class GasPassiveVentComponent : Component
	{
		// Token: 0x04001889 RID: 6281
		[Nullable(1)]
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName = "pipe";
	}
}
