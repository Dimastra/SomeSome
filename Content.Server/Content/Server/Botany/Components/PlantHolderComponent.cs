using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Botany.Components
{
	// Token: 0x02000701 RID: 1793
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PlantHolderComponent : Component
	{
		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x06002597 RID: 9623 RVA: 0x000C6978 File Offset: 0x000C4B78
		// (set) Token: 0x06002598 RID: 9624 RVA: 0x000C6980 File Offset: 0x000C4B80
		[ViewVariables]
		public float PestLevel { get; set; }

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x06002599 RID: 9625 RVA: 0x000C6989 File Offset: 0x000C4B89
		// (set) Token: 0x0600259A RID: 9626 RVA: 0x000C6991 File Offset: 0x000C4B91
		[ViewVariables]
		public float WeedLevel { get; set; }

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x0600259B RID: 9627 RVA: 0x000C699A File Offset: 0x000C4B9A
		// (set) Token: 0x0600259C RID: 9628 RVA: 0x000C69A2 File Offset: 0x000C4BA2
		[ViewVariables]
		public float Toxins { get; set; }

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x0600259D RID: 9629 RVA: 0x000C69AB File Offset: 0x000C4BAB
		// (set) Token: 0x0600259E RID: 9630 RVA: 0x000C69B3 File Offset: 0x000C4BB3
		[ViewVariables]
		public int Age { get; set; }

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x0600259F RID: 9631 RVA: 0x000C69BC File Offset: 0x000C4BBC
		// (set) Token: 0x060025A0 RID: 9632 RVA: 0x000C69C4 File Offset: 0x000C4BC4
		[ViewVariables]
		public int SkipAging { get; set; }

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x060025A1 RID: 9633 RVA: 0x000C69CD File Offset: 0x000C4BCD
		// (set) Token: 0x060025A2 RID: 9634 RVA: 0x000C69D5 File Offset: 0x000C4BD5
		[ViewVariables]
		public bool Dead { get; set; }

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x060025A3 RID: 9635 RVA: 0x000C69DE File Offset: 0x000C4BDE
		// (set) Token: 0x060025A4 RID: 9636 RVA: 0x000C69E6 File Offset: 0x000C4BE6
		[ViewVariables]
		public bool Harvest { get; set; }

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x060025A5 RID: 9637 RVA: 0x000C69EF File Offset: 0x000C4BEF
		// (set) Token: 0x060025A6 RID: 9638 RVA: 0x000C69F7 File Offset: 0x000C4BF7
		[ViewVariables]
		public bool Sampled { get; set; }

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x060025A7 RID: 9639 RVA: 0x000C6A00 File Offset: 0x000C4C00
		// (set) Token: 0x060025A8 RID: 9640 RVA: 0x000C6A08 File Offset: 0x000C4C08
		[ViewVariables]
		public int YieldMod { get; set; } = 1;

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x060025A9 RID: 9641 RVA: 0x000C6A11 File Offset: 0x000C4C11
		// (set) Token: 0x060025AA RID: 9642 RVA: 0x000C6A19 File Offset: 0x000C4C19
		[ViewVariables]
		public float MutationMod { get; set; } = 1f;

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x060025AB RID: 9643 RVA: 0x000C6A22 File Offset: 0x000C4C22
		// (set) Token: 0x060025AC RID: 9644 RVA: 0x000C6A2A File Offset: 0x000C4C2A
		[ViewVariables]
		public float MutationLevel { get; set; }

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x060025AD RID: 9645 RVA: 0x000C6A33 File Offset: 0x000C4C33
		// (set) Token: 0x060025AE RID: 9646 RVA: 0x000C6A3B File Offset: 0x000C4C3B
		[ViewVariables]
		public float Health { get; set; }

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x060025AF RID: 9647 RVA: 0x000C6A44 File Offset: 0x000C4C44
		// (set) Token: 0x060025B0 RID: 9648 RVA: 0x000C6A4C File Offset: 0x000C4C4C
		[ViewVariables]
		public float WeedCoefficient { get; set; } = 1f;

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x060025B1 RID: 9649 RVA: 0x000C6A55 File Offset: 0x000C4C55
		// (set) Token: 0x060025B2 RID: 9650 RVA: 0x000C6A5D File Offset: 0x000C4C5D
		[Nullable(2)]
		[ViewVariables]
		public SeedData Seed { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x060025B3 RID: 9651 RVA: 0x000C6A66 File Offset: 0x000C4C66
		// (set) Token: 0x060025B4 RID: 9652 RVA: 0x000C6A6E File Offset: 0x000C4C6E
		[ViewVariables]
		public bool ImproperHeat { get; set; }

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x060025B5 RID: 9653 RVA: 0x000C6A77 File Offset: 0x000C4C77
		// (set) Token: 0x060025B6 RID: 9654 RVA: 0x000C6A7F File Offset: 0x000C4C7F
		[ViewVariables]
		public bool ImproperPressure { get; set; }

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x060025B7 RID: 9655 RVA: 0x000C6A88 File Offset: 0x000C4C88
		// (set) Token: 0x060025B8 RID: 9656 RVA: 0x000C6A90 File Offset: 0x000C4C90
		[ViewVariables]
		public bool ImproperLight { get; set; }

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x060025B9 RID: 9657 RVA: 0x000C6A99 File Offset: 0x000C4C99
		// (set) Token: 0x060025BA RID: 9658 RVA: 0x000C6AA1 File Offset: 0x000C4CA1
		[ViewVariables]
		public bool ForceUpdate { get; set; }

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x060025BB RID: 9659 RVA: 0x000C6AAA File Offset: 0x000C4CAA
		// (set) Token: 0x060025BC RID: 9660 RVA: 0x000C6AB2 File Offset: 0x000C4CB2
		[DataField("solution", false, 1, false, false, null)]
		public string SoilSolutionName { get; set; } = "soil";

		// Token: 0x0400172F RID: 5935
		[ViewVariables]
		public TimeSpan NextUpdate = TimeSpan.Zero;

		// Token: 0x04001730 RID: 5936
		public TimeSpan UpdateDelay = TimeSpan.FromSeconds(3.0);

		// Token: 0x04001731 RID: 5937
		[ViewVariables]
		public int LastProduce;

		// Token: 0x04001732 RID: 5938
		[ViewVariables]
		public int MissingGas;

		// Token: 0x04001733 RID: 5939
		public readonly TimeSpan CycleDelay = TimeSpan.FromSeconds(15.0);

		// Token: 0x04001734 RID: 5940
		[ViewVariables]
		public TimeSpan LastCycle = TimeSpan.Zero;

		// Token: 0x04001735 RID: 5941
		[ViewVariables]
		public bool UpdateSpriteAfterUpdate;

		// Token: 0x04001736 RID: 5942
		[ViewVariables]
		[DataField("drawWarnings", false, 1, false, false, null)]
		public bool DrawWarnings;

		// Token: 0x04001737 RID: 5943
		[ViewVariables]
		public float WaterLevel = 100f;

		// Token: 0x04001738 RID: 5944
		[ViewVariables]
		public float NutritionLevel = 100f;
	}
}
