using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Damage
{
	// Token: 0x02000364 RID: 868
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DamageVisualsComponent : Component
	{
		// Token: 0x04000B30 RID: 2864
		[DataField("thresholds", false, 1, true, false, null)]
		public List<FixedPoint2> Thresholds = new List<FixedPoint2>();

		// Token: 0x04000B31 RID: 2865
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("targetLayers", false, 1, false, false, null)]
		public List<Enum> TargetLayers;

		// Token: 0x04000B32 RID: 2866
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("damageOverlayGroups", false, 1, false, false, null)]
		public readonly Dictionary<string, DamageVisualizerSprite> DamageOverlayGroups;

		// Token: 0x04000B33 RID: 2867
		[DataField("overlay", false, 1, false, false, null)]
		public readonly bool Overlay = true;

		// Token: 0x04000B34 RID: 2868
		[Nullable(2)]
		[DataField("damageGroup", false, 1, false, false, null)]
		public readonly string DamageGroup;

		// Token: 0x04000B35 RID: 2869
		[DataField("damageDivisor", false, 1, false, false, null)]
		public float Divisor = 1f;

		// Token: 0x04000B36 RID: 2870
		[DataField("trackAllDamage", false, 1, false, false, null)]
		public readonly bool TrackAllDamage;

		// Token: 0x04000B37 RID: 2871
		[Nullable(2)]
		[DataField("damageOverlay", false, 1, false, false, null)]
		public readonly DamageVisualizerSprite DamageOverlay;

		// Token: 0x04000B38 RID: 2872
		public readonly List<Enum> TargetLayerMapKeys = new List<Enum>();

		// Token: 0x04000B39 RID: 2873
		public bool Disabled;

		// Token: 0x04000B3A RID: 2874
		public bool Valid = true;

		// Token: 0x04000B3B RID: 2875
		public FixedPoint2 LastDamageThreshold = FixedPoint2.Zero;

		// Token: 0x04000B3C RID: 2876
		public readonly Dictionary<object, bool> DisabledLayers = new Dictionary<object, bool>();

		// Token: 0x04000B3D RID: 2877
		public readonly Dictionary<object, string> LayerMapKeyStates = new Dictionary<object, string>();

		// Token: 0x04000B3E RID: 2878
		public readonly Dictionary<string, FixedPoint2> LastThresholdPerGroup = new Dictionary<string, FixedPoint2>();

		// Token: 0x04000B3F RID: 2879
		public string TopMostLayerKey;
	}
}
