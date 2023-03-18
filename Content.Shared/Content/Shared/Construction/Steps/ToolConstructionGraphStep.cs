using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Tools;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Construction.Steps
{
	// Token: 0x02000578 RID: 1400
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ToolConstructionGraphStep : ConstructionGraphStep
	{
		// Token: 0x17000364 RID: 868
		// (get) Token: 0x0600111F RID: 4383 RVA: 0x000386EC File Offset: 0x000368EC
		[DataField("tool", false, 1, true, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string Tool { get; } = string.Empty;

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06001120 RID: 4384 RVA: 0x000386F4 File Offset: 0x000368F4
		[DataField("fuel", false, 1, false, false, null)]
		public float Fuel { get; } = 10f;

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06001121 RID: 4385 RVA: 0x000386FC File Offset: 0x000368FC
		[DataField("examine", false, 1, false, false, null)]
		public string ExamineOverride { get; } = string.Empty;

		// Token: 0x06001122 RID: 4386 RVA: 0x00038704 File Offset: 0x00036904
		public override void DoExamine(ExaminedEvent examinedEvent)
		{
			if (!string.IsNullOrEmpty(this.ExamineOverride))
			{
				examinedEvent.PushMarkup(Loc.GetString(this.ExamineOverride));
				return;
			}
			ToolQualityPrototype quality;
			if (string.IsNullOrEmpty(this.Tool) || !IoCManager.Resolve<IPrototypeManager>().TryIndex<ToolQualityPrototype>(this.Tool, ref quality))
			{
				return;
			}
			examinedEvent.PushMarkup(Loc.GetString("construction-use-tool-entity", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("toolName", Loc.GetString(quality.ToolName))
			}));
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00038784 File Offset: 0x00036984
		public override ConstructionGuideEntry GenerateGuideEntry()
		{
			ToolQualityPrototype quality = IoCManager.Resolve<IPrototypeManager>().Index<ToolQualityPrototype>(this.Tool);
			return new ConstructionGuideEntry
			{
				Localization = "construction-presenter-tool-step",
				Arguments = new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("tool", quality.ToolName)
				},
				Icon = quality.Icon
			};
		}
	}
}
