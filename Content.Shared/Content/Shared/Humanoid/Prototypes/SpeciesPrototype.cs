using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Humanoid.Prototypes
{
	// Token: 0x02000418 RID: 1048
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("species", 1)]
	public sealed class SpeciesPrototype : IPrototype
	{
		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000C62 RID: 3170 RVA: 0x00028C11 File Offset: 0x00026E11
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000C63 RID: 3171 RVA: 0x00028C19 File Offset: 0x00026E19
		[DataField("name", false, 1, true, false, null)]
		public string Name { get; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000C64 RID: 3172 RVA: 0x00028C21 File Offset: 0x00026E21
		[DataField("descriptor", false, 1, false, false, null)]
		public string Descriptor { get; } = "humanoid";

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000C65 RID: 3173 RVA: 0x00028C29 File Offset: 0x00026E29
		[DataField("roundStart", false, 1, true, false, null)]
		public bool RoundStart { get; }

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000C66 RID: 3174 RVA: 0x00028C31 File Offset: 0x00026E31
		[DataField("sprites", false, 1, false, false, null)]
		public string SpriteSet { get; }

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000C67 RID: 3175 RVA: 0x00028C39 File Offset: 0x00026E39
		[DataField("defaultSkinTone", false, 1, false, false, null)]
		public Color DefaultSkinTone { get; } = Color.White;

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000C68 RID: 3176 RVA: 0x00028C41 File Offset: 0x00026E41
		[DataField("defaultHumanSkinTone", false, 1, false, false, null)]
		public int DefaultHumanSkinTone { get; } = 20;

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000C69 RID: 3177 RVA: 0x00028C49 File Offset: 0x00026E49
		[DataField("markingLimits", false, 1, false, false, null)]
		public string MarkingPoints { get; }

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000C6A RID: 3178 RVA: 0x00028C51 File Offset: 0x00026E51
		[DataField("prototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype { get; }

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000C6B RID: 3179 RVA: 0x00028C59 File Offset: 0x00026E59
		[DataField("dollPrototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string DollPrototype { get; }

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000C6C RID: 3180 RVA: 0x00028C61 File Offset: 0x00026E61
		[DataField("skinColoration", false, 1, true, false, null)]
		public HumanoidSkinColor SkinColoration { get; }

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000C6D RID: 3181 RVA: 0x00028C69 File Offset: 0x00026E69
		[DataField("maleFirstNames", false, 1, false, false, null)]
		public string MaleFirstNames { get; } = "names_first_male";

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000C6E RID: 3182 RVA: 0x00028C71 File Offset: 0x00026E71
		[DataField("femaleFirstNames", false, 1, false, false, null)]
		public string FemaleFirstNames { get; } = "names_first_female";

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000C6F RID: 3183 RVA: 0x00028C79 File Offset: 0x00026E79
		[DataField("maleLastNames", false, 1, false, false, null)]
		public string MaleLastNames { get; } = "names_last_male";

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x00028C81 File Offset: 0x00026E81
		[DataField("femaleLastNames", false, 1, false, false, null)]
		public string FemaleLastNames { get; } = "names_last_female";

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000C71 RID: 3185 RVA: 0x00028C89 File Offset: 0x00026E89
		[DataField("naming", false, 1, false, false, null)]
		public SpeciesNaming Naming { get; }

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000C72 RID: 3186 RVA: 0x00028C91 File Offset: 0x00026E91
		[DataField("sexes", false, 1, false, false, null)]
		public List<Sex> Sexes { get; } = new List<Sex>
		{
			Sex.Male,
			Sex.Female
		};

		// Token: 0x04000C61 RID: 3169
		[DataField("minAge", false, 1, false, false, null)]
		public int MinAge = 18;

		// Token: 0x04000C62 RID: 3170
		[DataField("youngAge", false, 1, false, false, null)]
		public int YoungAge = 30;

		// Token: 0x04000C63 RID: 3171
		[DataField("oldAge", false, 1, false, false, null)]
		public int OldAge = 60;

		// Token: 0x04000C64 RID: 3172
		[DataField("maxAge", false, 1, false, false, null)]
		public int MaxAge = 120;
	}
}
