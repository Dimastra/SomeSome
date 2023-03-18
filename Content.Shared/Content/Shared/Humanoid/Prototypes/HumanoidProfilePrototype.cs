using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Prototypes
{
	// Token: 0x02000414 RID: 1044
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("humanoidProfile", 1)]
	public sealed class HumanoidProfilePrototype : IPrototype
	{
		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000C4F RID: 3151 RVA: 0x00028B2D File Offset: 0x00026D2D
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000C50 RID: 3152 RVA: 0x00028B35 File Offset: 0x00026D35
		[DataField("profile", false, 1, false, false, null)]
		public HumanoidCharacterProfile Profile { get; } = HumanoidCharacterProfile.Default();

		// Token: 0x04000C40 RID: 3136
		[DataField("customBaseLayers", false, 1, false, false, null)]
		public Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> CustomBaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo>();
	}
}
