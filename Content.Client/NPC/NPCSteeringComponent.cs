using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.NPC
{
	// Token: 0x0200020B RID: 523
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class NPCSteeringComponent : Component
	{
		// Token: 0x040006C0 RID: 1728
		public Vector2 Direction;

		// Token: 0x040006C1 RID: 1729
		public float[] DangerMap = Array.Empty<float>();

		// Token: 0x040006C2 RID: 1730
		public float[] InterestMap = Array.Empty<float>();

		// Token: 0x040006C3 RID: 1731
		public List<Vector2> DangerPoints = new List<Vector2>();
	}
}
