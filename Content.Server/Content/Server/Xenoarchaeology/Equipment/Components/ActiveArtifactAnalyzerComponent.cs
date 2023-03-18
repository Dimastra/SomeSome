using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.Equipment.Components
{
	// Token: 0x02000065 RID: 101
	[RegisterComponent]
	public sealed class ActiveArtifactAnalyzerComponent : Component
	{
		// Token: 0x040000F4 RID: 244
		[DataField("startTime", false, 1, false, false, typeof(TimespanSerializer))]
		public TimeSpan StartTime;

		// Token: 0x040000F5 RID: 245
		[ViewVariables]
		public EntityUid Artifact;
	}
}
