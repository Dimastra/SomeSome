using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.Equipment.Components
{
	// Token: 0x02000068 RID: 104
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ArtifactAnalyzerComponent : Component
	{
		// Token: 0x040000FC RID: 252
		[DataField("analysisDuration", false, 1, false, false, typeof(TimespanSerializer))]
		public TimeSpan AnalysisDuration = TimeSpan.FromSeconds(60.0);

		// Token: 0x040000FD RID: 253
		[ViewVariables]
		public float AnalysisDurationMulitplier = 1f;

		// Token: 0x040000FE RID: 254
		[DataField("machinePartAnalysisDuration", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartAnalysisDuration = "ScanningModule";

		// Token: 0x040000FF RID: 255
		[DataField("partRatingAnalysisDurationMultiplier", false, 1, false, false, null)]
		public float PartRatingAnalysisDurationMultiplier = 0.75f;

		// Token: 0x04000100 RID: 256
		[ViewVariables]
		public EntityUid? Console;

		// Token: 0x04000101 RID: 257
		[ViewVariables]
		public HashSet<EntityUid> Contacts = new HashSet<EntityUid>();

		// Token: 0x04000102 RID: 258
		[ViewVariables]
		public bool ReadyToPrint;

		// Token: 0x04000103 RID: 259
		[DataField("scanFinishedSound", false, 1, false, false, null)]
		public readonly SoundSpecifier ScanFinishedSound = new SoundPathSpecifier("/Audio/Machines/scan_finish.ogg", null);

		// Token: 0x04000104 RID: 260
		[ViewVariables]
		public EntityUid? LastAnalyzedArtifact;

		// Token: 0x04000105 RID: 261
		[Nullable(2)]
		[ViewVariables]
		public ArtifactNode LastAnalyzedNode;

		// Token: 0x04000106 RID: 262
		[ViewVariables]
		public int? LastAnalyzerPointValue;
	}
}
