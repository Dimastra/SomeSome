using System;
using System.Runtime.CompilerServices;
using Content.Shared.Cloning;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Materials;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Cloning.Components
{
	// Token: 0x02000645 RID: 1605
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class CloningPodComponent : Component
	{
		// Token: 0x040014F7 RID: 5367
		public const string PodPort = "CloningPodReceiver";

		// Token: 0x040014F8 RID: 5368
		[ViewVariables]
		public ContainerSlot BodyContainer;

		// Token: 0x040014F9 RID: 5369
		[ViewVariables]
		public float CloningProgress;

		// Token: 0x040014FA RID: 5370
		[ViewVariables]
		public int UsedBiomass = 70;

		// Token: 0x040014FB RID: 5371
		[ViewVariables]
		public bool FailedClone;

		// Token: 0x040014FC RID: 5372
		[DataField("requiredMaterial", false, 1, false, false, typeof(PrototypeIdSerializer<MaterialPrototype>))]
		[ViewVariables]
		public string RequiredMaterial = "Biomass";

		// Token: 0x040014FD RID: 5373
		[DataField("baseCloningTime", false, 1, false, false, null)]
		public float BaseCloningTime = 30f;

		// Token: 0x040014FE RID: 5374
		[DataField("partRatingSpeedMultiplier", false, 1, false, false, null)]
		public float PartRatingSpeedMultiplier = 0.75f;

		// Token: 0x040014FF RID: 5375
		[DataField("machinePartCloningSpeed", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartCloningSpeed = "ScanningModule";

		// Token: 0x04001500 RID: 5376
		[ViewVariables]
		public float CloningTime = 30f;

		// Token: 0x04001501 RID: 5377
		[DataField("partRatingMaterialMultiplier", false, 1, false, false, null)]
		public float PartRatingMaterialMultiplier = 0.85f;

		// Token: 0x04001502 RID: 5378
		[ViewVariables]
		public float BiomassRequirementMultiplier = 1f;

		// Token: 0x04001503 RID: 5379
		[DataField("machinePartMaterialUse", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartMaterialUse = "Manipulator";

		// Token: 0x04001504 RID: 5380
		[ViewVariables]
		public CloningPodStatus Status;

		// Token: 0x04001505 RID: 5381
		[ViewVariables]
		public EntityUid? ConnectedConsole;
	}
}
