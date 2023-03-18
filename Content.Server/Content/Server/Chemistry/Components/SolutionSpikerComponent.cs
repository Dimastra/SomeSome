using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006B2 RID: 1714
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SolutionSpikerComponent : Component
	{
		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x060023B6 RID: 9142 RVA: 0x000BA59E File Offset: 0x000B879E
		[DataField("sourceSolution", false, 1, false, false, null)]
		public string SourceSolution { get; } = string.Empty;

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x060023B7 RID: 9143 RVA: 0x000BA5A6 File Offset: 0x000B87A6
		[DataField("ignoreEmpty", false, 1, false, false, null)]
		public bool IgnoreEmpty { get; }

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x060023B8 RID: 9144 RVA: 0x000BA5AE File Offset: 0x000B87AE
		[DataField("popup", false, 1, false, false, null)]
		public string Popup { get; } = "spike-solution-generic";

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x060023B9 RID: 9145 RVA: 0x000BA5B6 File Offset: 0x000B87B6
		[DataField("popupEmpty", false, 1, false, false, null)]
		public string PopupEmpty { get; } = "spike-solution-empty-generic";
	}
}
