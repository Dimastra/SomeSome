using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Shared.Medical.Cryogenics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Medical.Components
{
	// Token: 0x020003BB RID: 955
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class CryoPodComponent : SharedCryoPodComponent
	{
		// Token: 0x170002CD RID: 717
		// (get) Token: 0x060013B5 RID: 5045 RVA: 0x000660EA File Offset: 0x000642EA
		// (set) Token: 0x060013B6 RID: 5046 RVA: 0x000660F2 File Offset: 0x000642F2
		[ViewVariables]
		[DataField("gasMixture", false, 1, false, false, null)]
		public GasMixture Air { get; set; } = new GasMixture(101.325f);
	}
}
