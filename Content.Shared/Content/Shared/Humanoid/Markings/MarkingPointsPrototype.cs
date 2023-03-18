using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x0200041F RID: 1055
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("markingPoints", 1)]
	public sealed class MarkingPointsPrototype : IPrototype
	{
		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x000293DB File Offset: 0x000275DB
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000C8D RID: 3213 RVA: 0x000293E3 File Offset: 0x000275E3
		[DataField("points", false, 1, true, false, null)]
		public Dictionary<MarkingCategories, MarkingPoints> Points { get; }

		// Token: 0x04000C81 RID: 3201
		[DataField("onlyWhitelisted", false, 1, false, false, null)]
		public bool OnlyWhitelisted;
	}
}
