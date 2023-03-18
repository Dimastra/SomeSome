using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Reaction
{
	// Token: 0x020005EB RID: 1515
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ReactionMixerComponent : Component
	{
		// Token: 0x0400112A RID: 4394
		[ViewVariables]
		[DataField("reactionTypes", false, 1, false, false, null)]
		public List<string> ReactionTypes;

		// Token: 0x0400112B RID: 4395
		[ViewVariables]
		[DataField("mixMessage", false, 1, false, false, null)]
		public string MixMessage = "default-mixing-success";
	}
}
