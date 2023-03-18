using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Decals
{
	// Token: 0x02000521 RID: 1313
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("palette", 1)]
	public sealed class ColorPalettePrototype : IPrototype
	{
		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x00033158 File Offset: 0x00031358
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06000FE1 RID: 4065 RVA: 0x00033160 File Offset: 0x00031360
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; }

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000FE2 RID: 4066 RVA: 0x00033168 File Offset: 0x00031368
		[DataField("colors", false, 1, false, false, null)]
		public Dictionary<string, Color> Colors { get; }
	}
}
