using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Electrocution
{
	// Token: 0x02000536 RID: 1334
	[RegisterComponent]
	public sealed class RandomInsulationComponent : Component
	{
		// Token: 0x040011EC RID: 4588
		[Nullable(1)]
		[DataField("list", false, 1, false, false, null)]
		public readonly float[] List = new float[1];
	}
}
