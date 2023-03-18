using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Client.Guidebook
{
	// Token: 0x020002EB RID: 747
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GetGuidesEvent : EntityEventArgs
	{
		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x060012DE RID: 4830 RVA: 0x00070856 File Offset: 0x0006EA56
		// (set) Token: 0x060012DF RID: 4831 RVA: 0x0007085E File Offset: 0x0006EA5E
		public Dictionary<string, GuideEntry> Guides { get; set; } = new Dictionary<string, GuideEntry>();
	}
}
