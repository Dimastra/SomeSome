using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Roles
{
	// Token: 0x020001E1 RID: 481
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("antag", 1)]
	public sealed class AntagPrototype : IPrototype
	{
		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x00013C2B File Offset: 0x00011E2B
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x00013C33 File Offset: 0x00011E33
		// (set) Token: 0x06000558 RID: 1368 RVA: 0x00013C3B File Offset: 0x00011E3B
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = "";

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000559 RID: 1369 RVA: 0x00013C44 File Offset: 0x00011E44
		// (set) Token: 0x0600055A RID: 1370 RVA: 0x00013C4C File Offset: 0x00011E4C
		[Nullable(2)]
		[DataField("description", false, 1, false, false, null)]
		public string Description { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x00013C55 File Offset: 0x00011E55
		// (set) Token: 0x0600055C RID: 1372 RVA: 0x00013C5D File Offset: 0x00011E5D
		[DataField("objective", false, 1, false, false, null)]
		public string Objective { get; private set; } = "";

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x0600055D RID: 1373 RVA: 0x00013C66 File Offset: 0x00011E66
		// (set) Token: 0x0600055E RID: 1374 RVA: 0x00013C6E File Offset: 0x00011E6E
		[DataField("antagonist", false, 1, false, false, null)]
		public bool Antagonist { get; private set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600055F RID: 1375 RVA: 0x00013C77 File Offset: 0x00011E77
		// (set) Token: 0x06000560 RID: 1376 RVA: 0x00013C7F File Offset: 0x00011E7F
		[DataField("setPreference", false, 1, false, false, null)]
		public bool SetPreference { get; private set; }

		// Token: 0x04000567 RID: 1383
		private string _name = string.Empty;

		// Token: 0x04000568 RID: 1384
		private string _objective = string.Empty;

		// Token: 0x04000569 RID: 1385
		[Nullable(2)]
		private string _description = string.Empty;
	}
}
