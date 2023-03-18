using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.PowerCell
{
	// Token: 0x0200024F RID: 591
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class PowerCellComponent : Component
	{
		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0001823F File Offset: 0x0001643F
		// (set) Token: 0x060006E7 RID: 1767 RVA: 0x00018247 File Offset: 0x00016447
		[ViewVariables]
		public bool IsRigged { get; set; }

		// Token: 0x040006A6 RID: 1702
		[Nullable(1)]
		public const string SolutionName = "powerCell";

		// Token: 0x040006A7 RID: 1703
		public const int PowerCellVisualsLevels = 2;
	}
}
