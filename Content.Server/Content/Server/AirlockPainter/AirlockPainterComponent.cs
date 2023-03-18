using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.AirlockPainter
{
	// Token: 0x020007F0 RID: 2032
	[RegisterComponent]
	public sealed class AirlockPainterComponent : Component
	{
		// Token: 0x04001B38 RID: 6968
		[Nullable(1)]
		[DataField("spraySound", false, 1, false, false, null)]
		public SoundSpecifier SpraySound = new SoundPathSpecifier("/Audio/Effects/spray2.ogg", null);

		// Token: 0x04001B39 RID: 6969
		[DataField("sprayTime", false, 1, false, false, null)]
		public float SprayTime = 3f;

		// Token: 0x04001B3A RID: 6970
		[DataField("isSpraying", false, 1, false, false, null)]
		public bool IsSpraying;

		// Token: 0x04001B3B RID: 6971
		public int Index;
	}
}
