using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Damage
{
	// Token: 0x02000365 RID: 869
	[DataDefinition]
	public sealed class DamageVisualizerSprite
	{
		// Token: 0x04000B40 RID: 2880
		[Nullable(1)]
		[DataField("sprite", false, 1, true, false, null)]
		public readonly string Sprite;

		// Token: 0x04000B41 RID: 2881
		[Nullable(2)]
		[DataField("color", false, 1, false, false, null)]
		public readonly string Color;
	}
}
