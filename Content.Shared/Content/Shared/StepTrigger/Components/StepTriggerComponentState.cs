using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StepTrigger.Components
{
	// Token: 0x02000150 RID: 336
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StepTriggerComponentState : ComponentState
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000407 RID: 1031 RVA: 0x0001016D File Offset: 0x0000E36D
		public float IntersectRatio { get; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x00010175 File Offset: 0x0000E375
		public float RequiredTriggerSpeed { get; }

		// Token: 0x06000409 RID: 1033 RVA: 0x0001017D File Offset: 0x0000E37D
		public StepTriggerComponentState(float intersectRatio, HashSet<EntityUid> currentlySteppedOn, HashSet<EntityUid> colliding, float requiredTriggerSpeed, bool active)
		{
			this.IntersectRatio = intersectRatio;
			this.CurrentlySteppedOn = currentlySteppedOn;
			this.RequiredTriggerSpeed = requiredTriggerSpeed;
			this.Active = active;
			this.Colliding = colliding;
		}

		// Token: 0x040003E0 RID: 992
		public readonly HashSet<EntityUid> CurrentlySteppedOn;

		// Token: 0x040003E1 RID: 993
		public readonly HashSet<EntityUid> Colliding;

		// Token: 0x040003E2 RID: 994
		public readonly bool Active;
	}
}
