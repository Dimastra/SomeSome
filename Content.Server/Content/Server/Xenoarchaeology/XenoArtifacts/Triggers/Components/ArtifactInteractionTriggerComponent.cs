using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x02000036 RID: 54
	[RegisterComponent]
	public sealed class ArtifactInteractionTriggerComponent : Component
	{
		// Token: 0x04000081 RID: 129
		[DataField("emptyHandActivation", false, 1, false, false, null)]
		[ViewVariables]
		public bool EmptyHandActivation = true;

		// Token: 0x04000082 RID: 130
		[DataField("attackActivation", false, 1, false, false, null)]
		[ViewVariables]
		public bool AttackActivation = true;

		// Token: 0x04000083 RID: 131
		[DataField("pullActivation", false, 1, false, false, null)]
		[ViewVariables]
		public bool PullActivation = true;
	}
}
