using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Client.Chemistry.Visualizers
{
	// Token: 0x020003CB RID: 971
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SolutionContainerVisualsComponent : Component
	{
		// Token: 0x04000C38 RID: 3128
		[DataField("maxFillLevels", false, 1, false, false, null)]
		public int MaxFillLevels;

		// Token: 0x04000C39 RID: 3129
		[Nullable(2)]
		[DataField("fillBaseName", false, 1, false, false, null)]
		public string FillBaseName;

		// Token: 0x04000C3A RID: 3130
		[DataField("layer", false, 1, false, false, null)]
		public SolutionContainerLayers FillLayer;

		// Token: 0x04000C3B RID: 3131
		[DataField("baseLayer", false, 1, false, false, null)]
		public SolutionContainerLayers BaseLayer = SolutionContainerLayers.Base;

		// Token: 0x04000C3C RID: 3132
		[DataField("overlayLayer", false, 1, false, false, null)]
		public SolutionContainerLayers OverlayLayer = SolutionContainerLayers.Overlay;

		// Token: 0x04000C3D RID: 3133
		[DataField("changeColor", false, 1, false, false, null)]
		public bool ChangeColor = true;

		// Token: 0x04000C3E RID: 3134
		[Nullable(2)]
		[DataField("emptySpriteName", false, 1, false, false, null)]
		public string EmptySpriteName;

		// Token: 0x04000C3F RID: 3135
		[DataField("emptySpriteColor", false, 1, false, false, null)]
		public Color EmptySpriteColor = Color.White;

		// Token: 0x04000C40 RID: 3136
		[DataField("metamorphic", false, 1, false, false, null)]
		public bool Metamorphic;

		// Token: 0x04000C41 RID: 3137
		[DataField("metamorphicDefaultSprite", false, 1, false, false, null)]
		public SpriteSpecifier MetamorphicDefaultSprite = SpriteSpecifier.Invalid;

		// Token: 0x04000C42 RID: 3138
		[DataField("metamorphicNameFull", false, 1, false, false, null)]
		public string MetamorphicNameFull = "transformable-container-component-glass";

		// Token: 0x04000C43 RID: 3139
		public string InitialName = string.Empty;

		// Token: 0x04000C44 RID: 3140
		public string InitialDescription = string.Empty;
	}
}
