using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.Equipment.Components
{
	// Token: 0x02000069 RID: 105
	[RegisterComponent]
	public sealed class BiasedArtifactComponent : Component
	{
		// Token: 0x04000107 RID: 263
		[ViewVariables]
		public EntityUid Provider;
	}
}
