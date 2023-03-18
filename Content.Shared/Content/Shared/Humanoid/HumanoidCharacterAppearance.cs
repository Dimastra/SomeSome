using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid
{
	// Token: 0x02000406 RID: 1030
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[NetSerializable]
	[Serializable]
	public sealed class HumanoidCharacterAppearance : ICharacterAppearance
	{
		// Token: 0x06000C03 RID: 3075 RVA: 0x000278D8 File Offset: 0x00025AD8
		public HumanoidCharacterAppearance(string hairStyleId, Color hairColor, string facialHairStyleId, Color facialHairColor, Color eyeColor, Color skinColor, List<Marking> markings)
		{
			this.HairStyleId = hairStyleId;
			this.HairColor = HumanoidCharacterAppearance.ClampColor(hairColor);
			this.FacialHairStyleId = facialHairStyleId;
			this.FacialHairColor = HumanoidCharacterAppearance.ClampColor(facialHairColor);
			this.EyeColor = HumanoidCharacterAppearance.ClampColor(eyeColor);
			this.SkinColor = HumanoidCharacterAppearance.ClampColor(skinColor);
			this.Markings = markings;
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000C04 RID: 3076 RVA: 0x00027934 File Offset: 0x00025B34
		[DataField("hair", false, 1, false, false, null)]
		public string HairStyleId { get; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000C05 RID: 3077 RVA: 0x0002793C File Offset: 0x00025B3C
		[DataField("hairColor", false, 1, false, false, null)]
		public Color HairColor { get; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000C06 RID: 3078 RVA: 0x00027944 File Offset: 0x00025B44
		[DataField("facialHair", false, 1, false, false, null)]
		public string FacialHairStyleId { get; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06000C07 RID: 3079 RVA: 0x0002794C File Offset: 0x00025B4C
		[DataField("facialHairColor", false, 1, false, false, null)]
		public Color FacialHairColor { get; }

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06000C08 RID: 3080 RVA: 0x00027954 File Offset: 0x00025B54
		[DataField("eyeColor", false, 1, false, false, null)]
		public Color EyeColor { get; }

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06000C09 RID: 3081 RVA: 0x0002795C File Offset: 0x00025B5C
		[DataField("skinColor", false, 1, false, false, null)]
		public Color SkinColor { get; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06000C0A RID: 3082 RVA: 0x00027964 File Offset: 0x00025B64
		[DataField("markings", false, 1, false, false, null)]
		public List<Marking> Markings { get; }

		// Token: 0x06000C0B RID: 3083 RVA: 0x0002796C File Offset: 0x00025B6C
		public HumanoidCharacterAppearance WithHairStyleName(string newName)
		{
			return new HumanoidCharacterAppearance(newName, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, this.SkinColor, this.Markings);
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x00027998 File Offset: 0x00025B98
		public HumanoidCharacterAppearance WithHairColor(Color newColor)
		{
			return new HumanoidCharacterAppearance(this.HairStyleId, newColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, this.SkinColor, this.Markings);
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x000279C4 File Offset: 0x00025BC4
		public HumanoidCharacterAppearance WithFacialHairStyleName(string newName)
		{
			return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, newName, this.FacialHairColor, this.EyeColor, this.SkinColor, this.Markings);
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x000279F0 File Offset: 0x00025BF0
		public HumanoidCharacterAppearance WithFacialHairColor(Color newColor)
		{
			return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, this.FacialHairStyleId, newColor, this.EyeColor, this.SkinColor, this.Markings);
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x00027A1C File Offset: 0x00025C1C
		public HumanoidCharacterAppearance WithEyeColor(Color newColor)
		{
			return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, newColor, this.SkinColor, this.Markings);
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x00027A48 File Offset: 0x00025C48
		public HumanoidCharacterAppearance WithSkinColor(Color newColor)
		{
			return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, newColor, this.Markings);
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x00027A74 File Offset: 0x00025C74
		public HumanoidCharacterAppearance WithMarkings(List<Marking> newMarkings)
		{
			return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, this.SkinColor, newMarkings);
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x00027AA0 File Offset: 0x00025CA0
		public static HumanoidCharacterAppearance Default()
		{
			return new HumanoidCharacterAppearance("HairBald", Color.Black, "FacialHairShaved", Color.Black, Color.Black, Content.Shared.Humanoid.SkinColor.ValidHumanSkinTone, new List<Marking>());
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x00027ACC File Offset: 0x00025CCC
		public static HumanoidCharacterAppearance DefaultWithSpecies(string species)
		{
			SpeciesPrototype speciesPrototype = IoCManager.Resolve<IPrototypeManager>().Index<SpeciesPrototype>(species);
			Color color;
			switch (speciesPrototype.SkinColoration)
			{
			case HumanoidSkinColor.HumanToned:
				color = Content.Shared.Humanoid.SkinColor.HumanSkinTone(speciesPrototype.DefaultHumanSkinTone);
				break;
			case HumanoidSkinColor.Hues:
				color = speciesPrototype.DefaultSkinTone;
				break;
			case HumanoidSkinColor.TintedHues:
				color = Content.Shared.Humanoid.SkinColor.TintedHues(speciesPrototype.DefaultSkinTone);
				break;
			default:
				color = Content.Shared.Humanoid.SkinColor.ValidHumanSkinTone;
				break;
			}
			Color skinColor = color;
			return new HumanoidCharacterAppearance("HairBald", Color.Black, "FacialHairShaved", Color.Black, Color.Black, skinColor, new List<Marking>());
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x00027B54 File Offset: 0x00025D54
		public static HumanoidCharacterAppearance Random(string species, Sex sex)
		{
			HumanoidCharacterAppearance.<>c__DisplayClass32_0 CS$<>8__locals1;
			CS$<>8__locals1.random = IoCManager.Resolve<IRobustRandom>();
			MarkingManager markingManager = IoCManager.Resolve<MarkingManager>();
			List<string> hairStyles = markingManager.MarkingsByCategoryAndSpecies(MarkingCategories.Hair, species).Keys.ToList<string>();
			List<string> facialHairStyles = markingManager.MarkingsByCategoryAndSpecies(MarkingCategories.FacialHair, species).Keys.ToList<string>();
			string newHairStyle = (hairStyles.Count > 0) ? RandomExtensions.Pick<string>(CS$<>8__locals1.random, hairStyles) : "HairBald";
			string newFacialHairStyle = (facialHairStyles.Count == 0 || sex == Sex.Female) ? "FacialHairShaved" : RandomExtensions.Pick<string>(CS$<>8__locals1.random, facialHairStyles);
			Color newHairColor = RandomExtensions.Pick<Color>(CS$<>8__locals1.random, HairStyles.RealisticHairColors);
			newHairColor = newHairColor.WithRed(HumanoidCharacterAppearance.<Random>g__RandomizeColor|32_0(newHairColor.R, ref CS$<>8__locals1)).WithGreen(HumanoidCharacterAppearance.<Random>g__RandomizeColor|32_0(newHairColor.G, ref CS$<>8__locals1)).WithBlue(HumanoidCharacterAppearance.<Random>g__RandomizeColor|32_0(newHairColor.B, ref CS$<>8__locals1));
			Color newEyeColor = RandomExtensions.Pick<Color>(CS$<>8__locals1.random, HumanoidCharacterAppearance.RealisticEyeColors);
			HumanoidSkinColor skinType = IoCManager.Resolve<IPrototypeManager>().Index<SpeciesPrototype>(species).SkinColoration;
			Color newSkinColor = Content.Shared.Humanoid.SkinColor.ValidHumanSkinTone;
			if (skinType != HumanoidSkinColor.HumanToned)
			{
				if (skinType - HumanoidSkinColor.Hues <= 1)
				{
					int rbyte = CS$<>8__locals1.random.Next(0, 255);
					int gbyte = CS$<>8__locals1.random.Next(0, 255);
					int bbyte = CS$<>8__locals1.random.Next(0, 255);
					newSkinColor..ctor((float)rbyte, (float)gbyte, (float)bbyte, 1f);
				}
			}
			else
			{
				newSkinColor = Content.Shared.Humanoid.SkinColor.HumanSkinTone(CS$<>8__locals1.random.Next(0, 100));
			}
			if (skinType == HumanoidSkinColor.TintedHues)
			{
				newSkinColor = Content.Shared.Humanoid.SkinColor.ValidTintedHuesSkinTone(newSkinColor);
			}
			return new HumanoidCharacterAppearance(newHairStyle, newHairColor, newFacialHairStyle, newHairColor, newEyeColor, newSkinColor, new List<Marking>());
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x00027CEC File Offset: 0x00025EEC
		public static Color ClampColor(Color color)
		{
			return new Color(color.RByte, color.GByte, color.BByte, byte.MaxValue);
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x00027D10 File Offset: 0x00025F10
		public static HumanoidCharacterAppearance EnsureValid(HumanoidCharacterAppearance appearance, string species, string[] sponsorMarkings)
		{
			string hairStyleId = appearance.HairStyleId;
			string facialHairStyleId = appearance.FacialHairStyleId;
			Color hairColor = HumanoidCharacterAppearance.ClampColor(appearance.HairColor);
			Color facialHairColor = HumanoidCharacterAppearance.ClampColor(appearance.FacialHairColor);
			Color eyeColor = HumanoidCharacterAppearance.ClampColor(appearance.EyeColor);
			IPrototypeManager proto = IoCManager.Resolve<IPrototypeManager>();
			MarkingManager markingManager = IoCManager.Resolve<MarkingManager>();
			if (!markingManager.MarkingsByCategory(MarkingCategories.Hair).ContainsKey(hairStyleId))
			{
				hairStyleId = "HairBald";
			}
			MarkingPrototype hairProto;
			if (proto.TryIndex<MarkingPrototype>(hairStyleId, ref hairProto) && hairProto.SponsorOnly && !sponsorMarkings.Contains(hairStyleId))
			{
				hairStyleId = "HairBald";
			}
			if (!markingManager.MarkingsByCategory(MarkingCategories.FacialHair).ContainsKey(facialHairStyleId))
			{
				facialHairStyleId = "FacialHairShaved";
			}
			MarkingPrototype facialHairProto;
			if (proto.TryIndex<MarkingPrototype>(facialHairStyleId, ref facialHairProto) && facialHairProto.SponsorOnly && !sponsorMarkings.Contains(facialHairStyleId))
			{
				facialHairStyleId = "FacialHairShaved";
			}
			MarkingSet markingSet = new MarkingSet();
			Color skinColor = appearance.SkinColor;
			SpeciesPrototype speciesProto;
			if (proto.TryIndex<SpeciesPrototype>(species, ref speciesProto))
			{
				markingSet = new MarkingSet(appearance.Markings, speciesProto.MarkingPoints, markingManager, proto);
				markingSet.EnsureValid(markingManager);
				markingSet.FilterSpecies(species, markingManager, null);
				markingSet.FilterSponsor(sponsorMarkings, markingManager, null);
				HumanoidSkinColor skinColoration = speciesProto.SkinColoration;
				if (skinColoration != HumanoidSkinColor.HumanToned)
				{
					if (skinColoration == HumanoidSkinColor.TintedHues)
					{
						if (!Content.Shared.Humanoid.SkinColor.VerifyTintedHues(skinColor))
						{
							skinColor = Content.Shared.Humanoid.SkinColor.ValidTintedHuesSkinTone(skinColor);
						}
					}
				}
				else if (!Content.Shared.Humanoid.SkinColor.VerifyHumanSkinTone(skinColor))
				{
					skinColor = Content.Shared.Humanoid.SkinColor.ValidHumanSkinTone;
				}
			}
			return new HumanoidCharacterAppearance(hairStyleId, hairColor, facialHairStyleId, facialHairColor, eyeColor, skinColor, markingSet.GetForwardEnumerator().ToList<Marking>());
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x00027E78 File Offset: 0x00026078
		public bool MemberwiseEquals(ICharacterAppearance maybeOther)
		{
			HumanoidCharacterAppearance other = maybeOther as HumanoidCharacterAppearance;
			return other != null && !(this.HairStyleId != other.HairStyleId) && this.HairColor.Equals(other.HairColor) && !(this.FacialHairStyleId != other.FacialHairStyleId) && this.FacialHairColor.Equals(other.FacialHairColor) && this.EyeColor.Equals(other.EyeColor) && this.SkinColor.Equals(other.SkinColor) && this.Markings.SequenceEqual(other.Markings);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x00027F82 File Offset: 0x00026182
		[CompilerGenerated]
		internal static float <Random>g__RandomizeColor|32_0(float channel, ref HumanoidCharacterAppearance.<>c__DisplayClass32_0 A_1)
		{
			return MathHelper.Clamp01(channel + (float)A_1.random.Next(-25, 25) / 100f);
		}

		// Token: 0x04000C0C RID: 3084
		private static IReadOnlyList<Color> RealisticEyeColors = new List<Color>
		{
			Color.Brown,
			Color.Gray,
			Color.Azure,
			Color.SteelBlue,
			Color.Black
		};
	}
}
