using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Binary.Components
{
	// Token: 0x0200076E RID: 1902
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasPortComponent : Component
	{
		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x06002852 RID: 10322 RVA: 0x000D32A3 File Offset: 0x000D14A3
		// (set) Token: 0x06002853 RID: 10323 RVA: 0x000D32AB File Offset: 0x000D14AB
		[ViewVariables]
		[DataField("pipe", false, 1, false, false, null)]
		public string PipeName { get; set; } = "connected";

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06002854 RID: 10324 RVA: 0x000D32B4 File Offset: 0x000D14B4
		[ViewVariables]
		public GasMixture Buffer { get; } = new GasMixture();
	}
}
