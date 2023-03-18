using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
	// Token: 0x02000522 RID: 1314
	[RegisterComponent]
	public sealed class TriggerOnTimedCollideComponent : Component
	{
		// Token: 0x0400118D RID: 4493
		[ViewVariables]
		[DataField("threshold", false, 1, false, false, null)]
		public float Threshold;

		// Token: 0x0400118E RID: 4494
		[Nullable(1)]
		[ViewVariables]
		public readonly Dictionary<EntityUid, float> Colliding = new Dictionary<EntityUid, float>();
	}
}
