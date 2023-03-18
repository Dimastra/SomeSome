using System;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Fluids
{
	// Token: 0x02000312 RID: 786
	[RegisterComponent]
	public sealed class PuddleVisualizerComponent : Component
	{
		// Token: 0x040009F3 RID: 2547
		[DataField("recolor", false, 1, false, false, null)]
		public bool Recolor = true;

		// Token: 0x040009F4 RID: 2548
		[DataField("customPuddleSprite", false, 1, false, false, null)]
		public bool CustomPuddleSprite;

		// Token: 0x040009F5 RID: 2549
		[Nullable(2)]
		[DataField("originalRsi", false, 1, false, false, null)]
		public RSI OriginalRsi;

		// Token: 0x040009F6 RID: 2550
		[DataField("wetFloorEffectThreshold", false, 1, false, false, null)]
		public FixedPoint2 WetFloorEffectThreshold = FixedPoint2.New(5);

		// Token: 0x040009F7 RID: 2551
		[DataField("wetFloorEffectAlpha", false, 1, false, false, null)]
		public float WetFloorEffectAlpha = 0.75f;
	}
}
