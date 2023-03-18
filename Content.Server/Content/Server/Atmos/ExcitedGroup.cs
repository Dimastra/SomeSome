using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos
{
	// Token: 0x02000731 RID: 1841
	public sealed class ExcitedGroup
	{
		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x0600269C RID: 9884 RVA: 0x000CC21E File Offset: 0x000CA41E
		// (set) Token: 0x0600269D RID: 9885 RVA: 0x000CC226 File Offset: 0x000CA426
		[ViewVariables]
		public int DismantleCooldown { get; set; }

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x0600269E RID: 9886 RVA: 0x000CC22F File Offset: 0x000CA42F
		// (set) Token: 0x0600269F RID: 9887 RVA: 0x000CC237 File Offset: 0x000CA437
		[ViewVariables]
		public int BreakdownCooldown { get; set; }

		// Token: 0x040017FF RID: 6143
		[ViewVariables]
		public bool Disposed;

		// Token: 0x04001800 RID: 6144
		[Nullable(1)]
		[ViewVariables]
		public readonly List<TileAtmosphere> Tiles = new List<TileAtmosphere>(100);
	}
}
