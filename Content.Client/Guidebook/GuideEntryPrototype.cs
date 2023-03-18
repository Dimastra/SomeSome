using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;

namespace Content.Client.Guidebook
{
	// Token: 0x020002ED RID: 749
	[Prototype("guideEntry", 1)]
	public sealed class GuideEntryPrototype : GuideEntry, IPrototype
	{
		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x060012E2 RID: 4834 RVA: 0x0007088D File Offset: 0x0006EA8D
		[Nullable(1)]
		public string ID
		{
			[NullableContext(1)]
			get
			{
				return this.Id;
			}
		}
	}
}
