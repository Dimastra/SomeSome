using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Shared.Atmos.Components
{
	// Token: 0x020006E1 RID: 1761
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class GasTileOverlayComponent : Component
	{
		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06001560 RID: 5472 RVA: 0x00045E88 File Offset: 0x00044088
		// (set) Token: 0x06001561 RID: 5473 RVA: 0x00045E90 File Offset: 0x00044090
		public GameTick ForceTick { get; set; }

		// Token: 0x0400157C RID: 5500
		public readonly HashSet<Vector2i> InvalidTiles = new HashSet<Vector2i>();

		// Token: 0x0400157D RID: 5501
		public readonly Dictionary<Vector2i, GasOverlayChunk> Chunks = new Dictionary<Vector2i, GasOverlayChunk>();
	}
}
