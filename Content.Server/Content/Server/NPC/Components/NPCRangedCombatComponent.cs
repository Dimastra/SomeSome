using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Components
{
	// Token: 0x02000370 RID: 880
	[RegisterComponent]
	public sealed class NPCRangedCombatComponent : Component
	{
		// Token: 0x04000B0F RID: 2831
		[ViewVariables]
		public EntityUid Target;

		// Token: 0x04000B10 RID: 2832
		[ViewVariables]
		public CombatStatus Status = CombatStatus.Normal;

		// Token: 0x04000B11 RID: 2833
		[ViewVariables]
		public Angle? RotationSpeed;

		// Token: 0x04000B12 RID: 2834
		[ViewVariables]
		public Angle AccuracyThreshold = Angle.FromDegrees(30.0);

		// Token: 0x04000B13 RID: 2835
		[ViewVariables]
		public float LOSAccumulator;

		// Token: 0x04000B14 RID: 2836
		[ViewVariables]
		public bool TargetInLOS;

		// Token: 0x04000B15 RID: 2837
		[ViewVariables]
		public float ShootDelay = 0.2f;

		// Token: 0x04000B16 RID: 2838
		[ViewVariables]
		public float ShootAccumulator;

		// Token: 0x04000B17 RID: 2839
		[Nullable(2)]
		[ViewVariables]
		public SoundSpecifier SoundTargetInLOS;
	}
}
