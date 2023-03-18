using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Radiation.Components
{
	// Token: 0x02000268 RID: 616
	[RegisterComponent]
	[Access]
	public sealed class RadiationGridResistanceComponent : Component
	{
		// Token: 0x0400079E RID: 1950
		[Nullable(1)]
		public readonly Dictionary<Vector2i, float> ResistancePerTile = new Dictionary<Vector2i, float>();
	}
}
