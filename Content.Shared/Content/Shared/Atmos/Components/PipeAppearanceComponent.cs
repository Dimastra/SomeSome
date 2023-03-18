using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Atmos.Components
{
	// Token: 0x020006E3 RID: 1763
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PipeAppearanceComponent : Component
	{
		// Token: 0x04001581 RID: 5505
		[DataField("rsi", false, 1, false, false, null)]
		public string RsiPath = "Structures/Piping/Atmospherics/pipe.rsi";

		// Token: 0x04001582 RID: 5506
		[DataField("baseState", false, 1, false, false, null)]
		public string State = "pipeConnector";
	}
}
