using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;

namespace Content.Client.AirlockPainter
{
	// Token: 0x02000481 RID: 1153
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirlockPainterEntry
	{
		// Token: 0x06001C70 RID: 7280 RVA: 0x000A4E62 File Offset: 0x000A3062
		public AirlockPainterEntry(string name, [Nullable(2)] Texture icon)
		{
			this.Name = name;
			this.Icon = icon;
		}

		// Token: 0x04000E39 RID: 3641
		public string Name;

		// Token: 0x04000E3A RID: 3642
		[Nullable(2)]
		public Texture Icon;
	}
}
