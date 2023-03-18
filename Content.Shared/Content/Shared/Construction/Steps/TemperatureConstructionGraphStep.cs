using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000577 RID: 1399
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class TemperatureConstructionGraphStep : ConstructionGraphStep
	{
		// Token: 0x0600111C RID: 4380 RVA: 0x000385F4 File Offset: 0x000367F4
		public override void DoExamine(ExaminedEvent examinedEvent)
		{
			float guideTemperature = (this.MinTemperature != null) ? this.MinTemperature.Value : ((this.MaxTemperature != null) ? this.MaxTemperature.Value : 0f);
			examinedEvent.PushMarkup(Loc.GetString("construction-temperature-default", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("temperature", guideTemperature)
			}));
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x00038668 File Offset: 0x00036868
		public override ConstructionGuideEntry GenerateGuideEntry()
		{
			float guideTemperature = (this.MinTemperature != null) ? this.MinTemperature.Value : ((this.MaxTemperature != null) ? this.MaxTemperature.Value : 0f);
			return new ConstructionGuideEntry
			{
				Localization = "construction-presenter-temperature-step",
				Arguments = new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("temperature", guideTemperature)
				}
			};
		}

		// Token: 0x04000FE4 RID: 4068
		[DataField("minTemperature", false, 1, false, false, null)]
		public float? MinTemperature;

		// Token: 0x04000FE5 RID: 4069
		[DataField("maxTemperature", false, 1, false, false, null)]
		public float? MaxTemperature;
	}
}
