using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Shared.Parallax.Biomes
{
	// Token: 0x020002A0 RID: 672
	[NullableContext(1)]
	public interface IBiomeWorldLayer : IBiomeLayer
	{
		// Token: 0x17000174 RID: 372
		// (get) Token: 0x0600077C RID: 1916
		List<string> AllowedTiles { get; }
	}
}
