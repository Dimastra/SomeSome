using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store
{
	// Token: 0x0200011D RID: 285
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("currency", 1)]
	[DataDefinition]
	[NetSerializable]
	[Serializable]
	public sealed class CurrencyPrototype : IPrototype
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600033E RID: 830 RVA: 0x0000E172 File Offset: 0x0000C372
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0000E17A File Offset: 0x0000C37A
		[DataField("displayName", false, 1, false, false, null)]
		public string DisplayName { get; } = string.Empty;

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000340 RID: 832 RVA: 0x0000E182 File Offset: 0x0000C382
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("cash", false, 1, false, false, null)]
		public Dictionary<FixedPoint2, string> Cash { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0000E18A File Offset: 0x0000C38A
		[DataField("canWithdraw", false, 1, false, false, null)]
		public bool CanWithdraw { get; } = 1;
	}
}
