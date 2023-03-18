using System;
using Content.Server.Pointing.EntitySystems;
using Content.Shared.Pointing.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Pointing.Components
{
	// Token: 0x020002CF RID: 719
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RoguePointingSystem)
	})]
	public sealed class RoguePointingArrowComponent : SharedRoguePointingArrowComponent
	{
		// Token: 0x0400088D RID: 2189
		[ViewVariables]
		public EntityUid? Chasing;

		// Token: 0x0400088E RID: 2190
		[ViewVariables]
		[DataField("turningDelay", false, 1, false, false, null)]
		public float TurningDelay = 2f;

		// Token: 0x0400088F RID: 2191
		[ViewVariables]
		[DataField("chasingSpeed", false, 1, false, false, null)]
		public float ChasingSpeed = 5f;

		// Token: 0x04000890 RID: 2192
		[ViewVariables]
		[DataField("chasingTime", false, 1, false, false, null)]
		public float ChasingTime = 1f;
	}
}
