using System;
using System.Runtime.CompilerServices;
using Content.Shared.Pointing.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Pointing.Components
{
	// Token: 0x020001B3 RID: 435
	[RegisterComponent]
	public sealed class PointingArrowComponent : SharedPointingArrowComponent
	{
		// Token: 0x04000584 RID: 1412
		[ViewVariables]
		[DataField("animationTime", false, 1, false, false, null)]
		public readonly float AnimationTime = 0.5f;

		// Token: 0x04000585 RID: 1413
		[ViewVariables]
		[DataField("offset", false, 1, false, false, null)]
		public readonly Vector2 Offset = new ValueTuple<float, float>(0f, 0.25f);

		// Token: 0x04000586 RID: 1414
		[Nullable(1)]
		public readonly string AnimationKey = "pointingarrow";
	}
}
