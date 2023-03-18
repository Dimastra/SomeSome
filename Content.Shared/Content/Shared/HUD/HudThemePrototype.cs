using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.HUD
{
	// Token: 0x02000426 RID: 1062
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("hudTheme", 1)]
	public sealed class HudThemePrototype : IPrototype
	{
		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x0002A3DC File Offset: 0x000285DC
		[DataField("name", false, 1, true, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06000CC7 RID: 3271 RVA: 0x0002A3E4 File Offset: 0x000285E4
		[IdDataField(1, null)]
		public string ID { get; } = string.Empty;

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x0002A3EC File Offset: 0x000285EC
		[DataField("path", false, 1, true, false, null)]
		public string Path { get; } = string.Empty;
	}
}
