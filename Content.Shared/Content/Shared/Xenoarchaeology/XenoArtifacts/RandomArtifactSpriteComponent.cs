using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x02000016 RID: 22
	[RegisterComponent]
	public sealed class RandomArtifactSpriteComponent : Component
	{
		// Token: 0x04000029 RID: 41
		[DataField("minSprite", false, 1, false, false, null)]
		public int MinSprite = 1;

		// Token: 0x0400002A RID: 42
		[DataField("maxSprite", false, 1, false, false, null)]
		public int MaxSprite = 14;

		// Token: 0x0400002B RID: 43
		[DataField("activationTime", false, 1, false, false, null)]
		public double ActivationTime = 2.0;

		// Token: 0x0400002C RID: 44
		public TimeSpan? ActivationStart;
	}
}
