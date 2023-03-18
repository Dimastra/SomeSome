using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Light;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Light.Visualizers
{
	// Token: 0x02000268 RID: 616
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(PoweredLightVisualizerSystem)
	})]
	public sealed class PoweredLightVisualsComponent : Component
	{
		// Token: 0x06000FBF RID: 4031 RVA: 0x0005EF1C File Offset: 0x0005D11C
		public PoweredLightVisualsComponent()
		{
			Dictionary<PoweredLightState, string> dictionary = new Dictionary<PoweredLightState, string>();
			dictionary[PoweredLightState.Empty] = "empty";
			dictionary[PoweredLightState.Off] = "off";
			dictionary[PoweredLightState.On] = "on";
			dictionary[PoweredLightState.Broken] = "broken";
			dictionary[PoweredLightState.Burned] = "burn";
			this.SpriteStateMap = dictionary;
			this.MinBlinkingAnimationCycleTime = 0.5f;
			this.MaxBlinkingAnimationCycleTime = 2f;
			base..ctor();
		}

		// Token: 0x040007BF RID: 1983
		[DataField("spriteStateMap", false, 1, false, false, null)]
		[ViewVariables]
		public readonly Dictionary<PoweredLightState, string> SpriteStateMap;

		// Token: 0x040007C0 RID: 1984
		[ViewVariables]
		public const string BlinkingAnimationKey = "poweredlight_blinking";

		// Token: 0x040007C1 RID: 1985
		[DataField("minBlinkingTime", false, 1, false, false, null)]
		[ViewVariables]
		public float MinBlinkingAnimationCycleTime;

		// Token: 0x040007C2 RID: 1986
		[DataField("maxBlinkingTime", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxBlinkingAnimationCycleTime;

		// Token: 0x040007C3 RID: 1987
		[Nullable(2)]
		[DataField("blinkingSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier BlinkingSound;

		// Token: 0x040007C4 RID: 1988
		[ViewVariables]
		public bool IsBlinking;
	}
}
