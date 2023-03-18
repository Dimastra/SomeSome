using System;
using System.Runtime.CompilerServices;
using Robust.Client.Animations;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Chemistry.Visualizers
{
	// Token: 0x020003C8 RID: 968
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FoamVisualizerSystem)
	})]
	public sealed class FoamVisualsComponent : Component
	{
		// Token: 0x04000C34 RID: 3124
		public const string AnimationKey = "foamdissolve_animation";

		// Token: 0x04000C35 RID: 3125
		[DataField("animationTime", false, 1, false, false, null)]
		public float AnimationTime = 0.6f;

		// Token: 0x04000C36 RID: 3126
		[DataField("animationState", false, 1, false, false, null)]
		public string State = "foam-dissolve";

		// Token: 0x04000C37 RID: 3127
		[ViewVariables]
		public Animation Animation;
	}
}
