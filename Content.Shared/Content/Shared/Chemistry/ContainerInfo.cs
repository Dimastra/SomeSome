using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005D3 RID: 1491
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ContainerInfo
	{
		// Token: 0x06001204 RID: 4612 RVA: 0x0003B375 File Offset: 0x00039575
		public ContainerInfo(string displayName, bool holdsReagents, FixedPoint2 currentVolume, FixedPoint2 maxVolume, [Nullable(new byte[]
		{
			1,
			0,
			1
		})] List<ValueTuple<string, FixedPoint2>> contents)
		{
			this.DisplayName = displayName;
			this.HoldsReagents = holdsReagents;
			this.CurrentVolume = currentVolume;
			this.MaxVolume = maxVolume;
			this.Contents = contents;
		}

		// Token: 0x040010D6 RID: 4310
		public readonly string DisplayName;

		// Token: 0x040010D7 RID: 4311
		public readonly bool HoldsReagents;

		// Token: 0x040010D8 RID: 4312
		public readonly FixedPoint2 CurrentVolume;

		// Token: 0x040010D9 RID: 4313
		public readonly FixedPoint2 MaxVolume;

		// Token: 0x040010DA RID: 4314
		[TupleElementNames(new string[]
		{
			"Id",
			"Quantity"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public readonly List<ValueTuple<string, FixedPoint2>> Contents;
	}
}
