using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x0200056F RID: 1391
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class ArbitraryInsertConstructionGraphStep : EntityInsertConstructionGraphStep
	{
		// Token: 0x1700035C RID: 860
		// (get) Token: 0x060010F9 RID: 4345 RVA: 0x00038048 File Offset: 0x00036248
		// (set) Token: 0x060010FA RID: 4346 RVA: 0x00038050 File Offset: 0x00036250
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = string.Empty;

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x060010FB RID: 4347 RVA: 0x00038059 File Offset: 0x00036259
		// (set) Token: 0x060010FC RID: 4348 RVA: 0x00038061 File Offset: 0x00036261
		[Nullable(2)]
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x060010FD RID: 4349 RVA: 0x0003806C File Offset: 0x0003626C
		public override void DoExamine(ExaminedEvent examinedEvent)
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return;
			}
			examinedEvent.Message.AddMarkup(Loc.GetString("construction-insert-arbitrary-entity", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("stepName", this.Name)
			}));
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x000380BC File Offset: 0x000362BC
		public override ConstructionGuideEntry GenerateGuideEntry()
		{
			return new ConstructionGuideEntry
			{
				Localization = "construction-presenter-arbitrary-step",
				Arguments = new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", this.Name)
				},
				Icon = this.Icon
			};
		}
	}
}
