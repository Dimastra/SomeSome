using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x02000062 RID: 98
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class FlyBySoundComponent : Component
	{
		// Token: 0x04000126 RID: 294
		[ViewVariables]
		[DataField("prob", false, 1, false, false, null)]
		public float Prob = 0.1f;

		// Token: 0x04000127 RID: 295
		[Nullable(1)]
		[ViewVariables]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound = new SoundCollectionSpecifier("BulletMiss", null)
		{
			Params = AudioParams.Default
		};

		// Token: 0x04000128 RID: 296
		[DataField("range", false, 1, false, false, null)]
		public float Range = 1.5f;
	}
}
