using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Component
{
	// Token: 0x02000371 RID: 881
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RgbLightControllerState : ComponentState
	{
		// Token: 0x06000A47 RID: 2631 RVA: 0x00022313 File Offset: 0x00020513
		public RgbLightControllerState(float cycleRate, List<int> layers)
		{
			this.CycleRate = cycleRate;
			this.Layers = layers;
		}

		// Token: 0x04000A23 RID: 2595
		public readonly float CycleRate;

		// Token: 0x04000A24 RID: 2596
		public List<int> Layers;
	}
}
