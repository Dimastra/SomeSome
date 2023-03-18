using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Dataset
{
	// Token: 0x0200052C RID: 1324
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("dataset", 1)]
	public sealed class DatasetPrototype : IPrototype
	{
		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x00033A68 File Offset: 0x00031C68
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001009 RID: 4105 RVA: 0x00033A70 File Offset: 0x00031C70
		[DataField("values", false, 1, false, false, null)]
		public IReadOnlyList<string> Values { get; } = new List<string>();
	}
}
