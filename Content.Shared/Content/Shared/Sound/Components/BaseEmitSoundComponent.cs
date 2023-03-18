using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Sound.Components
{
	// Token: 0x02000186 RID: 390
	public abstract class BaseEmitSoundComponent : Component
	{
		// Token: 0x04000458 RID: 1112
		public static readonly AudioParams DefaultParams = AudioParams.Default.WithVolume(-2f);

		// Token: 0x04000459 RID: 1113
		[Nullable(2)]
		[ViewVariables]
		[DataField("sound", false, 1, true, false, null)]
		public SoundSpecifier Sound;
	}
}
