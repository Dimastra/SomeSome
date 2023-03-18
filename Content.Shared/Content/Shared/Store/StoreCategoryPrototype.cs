using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store
{
	// Token: 0x02000122 RID: 290
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("storeCategory", 1)]
	[NetSerializable]
	[DataDefinition]
	[Serializable]
	public sealed class StoreCategoryPrototype : IPrototype
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0000E6AA File Offset: 0x0000C8AA
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600035C RID: 860 RVA: 0x0000E6B2 File Offset: 0x0000C8B2
		// (set) Token: 0x0600035D RID: 861 RVA: 0x0000E6BA File Offset: 0x0000C8BA
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = "";

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600035E RID: 862 RVA: 0x0000E6C3 File Offset: 0x0000C8C3
		[DataField("priority", false, 1, false, false, null)]
		public int Priority { get; }

		// Token: 0x0400037A RID: 890
		private string _name = string.Empty;
	}
}
