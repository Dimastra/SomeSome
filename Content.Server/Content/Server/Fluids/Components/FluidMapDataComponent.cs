using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Fluids.Components
{
	// Token: 0x020004F6 RID: 1270
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FluidSpreaderSystem)
	})]
	public sealed class FluidMapDataComponent : Component
	{
		// Token: 0x06001A29 RID: 6697 RVA: 0x0008A0A8 File Offset: 0x000882A8
		public void UpdateGoal(TimeSpan? start = null)
		{
			this.GoalTime = (start ?? this.GoalTime) + this.Delay;
		}

		// Token: 0x04001080 RID: 4224
		[DataField("goalTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan GoalTime;

		// Token: 0x04001081 RID: 4225
		[DataField("delay", false, 1, false, false, null)]
		public TimeSpan Delay = TimeSpan.FromSeconds(2.0);

		// Token: 0x04001082 RID: 4226
		[Nullable(1)]
		[DataField("puddles", false, 1, false, false, null)]
		public HashSet<EntityUid> Puddles = new HashSet<EntityUid>();
	}
}
