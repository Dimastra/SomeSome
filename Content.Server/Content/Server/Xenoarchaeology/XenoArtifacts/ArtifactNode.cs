using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Xenoarchaeology.XenoArtifacts;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x0200001E RID: 30
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ArtifactNode : ICloneable
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00003A94 File Offset: 0x00001C94
		public object Clone()
		{
			return new ArtifactNode
			{
				Id = this.Id,
				Depth = this.Depth,
				Edges = this.Edges,
				Discovered = this.Discovered,
				Trigger = this.Trigger,
				Triggered = this.Triggered,
				Effect = this.Effect,
				NodeData = this.NodeData
			};
		}

		// Token: 0x0400004D RID: 77
		[ViewVariables]
		public int Id;

		// Token: 0x0400004E RID: 78
		[ViewVariables]
		public int Depth;

		// Token: 0x0400004F RID: 79
		[ViewVariables]
		public List<ArtifactNode> Edges = new List<ArtifactNode>();

		// Token: 0x04000050 RID: 80
		[ViewVariables]
		public bool Discovered;

		// Token: 0x04000051 RID: 81
		[ViewVariables]
		public ArtifactTriggerPrototype Trigger;

		// Token: 0x04000052 RID: 82
		[ViewVariables]
		public bool Triggered;

		// Token: 0x04000053 RID: 83
		[ViewVariables]
		public ArtifactEffectPrototype Effect;

		// Token: 0x04000054 RID: 84
		[ViewVariables]
		public Dictionary<string, object> NodeData = new Dictionary<string, object>();
	}
}
