using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Salvage
{
	// Token: 0x02000218 RID: 536
	[RegisterComponent]
	public sealed class SalvageGridComponent : Component
	{
		// Token: 0x04000682 RID: 1666
		[Nullable(2)]
		public SalvageMagnetComponent SpawnerMagnet;
	}
}
