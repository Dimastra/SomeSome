using System;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Fluids.Components
{
	// Token: 0x020004F7 RID: 1271
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(PuddleSystem)
	})]
	public sealed class PuddleComponent : Component
	{
		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06001A2B RID: 6699 RVA: 0x0008A107 File Offset: 0x00088307
		// (set) Token: 0x06001A2C RID: 6700 RVA: 0x0008A10F File Offset: 0x0008830F
		[DataField("solution", false, 1, false, false, null)]
		public string SolutionName { get; set; } = "puddle";

		// Token: 0x04001083 RID: 4227
		public const string DefaultSolutionName = "puddle";

		// Token: 0x04001084 RID: 4228
		private static readonly FixedPoint2 DefaultSlipThreshold = FixedPoint2.New(-1);

		// Token: 0x04001085 RID: 4229
		public static readonly FixedPoint2 DefaultOverflowVolume = FixedPoint2.New(20);

		// Token: 0x04001086 RID: 4230
		[DataField("slipThreshold", false, 1, false, false, null)]
		public FixedPoint2 SlipThreshold = PuddleComponent.DefaultSlipThreshold;

		// Token: 0x04001087 RID: 4231
		[DataField("spillSound", false, 1, false, false, null)]
		public SoundSpecifier SpillSound = new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg", null);

		// Token: 0x04001088 RID: 4232
		[DataField("overflowVolume", false, 1, false, false, null)]
		public FixedPoint2 OverflowVolume = PuddleComponent.DefaultOverflowVolume;

		// Token: 0x04001089 RID: 4233
		[DataField("opacityModifier", false, 1, false, false, null)]
		public float OpacityModifier = 1f;
	}
}
