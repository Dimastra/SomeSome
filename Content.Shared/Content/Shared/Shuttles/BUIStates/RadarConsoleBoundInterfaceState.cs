using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates
{
	// Token: 0x020001D4 RID: 468
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Virtual]
	[Serializable]
	public class RadarConsoleBoundInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06000547 RID: 1351 RVA: 0x00013A85 File Offset: 0x00011C85
		public RadarConsoleBoundInterfaceState(float maxRange, EntityCoordinates? coordinates, Angle? angle, List<DockingInterfaceState> docks)
		{
			this.MaxRange = maxRange;
			this.Coordinates = coordinates;
			this.Angle = angle;
			this.Docks = docks;
		}

		// Token: 0x04000544 RID: 1348
		public readonly float MaxRange;

		// Token: 0x04000545 RID: 1349
		public EntityCoordinates? Coordinates;

		// Token: 0x04000546 RID: 1350
		public Angle? Angle;

		// Token: 0x04000547 RID: 1351
		public readonly List<DockingInterfaceState> Docks;
	}
}
