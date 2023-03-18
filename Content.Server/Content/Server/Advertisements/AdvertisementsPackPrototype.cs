using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Advertisements
{
	// Token: 0x020007F8 RID: 2040
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("advertisementsPack", 1)]
	[Serializable]
	public sealed class AdvertisementsPackPrototype : IPrototype
	{
		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x06002C1A RID: 11290 RVA: 0x000E6D42 File Offset: 0x000E4F42
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x06002C1B RID: 11291 RVA: 0x000E6D4A File Offset: 0x000E4F4A
		[DataField("advertisements", false, 1, false, false, null)]
		public List<string> Advertisements { get; } = new List<string>();
	}
}
