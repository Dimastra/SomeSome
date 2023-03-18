using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.White.TTS;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Humanoid
{
	// Token: 0x02000404 RID: 1028
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class HumanoidAppearanceComponent : Component
	{
		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x000277A1 File Offset: 0x000259A1
		// (set) Token: 0x06000BFB RID: 3067 RVA: 0x000277A9 File Offset: 0x000259A9
		[DataField("species", false, 1, false, false, typeof(PrototypeIdSerializer<SpeciesPrototype>))]
		public string Species { get; set; } = string.Empty;

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x000277B2 File Offset: 0x000259B2
		[Nullable(2)]
		[DataField("initial", false, 1, false, false, typeof(PrototypeIdSerializer<HumanoidProfilePrototype>))]
		public string Initial { [NullableContext(2)] get; }

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06000BFD RID: 3069 RVA: 0x000277BA File Offset: 0x000259BA
		// (set) Token: 0x06000BFE RID: 3070 RVA: 0x000277C2 File Offset: 0x000259C2
		[DataField("skinColor", false, 1, false, false, null)]
		public Color SkinColor { get; set; } = Color.FromHex("#C0967F", null);

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000BFF RID: 3071 RVA: 0x000277CB File Offset: 0x000259CB
		// (set) Token: 0x06000C00 RID: 3072 RVA: 0x000277D3 File Offset: 0x000259D3
		[DataField("voice", false, 1, false, false, typeof(PrototypeIdSerializer<TTSVoicePrototype>))]
		public string Voice { get; set; } = "Garithos";

		// Token: 0x04000BEE RID: 3054
		[DataField("markingSet", false, 1, false, false, null)]
		public MarkingSet MarkingSet = new MarkingSet();

		// Token: 0x04000BEF RID: 3055
		[DataField("baseLayers", false, 1, false, false, null)]
		public Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> BaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>();

		// Token: 0x04000BF0 RID: 3056
		[DataField("permanentlyHidden", false, 1, false, false, null)]
		public HashSet<HumanoidVisualLayers> PermanentlyHidden = new HashSet<HumanoidVisualLayers>();

		// Token: 0x04000BF1 RID: 3057
		[DataField("gender", false, 1, false, false, null)]
		[ViewVariables]
		public Gender Gender;

		// Token: 0x04000BF2 RID: 3058
		[DataField("age", false, 1, false, false, null)]
		[ViewVariables]
		public int Age = 18;

		// Token: 0x04000BF3 RID: 3059
		[DataField("customBaseLayers", false, 1, false, false, null)]
		public Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> CustomBaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo>();

		// Token: 0x04000BF7 RID: 3063
		[DataField("hiddenLayers", false, 1, false, false, null)]
		public HashSet<HumanoidVisualLayers> HiddenLayers = new HashSet<HumanoidVisualLayers>();

		// Token: 0x04000BF8 RID: 3064
		[DataField("sex", false, 1, false, false, null)]
		public Sex Sex;

		// Token: 0x04000BF9 RID: 3065
		[DataField("eyeColor", false, 1, false, false, null)]
		public Color EyeColor = Color.Brown;
	}
}
