using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.Equipment.Components
{
	// Token: 0x02000067 RID: 103
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AnalysisConsoleComponent : Component
	{
		// Token: 0x040000F8 RID: 248
		[ViewVariables]
		public EntityUid? AnalyzerEntity;

		// Token: 0x040000F9 RID: 249
		[DataField("linkingPort", false, 1, false, false, typeof(PrototypeIdSerializer<TransmitterPortPrototype>))]
		public readonly string LinkingPort = "ArtifactAnalyzerSender";

		// Token: 0x040000FA RID: 250
		[DataField("destroySound", false, 1, false, false, null)]
		public SoundSpecifier DestroySound = new SoundPathSpecifier("/Audio/Effects/radpulse11.ogg", null);

		// Token: 0x040000FB RID: 251
		[DataField("reportEntityId", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ReportEntityId = "Paper";
	}
}
