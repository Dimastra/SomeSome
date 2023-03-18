using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Light.Component
{
	// Token: 0x0200036B RID: 875
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class LightBulbComponent : Component
	{
		// Token: 0x04000A05 RID: 2565
		[DataField("color", false, 1, false, false, null)]
		[ViewVariables]
		public Color Color = Color.White;

		// Token: 0x04000A06 RID: 2566
		[DataField("bulb", false, 1, false, false, null)]
		[ViewVariables]
		public LightBulbType Type = LightBulbType.Tube;

		// Token: 0x04000A07 RID: 2567
		[DataField("startingState", false, 1, false, false, null)]
		public LightBulbState State;

		// Token: 0x04000A08 RID: 2568
		[DataField("BurningTemperature", false, 1, false, false, null)]
		[ViewVariables]
		public int BurningTemperature = 1400;

		// Token: 0x04000A09 RID: 2569
		[DataField("lightEnergy", false, 1, false, false, null)]
		[ViewVariables]
		public float LightEnergy = 0.8f;

		// Token: 0x04000A0A RID: 2570
		[DataField("lightRadius", false, 1, false, false, null)]
		[ViewVariables]
		public float LightRadius = 10f;

		// Token: 0x04000A0B RID: 2571
		[DataField("lightSoftness", false, 1, false, false, null)]
		[ViewVariables]
		public float LightSoftness = 1f;

		// Token: 0x04000A0C RID: 2572
		[DataField("PowerUse", false, 1, false, false, null)]
		[ViewVariables]
		public int PowerUse = 60;

		// Token: 0x04000A0D RID: 2573
		[DataField("breakSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier BreakSound = new SoundCollectionSpecifier("GlassBreak", null);

		// Token: 0x04000A0E RID: 2574
		[DataField("normalSpriteState", false, 1, false, false, null)]
		[ViewVariables]
		public string NormalSpriteState = "normal";

		// Token: 0x04000A0F RID: 2575
		[DataField("brokenSpriteState", false, 1, false, false, null)]
		[ViewVariables]
		public string BrokenSpriteState = "broken";

		// Token: 0x04000A10 RID: 2576
		[DataField("burnedSpriteState", false, 1, false, false, null)]
		[ViewVariables]
		public string BurnedSpriteState = "burned";
	}
}
