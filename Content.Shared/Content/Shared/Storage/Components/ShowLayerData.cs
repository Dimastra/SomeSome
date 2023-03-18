using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000143 RID: 323
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ShowLayerData : ICloneable
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x0000FB70 File Offset: 0x0000DD70
		// (set) Token: 0x060003E7 RID: 999 RVA: 0x0000FB78 File Offset: 0x0000DD78
		public IReadOnlyList<string> QueuedEntities { get; internal set; }

		// Token: 0x060003E8 RID: 1000 RVA: 0x0000FB81 File Offset: 0x0000DD81
		public ShowLayerData()
		{
			this.QueuedEntities = new List<string>();
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0000FB94 File Offset: 0x0000DD94
		public ShowLayerData(IEnumerable<string> other)
		{
			this.QueuedEntities = new List<string>(other);
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0000FBA8 File Offset: 0x0000DDA8
		public object Clone()
		{
			return new ShowLayerData(this.QueuedEntities);
		}
	}
}
