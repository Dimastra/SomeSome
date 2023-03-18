using System;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Piping.Other.Components
{
	// Token: 0x0200075D RID: 1885
	[RegisterComponent]
	public sealed class GasMinerComponent : Component
	{
		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x060027E4 RID: 10212 RVA: 0x000D15E6 File Offset: 0x000CF7E6
		// (set) Token: 0x060027E5 RID: 10213 RVA: 0x000D15EE File Offset: 0x000CF7EE
		public bool Enabled { get; set; } = true;

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x060027E6 RID: 10214 RVA: 0x000D15F7 File Offset: 0x000CF7F7
		// (set) Token: 0x060027E7 RID: 10215 RVA: 0x000D15FF File Offset: 0x000CF7FF
		public bool Broken { get; set; }

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x060027E8 RID: 10216 RVA: 0x000D1608 File Offset: 0x000CF808
		// (set) Token: 0x060027E9 RID: 10217 RVA: 0x000D1610 File Offset: 0x000CF810
		[ViewVariables]
		[DataField("maxExternalAmount", false, 1, false, false, null)]
		public float MaxExternalAmount { get; set; } = float.PositiveInfinity;

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x060027EA RID: 10218 RVA: 0x000D1619 File Offset: 0x000CF819
		// (set) Token: 0x060027EB RID: 10219 RVA: 0x000D1621 File Offset: 0x000CF821
		[ViewVariables]
		[DataField("maxExternalPressure", false, 1, false, false, null)]
		public float MaxExternalPressure { get; set; } = 6500f;

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x060027EC RID: 10220 RVA: 0x000D162A File Offset: 0x000CF82A
		// (set) Token: 0x060027ED RID: 10221 RVA: 0x000D1632 File Offset: 0x000CF832
		[ViewVariables]
		[DataField("spawnGas", false, 1, false, false, null)]
		public Gas? SpawnGas { get; set; }

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x060027EE RID: 10222 RVA: 0x000D163B File Offset: 0x000CF83B
		// (set) Token: 0x060027EF RID: 10223 RVA: 0x000D1643 File Offset: 0x000CF843
		[ViewVariables]
		[DataField("spawnTemperature", false, 1, false, false, null)]
		public float SpawnTemperature { get; set; } = 293.15f;

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x060027F0 RID: 10224 RVA: 0x000D164C File Offset: 0x000CF84C
		// (set) Token: 0x060027F1 RID: 10225 RVA: 0x000D1654 File Offset: 0x000CF854
		[ViewVariables]
		[DataField("spawnAmount", false, 1, false, false, null)]
		public float SpawnAmount { get; set; } = 2078.5598f;
	}
}
