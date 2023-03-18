using System;
using System.Runtime.CompilerServices;
using Content.Shared.Explosion;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
	// Token: 0x02000519 RID: 1305
	[RegisterComponent]
	public sealed class ExplosiveComponent : Component
	{
		// Token: 0x0400116C RID: 4460
		[Nullable(1)]
		[ViewVariables]
		[DataField("explosionType", false, 1, true, false, typeof(PrototypeIdSerializer<ExplosionPrototype>))]
		public string ExplosionType;

		// Token: 0x0400116D RID: 4461
		[ViewVariables]
		[DataField("maxIntensity", false, 1, false, false, null)]
		public float MaxIntensity = 4f;

		// Token: 0x0400116E RID: 4462
		[ViewVariables]
		[DataField("intensitySlope", false, 1, false, false, null)]
		public float IntensitySlope = 1f;

		// Token: 0x0400116F RID: 4463
		[ViewVariables]
		[DataField("totalIntensity", false, 1, false, false, null)]
		public float TotalIntensity = 10f;

		// Token: 0x04001170 RID: 4464
		[ViewVariables]
		[DataField("tileBreakScale", false, 1, false, false, null)]
		public float TileBreakScale = 1f;

		// Token: 0x04001171 RID: 4465
		[ViewVariables]
		[DataField("maxTileBreak", false, 1, false, false, null)]
		public int MaxTileBreak = int.MaxValue;

		// Token: 0x04001172 RID: 4466
		[ViewVariables]
		[DataField("canCreateVacuum", false, 1, false, false, null)]
		public bool CanCreateVacuum = true;

		// Token: 0x04001173 RID: 4467
		public bool Exploded;

		// Token: 0x04001174 RID: 4468
		[ViewVariables]
		[DataField("canShakeGrid", false, 1, false, false, null)]
		public bool CanShakeGrid;
	}
}
