using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid
{
	// Token: 0x02000411 RID: 1041
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class HumanoidMarkingModifierState : BoundUserInterfaceState
	{
		// Token: 0x06000C43 RID: 3139 RVA: 0x0002892A File Offset: 0x00026B2A
		public HumanoidMarkingModifierState(MarkingSet markingSet, string species, Color skinColor, Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> customBaseLayers)
		{
			this.MarkingSet = markingSet;
			this.Species = species;
			this.SkinColor = skinColor;
			this.CustomBaseLayers = customBaseLayers;
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000C44 RID: 3140 RVA: 0x0002894F File Offset: 0x00026B4F
		public MarkingSet MarkingSet { get; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000C45 RID: 3141 RVA: 0x00028957 File Offset: 0x00026B57
		public string Species { get; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000C46 RID: 3142 RVA: 0x0002895F File Offset: 0x00026B5F
		public Color SkinColor { get; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000C47 RID: 3143 RVA: 0x00028967 File Offset: 0x00026B67
		public Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> CustomBaseLayers { get; }
	}
}
