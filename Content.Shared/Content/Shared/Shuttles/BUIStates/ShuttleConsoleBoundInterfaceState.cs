using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates
{
	// Token: 0x020001D7 RID: 471
	[NetSerializable]
	[Serializable]
	public sealed class ShuttleConsoleBoundInterfaceState : RadarConsoleBoundInterfaceState
	{
		// Token: 0x06000549 RID: 1353 RVA: 0x00013AB2 File Offset: 0x00011CB2
		[NullableContext(1)]
		public ShuttleConsoleBoundInterfaceState(FTLState ftlState, TimeSpan ftlTime, [TupleElementNames(new string[]
		{
			"Entity",
			"Destination",
			"Enabled"
		})] [Nullable(new byte[]
		{
			1,
			0,
			1
		})] List<ValueTuple<EntityUid, string, bool>> destinations, float maxRange, EntityCoordinates? coordinates, Angle? angle, List<DockingInterfaceState> docks) : base(maxRange, coordinates, angle, docks)
		{
			this.FTLState = ftlState;
			this.FTLTime = ftlTime;
			this.Destinations = destinations;
		}

		// Token: 0x04000550 RID: 1360
		public readonly FTLState FTLState;

		// Token: 0x04000551 RID: 1361
		public readonly TimeSpan FTLTime;

		// Token: 0x04000552 RID: 1362
		[TupleElementNames(new string[]
		{
			"Entity",
			"Destination",
			"Enabled"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public List<ValueTuple<EntityUid, string, bool>> Destinations;
	}
}
