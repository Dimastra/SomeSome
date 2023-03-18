using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Humanoid
{
	// Token: 0x02000405 RID: 1029
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class HumanoidAppearanceState : ComponentState
	{
		// Token: 0x06000C02 RID: 3074 RVA: 0x00027870 File Offset: 0x00025A70
		public HumanoidAppearanceState(MarkingSet currentMarkings, HashSet<HumanoidVisualLayers> permanentlyHidden, HashSet<HumanoidVisualLayers> hiddenLayers, Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> customBaseLayers, Sex sex, Gender gender, int age, string species, Color skinColor, Color eyeColor)
		{
			this.Markings = currentMarkings;
			this.PermanentlyHidden = permanentlyHidden;
			this.HiddenLayers = hiddenLayers;
			this.CustomBaseLayers = customBaseLayers;
			this.Sex = sex;
			this.Gender = gender;
			this.Age = age;
			this.Species = species;
			this.SkinColor = skinColor;
			this.EyeColor = eyeColor;
		}

		// Token: 0x04000BFB RID: 3067
		public readonly MarkingSet Markings;

		// Token: 0x04000BFC RID: 3068
		public readonly HashSet<HumanoidVisualLayers> PermanentlyHidden;

		// Token: 0x04000BFD RID: 3069
		public readonly HashSet<HumanoidVisualLayers> HiddenLayers;

		// Token: 0x04000BFE RID: 3070
		public readonly Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> CustomBaseLayers;

		// Token: 0x04000BFF RID: 3071
		public readonly Sex Sex;

		// Token: 0x04000C00 RID: 3072
		public readonly Gender Gender;

		// Token: 0x04000C01 RID: 3073
		public readonly int Age = 18;

		// Token: 0x04000C02 RID: 3074
		public readonly string Species;

		// Token: 0x04000C03 RID: 3075
		public readonly Color SkinColor;

		// Token: 0x04000C04 RID: 3076
		public readonly Color EyeColor;

		// Token: 0x020007F7 RID: 2039
		[NullableContext(2)]
		[Nullable(0)]
		[DataDefinition]
		[NetSerializable]
		[Serializable]
		public readonly struct CustomBaseLayerInfo
		{
			// Token: 0x06001896 RID: 6294 RVA: 0x0004E40C File Offset: 0x0004C60C
			public CustomBaseLayerInfo(string id, Color? color = null)
			{
				this.ID = id;
				this.Color = color;
			}

			// Token: 0x17000506 RID: 1286
			// (get) Token: 0x06001898 RID: 6296 RVA: 0x0004E425 File Offset: 0x0004C625
			// (set) Token: 0x06001897 RID: 6295 RVA: 0x0004E41C File Offset: 0x0004C61C
			[DataField("id", false, 1, false, false, typeof(PrototypeIdSerializer<HumanoidSpeciesSpriteLayer>))]
			public string ID { get; set; }

			// Token: 0x17000507 RID: 1287
			// (get) Token: 0x0600189A RID: 6298 RVA: 0x0004E436 File Offset: 0x0004C636
			// (set) Token: 0x06001899 RID: 6297 RVA: 0x0004E42D File Offset: 0x0004C62D
			[DataField("color", false, 1, false, false, null)]
			public Color? Color { get; set; }
		}
	}
}
