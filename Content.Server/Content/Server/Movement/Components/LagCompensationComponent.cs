using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Server.Movement.Components
{
	// Token: 0x02000392 RID: 914
	[RegisterComponent]
	public sealed class LagCompensationComponent : Component
	{
		// Token: 0x04000B76 RID: 2934
		[Nullable(new byte[]
		{
			1,
			0
		})]
		[ViewVariables]
		public readonly Queue<ValueTuple<TimeSpan, EntityCoordinates, Angle>> Positions = new Queue<ValueTuple<TimeSpan, EntityCoordinates, Angle>>();
	}
}
