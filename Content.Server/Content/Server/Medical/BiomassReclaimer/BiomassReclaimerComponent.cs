using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Medical.BiomassReclaimer
{
	// Token: 0x020003C2 RID: 962
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BiomassReclaimerComponent : Component
	{
		// Token: 0x04000C26 RID: 3110
		[ViewVariables]
		public float RandomMessTimer;

		// Token: 0x04000C27 RID: 3111
		[ViewVariables]
		[DataField("randomMessInterval", false, 1, false, false, null)]
		public TimeSpan RandomMessInterval = TimeSpan.FromSeconds(5.0);

		// Token: 0x04000C28 RID: 3112
		[ViewVariables]
		public float ProcessingTimer;

		// Token: 0x04000C29 RID: 3113
		[ViewVariables]
		public int CurrentExpectedYield;

		// Token: 0x04000C2A RID: 3114
		[Nullable(2)]
		[ViewVariables]
		public string BloodReagent;

		// Token: 0x04000C2B RID: 3115
		public List<EntitySpawnEntry> SpawnedEntities = new List<EntitySpawnEntry>();

		// Token: 0x04000C2C RID: 3116
		[ViewVariables]
		public float YieldPerUnitMass;

		// Token: 0x04000C2D RID: 3117
		[DataField("baseYieldPerUnitMass", false, 1, false, false, null)]
		public float BaseYieldPerUnitMass = 0.4f;

		// Token: 0x04000C2E RID: 3118
		[DataField("machinePartYieldAmount", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartYieldAmount = "Manipulator";

		// Token: 0x04000C2F RID: 3119
		[DataField("partRatingYieldAmountMultiplier", false, 1, false, false, null)]
		public float PartRatingYieldAmountMultiplier = 1.25f;

		// Token: 0x04000C30 RID: 3120
		[ViewVariables]
		public float ProcessingTimePerUnitMass;

		// Token: 0x04000C31 RID: 3121
		[DataField("baseProcessingTimePerUnitMass", false, 1, false, false, null)]
		public float BaseProcessingTimePerUnitMass = 0.5f;

		// Token: 0x04000C32 RID: 3122
		[DataField("machinePartProcessSpeed", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartProcessingSpeed = "Laser";

		// Token: 0x04000C33 RID: 3123
		[DataField("partRatingSpeedMultiplier", false, 1, false, false, null)]
		public float PartRatingSpeedMultiplier = 1.35f;

		// Token: 0x04000C34 RID: 3124
		[ViewVariables]
		[DataField("safetyEnabled", false, 1, false, false, null)]
		public bool SafetyEnabled = true;
	}
}
