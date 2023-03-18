using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Gravity
{
	// Token: 0x02000443 RID: 1091
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedFloatingVisualizerSystem)
	})]
	public sealed class FloatingVisualsComponent : Component
	{
		// Token: 0x04000CC5 RID: 3269
		[ViewVariables]
		[DataField("animationTime", false, 1, false, false, null)]
		public float AnimationTime = 2f;

		// Token: 0x04000CC6 RID: 3270
		[ViewVariables]
		[DataField("offset", false, 1, false, false, null)]
		public Vector2 Offset = new Vector2(0f, 0.2f);

		// Token: 0x04000CC7 RID: 3271
		[ViewVariables]
		public bool CanFloat;

		// Token: 0x04000CC8 RID: 3272
		[Nullable(1)]
		public readonly string AnimationKey = "gravity";
	}
}
