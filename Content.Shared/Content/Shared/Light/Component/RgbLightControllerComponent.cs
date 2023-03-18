using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Light.Component
{
	// Token: 0x02000370 RID: 880
	[NullableContext(2)]
	[Nullable(0)]
	[NetworkedComponent]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedRgbLightControllerSystem)
	})]
	public sealed class RgbLightControllerComponent : Component
	{
		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000A44 RID: 2628 RVA: 0x000222EF File Offset: 0x000204EF
		// (set) Token: 0x06000A45 RID: 2629 RVA: 0x000222F7 File Offset: 0x000204F7
		[DataField("cycleRate", false, 1, false, false, null)]
		public float CycleRate { get; set; } = 0.1f;

		// Token: 0x04000A1E RID: 2590
		[DataField("layers", false, 1, false, false, null)]
		public List<int> Layers;

		// Token: 0x04000A1F RID: 2591
		public Color OriginalLightColor;

		// Token: 0x04000A20 RID: 2592
		public Dictionary<int, Color> OriginalLayerColors;

		// Token: 0x04000A21 RID: 2593
		public EntityUid? Holder;

		// Token: 0x04000A22 RID: 2594
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public List<string> HolderLayers;
	}
}
