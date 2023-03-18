using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StepTrigger.Components
{
	// Token: 0x0200014E RID: 334
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(StepTriggerSystem)
	})]
	public sealed class StepTriggerComponent : Component
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x000100F7 File Offset: 0x0000E2F7
		// (set) Token: 0x06000400 RID: 1024 RVA: 0x000100FF File Offset: 0x0000E2FF
		[DataField("active", false, 1, false, false, null)]
		public bool Active { get; set; } = true;

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x00010108 File Offset: 0x0000E308
		// (set) Token: 0x06000402 RID: 1026 RVA: 0x00010110 File Offset: 0x0000E310
		[DataField("intersectRatio", false, 1, false, false, null)]
		public float IntersectRatio { get; set; } = 0.3f;

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x00010119 File Offset: 0x0000E319
		// (set) Token: 0x06000404 RID: 1028 RVA: 0x00010121 File Offset: 0x0000E321
		[DataField("requiredTriggeredSpeed", false, 1, false, false, null)]
		public float RequiredTriggerSpeed { get; set; } = 3.5f;

		// Token: 0x040003D9 RID: 985
		[ViewVariables]
		public readonly HashSet<EntityUid> Colliding = new HashSet<EntityUid>();

		// Token: 0x040003DA RID: 986
		[ViewVariables]
		public readonly HashSet<EntityUid> CurrentlySteppedOn = new HashSet<EntityUid>();
	}
}
