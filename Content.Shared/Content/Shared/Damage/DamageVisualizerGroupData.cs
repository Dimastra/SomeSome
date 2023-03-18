using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Damage
{
	// Token: 0x02000533 RID: 1331
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DamageVisualizerGroupData : ICloneable
	{
		// Token: 0x0600102D RID: 4141 RVA: 0x00034656 File Offset: 0x00032856
		public DamageVisualizerGroupData(List<string> groupList)
		{
			this.GroupList = groupList;
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x00034665 File Offset: 0x00032865
		public object Clone()
		{
			return new DamageVisualizerGroupData(new List<string>(this.GroupList));
		}

		// Token: 0x04000F44 RID: 3908
		public List<string> GroupList;
	}
}
