using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Rotation
{
	// Token: 0x02000166 RID: 358
	[RegisterComponent]
	public sealed class RotationVisualsComponent : Component
	{
		// Token: 0x040004B4 RID: 1204
		public static readonly Angle DefaultRotation = Angle.FromDegrees(90.0);

		// Token: 0x040004B5 RID: 1205
		[ViewVariables]
		public Angle VerticalRotation = 0f;

		// Token: 0x040004B6 RID: 1206
		[ViewVariables]
		public Angle HorizontalRotation = RotationVisualsComponent.DefaultRotation;

		// Token: 0x040004B7 RID: 1207
		[ViewVariables]
		public float AnimationTime = 0.125f;
	}
}
