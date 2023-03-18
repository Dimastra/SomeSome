using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000144 RID: 324
	[RegisterComponent]
	public sealed class StorageFillVisualizerComponent : Component
	{
		// Token: 0x040003C6 RID: 966
		[DataField("maxFillLevels", false, 1, true, false, null)]
		public int MaxFillLevels;

		// Token: 0x040003C7 RID: 967
		[Nullable(1)]
		[DataField("fillBaseName", false, 1, true, false, null)]
		public string FillBaseName;
	}
}
