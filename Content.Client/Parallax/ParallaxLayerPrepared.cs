using System;
using System.Runtime.CompilerServices;
using Content.Client.Parallax.Data;
using Robust.Client.Graphics;

namespace Content.Client.Parallax
{
	// Token: 0x020001DA RID: 474
	[NullableContext(1)]
	[Nullable(0)]
	public struct ParallaxLayerPrepared
	{
		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000C47 RID: 3143 RVA: 0x00048041 File Offset: 0x00046241
		// (set) Token: 0x06000C48 RID: 3144 RVA: 0x00048049 File Offset: 0x00046249
		public Texture Texture { readonly get; set; }

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000C49 RID: 3145 RVA: 0x00048052 File Offset: 0x00046252
		// (set) Token: 0x06000C4A RID: 3146 RVA: 0x0004805A File Offset: 0x0004625A
		public ParallaxLayerConfig Config { readonly get; set; }
	}
}
