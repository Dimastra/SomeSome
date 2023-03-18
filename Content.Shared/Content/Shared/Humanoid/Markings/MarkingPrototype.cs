using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x02000420 RID: 1056
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("marking", 1)]
	public sealed class MarkingPrototype : IPrototype
	{
		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06000C8F RID: 3215 RVA: 0x000293F3 File Offset: 0x000275F3
		[IdDataField(1, null)]
		public string ID { get; } = "uwu";

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x000293FB File Offset: 0x000275FB
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x00029403 File Offset: 0x00027603
		public string Name { get; private set; }

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x0002940C File Offset: 0x0002760C
		[DataField("bodyPart", false, 1, true, false, null)]
		public HumanoidVisualLayers BodyPart { get; }

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000C93 RID: 3219 RVA: 0x00029414 File Offset: 0x00027614
		[DataField("markingCategory", false, 1, true, false, null)]
		public MarkingCategories MarkingCategory { get; }

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000C94 RID: 3220 RVA: 0x0002941C File Offset: 0x0002761C
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("speciesRestriction", false, 1, false, false, null)]
		public List<string> SpeciesRestrictions { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000C95 RID: 3221 RVA: 0x00029424 File Offset: 0x00027624
		[DataField("followSkinColor", false, 1, false, false, null)]
		public bool FollowSkinColor { get; }

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06000C96 RID: 3222 RVA: 0x0002942C File Offset: 0x0002762C
		// (set) Token: 0x06000C97 RID: 3223 RVA: 0x00029434 File Offset: 0x00027634
		[DataField("sprites", false, 1, true, false, null)]
		public List<SpriteSpecifier> Sprites { get; private set; }

		// Token: 0x06000C98 RID: 3224 RVA: 0x0002943D File Offset: 0x0002763D
		public Marking AsMarking()
		{
			return new Marking(this.ID, this.Sprites.Count);
		}

		// Token: 0x04000C88 RID: 3208
		[DataField("sponsorOnly", false, 1, false, false, null)]
		public bool SponsorOnly;
	}
}
