using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Atmos.Components
{
	// Token: 0x02000460 RID: 1120
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class FireVisualsComponent : Component
	{
		// Token: 0x04000DE4 RID: 3556
		[DataField("fireStackAlternateState", false, 1, false, false, null)]
		public int FireStackAlternateState = 3;

		// Token: 0x04000DE5 RID: 3557
		[DataField("normalState", false, 1, false, false, null)]
		public string NormalState;

		// Token: 0x04000DE6 RID: 3558
		[DataField("alternateState", false, 1, false, false, null)]
		public string AlternateState;

		// Token: 0x04000DE7 RID: 3559
		[DataField("sprite", false, 1, false, false, null)]
		public string Sprite;

		// Token: 0x04000DE8 RID: 3560
		[DataField("lightEnergyPerStack", false, 1, false, false, null)]
		public float LightEnergyPerStack = 0.5f;

		// Token: 0x04000DE9 RID: 3561
		[DataField("lightRadiusPerStack", false, 1, false, false, null)]
		public float LightRadiusPerStack = 0.3f;

		// Token: 0x04000DEA RID: 3562
		[DataField("maxLightEnergy", false, 1, false, false, null)]
		public float MaxLightEnergy = 10f;

		// Token: 0x04000DEB RID: 3563
		[DataField("maxLightRadius", false, 1, false, false, null)]
		public float MaxLightRadius = 4f;

		// Token: 0x04000DEC RID: 3564
		[DataField("lightColor", false, 1, false, false, null)]
		public Color LightColor = Color.Orange;

		// Token: 0x04000DED RID: 3565
		public EntityUid? LightEntity;
	}
}
