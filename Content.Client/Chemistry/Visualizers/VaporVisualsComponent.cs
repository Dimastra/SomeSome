using System;
using System.Runtime.CompilerServices;
using Robust.Client.Animations;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Chemistry.Visualizers
{
	// Token: 0x020003CF RID: 975
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(VaporVisualizerSystem)
	})]
	public sealed class VaporVisualsComponent : Component
	{
		// Token: 0x04000C48 RID: 3144
		public const string AnimationKey = "flick_animation";

		// Token: 0x04000C49 RID: 3145
		[DataField("animationTime", false, 1, false, false, null)]
		public float AnimationTime = 0.25f;

		// Token: 0x04000C4A RID: 3146
		[DataField("animationState", false, 1, false, false, null)]
		public string AnimationState = "chempuff";

		// Token: 0x04000C4B RID: 3147
		public Animation VaporFlick;
	}
}
