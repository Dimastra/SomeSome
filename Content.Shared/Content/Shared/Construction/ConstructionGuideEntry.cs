using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Construction
{
	// Token: 0x02000569 RID: 1385
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ConstructionGuideEntry
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x060010E3 RID: 4323 RVA: 0x00037C49 File Offset: 0x00035E49
		// (set) Token: 0x060010E4 RID: 4324 RVA: 0x00037C51 File Offset: 0x00035E51
		public int? EntryNumber { get; set; }

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x060010E5 RID: 4325 RVA: 0x00037C5A File Offset: 0x00035E5A
		// (set) Token: 0x060010E6 RID: 4326 RVA: 0x00037C62 File Offset: 0x00035E62
		public int Padding { get; set; }

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x060010E7 RID: 4327 RVA: 0x00037C6B File Offset: 0x00035E6B
		// (set) Token: 0x060010E8 RID: 4328 RVA: 0x00037C73 File Offset: 0x00035E73
		public string Localization { get; set; } = string.Empty;

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x060010E9 RID: 4329 RVA: 0x00037C7C File Offset: 0x00035E7C
		// (set) Token: 0x060010EA RID: 4330 RVA: 0x00037C84 File Offset: 0x00035E84
		[Nullable(new byte[]
		{
			2,
			0,
			1,
			1
		})]
		public ValueTuple<string, object>[] Arguments { [return: Nullable(new byte[]
		{
			2,
			0,
			1,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			0,
			1,
			1
		})] set; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x060010EB RID: 4331 RVA: 0x00037C8D File Offset: 0x00035E8D
		// (set) Token: 0x060010EC RID: 4332 RVA: 0x00037C95 File Offset: 0x00035E95
		[Nullable(2)]
		public SpriteSpecifier Icon { [NullableContext(2)] get; [NullableContext(2)] set; }
	}
}
