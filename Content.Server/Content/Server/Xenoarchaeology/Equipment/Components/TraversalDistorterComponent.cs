using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.Equipment.Components
{
	// Token: 0x0200006C RID: 108
	[RegisterComponent]
	public sealed class TraversalDistorterComponent : Component
	{
		// Token: 0x04000108 RID: 264
		[ViewVariables]
		public float BiasChance;

		// Token: 0x04000109 RID: 265
		[DataField("baseBiasChance", false, 1, false, false, null)]
		public float BaseBiasChance = 0.7f;

		// Token: 0x0400010A RID: 266
		[Nullable(1)]
		[DataField("machinePartBiasChance", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartBiasChance = "ScanningModule";

		// Token: 0x0400010B RID: 267
		[DataField("partRatingBiasChance", false, 1, false, false, null)]
		public float PartRatingBiasChance = 1.1f;

		// Token: 0x0400010C RID: 268
		[ViewVariables]
		public BiasDirection BiasDirection;

		// Token: 0x0400010D RID: 269
		public TimeSpan NextActivation;

		// Token: 0x0400010E RID: 270
		public TimeSpan ActivationDelay = TimeSpan.FromSeconds(1.0);
	}
}
