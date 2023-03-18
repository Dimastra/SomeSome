using System;
using System.Runtime.CompilerServices;
using Content.Shared.Materials;
using Content.Shared.Radio;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Anomaly.Components
{
	// Token: 0x020007C9 RID: 1993
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AnomalyGeneratorComponent : Component
	{
		// Token: 0x04001ACC RID: 6860
		[DataField("cooldownEndTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan CooldownEndTime = TimeSpan.Zero;

		// Token: 0x04001ACD RID: 6861
		[DataField("cooldownLength", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan CooldownLength = TimeSpan.FromMinutes(5.0);

		// Token: 0x04001ACE RID: 6862
		[DataField("generationLength", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan GenerationLength = TimeSpan.FromSeconds(8.0);

		// Token: 0x04001ACF RID: 6863
		[DataField("requiredMaterial", false, 1, false, false, typeof(PrototypeIdSerializer<MaterialPrototype>))]
		[ViewVariables]
		public string RequiredMaterial = "Plasma";

		// Token: 0x04001AD0 RID: 6864
		[DataField("materialPerAnomaly", false, 1, false, false, null)]
		[ViewVariables]
		public int MaterialPerAnomaly = 1500;

		// Token: 0x04001AD1 RID: 6865
		[DataField("spawnerPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		[ViewVariables]
		public string SpawnerPrototype = "RandomAnomalySpawner";

		// Token: 0x04001AD2 RID: 6866
		[DataField("scienceChannel", false, 1, false, false, typeof(PrototypeIdSerializer<RadioChannelPrototype>))]
		public string ScienceChannel = "Science";

		// Token: 0x04001AD3 RID: 6867
		[Nullable(2)]
		[DataField("generatingSound", false, 1, false, false, null)]
		public SoundSpecifier GeneratingSound;

		// Token: 0x04001AD4 RID: 6868
		[Nullable(2)]
		[DataField("generatingFinishedSound", false, 1, false, false, null)]
		public SoundSpecifier GeneratingFinishedSound;
	}
}
