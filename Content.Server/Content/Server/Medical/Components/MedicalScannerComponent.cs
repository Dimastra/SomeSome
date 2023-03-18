using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Content.Shared.MedicalScanner;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Medical.Components
{
	// Token: 0x020003BE RID: 958
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MedicalScannerComponent : SharedMedicalScannerComponent
	{
		// Token: 0x04000C1A RID: 3098
		public const string ScannerPort = "MedicalScannerReceiver";

		// Token: 0x04000C1B RID: 3099
		public ContainerSlot BodyContainer;

		// Token: 0x04000C1C RID: 3100
		public EntityUid? ConnectedConsole;

		// Token: 0x04000C1D RID: 3101
		[ViewVariables]
		public float CloningFailChanceMultiplier = 1f;

		// Token: 0x04000C1E RID: 3102
		[DataField("machinePartCloningFailChance", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartCloningFailChance = "ScanningModule";

		// Token: 0x04000C1F RID: 3103
		[DataField("partRatingCloningFailChanceMultiplier", false, 1, false, false, null)]
		public float PartRatingFailMultiplier = 0.75f;
	}
}
