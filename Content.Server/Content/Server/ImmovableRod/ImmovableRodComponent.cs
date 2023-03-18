using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.ImmovableRod
{
	// Token: 0x02000453 RID: 1107
	[RegisterComponent]
	public sealed class ImmovableRodComponent : Component
	{
		// Token: 0x04000DF6 RID: 3574
		public int MobCount;

		// Token: 0x04000DF7 RID: 3575
		[Nullable(1)]
		[DataField("hitSound", false, 1, false, false, null)]
		public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Effects/bang.ogg", null);

		// Token: 0x04000DF8 RID: 3576
		[DataField("hitSoundProbability", false, 1, false, false, null)]
		public float HitSoundProbability = 0.1f;

		// Token: 0x04000DF9 RID: 3577
		[DataField("minSpeed", false, 1, false, false, null)]
		public float MinSpeed = 10f;

		// Token: 0x04000DFA RID: 3578
		[DataField("maxSpeed", false, 1, false, false, null)]
		public float MaxSpeed = 35f;

		// Token: 0x04000DFB RID: 3579
		[DataField("randomizeVelocity", false, 1, false, false, null)]
		public bool RandomizeVelocity = true;

		// Token: 0x04000DFC RID: 3580
		[DataField("directionOverride", false, 1, false, false, null)]
		public Angle DirectionOverride = Angle.Zero;

		// Token: 0x04000DFD RID: 3581
		[DataField("destroyTiles", false, 1, false, false, null)]
		public bool DestroyTiles = true;
	}
}
