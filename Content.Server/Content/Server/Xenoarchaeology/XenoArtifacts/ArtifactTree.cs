using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x0200001D RID: 29
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ArtifactTree
	{
		// Token: 0x0400004B RID: 75
		[ViewVariables]
		public ArtifactNode StartNode;

		// Token: 0x0400004C RID: 76
		[ViewVariables]
		public readonly List<ArtifactNode> AllNodes = new List<ArtifactNode>();
	}
}
